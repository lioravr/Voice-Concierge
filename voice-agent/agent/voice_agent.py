"""
LiveKit Voice Agent Implementation - Using LiveKit Agents v1.3.x API
Integrated with backend FAQ search and unanswered question tracking
"""
import structlog
import json
from livekit.agents import (
    Agent,
    AgentSession,
    AgentServer,
    RunContext,
    cli,
    llm,
)
from livekit.plugins import openai, silero

from .config import AgentConfig
from .backend_client import BackendClient

logger = structlog.get_logger()

# Create the server instance
server = AgentServer()
    
# Global backend client instance (will be set in agent_entrypoint)
_backend_client = None


async def search_faq_database(query: str) -> str:
    """
    Search the FAQ database for answers to guest questions.
    Use this function whenever a guest asks a question about the resort.
    Returns relevant FAQ information or indicates if no answer was found.
    """
    logger.info("searching_faq_database", query=query)
    
    if _backend_client is None:
        logger.error("backend_client_not_initialized")
        return "I apologize, but I'm unable to search for that information right now. Please contact the front desk for assistance."
    
    try:
        # Search FAQs with semantic search
        results = await _backend_client.search_faqs(
            query=query,
            limit=3,
            threshold=0.3
        )
        
        if not results:
            # No results found - record as unanswered
            logger.info("no_faq_results_found", query=query)
            await _backend_client.record_unanswered_question(query)
            return "I don't have specific information about that in my knowledge base. I've recorded your question for our team to review. Is there anything else I can help you with?"
        
        # Check similarity of best result
        best_result = results[0]
        similarity = best_result.get("similarity", 0)
        
        logger.info("faq_search_results", 
                   query=query, 
                   result_count=len(results),
                   best_similarity=similarity)
        
        # If similarity is low, record as unanswered
        if similarity < 0.7:
            logger.info("low_similarity_recording_question", 
                       query=query, 
                       similarity=similarity)
            await _backend_client.record_unanswered_question(query)
        
        # Build context from results
        faq_info = []
        for idx, result in enumerate(results[:3], 1):
            faq = result.get("faq", {})
            sim = result.get("similarity", 0)
            question = faq.get("question", "")
            answer = faq.get("answer", "")
            category = faq.get("category", "General")
            
            faq_info.append(
                f"FAQ {idx} (Similarity: {sim:.0%}, Category: {category}):\n"
                f"Q: {question}\n"
                f"A: {answer}"
            )
        
        context = "\n\n".join(faq_info)
        
        if similarity < 0.7:
            context += "\n\nNote: The confidence in this answer is moderate. The question has been recorded for review."
        
        return context
        
    except Exception as e:
        logger.error("faq_search_error", query=query, error=str(e))
        await _backend_client.record_unanswered_question(query)
        return "I apologize, but I encountered an issue searching for that information. I've recorded your question for our team. Please contact the front desk for immediate assistance."


