# Seed Data

This directory contains JSON files with seed data for The Meridian Casino & Resort.

## Files

### `voices.json`
Contains voice configuration data for the voice concierge system.

**Structure:**
```json
[
  {
    "voiceId": 1,
    "name": "James",
    "description": "Voice description",
    "gender": "Male",
    "accent": "British",
    "providerVoiceId": "onyx"
  }
]
```

**Fields:**
- `voiceId`: Unique identifier (integer)
- `name`: Display name of the voice
- `description`: Description of voice characteristics
- `gender`: Male/Female
- `accent`: Accent type (British, American, European, etc.)
- `providerVoiceId`: OpenAI TTS voice ID (e.g., "onyx", "nova", "shimmer", "echo", "alloy", "fable")

**Available OpenAI Voices:**
- `alloy` - Neutral
- `echo` - Male, American
- `fable` - British accent
- `onyx` - Deep, male
- `nova` - Female, warm
- `shimmer` - Female, gentle

### `faqs.json`
Contains frequently asked questions and answers about the resort.

**Structure:**
```json
[
  {
    "category": "Gaming",
    "question": "What time does the casino open?",
    "answer": "The Meridian Casino is open 24 hours..."
  }
]
```

**Fields:**
- `category`: Category name (Gaming, Dining, Services, Amenities, Entertainment, General, Accommodations)
- `question`: The FAQ question (used to generate embeddings for semantic search)
- `answer`: The detailed answer

**Categories:**
- `Gaming`: Casino floor, poker room, table games, slots
- `Dining`: Restaurants, lounges, room service
- `Services`: Concierge, check-in/out, parking, celebrations
- `Amenities`: Pool, spa, fitness center, business center
- `Entertainment`: Shows, events, attractions
- `General`: Address, phone, policies, resort fee
- `Accommodations`: Room types, amenities, WiFi

## How It Works

1. **Startup:** When the backend starts, `DatabaseSeeder.cs` checks if the database is empty
2. **Load JSON:** If empty, it loads data from these JSON files via `MeridianSeedData.cs`
3. **Generate Embeddings:** For each FAQ, it generates an AI embedding using OpenAI
4. **Store:** Data is saved to PostgreSQL with vector embeddings for semantic search

## Editing Data

### To Add a New FAQ:
1. Open `faqs.json`
2. Add a new object with `category`, `question`, and `answer`
3. Restart the backend or use the Admin Panel to add it through the UI

### To Add a New Voice:
1. Open `voices.json`
2. Add a new object with all required fields
3. Choose a valid OpenAI voice ID for `providerVoiceId`
4. Restart the backend

### To Update Existing Data:
1. Edit the JSON files directly
2. Delete the database or use the reseed endpoint (if implemented)
3. Restart the backend to load fresh data

## Notes

- JSON files must be valid JSON format
- The backend caches the loaded data, so changes require a restart
- FAQ embeddings are generated only on first load (takes ~5-10 seconds for all FAQs)
- Voice with `voiceId: 1` is set as the default active voice
