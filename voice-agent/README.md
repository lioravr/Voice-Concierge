# Voice Concierge Agent

Python-based voice agent using LiveKit for real-time voice interaction with guests of The Meridian Casino & Resort.

## Overview

This agent provides:
- **Real-time Voice Interaction**: Natural conversation using STT/TTS
- **Semantic Search**: Connects to backend API for FAQ retrieval
- **OpenAI LLM**: GPT-4 powered conversation management
- **LiveKit Integration**: Professional voice communication infrastructure
- **Unanswered Question Tracking**: Records questions that can't be answered

## Architecture

```
┌─────────────┐      ┌──────────────┐      ┌──────────────┐
│   Guest     │◄────►│  LiveKit     │◄────►│ Voice Agent  │
│ (Browser/   │ Voice│  Server      │ WS   │  (Python)    │
│  Phone)     │      └──────────────┘      └──────┬───────┘
└─────────────┘                                    │
                                                   │ HTTP
                                          ┌────────▼────────┐
                                          │  Backend API    │
                                          │  (.NET)         │
                                          └─────────────────┘
```

## Features

### Core Capabilities
- ✅ **Voice Activity Detection (VAD)**: Silero VAD for speech detection
- ✅ **Speech-to-Text**: OpenAI Whisper integration
- ✅ **Text-to-Speech**: OpenAI TTS with multiple voices
- ✅ **LLM Integration**: GPT-4 for natural conversation
- ✅ **Semantic Search**: Vector-based FAQ retrieval
- ✅ **Conversation Context**: Maintains conversation history
- ✅ **Voice Configuration**: Supports multiple voice personalities

### Voice Agent Behavior
- **Professional & Friendly**: Warm, welcoming tone
- **Concise Responses**: Optimized for voice (2-3 sentences)
- **Context-Aware**: Maintains conversation flow
- **Honest**: Admits when information isn't available
- **Proactive**: Records unanswered questions for staff

## Configuration

### Environment Variables

Create a `.env` file in the `voice-agent` directory:

```bash
# LiveKit Configuration
LIVEKIT_URL=wss://your-livekit-server.com
LIVEKIT_API_KEY=your-api-key
LIVEKIT_API_SECRET=your-api-secret

# OpenAI Configuration
OPENAI_API_KEY=your-openai-api-key

# Backend API
BACKEND_API_URL=http://backend:5000
BACKEND_TIMEOUT=10

# Agent Behavior (Optional)
AGENT_NAME=Meridian Voice Concierge
DEFAULT_VOICE=nova
SPEECH_SPEED=1.0
SEARCH_LIMIT=3
SIMILARITY_THRESHOLD=0.3
```

### Voice Options

OpenAI TTS voices available:
- `alloy` - Neutral, balanced
- `echo` - Male, warm
- `fable` - British, expressive
- `onyx` - Male, deep, authoritative
- `nova` - Female, energetic (default)
- `shimmer` - Female, soft, clear

## Installation

### Local Development

```bash
# Navigate to voice-agent directory
cd voice-agent

# Create virtual environment
python -m venv venv
source venv/bin/activate  # On Windows: venv\Scripts\activate

# Install dependencies
pip install -r requirements.txt

# Run the agent
python -m agent.main
```

### Docker

```bash
# Build the image
docker build -t voice-concierge-agent .

# Run with docker-compose (recommended)
docker-compose up voice-agent
```

## Usage

### Starting the Agent

The agent connects to LiveKit and waits for incoming connections:

```bash
python -m agent.main
```

Expected output:
```
agent_configuration_loaded agent_name=Meridian Voice Concierge
starting_livekit_worker
Agent worker started, waiting for connections...
```

### Connecting Guests

Guests can connect via:
1. **Web Browser**: LiveKit React SDK (Admin Panel Playground)
2. **Mobile App**: LiveKit iOS/Android SDK
3. **Phone**: SIP integration (if configured)

### Conversation Flow

1. **Guest connects** → Agent sends greeting
2. **Guest asks question** → STT converts to text
3. **Agent searches FAQs** → Backend semantic search
4. **LLM generates response** → Natural, conversational answer
5. **TTS speaks response** → Guest hears answer
6. **Loop continues** → Until guest disconnects

### Example Conversation

```
Agent: "Welcome to The Meridian Casino & Resort! I'm your voice concierge. 
        How may I assist you today?"

Guest: "What time does the casino open?"

Agent: "The Meridian Casino is open 24 hours a day, every day of the year. 
        You're welcome to enjoy slots, table games, and poker anytime!"

Guest: "Do you have a pool?"

Agent: "Yes! Our rooftop pool deck is open daily from 8 AM to 8 PM. 
        We offer cabanas, a hot tub, and poolside service. 
        The pool is heated year-round for your comfort."
```

## Components

### `config.py`
- Configuration management
- Environment variable loading
- Validation

### `backend_client.py`
- HTTP client for backend API
- FAQ semantic search
- Unanswered question recording
- Voice configuration retrieval

### `conversation.py`
- Conversation management
- OpenAI LLM integration
- System prompt definition
- Context building from FAQs
- Response generation

### `voice_agent.py`
- LiveKit agent implementation
- VAD, STT, TTS integration
- Custom LLM adapter
- Session monitoring
- Participant handling

### `main.py`
- Entry point
- Configuration loading
- Structured logging setup
- LiveKit worker initialization

## Logging

The agent uses structured logging with `structlog`:

```python
logger.info(
    "question_processed",
    question="What time is checkout?",
    response_length=87,
    has_relevant_info=True
)
```

Logs include:
- Agent lifecycle events
- Participant connections
- Question processing
- FAQ searches
- LLM responses
- Errors and warnings

## Error Handling

### Backend API Unavailable
- Agent continues to function
- Falls back to cached responses or LLM knowledge
- Logs warning

### OpenAI API Errors
- Retries with exponential backoff
- Falls back to direct FAQ answers
- Returns friendly error message

### No Relevant FAQs
- LLM generates apologetic response
- Question recorded as unanswered
- Offers alternative assistance

## Performance

### Latency
- **STT**: ~100-300ms (OpenAI Whisper)
- **FAQ Search**: ~50-150ms (backend API)
- **LLM Generation**: ~500-1500ms (GPT-4)
- **TTS**: ~200-500ms (OpenAI TTS)
- **Total**: ~1-2 seconds typical response time

### Optimization
- Conversation history limited to 10 exchanges
- LLM max tokens: 150 (concise responses)
- FAQ search limited to top 3 results
- Concurrent request handling

## Testing

### Unit Tests
```bash
pytest tests/
```

### Integration Test
```bash
# Test backend connectivity
python -c "from agent.backend_client import BackendClient; import asyncio; asyncio.run(BackendClient('http://localhost:5000').health_check())"
```

### Manual Testing
Use the Admin Panel Playground to test voice interaction.

## Troubleshooting

### Agent Won't Start
- Check environment variables
- Verify LiveKit credentials
- Ensure backend API is accessible

### No Audio
- Check microphone permissions
- Verify LiveKit server connection
- Test STT/TTS configuration

### Poor Response Quality
- Adjust similarity threshold
- Increase search limit
- Review system prompt
- Check FAQ quality

### High Latency
- Use GPT-3.5-turbo instead of GPT-4
- Reduce max_tokens
- Optimize backend API
- Use CDN for LiveKit

## Future Enhancements

- [ ] Multi-language support
- [ ] Voice authentication
- [ ] Emotion detection
- [ ] Custom wake word
- [ ] Analytics dashboard
- [ ] A/B testing for responses

## License

Part of The Meridian Voice Concierge project.
