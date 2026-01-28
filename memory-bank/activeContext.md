# Active Context: Voice Concierge

## Current Status
**Project Phase**: Phase 2 - Database Design & Setup
**Date**: January 28, 2026
**Status**: PR #1 Merged ✅ | PR #2 Awaiting Review 🔄

## What We Just Did
1. ✅ Created and merged PR #1 (Domain Layer)
   - Added 3 domain entities with Pgvector support
   - Added 3 repository interfaces
   - Core project successfully compiles
2. ✅ Created and pushed PR #2 (Infrastructure Layer)
   - Implemented ApplicationDbContext with pgvector extension
   - Created 3 entity configurations with indexes
   - Implemented all 3 repositories with semantic search
   - Updated Program.cs with EF Core, DI, CORS, health checks
   - Created initial EF Core migration (InitialCreate)
   - Added all required NuGet packages
   - Solution builds successfully (0 errors)

## Current Focus
**Database Layer Complete - Awaiting PR #2 Merge:**
- Infrastructure layer with EF Core fully implemented
- PostgreSQL schema with pgvector configured
- Repository pattern with semantic search operational
- Ready to proceed with service layer and REST API controllers

## Next Immediate Steps

### After PR #2 Merges: Phase 3 - Service Layer & API Controllers

**Step 1: Service Layer Development**
- Create `IEmbeddingService` interface and OpenAI implementation
- Create `ISemanticSearchService` with pgvector integration
- Create `FAQService` with business logic (CRUD + search)
- Create `UnansweredQuestionService` with conversion logic
- Create `VoiceConfigurationService` with activation logic

**Step 2: REST API Controllers**
- `FAQController` - CRUD operations + semantic search endpoint
- `UnansweredQuestionsController` - Queue management + convert to FAQ
- `VoiceConfigurationsController` - List, get active, set active

**Step 3: Seed Data Migration**
- Create second migration with all Meridian Casino information
- Generate embeddings for seed FAQs using OpenAI
- Seed 4 voice personality configurations
- Test database with full dataset

**Step 4: Integration Testing**
- Test semantic search with varied question phrasings
- Test FAQ CRUD operations
- Test unanswered question workflow
- Validate voice configuration activation

## Recent Decisions

### Technology Choices (Confirmed)
✅ **Backend**: .NET Core 8.0 (ASP.NET Core Web API)
✅ **Voice Agent**: Python 3.11+ with LiveKit Agents SDK
✅ **Database**: PostgreSQL 16 with pgvector 0.5+
✅ **LLM**: OpenAI (GPT-4 or GPT-3.5-turbo)
✅ **Embeddings**: OpenAI text-embedding-3-small (1536 dimensions)
✅ **Admin Panel**: React 18 with TypeScript, Vite, TailwindCSS
✅ **Scope**: Full implementation (core + all bonus features)

### Architecture Decisions (Implemented)
✅ **Clean Architecture** for backend - 3 projects (API, Core, Infrastructure)
✅ **Repository Pattern** - Interfaces in Core, implementations in Infrastructure
✅ **Service Layer** for business logic - Next phase
✅ **Semantic Search** with cosine distance - Implemented in FAQRepository
✅ **Event-Driven** voice agent with LiveKit - Planned
✅ **Component-Based** React SPA - Planned

### Implementation Strategy (Active)
✅ **PR-Based Workflow**: Small, focused pull requests
  - PR #1: Domain Layer (Merged)
  - PR #2: Infrastructure Layer (In Review)
  - PR #3: Service Layer & Controllers (Next)
✅ **Build Verification**: Every PR must compile successfully
✅ **Incremental Testing**: Test each layer as built
✅ **Memory Bank Updates**: After major milestones

## Active Questions & Considerations

### Voice Provider Selection
**Decision Needed**: Which TTS/STT service?
- **Option A**: OpenAI Whisper (STT) + OpenAI TTS
  - Pros: Single provider, simpler setup
  - Cons: Voice quality may be limited
- **Option B**: Deepgram (STT) + ElevenLabs (TTS)
  - Pros: Superior voice quality
  - Cons: Multiple providers, more configuration
- **Recommendation**: Start with OpenAI, can swap later

### Semantic Search Threshold
**Decision Needed**: Cosine similarity threshold for "good match"
- Too low (< 0.2): May return irrelevant answers
- Too high (> 0.4): May miss valid matches
- **Recommendation**: Start with 0.3, tune based on testing
- Should be configurable for experimentation

