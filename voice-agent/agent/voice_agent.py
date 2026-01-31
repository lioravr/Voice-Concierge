"""
LiveKit Voice Agent Implementation - Using LiveKit Agents v1.3.x API
"""
import structlog
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
    
    # Create agent with detailed instructions
    agent = Agent(
        instructions="""You are the helpful voice concierge for The Meridian Casino & Resort, a luxury destination in Las Vegas.

**Your Role:**
- Answer guest questions about the resort, amenities, services, and policies
- Be warm, friendly, professional, and concise
- Provide specific information when possible (times, locations, features)

**Key Information:**
- Check-in: 3:00 PM, Check-out: 11:00 AM
- Resort fee: $45/night (includes WiFi, pool access, fitness center)
- Parking: Valet $35/day, Self-parking $20/day
- Pet policy: Up to 2 pets (40 lbs each), $75/pet/night
- Pool hours: 8 AM - 10 PM daily
- Fitness center: 24 hours, complimentary
- Spa: 9 AM - 9 PM, appointments recommended
- Casino: 24 hours, 18+ only
- Restaurants: Multiple options, some require reservations
- Room service: 24 hours available
- WiFi: Complimentary for guests

If you're not sure about something specific, offer to connect them with the front desk."""
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
