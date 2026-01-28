# Project Brief: Voice Concierge for The Meridian Casino & Resort

## Project Name
Voice Concierge System

## Project Type
Software Engineer Take-Home Assessment - Full-Stack Voice AI Application

## Core Purpose
Build a voice-based concierge system that allows guests of The Meridian Casino & Resort to ask questions and receive instant, spoken answers about property amenities, services, and information 24/7.

## Problem Statement
- Concierge desk and phone lines are overwhelmed with repetitive questions
- Guests expect instant answers at any time (including 3 AM)
- No visibility into questions the property cannot answer
- Missing opportunities to improve service based on guest inquiries

## Primary Goals

### Core Goals (Required)
1. Provide 24/7 instant voice support for common guest questions
2. Capture unanswered questions for later review and analysis
3. Maintain luxury, personalized feel expected from The Meridian brand
4. Provide playground interface to test the voice concierge

### Bonus Goals (Optional - We're implementing ALL)
1. Allow staff to manage FAQ content through admin panel
2. Review and convert unanswered questions to FAQs
3. Configure voice personality through admin interface
4. Integrate playground into admin panel

## Target Users

### Primary: Guests
- Hotel guests or casino visitors
- Need quick, accurate information without waiting
- Expect polished, professional luxury experience
- Ask about gaming, dining, accommodations, entertainment, partners

### Secondary: Concierge Manager
- Manages FAQ content and accuracy
- Reviews unanswered questions
- Adds new information based on inquiries
- Configures voice personality to match brand
- Tests concierge before deploying changes

## Success Criteria

### Core Requirements
- Natural voice conversation about property information
- Accurate FAQ lookup with semantic search
- Records unanswered questions to database
- Web-based playground for testing
- Feels like speaking with knowledgeable, professional concierge

### Bonus Requirements
- Admin can manage FAQ items (CRUD operations)
- Admin can review and convert unanswered questions
- Admin can choose between 4 distinct voices
- Admin can preview voices before selecting
- Admin can test full voice experience within admin panel

## Key Constraints
- Must run locally via Docker with single command
- Backend: .NET, Java, Node.js, or Python
- Voice Agent: Python or JavaScript/TypeScript
- Voice Infrastructure: LiveKit (required)
- Admin Panel: React (for bonus)
- LLM: OpenAI (our choice)
- Database: PostgreSQL with pgvector (our choice)

## Out of Scope
- Actual reservation booking (information only)
- Payment processing
- Integration with property management systems
- User authentication for admin panel
- Guest-facing standalone website
- Mobile app
- Multi-language support

## Deliverables

### Core Deliverables
- ✅ Working voice agent (answers FAQ questions)
- ✅ Backend API (FAQ search, unanswered question recording)
- ✅ Database seeded with Meridian property information
- ✅ Playground interface for testing
- ✅ Single command deployment (docker-compose up)
- ✅ README with setup instructions
- ✅ Technical decisions document

### Bonus Deliverables (All included)
- ✅ React admin panel with FAQ management
- ✅ Unanswered questions queue with convert-to-FAQ
- ✅ Voice configuration UI (4 voices + preview)
- ✅ Integrated playground in admin panel

## Project Timeline
Take-home assessment - implement at your own pace with comprehensive coverage

## Contact
- Email: jenya@rocketdreams.co
- Phone: +972 54-524-1285
