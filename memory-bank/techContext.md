# Technical Context: Voice Concierge

## Technology Stack

### Backend API
- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12
- **ORM**: Entity Framework Core 8.0
- **API Style**: RESTful
- **Documentation**: Swagger/OpenAPI

**Key Libraries:**
- `Microsoft.EntityFrameworkCore.Design` - Migrations
- `Npgsql.EntityFrameworkCore.PostgreSQL` - PostgreSQL provider
- `Pgvector.EntityFrameworkCore` - Vector similarity extension
- `OpenAI` - Official OpenAI .NET SDK
- `FluentValidation` - Input validation
- `Serilog` - Structured logging

### Voice Agent
- **Language**: Python 3.11+
- **Framework**: LiveKit Agents SDK
- **Runtime**: asyncio-based

**Key Libraries:**
- `livekit` - WebRTC infrastructure
- `livekit-agents` - Agent framework
- `openai` - LLM and embeddings
- `httpx` - Async HTTP client for API calls
- `python-dotenv` - Environment configuration
- `pydantic` - Data validation

### Admin Panel
- **Framework**: React 18
- **Language**: TypeScript 5
- **Build Tool**: Vite
- **Styling**: TailwindCSS 3
- **State**: Zustand or React Context
- **Routing**: React Router v6

**Key Libraries:**
- `@tanstack/react-query` - Server state management
- `axios` - HTTP client
- `react-hook-form` - Form handling
- `zod` - Schema validation
- `lucide-react` - Icons
- `sonner` - Toast notifications
- `livekit-client` - LiveKit React SDK (for playground)

### Database
- **DBMS**: PostgreSQL 16
- **Extension**: pgvector 0.5+
- **Connection Pooling**: Built-in with Npgsql

**Schema:**
- `faqs` - FAQ items with embeddings
- `unanswered_questions` - Unknown questions tracking
- `voice_configurations` - Voice personality options

### External Services

#### OpenAI
- **Embeddings**: `text-embedding-3-small` (1536 dimensions)
- **LLM**: `gpt-4-turbo-preview` or `gpt-3.5-turbo`
- **Rate Limits**: Handle appropriately
- **Cost Optimization**: Use smaller model for embeddings

#### LiveKit
- **Infrastructure**: LiveKit Cloud or self-hosted
- **Connection**: WebRTC-based
- **Authentication**: API key + secret
- **Room Management**: Automatic

#### STT/TTS Options
**Option 1: OpenAI**
- STT: Whisper API
- TTS: OpenAI TTS API (voices: alloy, echo, fable, onyx, nova, shimmer)

**Option 2: Alternative Providers**
- Deepgram (STT)
- ElevenLabs (TTS) - premium voice quality
- Google Cloud Speech/TTS
- Azure Cognitive Services

### Infrastructure & DevOps

#### Containerization
- **Platform**: Docker
- **Orchestration**: Docker Compose
- **Base Images**:
  - Backend: `mcr.microsoft.com/dotnet/aspnet:8.0`
  - Voice Agent: `python:3.11-slim`
  - Admin Panel: `node:20-alpine` (build), `nginx:alpine` (serve)
  - Database: `pgvector/pgvector:pg16`

#### Environment Management
- `.env` file for local development
- Docker Compose environment variables
- Secrets not committed to repository

## Development Setup

### Prerequisites
- **Docker Desktop**: v24+ (with Compose V2)
- **Docker Compose**: v2.20+
- **Git**: v2.30+
- **Text Editor/IDE**: Visual Studio Code, Rider, or similar

### Optional (for local development without Docker)
- **.NET SDK**: 8.0+
- **Python**: 3.11+
- **Node.js**: 20+
- **PostgreSQL**: 16+ with pgvector

### Required Credentials
- OpenAI API Key
- LiveKit API Key + Secret + URL
- STT/TTS provider credentials (if not using OpenAI)

## Development Workflow

### Local Development

#### 1. Clone Repository
```bash
git clone <repository-url>
cd Voice-Concierge
```

#### 2. Configure Environment
```bash
cp .env.example .env
# Edit .env with your credentials
```

#### 3. Start Services
```bash
docker-compose up --build
```