@server.rtc_session()
async def agent_entrypoint(ctx: RunContext):
    """Agent entrypoint - called when a participant joins"""
    
    global _backend_client
    
    logger.info("initializing_agent", room=ctx.room.name)
    
    # Load configuration
    config = AgentConfig.from_env()
    config.validate()
    
    # Initialize backend client
    backend = BackendClient(
        base_url=config.backend_api_url,
        timeout=config.backend_timeout
    )
    
    # Set global backend client for function calls
    _backend_client = backend
    
    # Get voice configuration
    tts_voice = config.default_voice
    voice_preference_id = None
    
    # Check if user has a voice preference in metadata
    if ctx.room.remote_participants:
        for participant in ctx.room.remote_participants.values():
            if hasattr(participant, 'metadata') and participant.metadata:
                try:
                    import json
                    metadata = json.loads(participant.metadata)
                    voice_preference_id = metadata.get("voice_preference")
                    if voice_preference_id:
                        logger.info("voice_preference_found", 
                                   participant=participant.identity,
                                   voice_id=voice_preference_id)
                        break
                except Exception as e:
                    logger.warning("failed_to_parse_metadata", error=str(e))
    
    # Get voice from backend based on preference or use active voice
    try:
        if voice_preference_id:
            # Get specific voice by ID
            voice_config = await backend.get_voice_by_id(voice_preference_id)
            if voice_config and voice_config.get("providerVoiceId"):
                logger.info("using_preferred_voice", 
                           name=voice_config.get("name"),
                           provider_voice_id=voice_config.get("providerVoiceId"))
                tts_voice = voice_config.get("providerVoiceId")
            else:
                logger.warning("preferred_voice_not_found", voice_id=voice_preference_id)
                # Fall back to active voice
                active_voice = await backend.get_active_voice()
                if active_voice and active_voice.get("providerVoiceId"):
                    tts_voice = active_voice.get("providerVoiceId")
        else:
            # No preference, use active voice
            active_voice = await backend.get_active_voice()
            if active_voice and active_voice.get("providerVoiceId"):
                logger.info("using_active_voice", 
                           name=active_voice.get("name"),
                           provider_voice_id=active_voice.get("providerVoiceId"))
                tts_voice = active_voice.get("providerVoiceId")
    except Exception as e:
        logger.warning("failed_to_get_voice_config", error=str(e))
    
    logger.info("final_voice_selection", voice=tts_voice)
    
    # Load VAD model
    vad = silero.VAD.load()
    
    logger.info("creating_session")
    
    # Create the agent session with OpenAI LLM
    session = AgentSession(
        stt=openai.STT(),
        llm=openai.LLM(),  # Uses default GPT-4 model
        tts=openai.TTS(voice=tts_voice, speed=config.speech_speed),
        vad=vad,
    )
    
    logger.info("session_created_starting")
    
    # Create function context for FAQ search
    fnc_ctx = llm.FunctionContext()
    fnc_ctx.ai_callable(
        name="search_faq_database",
        description="Search the FAQ database for answers to guest questions about The Meridian Resort"
    )(search_faq_database)
    
    # Create agent with FAQ search capability
    agent = Agent(
        instructions="""You are the helpful voice concierge for The Meridian Casino & Resort, a luxury destination in Las Vegas.

**Your Role:**
- Answer guest questions about the resort, amenities, services, and policies
- Be warm, friendly, professional, and concise
- Keep responses brief (2-3 sentences) since this is voice interaction
- Use natural, conversational language

**IMPORTANT - How to Answer Questions:**
1. When a guest asks ANY question about the resort, ALWAYS use the search_faq_database function first
2. The function will search our knowledge base and return relevant FAQs
3. Base your answer on the information provided by the function
4. If the function indicates no information was found, politely tell the guest their question has been recorded
5. Speak naturally and conversationally - avoid reading the FAQ verbatim

**Examples:**
- Guest: "What time is check-in?"
  → Call search_faq_database("What time is check-in?")
  → Use the returned FAQ answer to respond naturally

- Guest: "Do you have a pool?"
  → Call search_faq_database("Do you have a pool?")
  → Use the returned information to respond

**Personality:**
- Warm and welcoming
- Professional but friendly
- Enthusiastic about the resort
- Conversational and natural

**Remember:**
- ALWAYS search the FAQ database for every question
- Keep responses concise for voice interaction
- Be honest if information isn't available
- Speak naturally, not like reading a script""",
        fnc_ctx=fnc_ctx
    )
    
    # Start the session with room and agent
    await session.start(room=ctx.room, agent=agent)
    
    logger.info("session_started")
    
    # Send welcome greeting
    try:
        await session.say("Welcome to The Meridian Casino and Resort! I'm your voice concierge. How may I assist you today?")
        logger.info("greeting_sent")
    except Exception as e:
        logger.warning("failed_to_send_greeting", error=str(e))
