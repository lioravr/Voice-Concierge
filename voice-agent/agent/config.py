"""
Configuration management for Voice Concierge Agent
"""
import os
from dataclasses import dataclass
from typing import Optional


@dataclass
class AgentConfig:
    """Voice agent configuration"""
    
    # LiveKit Configuration
    livekit_url: str
    livekit_api_key: str
    livekit_api_secret: str
    
    # OpenAI Configuration
    openai_api_key: str
    
    # Backend API Configuration
    backend_api_url: str
    backend_timeout: int = 10
    
    # Agent Behavior
    agent_name: str = "Meridian Voice Concierge"
    greeting_message: str = "Welcome to The Meridian Casino & Resort! I'm your voice concierge. How may I assist you today?"
    fallback_message: str = "I apologize, but I don't have information about that. Would you like me to note this question for our team?"
    
    # Voice Configuration
    default_voice: str = "nova"  # OpenAI TTS voice
    speech_speed: float = 1.0
    
    # Search Configuration
    search_limit: int = 3
    similarity_threshold: float = 0.3
    
    @classmethod
    def from_env(cls) -> "AgentConfig":
        """Load configuration from environment variables"""
        return cls(
            # LiveKit
            livekit_url=os.getenv("LIVEKIT_URL", ""),
            livekit_api_key=os.getenv("LIVEKIT_API_KEY", ""),
            livekit_api_secret=os.getenv("LIVEKIT_API_SECRET", ""),
            
            # OpenAI
            openai_api_key=os.getenv("OPENAI_API_KEY", ""),
            
            # Backend API
            backend_api_url=os.getenv("BACKEND_API_URL", "http://localhost:5000"),
            backend_timeout=int(os.getenv("BACKEND_TIMEOUT", "10")),
            
            # Agent Behavior (can be overridden)
            agent_name=os.getenv("AGENT_NAME", "Meridian Voice Concierge"),
            greeting_message=os.getenv(
                "GREETING_MESSAGE",
                "Welcome to The Meridian Casino & Resort! I'm your voice concierge. How may I assist you today?"
            ),
            
            # Voice
            default_voice=os.getenv("DEFAULT_VOICE", "nova"),
            speech_speed=float(os.getenv("SPEECH_SPEED", "1.0")),
            
            # Search
            search_limit=int(os.getenv("SEARCH_LIMIT", "3")),
            similarity_threshold=float(os.getenv("SIMILARITY_THRESHOLD", "0.3")),
        )
    
    def validate(self) -> None:
        """Validate required configuration"""
        required_fields = [
            ("livekit_url", self.livekit_url),
            ("livekit_api_key", self.livekit_api_key),
            ("livekit_api_secret", self.livekit_api_secret),
            ("openai_api_key", self.openai_api_key),
            ("backend_api_url", self.backend_api_url),
        ]
        
        missing = [name for name, value in required_fields if not value]
        if missing:
            raise ValueError(f"Missing required configuration: {', '.join(missing)}")
