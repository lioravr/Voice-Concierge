"""
Voice Concierge Agent
Main entry point for the LiveKit voice agent using v1.3.x API
"""
import structlog
from dotenv import load_dotenv
from livekit.agents import cli

from .voice_agent import server

# Load environment variables
load_dotenv()

# Configure structured logging
structlog.configure(
    processors=[
        structlog.contextvars.merge_contextvars,
        structlog.processors.add_log_level,
        structlog.processors.TimeStamper(fmt="iso"),
        structlog.dev.ConsoleRenderer()
    ]
)

logger = structlog.get_logger()


def main():
    """Main entry point for the voice agent"""
    logger.info("starting_voice_concierge_agent")
    
    # Run the LiveKit agent server
    # The server is already configured in voice_agent.py
    cli.run_app(server)


if __name__ == "__main__":
    main()