#### 4. Access Applications
- Admin Panel: `http://localhost:3000`
- Backend API: `http://localhost:5000`
- API Docs: `http://localhost:5000/swagger`
- Voice Agent: `http://localhost:8080` (WebSocket endpoint)
- Database: `localhost:5432`

### Development Commands

#### Backend (.NET)
```bash
# Run migrations
cd backend/src/VoiceConcierge.API
dotnet ef database update

# Add new migration
dotnet ef migrations add MigrationName

# Run locally
dotnet run

# Run tests
dotnet test
```

#### Voice Agent (Python)
```bash
cd voice-agent

# Install dependencies
pip install -r requirements.txt

# Run locally
python agent/main.py

# Lint
flake8 agent/

# Format
black agent/
```

#### Admin Panel (React)
```bash
cd admin-panel

# Install dependencies
npm install

# Run dev server
npm run dev

# Build for production
npm run build

# Lint
npm run lint

# Type check
npm run type-check
```

## Database Management

### Connection String Format
```
Host=localhost;Port=5432;Database=voice_concierge;Username=postgres;Password=password
```

### Migrations Strategy
- **Entity Framework Core Migrations** for schema changes
- **Seed Data Migration** for initial FAQ and voice configuration data
- **Version Control**: All migrations committed to repository

### Backup Strategy (Production)
- Regular PostgreSQL dumps
- Separate backup for embeddings (expensive to regenerate)

## API Endpoints

### Base URL
`http://localhost:5000/api`

### FAQ Endpoints
```
GET    /faqs                    # List all FAQs
GET    /faqs/{id}              # Get FAQ by ID
POST   /faqs/search            # Semantic search
POST   /faqs                    # Create FAQ
PUT    /faqs/{id}              # Update FAQ
DELETE /faqs/{id}              # Delete FAQ
```

### Unanswered Questions Endpoints
```
GET    /unanswered-questions            # List pending questions
POST   /unanswered-questions            # Record question
POST   /unanswered-questions/{id}/convert   # Convert to FAQ
DELETE /unanswered-questions/{id}       # Dismiss question
```

### Voice Configuration Endpoints
```
GET    /voice-configurations          # List all voices
GET    /voice-configurations/active   # Get active voice
PUT    /voice-configurations/{id}/activate  # Set active voice
GET    /voice-configurations/{id}/preview   # Get preview audio URL
```

## Configuration Files

### docker-compose.yml
```yaml
services:
  postgres:
    image: pgvector/pgvector:pg16
    environment:
      POSTGRES_DB: voice_concierge
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: ${DB_PASSWORD}
    volumes:
      - postgres-data:/var/lib/postgresql/data
    ports:
      - "5432:5432"

  backend:
    build: ./backend
    environment:
      ConnectionStrings__DefaultConnection: ${DB_CONNECTION_STRING}
      OpenAI__ApiKey: ${OPENAI_API_KEY}
    depends_on:
      - postgres
    ports:
      - "5000:8080"

  voice-agent:
    build: ./voice-agent
    environment:
      LIVEKIT_URL: ${LIVEKIT_URL}
      LIVEKIT_API_KEY: ${LIVEKIT_API_KEY}
      LIVEKIT_API_SECRET: ${LIVEKIT_API_SECRET}
      OPENAI_API_KEY: ${OPENAI_API_KEY}
      BACKEND_API_URL: http://backend:8080
    depends_on:
      - backend
    ports:
      - "8080:8080"

  admin-panel:
    build: ./admin-panel
    environment:
      VITE_API_URL: http://localhost:5000/api
    depends_on:
      - backend
    ports:
      - "3000:80"

volumes:
  postgres-data:
```

### .env.example
```env
# Database
DB_PASSWORD=secure_password_here
DB_CONNECTION_STRING=Host=postgres;Port=5432;Database=voice_concierge;Username=postgres;Password=secure_password_here

# OpenAI
OPENAI_API_KEY=sk-...

# LiveKit
LIVEKIT_URL=wss://your-livekit-server.com
LIVEKIT_API_KEY=APIkey...
LIVEKIT_API_SECRET=secret...

# Voice Provider (if not using OpenAI)
TTS_PROVIDER=openai
TTS_API_KEY=...
STT_PROVIDER=openai
STT_API_KEY=...
```

