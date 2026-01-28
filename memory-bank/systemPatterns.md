# System Patterns: Voice Concierge Architecture

## High-Level Architecture

```mermaid
graph TB
    subgraph client [Client Layer]
        Guest[Guest User]
        Admin[Admin User]
    end
    
    subgraph frontend [Frontend Applications]
        Playground[Voice Playground<br/>Web Interface]
        AdminPanel[Admin Panel<br/>React SPA]
    end
    
    subgraph services [Application Services]
        VoiceAgent[Voice Agent<br/>Python + LiveKit]
        BackendAPI[Backend API<br/>.NET Core]
    end
    
    subgraph data [Data Layer]
        PostgreSQL[(PostgreSQL<br/>+ pgvector)]
    end
    
    subgraph external [External Services]
        LiveKitCloud[LiveKit Cloud<br/>WebRTC Infrastructure]
        OpenAI[OpenAI API<br/>LLM + Embeddings]
        TTS[TTS Service<br/>Voice Synthesis]
        STT[STT Service<br/>Speech Recognition]
    end
    
    Guest --> Playground
    Admin --> AdminPanel
    
    Playground --> VoiceAgent
    AdminPanel --> BackendAPI
    
    VoiceAgent --> LiveKitCloud
    VoiceAgent --> BackendAPI
    VoiceAgent --> OpenAI
    VoiceAgent --> TTS
    VoiceAgent --> STT
    
    BackendAPI --> PostgreSQL
```

## Component Architecture

### 1. Backend API (.NET Core)

**Clean Architecture Pattern**

```mermaid
graph LR
    subgraph presentation [Presentation Layer]
        Controllers[API Controllers]
        DTOs[DTOs & ViewModels]
    end
    
    subgraph application [Application Layer]
        Services[Application Services]
        Interfaces[Service Interfaces]
    end
    
    subgraph domain [Domain Layer]
        Entities[Domain Entities]
        DomainLogic[Business Rules]
    end
    
    subgraph infrastructure [Infrastructure Layer]
        Repositories[Repositories]
        DBContext[EF Core Context]
        External[External Services]
    end
    
    Controllers --> Services
    Services --> Interfaces
    Services --> Entities
    Repositories --> DBContext
    Services --> Repositories
    External --> OpenAI
```

**Project Structure**
```
backend/
├── src/
│   ├── VoiceConcierge.API/              # Presentation Layer
│   │   ├── Controllers/                 # REST endpoints
│   │   ├── DTOs/                        # Data transfer objects
│   │   ├── Middleware/                  # Error handling, logging
│   │   └── Program.cs                   # App configuration
│   │
│   ├── VoiceConcierge.Core/            # Domain + Application Layer
│   │   ├── Domain/
│   │   │   ├── Entities/               # FAQ, UnansweredQuestion, VoiceConfig
│   │   │   └── Interfaces/             # Repository contracts
│   │   ├── Services/                   # Business logic
│   │   │   ├── FAQService
│   │   │   ├── EmbeddingService
│   │   │   ├── SemanticSearchService
│   │   │   └── VoiceService
│   │   └── DTOs/                       # Shared data contracts
│   │
│   └── VoiceConcierge.Infrastructure/  # Infrastructure Layer
│       ├── Data/
│       │   ├── ApplicationDbContext    # EF Core context
│       │   ├── Migrations/             # Database migrations
│       │   └── Repositories/           # Data access implementations
│       └── External/
│           └── OpenAIClient            # OpenAI API integration
```

### 2. Voice Agent (Python)

**Event-Driven Architecture with LiveKit**

```mermaid
graph TD
    RoomManager[Room Manager] --> EventLoop[Event Loop]
    
    EventLoop --> AudioIn[Audio Input Stream]
    EventLoop --> AudioOut[Audio Output Stream]
    
    AudioIn --> STTProcessor[STT Processor]
    STTProcessor --> QuestionHandler[Question Handler]
    
    QuestionHandler --> APIClient[Backend API Client]
    APIClient --> FAQSearch[FAQ Search Endpoint]
    
    FAQSearch --> AnswerFound{Answer Found?}
    
    AnswerFound -->|Yes| LLMFormatter[LLM Response Formatter]
    AnswerFound -->|No| RecordQuestion[Record to Unanswered]
    
    LLMFormatter --> ResponseGenerator[Generate Natural Response]
    RecordQuestion --> ApologyGenerator[Generate Graceful Apology]
    
    ResponseGenerator --> TTSProcessor[TTS Processor]
    ApologyGenerator --> TTSProcessor
    
    TTSProcessor --> AudioOut
```

**Project Structure**
```
voice-agent/
├── agent/
│   ├── main.py                    # Entry point, LiveKit connection
│   ├── conversation_manager.py    # Conversation state and flow
│   ├── question_handler.py        # Question processing logic
│   ├── api_client.py              # Backend API integration
│   ├── llm_service.py             # OpenAI LLM integration
│   ├── voice_config.py            # Voice configuration management
│   └── prompts/
│       ├── system_prompt.py       # Base concierge personality
│       └── response_templates.py  # Response formatting
├── requirements.txt
└── Dockerfile
```

