"""
LiveKit Voice Agent Implementation with PostgreSQL FAQ Database Integration
Uses custom FAQLLM that queries backend database for every question
"""
import structlog
import json
from livekit.agents import (
    Agent,
    AgentSession,
    AgentServer,
    RunContext,
    cli,
)
from livekit.plugins import openai, silero

from .config import AgentConfig
from .backend_client import BackendClient
from .conversation import ConversationManager
from .faq_llm import FAQLLM

logger = structlog.get_logger()

# Create the server instance
server = AgentServer()


@server.rtc_session()
async def agent_entrypoint(ctx: RunContext):
    """Agent entrypoint - called when a participant joins"""
    
    logger.info("initializing_agent", room=ctx.room.name)
    
    # Load configuration
    config = AgentConfig.from_env()
    config.validate()
    
    # Initialize backend client
    backend = BackendClient(
        base_url=config.backend_api_url,
        timeout=config.backend_timeout
    )
    
    # Initialize conversation manager (handles PostgreSQL FAQ search)
    conversation = ConversationManager(config=config, backend=backend)
    logger.info("conversation_manager_initialized")
    
    # Get voice configuration from backend database
    tts_voice = config.default_voice
    voice_preference_id = None
    
    # Check if user has a voice preference in metadata
    if ctx.room.remote_participants:
        for participant in ctx.room.remote_participants.values():
            if hasattr(participant, 'metadata') and participant.metadata:
                try:
                    metadata = json.loads(participant.metadata)
                    voice_preference_id = metadata.get("voice_preference")
                    if voice_preference_id:
                        logger.info("voice_preference_found", 
                                   participant=participant.identity,
                                   voice_id=voice_preference_id)
                        break
                except Exception as e:
                    logger.warning("failed_to_parse_metadata", error=str(e))
    
    # Get voice from backend
    try:
        if voice_preference_id:
            voice_config = await backend.get_voice_by_id(voice_preference_id)
            if voice_config and voice_config.get("providerVoiceId"):
                logger.info("using_preferred_voice", 
                           name=voice_config.get("name"),
                           provider_voice_id=voice_config.get("providerVoiceId"))
                tts_voice = voice_config.get("providerVoiceId")
            else:
                active_voice = await backend.get_active_voice()
                if active_voice and active_voice.get("providerVoiceId"):
                    tts_voice = active_voice.get("providerVoiceId")
        else:
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
    
    # Create custom FAQ-aware LLM
    faq_llm = FAQLLM(conversation_manager=conversation)
    
    # Create the agent session with our custom FAQ LLM
    session = AgentSession(
        stt=openai.STT(),
        llm=faq_llm,  # Our custom LLM that queries PostgreSQL
        tts=openai.TTS(voice=tts_voice, speed=config.speech_speed),
        vad=vad,
    )
    
    logger.info("session_created_with_faq_llm")
    
    # Create agent with minimal instructions (FAQ LLM handles everything)
    agent = Agent(
        instructions="""You are the voice concierge for The Meridian Casino & Resort.
Your responses come from our FAQ database. Be warm, friendly, and professional."""
    )
    
    # Start the session with room and agent
    await session.start(room=ctx.room, agent=agent)
    
    logger.info("session_started_postgresql_faq_integration_active")
    
    # Send personalized greeting from database
    try:
        greeting = await conversation.get_greeting()
        await session.say(greeting)
        logger.info("greeting_sent", greeting=greeting[:50])
    except Exception as e:
        logger.warning("failed_to_send_greeting", error=str(e))
        # Fallback greeting
        await session.say("Welcome to The Meridian Casino and Resort! How may I assist you today?")