## Code Organization Principles

### Backend (.NET)
- **Clean Architecture**: Separation of concerns across layers
- **Dependency Injection**: All services registered in DI container
- **Async/Await**: All I/O operations asynchronous
- **SOLID Principles**: Interface-based design
- **Repository Pattern**: Abstract data access

### Voice Agent (Python)
- **Single Responsibility**: Each module has clear purpose
- **Async First**: Use asyncio throughout
- **Error Handling**: Try/except with graceful fallbacks
- **Configuration**: Environment-based settings
- **Logging**: Structured logging for debugging

### Admin Panel (React)
- **Component Composition**: Reusable, small components
- **TypeScript**: Full type safety
- **Hooks**: Modern React patterns
- **Separation of Concerns**: Components vs business logic
- **Responsive Design**: TailwindCSS utilities

## Testing Strategy

### Backend Testing
- **Unit Tests**: Service layer logic
- **Integration Tests**: Repository + database
- **API Tests**: Controller endpoints
- **Framework**: xUnit + Moq

### Voice Agent Testing
- **Unit Tests**: Question handling logic
- **Integration Tests**: API client
- **Framework**: pytest

### Admin Panel Testing
- **Component Tests**: React Testing Library
- **E2E Tests**: Playwright (optional)

## Performance Considerations

### Embedding Generation
- **Caching**: Store embeddings in database
- **Batch Processing**: Generate multiple embeddings together
- **Cost**: ~$0.0001 per 1K tokens (text-embedding-3-small)

### Database Queries
- **Indexes**: ivfflat index on embedding column
- **Connection Pooling**: Configured in connection string
- **Query Optimization**: Limit result sets

### Voice Latency
- **Target**: < 3 seconds from question to answer
- **Optimization**:
  - Fast semantic search (pgvector)
  - Streaming TTS (if supported)
  - Minimal API calls
  - Efficient LLM prompts

### Admin Panel
- **Code Splitting**: Lazy load routes
- **API Caching**: React Query cache
- **Optimistic Updates**: Immediate UI feedback

## Logging & Monitoring

### Backend Logging
- **Library**: Serilog
- **Format**: Structured JSON
- **Levels**: Debug, Info, Warning, Error
- **Sinks**: Console, File (optional)

### Voice Agent Logging
- **Library**: Python logging
- **Format**: Structured
- **Levels**: DEBUG, INFO, WARNING, ERROR
- **Output**: stdout for Docker

### Key Metrics to Track
- FAQ search response time
- Embedding generation time
- Voice latency (end-to-end)
- Unanswered question frequency
- API error rates

## Security Considerations

### API Security
- **Input Validation**: All endpoints validate input
- **SQL Injection**: Parameterized queries via EF Core
- **CORS**: Configured for frontend origin
- **Rate Limiting**: Consider for production

### Secrets Management
- **Development**: .env file (gitignored)
- **Production**: Environment variables or secrets manager
- **Never Commit**: API keys, passwords, connection strings

### Voice Security
- **LiveKit Tokens**: Short-lived, room-specific
- **No PII Logging**: Avoid logging conversation content
- **TLS/SSL**: All external communication encrypted

## Troubleshooting

### Common Issues

**Database Connection Failed**
- Check PostgreSQL container is running
- Verify connection string in .env
- Ensure pgvector extension is installed

**OpenAI API Errors**
- Verify API key is valid
- Check rate limits
- Ensure sufficient credits

**LiveKit Connection Issues**
- Verify credentials
- Check firewall settings
- Ensure WebSocket support

**Admin Panel Can't Reach API**
- Check CORS configuration
- Verify API URL in frontend .env
- Ensure backend container is running

## Future Technical Enhancements

### Phase 2 Improvements
- **Caching Layer**: Redis for frequently accessed FAQs
- **Async Processing**: Message queue for embeddings
- **Authentication**: JWT-based auth for admin panel
- **Analytics**: Track conversation metrics
- **Multi-tenancy**: Support multiple properties

### Scalability Enhancements
- **Load Balancing**: Multiple API instances
- **Database Replicas**: Read replicas for queries
- **CDN**: Static asset delivery
- **Kubernetes**: Container orchestration
