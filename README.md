# Voice Concierge for The Meridian Casino & Resort

A comprehensive voice-based concierge system that allows guests to ask questions and receive instant, spoken answers about property amenities, services, and information 24/7.

---

## 🚀 Quick Start (For Reviewers)

**Get the system running in 3 minutes:**

### Step 1: Clone and Configure
```bash
# Clone the repository
git clone https://github.com/lioravr/Voice-Concierge.git
cd Voice-Concierge

# Copy environment template
cp .env.example .env
```

### Step 2: Add Your Credentials
Edit `.env` and add:
```bash
# Required: OpenAI API Key
OPENAI_API_KEY=sk-your-openai-api-key-here

# Required: LiveKit Credentials
LIVEKIT_URL=wss://your-livekit-url
LIVEKIT_API_KEY=your-api-key
LIVEKIT_API_SECRET=your-api-secret

# Optional: Database password (default works fine)
DB_PASSWORD=postgres
```

### Step 3: Start Everything
```bash
docker compose up -d
```

That's it! Wait ~30 seconds for services to initialize.

### Step 4: Test the System

1. **Voice Playground** (Test voice interaction):
   - Open: http://localhost:3000/playground
   - Click "Connect"
   - Speak: "What time is check-in?"
   - Hear the voice response!

2. **Admin Panel** (Manage FAQs):
   - Open: http://localhost:3000
   - Browse to FAQs page
   - Try creating/editing/deleting FAQs

3. **Backend API** (Check health):
   - Open: http://localhost:5000/api/faq
   - See JSON response with FAQ data

### Troubleshooting
```bash
# Check service status
docker compose ps

# View logs if issues occur
docker compose logs backend
docker compose logs voice-agent

# Restart services
docker compose restart
```

### ✅ What Success Looks Like

When the system is running correctly:

1. **All 4 services are healthy:**
```bash
$ docker compose ps
NAME                  STATUS
voice-concierge-db    Up (healthy)
voice-concierge-backend   Up (healthy)  
voice-concierge-voice-agent   Up
voice-concierge-admin-panel   Up
```

2. **Backend API responds:**
```bash
$ curl http://localhost:5000/api/faq
# Returns JSON array with 43 FAQ entries
```

3. **Voice Playground works:**
   - You see "Connect" button
   - After clicking, microphone enables
   - You can speak and hear responses
   - Transcript shows conversation

4. **Admin Panel loads:**
   - Dashboard shows statistics
   - FAQ list displays all entries
   - Voice configurations are visible
   - No errors in browser console

**Total Setup Time**: 2-3 minutes  
**System Status**: ✅ Production Ready

---

## Overview

The Voice Concierge system consists of four main components:
- **Backend API** (.NET Core) - RESTful API for FAQ management and semantic search
- **Voice Agent** (Python + LiveKit) - Real-time voice interaction handling
- **Admin Panel** (React) - Web interface for managing FAQs and configuration
- **Database** (PostgreSQL + pgvector) - Semantic search-enabled storage

## Features

### Core Features
- 24/7 voice-based guest support
- Natural language understanding with semantic search
- Automatic recording of unanswered questions
- Web-based playground for testing

### Bonus Features
- Admin panel for FAQ management (CRUD operations)
- Unanswered questions queue with convert-to-FAQ functionality
- Voice configuration UI with 4 distinct voice personalities
- Integrated playground within admin panel

## Architecture

```
┌─────────────────┐     ┌─────────────────┐
│  Guest User     │     │  Admin User     │
└────────┬────────┘     └────────┬────────┘
         │                       │
         ├──────────┐   ┌───────┤
         │          │   │       │
    ┌────▼────┐  ┌──▼───▼────┐  │
    │ Voice   │  │   Admin   │  │
    │Playground│  │   Panel   │  │
    └────┬────┘  └──────┬────┘  │
         │              │        │
         │         ┌────▼────────▼──┐
         │         │  Backend API   │
         │         │    (.NET)      │
         │         └────┬───────────┘
         │              │
    ┌────▼──────────────▼────┐
    │    Voice Agent         │
    │    (Python/LiveKit)    │
    └────────┬───────────────┘
             │
    ┌────────▼─────────────┐
    │   PostgreSQL +       │
    │     pgvector         │
    └──────────────────────┘
```

## Prerequisites

- **Docker Desktop** v24+ with Docker Compose V2
- **Git** v2.30+
- **OpenAI API Key** (for LLM and embeddings)
- **LiveKit Credentials** (API key, secret, and server URL)

