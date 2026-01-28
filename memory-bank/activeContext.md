# Active Context: Voice Concierge

## Current Status
**Project Phase**: Initial Setup - Memory Bank Creation
**Date**: January 28, 2026
**Status**: Planning Complete, Ready to Begin Implementation

## What We Just Did
1. ✅ Read and analyzed complete PRD document
2. ✅ Created comprehensive implementation plan
3. ✅ Established memory bank structure
4. ✅ Documented architecture and technical decisions

## Current Focus
**Creating foundation for implementation:**
- Setting up project structure and repository layout
- Defining clear architectural boundaries and layers
- Establishing development workflow and patterns

## Next Immediate Steps

### Step 1: Project Structure Setup
Create the complete directory structure for all components:
- Backend API (.NET solution with 3 projects)
- Voice Agent (Python application)
- Admin Panel (React application)
- Docker configuration files
- Documentation files

### Step 2: Docker Compose Configuration
Set up orchestration for entire system:
- PostgreSQL with pgvector
- .NET backend API
- Python voice agent
- React admin panel
- Network configuration
- Volume management

### Step 3: Environment Configuration
Create configuration files:
- `.env.example` with all required variables
- `.gitignore` to protect secrets
- Docker environment variable mapping
- README.md with setup instructions

### Step 4: Database Foundation
Initialize database schema:
- Create EF Core DbContext
- Define domain entities
- Create initial migration with pgvector
- Design seed data structure

## Recent Decisions

### Technology Choices
✅ **Backend**: .NET Core 8.0 (chosen by user)
✅ **Voice Agent**: Python 3.11+ with LiveKit
✅ **Database**: PostgreSQL 16 with pgvector
✅ **LLM**: OpenAI (GPT-4 or GPT-3.5)
✅ **Admin Panel**: React 18 with TypeScript
✅ **Scope**: Full implementation (core + all bonus features)

### Architecture Decisions
✅ **Clean Architecture** for backend (.NET)
✅ **Repository Pattern** for data access
✅ **Service Layer** for business logic
✅ **Semantic Search** with OpenAI embeddings + pgvector cosine similarity
✅ **Event-Driven** voice agent with LiveKit
✅ **Component-Based** React SPA for admin

### Implementation Strategy
✅ **Single Command Deployment**: docker-compose up
✅ **Development First**: Build locally, containerize after
✅ **Incremental Testing**: Test each component as built
✅ **Documentation Alongside**: Write docs during development

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
**None** - Ready to begin implementation

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
- Wants clear **PRs and layers** across project
- Values **memory bank** for context preservation

### Current User Request
"Let's start with clear PRs and layers across this project. First I want from you to create a memory bank to this solution."

**Status**: ✅ Memory bank created
**Next**: Await user confirmation to proceed with implementation
