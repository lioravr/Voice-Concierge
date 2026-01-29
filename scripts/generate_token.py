#!/usr/bin/env python3
"""
Generate a LiveKit access token for testing the voice agent
"""
import os
import sys
from livekit import api

# Load credentials from environment variables (REQUIRED)
LIVEKIT_API_KEY = os.getenv('LIVEKIT_API_KEY')
LIVEKIT_API_SECRET = os.getenv('LIVEKIT_API_SECRET')

if not LIVEKIT_API_KEY or not LIVEKIT_API_SECRET:
    print("❌ Error: LIVEKIT_API_KEY and LIVEKIT_API_SECRET must be set")
    print("\nUsage:")
    print("  export LIVEKIT_API_KEY=your-key")
    print("  export LIVEKIT_API_SECRET=your-secret")
    print("  python generate_token.py [room-name] [participant-name]")
    sys.exit(1)

def generate_token(room_name: str = "test-room", participant_name: str = "guest"):
    """Generate a LiveKit access token"""
    token = api.AccessToken(LIVEKIT_API_KEY, LIVEKIT_API_SECRET)
    token.with_identity(participant_name)
    token.with_name(participant_name)
    token.with_grants(api.VideoGrants(
        room_join=True,
        room=room_name,
        can_publish=True,
        can_subscribe=True,
    ))
    
    return token.to_jwt()

if __name__ == "__main__":
    room = sys.argv[1] if len(sys.argv) > 1 else "test-room"
    participant = sys.argv[2] if len(sys.argv) > 2 else "guest"
    
    token = generate_token(room, participant)
    
    print(f"\n{'='*60}")
    print(f"LiveKit Access Token Generated")
    print(f"{'='*60}")
    print(f"Room: {room}")
    print(f"Participant: {participant}")
    print(f"\nToken:\n{token}")
    print(f"\n{'='*60}")
    print(f"\nUse at: https://meet.livekit.io/custom")
    print(f"{'='*60}\n")