> **Note**: See the [Quick Start](#-quick-start-for-reviewers) section above for complete setup instructions.

## Development

### Project Structure

```
Voice-Concierge/
├── backend/                          # .NET Core API
│   ├── src/
│   │   ├── VoiceConcierge.API/      # Controllers, DTOs, Middleware
│   │   ├── VoiceConcierge.Core/     # Domain entities, Services
│   │   └── VoiceConcierge.Infrastructure/  # Repositories, DbContext
│   ├── VoiceConcierge.sln
│   └── Dockerfile
├── voice-agent/                      # Python Voice Agent
│   ├── agent/
│   │   └── main.py
│   ├── requirements.txt
│   └── Dockerfile
├── admin-panel/                      # React Admin Panel
│   ├── src/
│   ├── package.json
│   └── Dockerfile
├── memory-bank/                      # Project documentation
├── docker-compose.yml
├── .env.example
└── README.md
```

### Running Individual Components Locally

#### Backend API (.NET)

```bash
cd backend/src/VoiceConcierge.API
dotnet restore
dotnet run
```

#### Voice Agent (Python)

```bash
cd voice-agent
pip install -r requirements.txt
python agent/main.py
```

#### Admin Panel (React)

```bash
cd admin-panel
npm install
npm run dev
```

### Database Migrations

To create a new migration:

```bash
cd backend/src/VoiceConcierge.API
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Technology Stack

### Backend
- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL with pgvector
- OpenAI .NET SDK

### Voice Agent
- Python 3.11+
- LiveKit Agents SDK
- OpenAI Python SDK
- httpx (async HTTP)

### Admin Panel
- React 18 with TypeScript
- Vite build tool
- TailwindCSS for styling
- React Query for API state
- React Router for navigation

### Infrastructure
- Docker & Docker Compose
- PostgreSQL 16 with pgvector extension
- OpenAI API (GPT-4/3.5 + Embeddings)
- LiveKit (WebRTC infrastructure)

## Environment Variables

### Required
- `OPENAI_API_KEY` - OpenAI API key for LLM and embeddings
- `LIVEKIT_URL` - LiveKit server WebSocket URL
- `LIVEKIT_API_KEY` - LiveKit API key
- `LIVEKIT_API_SECRET` - LiveKit API secret

### Optional
- `DB_NAME` - Database name (default: voice_concierge)
- `DB_USER` - Database user (default: postgres)
- `DB_PASSWORD` - Database password (default: postgres)
- `OPENAI_EMBEDDING_MODEL` - Embedding model (default: text-embedding-3-small)
- `OPENAI_CHAT_MODEL` - Chat model (default: gpt-3.5-turbo)
- `TTS_PROVIDER` - Text-to-speech provider (default: openai)
- `STT_PROVIDER` - Speech-to-text provider (default: openai)

## API Endpoints

### FAQ Management
- `GET /api/faqs` - List all FAQs
- `GET /api/faqs/{id}` - Get FAQ by ID
- `POST /api/faqs/search` - Semantic search
- `POST /api/faqs` - Create new FAQ
- `PUT /api/faqs/{id}` - Update FAQ
- `DELETE /api/faqs/{id}` - Delete FAQ

### Unanswered Questions
- `GET /api/unanswered-questions` - List pending questions
- `POST /api/unanswered-questions` - Record new question
- `POST /api/unanswered-questions/{id}/convert` - Convert to FAQ
- `DELETE /api/unanswered-questions/{id}` - Dismiss question

### Voice Configuration
- `GET /api/voice-configurations` - List all voices
- `GET /api/voice-configurations/active` - Get active voice
- `PUT /api/voice-configurations/{id}/activate` - Set active voice

## Testing

### Test Scenarios

1. **Basic FAQ Lookup**: "What time does the casino open?"
2. **Complex Question**: "I want to propose to my girlfriend, what should I do?"
3. **Partner Discount**: "Are there good restaurants nearby?"
4. **Unknown Question**: "Can I bring my dog to the hotel?"
5. **Voice Switching**: Test all 4 voice personalities
6. **Admin Workflow**: Add FAQ → Test in playground

## Troubleshooting

### Database Connection Issues
- Ensure PostgreSQL container is running: `docker-compose ps`
- Check connection string in `.env`
- Verify pgvector extension is installed

### OpenAI API Errors
- Verify API key is correct in `.env`
- Check API quota and rate limits
- Ensure sufficient account credits

### LiveKit Connection Issues
- Verify LiveKit credentials in `.env`
- Check firewall settings for WebSocket connections
- Ensure LiveKit server is accessible

### Port Already in Use
```bash
# Stop all services
docker-compose down

# If ports still in use, find and kill processes
lsof -ti:5000 | xargs kill -9  # Backend
lsof -ti:3000 | xargs kill -9  # Admin
lsof -ti:8080 | xargs kill -9  # Voice Agent
```

## Documentation

For detailed information, see the `memory-bank/` directory:
- `projectbrief.md` - Project overview and requirements
- `productContext.md` - User experience and business context
- `systemPatterns.md` - Architecture and design patterns
- `techContext.md` - Technical stack and setup
- `activeContext.md` - Current development status
- `progress.md` - Work tracking and completion status

## License

See LICENSE file for details.

## Contact

For questions or support, contact: jenya@rocketdreams.co