### 3. Admin Panel (React)

**Component-Based SPA Architecture**

```mermaid
graph TD
    App[App Root] --> Router[React Router]
    
    Router --> Layout[Admin Layout]
    
    Layout --> Sidebar[Sidebar Navigation]
    Layout --> Content[Content Area]
    
    Content --> Dashboard[Dashboard Page]
    Content --> FAQPage[FAQ Management]
    Content --> QuestionsPage[Unanswered Questions]
    Content --> VoicePage[Voice Configuration]
    Content --> PlaygroundPage[Integrated Playground]
    
    FAQPage --> FAQList[FAQ List Component]
    FAQPage --> FAQForm[FAQ Form Modal]
    
    QuestionsPage --> QuestionQueue[Question Queue Table]
    QuestionsPage --> ConvertModal[Convert to FAQ Modal]
    
    VoicePage --> VoiceCards[Voice Option Cards]
    VoicePage --> VoicePreview[Audio Preview Player]
    
    PlaygroundPage --> VoiceInterface[LiveKit Voice Interface]
    
    FAQList --> APIService[API Service Layer]
    QuestionQueue --> APIService
    VoiceCards --> APIService
```

**Project Structure**
```
admin-panel/
├── src/
│   ├── components/
│   │   ├── layout/
│   │   │   ├── Sidebar.tsx
│   │   │   └── Header.tsx
│   │   ├── faq/
│   │   │   ├── FAQList.tsx
│   │   │   ├── FAQForm.tsx
│   │   │   └── FAQTable.tsx
│   │   ├── questions/
│   │   │   ├── QuestionQueue.tsx
│   │   │   └── ConvertModal.tsx
│   │   ├── voice/
│   │   │   ├── VoiceCard.tsx
│   │   │   └── VoicePreview.tsx
│   │   └── playground/
│   │       └── VoiceInterface.tsx
│   ├── pages/
│   │   ├── Dashboard.tsx
│   │   ├── FAQManagement.tsx
│   │   ├── UnansweredQuestions.tsx
│   │   ├── VoiceConfiguration.tsx
│   │   └── Playground.tsx
│   ├── services/
│   │   └── api.ts              # Axios client, API calls
│   ├── types/
│   │   └── models.ts           # TypeScript interfaces
│   ├── App.tsx
│   └── main.tsx
```

## Key Design Patterns

### 1. Repository Pattern (Backend)

**Purpose**: Abstract data access, enable testability

```csharp
// Interface in Core
public interface IFAQRepository
{
    Task<List<FAQ>> GetAllAsync();
    Task<FAQ?> GetByIdAsync(Guid id);
    Task<List<SemanticSearchResult>> SearchAsync(float[] embedding, int limit);
    Task<FAQ> CreateAsync(FAQ faq);
    Task UpdateAsync(FAQ faq);
    Task DeleteAsync(Guid id);
}

// Implementation in Infrastructure
public class FAQRepository : IFAQRepository
{
    private readonly ApplicationDbContext _context;
    // Implementation with EF Core
}
```

### 2. Service Layer Pattern (Backend)

**Purpose**: Encapsulate business logic, orchestrate operations

```csharp
public class FAQService
{
    private readonly IFAQRepository _faqRepository;
    private readonly IEmbeddingService _embeddingService;
    
    public async Task<FAQ> CreateFAQAsync(CreateFAQDto dto)
    {
        // Generate embedding
        var embedding = await _embeddingService.GenerateAsync(dto.Question);
        
        // Create entity
        var faq = new FAQ 
        { 
            Question = dto.Question,
            Answer = dto.Answer,
            Embedding = embedding
        };
        
        // Save
        return await _faqRepository.CreateAsync(faq);
    }
}
```

### 3. Event-Driven Pattern (Voice Agent)

**Purpose**: Handle real-time voice interactions asynchronously

```python
@voice_agent.on("track_subscribed")
async def on_track_subscribed(track, participant):
    """Handle incoming audio from guest"""
    audio_stream = AudioInputStream(track)
    
    async for text in speech_to_text(audio_stream):
        response = await handle_question(text)
        await speak_response(response)
```

### 4. Semantic Search Pattern

**Purpose**: Natural language understanding for FAQ matching

**Flow:**
1. Generate embedding for incoming question (OpenAI API)
2. Compute cosine similarity with all FAQ embeddings (pgvector)
3. Return matches above similarity threshold
4. Select best match (lowest distance)

```sql
-- PostgreSQL with pgvector
SELECT 
    id, 
    question, 
    answer,
    embedding <=> $1 as distance
FROM faqs
WHERE embedding <=> $1 < 0.3  -- similarity threshold
ORDER BY distance
LIMIT 1;
```

### 5. Adapter Pattern (External Services)

**Purpose**: Isolate external service dependencies

