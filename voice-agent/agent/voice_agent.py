"""
LiveKit Voice Agent Implementation
"""
import structlog
from livekit import rtc
from livekit.agents import (
    AutoSubscribe,
    JobContext,
    WorkerOptions,
    cli,
    llm,
)
from livekit.agents.voice_assistant import VoiceAssistant
from livekit.plugins import openai, silero

from .config import AgentConfig
from .backend_client import BackendClient
from .conversation import ConversationManager

logger = structlog.get_logger()


class MeridianVoiceAgent:
    """Voice Concierge Agent for The Meridian Casino & Resort"""
    
    def __init__(self, config: AgentConfig):
        self.config = config
        self.backend = BackendClient(
            base_url=config.backend_api_url,
            timeout=config.backend_timeout
        )
        self.conversation = ConversationManager(
            config=config,
            backend=self.backend
        )
    
    async def entrypoint(self, ctx: JobContext):
        """Main entrypoint for LiveKit agent"""
        logger.info(
            "agent_starting",
            room=ctx.room.name,
            agent=self.config.agent_name
        )
        
        # Check backend health
        is_healthy = await self.backend.health_check()
        if not is_healthy:
            logger.warning("backend_api_unhealthy")
        
        # Get active voice configuration
        voice_config = await self.backend.get_active_voice()
        voice_id = self.config.default_voice
        
        if voice_config and voice_config.get("providerVoiceId"):
            voice_id = voice_config["providerVoiceId"]
            logger.info(
                "using_configured_voice",
                voice_name=voice_config.get("name"),
                voice_id=voice_id
            )
        
        # Connect to the room
        await ctx.connect(auto_subscribe=AutoSubscribe.AUDIO_ONLY)
        
        # Wait for first participant
        participant = await ctx.wait_for_participant()
        logger.info(
            "participant_joined",
            participant_id=participant.identity
        )
        
        # Create LLM adapter for our custom conversation handler
        llm_adapter = CustomLLMAdapter(self.conversation)
        
        # Initialize Voice Assistant
        assistant = VoiceAssistant(
            vad=silero.VAD.load(),  # Voice Activity Detection
            stt=openai.STT(),  # Speech-to-Text
            llm=llm_adapter,  # Our custom LLM handler
            tts=openai.TTS(voice=voice_id, speed=self.config.speech_speed),  # Text-to-Speech
            chat_ctx=llm.ChatContext(),  # Managed by our handler
        )
        
        # Start the assistant
        assistant.start(ctx.room, participant)
        
        # Send greeting
        greeting = await self.conversation.get_greeting()
        await assistant.say(greeting, allow_interruptions=True)
        
        logger.info("voice_assistant_started")
        
        # Monitor session
        await self._monitor_session(ctx, assistant)
    
    async def _monitor_session(self, ctx: JobContext, assistant: VoiceAssistant):
        """Monitor the conversation session"""
        
        @ctx.room.on("participant_disconnected")
        def on_participant_disconnected(participant: rtc.RemoteParticipant):
            logger.info(
                "participant_disconnected",
                participant_id=participant.identity
            )
        
        @ctx.room.on("track_published")
        def on_track_published(
            publication: rtc.RemoteTrackPublication,
            participant: rtc.RemoteParticipant
        ):
            logger.info(
                "track_published",
                participant_id=participant.identity,
                track_sid=publication.sid,
                track_kind=publication.kind
            )
        
        # Keep session alive
        try:
            await ctx.wait_for_completion()
        finally:
            logger.info("session_completed")
            await self.backend.close()
    
    async def cleanup(self):
        """Cleanup resources"""
        await self.backend.close()


class CustomLLMAdapter(llm.LLM):
    """Custom LLM adapter that uses our ConversationManager"""
    
    def __init__(self, conversation_manager: ConversationManager):
        super().__init__()
        self.conversation = conversation_manager
    
    async def chat(
        self,
        chat_ctx: llm.ChatContext,
        *,
        conn_options: llm.LLMOptions = llm.LLMOptions(),
    ) -> "llm.LLMStream":
        """Process chat request"""
        
        # Get the last user message
        user_messages = [msg for msg in chat_ctx.messages if msg.role == "user"]
        if not user_messages:
            return self._empty_stream()
        
        last_user_message = user_messages[-1].content
        
        logger.info("processing_user_message", message=last_user_message)
        
        # Process through our conversation manager
        response = await self.conversation.process_question(last_user_message)
        
        # Create stream with response
        return CustomLLMStream(response)


class CustomLLMStream(llm.LLMStream):
    """Custom LLM stream that wraps our response"""
    
    def __init__(self, response: str):
        super().__init__()
        self._response = response
        self._sent = False
    
    async def aclose(self) -> None:
        """Close the stream"""
        pass
    
    def __aiter__(self):
        return self
    
    async def __anext__(self) -> llm.ChatChunk:
        if self._sent:
            raise StopAsyncIteration
        
        self._sent = True
        
        return llm.ChatChunk(
            choices=[
                llm.Choice(
                    delta=llm.ChoiceDelta(
                        content=self._response,
                        role="assistant"
                    )
                )
            ]
        )


def create_agent(config: AgentConfig):
    """Factory function to create and return the agent entrypoint"""
    agent = MeridianVoiceAgent(config)
    return agent.entrypoint
