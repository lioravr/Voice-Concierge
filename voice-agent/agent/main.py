"""
Voice Concierge Agent
Main entry point for the LiveKit voice agent
"""
import structlog
from dotenv import load_dotenv
from livekit.agents import WorkerOptions, cli

from .config import AgentConfig
from .voice_agent import create_agent

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
    
    # Load configuration
    try:
        config = AgentConfig.from_env()
        config.validate()
        
        logger.info(
            "agent_configuration_loaded",
            agent_name=config.agent_name,
            backend_url=config.backend_api_url,
            livekit_url=config.livekit_url
        )
    
    except ValueError as e:
        logger.error("configuration_error", error=str(e))
        raise
    
    # Create agent entrypoint
    agent_entrypoint = create_agent(config)
    
    # Start LiveKit worker
    logger.info("starting_livekit_worker")
    
    cli.run_app(
        WorkerOptions(
            entrypoint_fnc=agent_entrypoint,
            api_key=config.livekit_api_key,
            api_secret=config.livekit_api_secret,
            ws_url=config.livekit_url,
        )
    )


if __name__ == "__main__":
    main()