```csharp
public interface IEmbeddingService
{
    Task<float[]> GenerateEmbeddingAsync(string text);
}

public class OpenAIEmbeddingService : IEmbeddingService
{
    // Can swap for different provider without changing consumers
}
```

## Data Flow Patterns

### FAQ Search Flow

```mermaid
sequenceDiagram
    participant Guest
    participant VoiceAgent
    participant API
    participant DB
    participant OpenAI

    Guest->>VoiceAgent: Speaks question
    VoiceAgent->>VoiceAgent: STT transcription
    VoiceAgent->>API: POST /api/faqs/search {query}
    API->>OpenAI: Generate embedding
    OpenAI-->>API: Embedding vector [1536]
    API->>DB: Semantic search (cosine similarity)
    DB-->>API: Best match or null
    
    alt Answer found
        API-->>VoiceAgent: {answer, confidence}
        VoiceAgent->>OpenAI: Format natural response
        OpenAI-->>VoiceAgent: Conversational text
        VoiceAgent->>VoiceAgent: TTS
        VoiceAgent-->>Guest: Speaks answer
    else No match
        API-->>VoiceAgent: null
        VoiceAgent->>API: POST /api/unanswered-questions
        API->>DB: Insert or increment frequency
        VoiceAgent->>OpenAI: Generate apology
        VoiceAgent->>VoiceAgent: TTS
        VoiceAgent-->>Guest: Speaks apology
    end
```

### Admin FAQ Update Flow

```mermaid
sequenceDiagram
    participant Admin
    participant AdminPanel
    participant API
    participant DB
    participant OpenAI

    Admin->>AdminPanel: Edit FAQ
    AdminPanel->>API: PUT /api/faqs/{id}
    API->>OpenAI: Regenerate embedding
    OpenAI-->>API: New embedding vector
    API->>DB: Update FAQ + embedding
    DB-->>API: Success
    API-->>AdminPanel: Updated FAQ
    AdminPanel-->>Admin: Success notification
```

## Component Communication

### Backend ↔ Voice Agent
- **Protocol**: HTTP REST
- **Format**: JSON
- **Endpoints**: 
  - `POST /api/faqs/search` - Semantic search
  - `POST /api/unanswered-questions` - Record unknown
  - `GET /api/voice-configurations/active` - Get active voice

### Admin Panel ↔ Backend API
- **Protocol**: HTTP REST
- **Format**: JSON
- **Authentication**: None (out of scope)
- **CORS**: Enabled for localhost

### Voice Agent ↔ LiveKit
- **Protocol**: WebRTC
- **Format**: Audio streams
- **Connection**: Persistent room connection

### Voice Agent ↔ OpenAI
- **Protocol**: HTTPS REST
- **Services Used**:
  - Embeddings API (text-embedding-3-small)
  - Chat Completions API (gpt-4 or gpt-3.5-turbo)
  - TTS API or alternative provider

## Deployment Architecture

```mermaid
graph TB
    subgraph docker [Docker Compose Environment]
        subgraph containers [Containers]
            DB[PostgreSQL<br/>Port: 5432]
            API[.NET API<br/>Port: 5000]
            Agent[Voice Agent<br/>Port: 8080]
            Panel[Admin Panel<br/>Port: 3000]
        end
        
        subgraph volumes [Persistent Volumes]
            DBData[postgres-data]
        end
        
        subgraph networks [Networks]
            Internal[voice-concierge-network]
        end
    end
    
    DB --> DBData
    API --> DB
    Agent --> API
    Panel --> API
    
    API -.-> Internal
    Agent -.-> Internal
    Panel -.-> Internal
    DB -.-> Internal
```

## Scalability Considerations

### Current Architecture (Single Instance)
- Suitable for assessment and moderate load
- Single voice agent instance
- Single API instance
- Single database

### Future Scaling Paths
1. **Horizontal Scaling**
   - Multiple voice agent instances (LiveKit handles load balancing)
   - Multiple API instances (behind load balancer)
   - Database read replicas

2. **Caching Layer**
   - Redis for frequent FAQ lookups
   - Cache active voice configuration
   - Cache embeddings (immutable)

3. **Message Queue**
   - Async embedding generation
   - Batch processing for unanswered questions
   - Event streaming for analytics

4. **CDN**
   - Voice preview audio files
   - Admin panel static assets

## Security Patterns

### API Security
- Input validation on all endpoints
- Parameterized queries (SQL injection prevention)
- CORS configuration
- Rate limiting (future)

### Data Security
- Environment variables for secrets
- No hardcoded credentials
- Database connection string in .env
- API keys in .env

### Voice Agent Security
- LiveKit token validation
- Secure WebRTC connections
- No PII logging

## Error Handling Strategy

### Backend API
- Global exception middleware
- Structured error responses
- HTTP status codes (400, 404, 500)
- Detailed logging

### Voice Agent
- Graceful degradation
- Connection retry logic
- Fallback responses
- Error logging without interrupting conversation

### Admin Panel
- Toast notifications for errors
- Form validation
- Loading states
- Retry mechanisms
