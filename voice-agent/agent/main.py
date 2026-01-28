"""
Voice Concierge Agent
Main entry point for the LiveKit voice agent
"""
import asyncio
import os
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

async def main():
    """Main entry point for the voice agent"""
    print("Voice Concierge Agent starting...")
    print(f"LiveKit URL: {os.getenv('LIVEKIT_URL', 'Not set')}")
    print(f"Backend API URL: {os.getenv('BACKEND_API_URL', 'Not set')}")
    
    # TODO: Initialize LiveKit agent
    # TODO: Set up conversation handlers
    # TODO: Connect to backend API
    
    print("Voice agent initialized (placeholder)")
    
    # Keep running
    await asyncio.Event().wait()

if __name__ == "__main__":
    asyncio.run(main())
