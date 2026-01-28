# Product Context: Voice Concierge

## Why This Exists

### Business Problem
The Meridian Casino & Resort, a luxury Las Vegas property, faces operational challenges:
- **Overwhelmed Staff**: Concierge desk and phone lines flooded with repetitive questions
- **24/7 Expectations**: Guests need instant answers at 3 AM when they want to know if poker room is open
- **Blind Spots**: No visibility into what guests are asking that staff cannot answer
- **Service Gaps**: Missing opportunities to improve service based on actual guest needs

### Business Value
- **Cost Reduction**: Deflect routine inquiries from human staff
- **Guest Satisfaction**: Instant answers without wait times
- **Service Intelligence**: Capture unanswered questions to identify gaps
- **Brand Consistency**: Maintain luxury hospitality standards through AI personality
- **Scalability**: Handle unlimited concurrent guest inquiries

## Problems It Solves

### For Guests
- ❌ **Before**: Wait on hold or walk to concierge desk for simple questions
- ✅ **After**: Ask voice concierge instantly from anywhere, get immediate answers

- ❌ **Before**: Limited by concierge desk hours or availability
- ✅ **After**: 24/7 access to property information

- ❌ **Before**: May not know what to ask or where to find information
- ✅ **After**: Natural conversation with intelligent search

### For Concierge Staff
- ❌ **Before**: Repeat same answers hundreds of times daily
- ✅ **After**: Focus on complex, high-value guest interactions

- ❌ **Before**: No data on what information guests need
- ✅ **After**: Queue of unanswered questions reveals service gaps

- ❌ **Before**: Cannot update property information easily
- ✅ **After**: Admin panel to manage FAQ content in real-time

### For Management
- ❌ **Before**: No metrics on guest information needs
- ✅ **After**: Analytics on question frequency and coverage gaps

- ❌ **Before**: Brand voice inconsistency across staff
- ✅ **After**: Configurable, consistent luxury hospitality tone

## How It Should Work

### Guest Experience Flow

```
Guest visits property → Opens playground interface → Speaks question
                                                              ↓
Voice concierge understands natural language ← Semantic search of FAQ database
                                                              ↓
                                    ┌─────────────────────────┴───────────────────────┐
                                    │                                                 │
                            Answer Found?                                    No Match Found
                                    │                                                 │
            Responds naturally with accurate information          Records question for review
                                    │                                                 │
                            Conversational, warm tone                    Apologizes gracefully
                                    │                                                 │
                            "Our poker room is open                "I don't have that information,
                             24/7, with tournaments                but I've noted your question.
                             at 11 AM and 7 PM..."                 You can reach our desk at
                                                                    extension 0..."
```

### Admin Experience Flow

```
Staff logs into admin panel
            │
    ┌───────┴─────────┬────────────────┬──────────────────┐
    │                 │                │                  │
FAQ Management   Unanswered Queue   Voice Config    Test Playground
    │                 │                │                  │
Add/Edit/Delete  View by frequency  Choose voice      Full conversation
FAQ items        Convert to FAQ     Preview voices    Testing with current
                 Dismiss irrelevant Set active        configuration
```

### Technical Flow

```
Voice Agent (Python/LiveKit)
            ↓
    STT (Speech to Text)
            ↓
    Question transcribed
            ↓
Backend API (.NET) ← FAQ Search Request
            ↓
Semantic Search (PostgreSQL + pgvector)
            ↓
    OpenAI Embeddings
            ↓
┌───────────┴────────────┐
│                        │
Match Found          No Match
    ↓                    ↓
Return answer      Record to DB
    ↓                    ↓
LLM (OpenAI GPT)   Return null
    ↓
Format natural response
    ↓
TTS (Text to Speech)
    ↓
Guest hears answer
```

## User Experience Goals

### Voice Personality
**Brand Alignment**: Luxury hospitality - warm, professional, refined
- **Tone**: Welcoming, sophisticated, helpful
- **Style**: Conversational but polished
- **Pacing**: Clear, unhurried, confident
- **Language**: Elegant without being stuffy

### Voice Options (4 Personalities)
1. **James**: Male, mature, warm British accent - professional and refined
2. **Sofia**: Female, friendly, subtle European accent - welcoming and elegant
3. **Marcus**: Male, American, confident and energetic - modern and approachable
4. **Elena**: Female, American, calm and reassuring - sophisticated and clear

### Conversation Quality
- **Natural**: Feels like talking to a knowledgeable person, not a robot
- **Contextual**: Understands intent, not just keywords
- **Comprehensive**: Provides complete information without overwhelming
- **Graceful Fallbacks**: When answer unknown, handles elegantly

### Example Conversations

**Scenario 1: Simple Query**
> Guest: "Is the poker room open right now?"
> 
> Concierge: "Yes, our poker room is open 24 hours a day, 7 days a week. We offer Texas Hold'em, Omaha, and Seven Card Stud. There's also a tournament starting at 7 PM tonight with a $200 buy-in if you're interested."

**Scenario 2: Complex Request**
> Guest: "I want to propose to my girlfriend this weekend. Can you help?"
> 
> Concierge: "How exciting! We have several special options. Our celebration packages start at $500 and include a room upgrade, champagne, and dinner credit. For something more elaborate, Eclipse Lounge offers private terrace reservations with stunning Strip views. I'd recommend speaking with our concierge desk to plan the perfect moment — would you like their direct number?"

**Scenario 3: Unknown Information**
> Guest: "Can I bring my dog to the hotel?"
> 
> Concierge: "I don't have information about pet policies at the moment, but I've noted your question for our team. You can also reach our front desk directly at extension 0. Is there anything else I can help you with?"

## Key Features

### 1. Semantic FAQ Search
- Not keyword matching - understands meaning
- "What time does casino open?" = "When can I gamble?" = "Casino hours?"
- Uses OpenAI embeddings + cosine similarity
- Configurable similarity threshold

### 2. Unanswered Question Intelligence
- Automatically detects when no good match exists
- Records question with timestamp
- Increments frequency for duplicate questions
- Enables data-driven FAQ expansion

### 3. Admin Content Management
- CRUD operations for FAQ entries
- Real-time updates (no system restart)
- Category organization
- Search and filter capabilities

### 4. Voice Personality Configuration
- 4 distinct voice options
- Preview before selecting
- Instant activation (applies to new conversations)
- Matches brand guidelines

### 5. Integrated Testing
- Playground embedded in admin panel
- Tests with live configuration
- Full voice conversation capability
- Validates changes before guest exposure

## Success Metrics (Subjective Assessment)

### Voice Quality
- ✅ Sounds natural and conversational
- ✅ Appropriate pacing and tone
- ✅ Clearly understandable
- ✅ Matches luxury brand expectations

### Search Accuracy
- ✅ Finds correct answers for varied phrasings
- ✅ Minimal false positives
- ✅ Appropriate "no answer" threshold

### User Experience
- ✅ Admin panel intuitive and efficient
- ✅ Playground easy to use
- ✅ Voice configuration straightforward
- ✅ FAQ management seamless

### Technical Performance
- ✅ Voice response latency < 3 seconds
- ✅ No audio glitches or interruptions
- ✅ Stable connection throughout conversation
- ✅ Single command deployment works flawlessly
