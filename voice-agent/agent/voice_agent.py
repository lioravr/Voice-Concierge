"""
LiveKit Voice Agent Implementation for v1.3.x API
"""
import structlog
from livekit.agents import (
    Agent,
    JobContext,
    JobProcess,
    AgentSession,
    AgentServer,
    RunContext,
    cli,
)
from livekit.plugins import openai, silero

from .config import AgentConfig
from .backend_client import BackendClient
from .conversation import ConversationManager

logger = structlog.get_logger()

# Create global server instance
server = AgentServer()


class MeridianVoiceAgent(Agent):
    """Voice Concierge Agent for The Meridian Casino & Resort"""
    
    def __init__(self, config: AgentConfig, backend: BackendClient, conversation: ConversationManager):
        # Initialize with instructions for the LLM
        super().__init__(
            instructions=f"""You are {config.agent_name}, a professional voice concierge for The Meridian Casino & Resort.
            
You provide exceptional service by:
- Greeting guests warmly and professionally
- Answering questions accurately using your knowledge base
- Being concise and helpful in your responses
- Maintaining a friendly, professional tone

When answering questions:
1. Search your knowledge base for relevant information
2. Provide clear, accurate answers
3. If unsure, politely indicate you'll check with a human concierge

Your responses are conversational, natural, and designed for voice interaction."""
        )
        self.config = config
        self.backend = backend
        self.conversation = conversation
        self._session_active = False
    
    async def on_session_start(self, context: RunContext):
        """Called when the agent session starts"""
        logger.info(
            "session_starting",
            room=context.room.name if context.room else "unknown"
        )
        self._session_active = True
        
        # Check backend health
        is_healthy = await self.backend.health_check()
        if not is_healthy:
            logger.warning("backend_api_unhealthy")
        
        # Send greeting after a brief delay
        greeting = await self.conversation.get_greeting()
        await context.session.say(greeting, allow_interruptions=True)
        
        logger.info("session_started_and_greeted")
    
    async def on_message_received(self, context: RunContext, message: str):
        """Called when a message is received from the user"""
        logger.info("processing_user_message", message=message)
        
        # Process through conversation manager
        response = await self.conversation.process_question(message)
        
        # Return the response to be spoken
        return response
    
    async def on_session_end(self, context: RunContext):
        """Called when the agent session ends"""
        logger.info("session_ending")
        self._session_active = False
        await self.backend.close()


def prewarm(proc: JobProcess):
    """Prewarm models and connections"""
    logger.info("prewarming_models")
    proc.userdata["vad"] = silero.VAD.load()


server.setup_fnc = prewarm


@server.rtc_session()
async def agent_entrypoint(ctx: JobContext):
    """Main entrypoint for LiveKit agent"""
    # Load config
    config = AgentConfig.from_env()
    
    # Setup logging context
    ctx.log_context_fields = {
        "room": ctx.room.name,
        "agent": config.agent_name
    }
    
    logger.info("initializing_agent", room=ctx.room.name)
    
    # Initialize backend client and conversation manager
    backend = BackendClient(
        base_url=config.backend_api_url,
        timeout=config.backend_timeout
    )
    conversation = ConversationManager(
        config=config,
        backend=backend
    )
    
    # Get active voice configuration from backend
    voice_config = await backend.get_active_voice()
    voice_id = config.default_voice
    
    if voice_config and voice_config.get("providerVoiceId"):
        voice_id = voice_config["providerVoiceId"]
        logger.info(
            "using_configured_voice",
            voice_name=voice_config.get("name"),
            voice_id=voice_id
        )
    
    # Create agent session with voice pipeline
    session = AgentSession(
        # Speech-to-Text
        stt=openai.STT(),
        # Large Language Model (handled by our custom agent)
        llm=openai.LLM(model=config.openai_chat_model),
        # Text-to-Speech with configured voice
        tts=openai.TTS(voice=voice_id, speed=config.speech_speed),
        # Voice Activity Detection
        vad=ctx.proc.userdata["vad"],
        # Allow interruptions
        preemptive_generation=True,
    )
    
    # Create and start the agent
    agent = MeridianVoiceAgent(config, backend, conversation)
    
    logger.info("starting_agent_session")
    
    # Start the session
    await session.start(
        agent=agent,
        room=ctx.room
    )
    
    # Connect to the room
    await ctx.connect()
    
    logger.info("agent_ready_and_connected")


def create_agent(config: AgentConfig):
    """Factory function for compatibility - returns the server"""
    return server