### Voice Personality Mapping
**Decision Needed**: Map voice descriptions to provider voice IDs
- James (British) → Which provider voice?
- Sofia (European) → Which provider voice?
- Marcus (American male) → Which provider voice?
- Elena (American female) → Which provider voice?
- **Action**: Research provider voice options during implementation

### Playground Implementation
**Decision Needed**: Custom vs LiveKit Playground
- **Option A**: Embed LiveKit Agents Playground (iframe)
  - Pros: Fast, pre-built
  - Cons: Less control, potential embedding limitations
- **Option B**: Custom implementation with LiveKit React SDK
  - Pros: Full control, better integration
  - Cons: More development time
- **Recommendation**: Custom for better admin panel integration

## Current Blockers
**None** - PR #2 awaiting user review and merge

### Resolved Issues This Session
1. ✅ Fixed repository implementations to match interface signatures
   - Changed return types from `IEnumerable` to `List`
   - Changed return types from `Task<T>` to `Task` where appropriate
   - Fixed `SearchByEmbeddingAsync` to accept `float[]` and return tuples with distance
2. ✅ Removed health check extension that wasn't compiling
   - Simplified to basic health checks for now
3. ✅ Successfully created EF Core migration with pgvector support

## Context for Next Session

### When Resuming Development
1. Review this activeContext.md for current state
2. Check progress.md for completed items
3. Follow implementation order from systemPatterns.md
4. Reference techContext.md for technical details
5. Validate against requirements in projectbrief.md

### Key Files to Reference
- **Architecture**: `memory-bank/systemPatterns.md`
- **Tech Stack**: `memory-bank/techContext.md`
- **Requirements**: `memory-bank/productContext.md`
- **Progress Tracking**: `memory-bank/progress.md`
- **Implementation Plan**: `.cursor/plans/voice_concierge_implementation_*.plan.md`

### Testing Scenarios to Keep in Mind
As we build, continuously test:
1. Simple FAQ lookup: "What time does the casino open?"
2. Complex question: "I want to propose to my girlfriend"
3. Partner discount: "Are there good restaurants nearby?"
4. Unknown question: "Can I bring my dog?"
5. Voice switching: Test all 4 voice options
6. Admin workflow: Add FAQ → Test in playground immediately

## Notes & Reminders

### Critical Success Factors
- **Voice Quality**: Natural, conversational, luxury brand feel
- **Search Accuracy**: Semantic matching must work well
- **User Experience**: Both guest and admin interfaces intuitive
- **Deployment**: Must work with single docker-compose up command
- **Documentation**: Clear setup instructions for assessment reviewer

### Don't Forget
- Seed database with all Meridian property information from PRD
- Implement graceful fallback for unanswered questions
- Make voice configuration dynamic (no restart required)
- Add proper error handling and logging throughout
- Test end-to-end flow before considering complete

### Code Quality Standards
- Follow SOLID principles
- Write clean, readable code
- Add comments for complex logic
- Consistent naming conventions
- Proper error messages
- Structured logging

## Communication with User

### User Preferences (from conversation)
- Wants **full implementation** (core + bonus)
- Prefers **.NET** for backend
- Chose **PostgreSQL** with pgvector
- Selected **OpenAI** as LLM provider
- Wants clear **PRs and layers** across project - ✅ Implementing
- Values **memory bank** for context preservation - ✅ Maintaining
- Requested to **split Phase 2 into 2 PRs** - ✅ Completed

### Recent User Requests
1. ✅ "Create a memory bank" - Completed
2. ✅ "Create a PR of new files to main" - PR #1 created and merged
3. ✅ "Move to next step" - Started Phase 2 (Database setup)
4. ✅ "Validate compile of the solution" - Verified successful build
5. ✅ "Separate to 2 PRs" - Split into PR #1 (Domain) and PR #2 (Infrastructure)
6. ✅ "Fix compile errors in PR #1" - Fixed and pushed
7. 🔄 "Continue to next step" - Ready for Phase 3 after PR #2 merge
8. ✅ "Update memory bank before continue" - In progress now

**Current Status**: Updating memory bank with Phase 2 progress
**Next**: Phase 3 - Service Layer & REST API Controllers
