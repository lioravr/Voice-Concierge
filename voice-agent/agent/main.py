"""
Voice Concierge Agent - Main Entry Point
"""
import structlog
from dotenv import load_dotenv
from livekit.agents import cli

from .voice_agent import server  # Import the server instance

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
    """Main entry point"""
    logger.info("starting_voice_concierge_agent")
    cli.run_app(server)  # Run the server


if __name__ == "__main__":
    main()
