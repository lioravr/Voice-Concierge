# Voice Concierge Admin Panel

React-based admin panel for managing the Voice Concierge system at The Meridian Casino & Resort.

## Features

### 1. FAQ Management
- ✅ **CRUD Operations**: Create, Read, Update, Delete FAQs
- ✅ **Semantic Search**: Test search with similarity scores
- ✅ **Category Organization**: Group FAQs by category
- ✅ **Real-time Updates**: Automatic list refresh after changes

### 2. Unanswered Questions Queue
- ✅ **Question Monitoring**: View all questions the agent couldn't answer
- ✅ **Frequency Tracking**: See how many times each question was asked
- ✅ **Convert to FAQ**: Quickly turn unanswered questions into FAQs
- ✅ **Dismiss**: Remove irrelevant questions
- ✅ **Auto-refresh**: Updates every 30 seconds

### 3. Voice Configuration
- ✅ **Voice Personalities**: View all available voice options
- ✅ **Active Voice Display**: See which voice is currently active
- ✅ **One-click Activation**: Change voice personality instantly
- ✅ **Voice Details**: Gender, accent, provider voice ID

### 4. Playground
- ✅ **Semantic Search Testing**: Test how the agent searches FAQs
- ✅ **Similarity Scores**: See match percentages for results
- ✅ **Response Preview**: See how the agent would respond
- ✅ **Active Voice Info**: Know which voice would speak the answer

## Technology Stack

- **React 18**: Modern UI library
- **TypeScript**: Type safety
- **Vite**: Fast build tool
- **React Router**: Navigation
- **TanStack Query**: Server state management
- **Axios**: HTTP client
- **Zustand**: Client state (if needed)
- **Tailwind CSS**: Utility-first styling
- **Lucide React**: Icon library
- **Sonner**: Toast notifications
- **React Hook Form**: Form management
- **Zod**: Schema validation

## Installation

### Prerequisites
- Node.js 18+ (or compatible runtime)
- npm or yarn or pnpm

### Setup

```bash
# Navigate to admin-panel directory
cd admin-panel

# Install dependencies
npm install

# Create environment file
cp .env.example .env

# Update .env with your backend API URL
# VITE_API_URL=http://localhost:5000/api

# Run development server
npm run dev
```

The admin panel will be available at `http://localhost:5173`

## Development

```bash
# Start dev server with hot reload
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Lint code
npm run lint
```

## Docker

```bash
# Build image
docker build -t voice-concierge-admin .

# Run container
docker run -p 3000:80 voice-concierge-admin

# Or use docker-compose
docker-compose up admin-panel
```

## Project Structure

```
admin-panel/
├── src/
│   ├── components/         # Reusable UI components
│   ├── pages/              # Page components
│   │   ├── FAQsPage.tsx
│   │   ├── UnansweredQuestionsPage.tsx
│   │   ├── VoiceConfigurationPage.tsx
│   │   └── PlaygroundPage.tsx
│   ├── layouts/            # Layout components
│   │   └── MainLayout.tsx
│   ├── hooks/              # Custom React hooks
│   │   ├── useFAQs.ts
│   │   ├── useUnansweredQuestions.ts
│   │   └── useVoiceConfigurations.ts
│   ├── lib/                # Utilities
│   │   └── api.ts          # API client
│   ├── types/              # TypeScript types
│   │   └── index.ts
│   ├── App.tsx             # Root component with router
│   └── main.tsx            # Entry point
├── public/                 # Static assets
├── package.json
├── vite.config.ts
├── tailwind.config.js
└── README.md
```

## Pages

### FAQs Page (`/`)
**Purpose:** Manage the FAQ knowledge base

**Features:**
- View all FAQs organized by category
- Search FAQs with semantic search
- Create new FAQ with question, answer, and optional category
- Edit existing FAQ
- Delete FAQ with confirmation
- See similarity scores during search

**Usage:**
1. Click "Add FAQ" to create a new entry
2. Use search bar to test semantic matching
3. Click edit icon to modify an FAQ
4. Click delete icon to remove an FAQ

### Unanswered Questions Page (`/unanswered`)
**Purpose:** Review and process questions the agent couldn't answer

**Features:**
- View all pending unanswered questions
- Sort by frequency (most asked first)
- Convert question to FAQ with custom answer
- Dismiss irrelevant questions
- See statistics (total questions, total asks, average frequency)

**Usage:**
1. Review questions sorted by frequency
2. Click "Convert to FAQ" to create an FAQ from the question
3. Provide answer and optional category
4. Or click "Dismiss" to remove the question

### Voice Configuration Page (`/voices`)
**Purpose:** Manage voice personalities for the agent

**Features:**
- View all 4 voice personality options
- See detailed descriptions, gender, accent
- View OpenAI provider voice ID
- Activate a voice with one click
- See currently active voice highlighted

**Available Voices:**
1. **James** - Male, British, mature and warm
2. **Sofia** - Female, European, friendly and elegant
3. **Marcus** - Male, American, confident and energetic
4. **Elena** - Female, American, calm and sophisticated

**Usage:**
1. Review voice options and descriptions
2. Click "Activate This Voice" on desired personality
3. Changes take effect immediately
4. Test in Playground before activating

### Playground Page (`/playground`)
**Purpose:** Test semantic search and preview agent responses

**Features:**
- Enter any question to test search
- See top matching FAQs with similarity scores
- Preview how the agent would respond
- Understand the conversation flow
- See which voice is active

**Usage:**
1. Type a question like "What time does the casino open?"
2. Click "Search FAQs"
3. Review matching results with similarity percentages
4. See simulated agent response
5. Use to verify FAQ quality and coverage

## API Integration

The admin panel connects to the .NET backend API:

**Endpoints Used:**
- `GET /api/faq` - List all FAQs
- `POST /api/faq` - Create FAQ
- `PUT /api/faq/{id}` - Update FAQ
- `DELETE /api/faq/{id}` - Delete FAQ
- `POST /api/faq/search` - Semantic search

- `GET /api/unansweredquestions` - List pending questions
- `POST /api/unansweredquestions/{id}/convert` - Convert to FAQ
- `DELETE /api/unansweredquestions/{id}` - Dismiss question

- `GET /api/voiceconfigurations` - List voices
- `GET /api/voiceconfigurations/active` - Get active voice
- `PUT /api/voiceconfigurations/{voiceId}/activate` - Set active voice

## Environment Variables

Create a `.env` file (use `.env.example` as template):

```env
# Backend API URL
VITE_API_URL=http://localhost:5000/api
```

For production, set this to your deployed backend URL.

## Deployment

### Production Build

```bash
npm run build
```

Outputs to `dist/` directory.

### Docker Deployment

The admin panel uses nginx to serve the static files:

```dockerfile
# Multi-stage build
FROM node:18 AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
```

### Environment Variables in Docker

Set `VITE_API_URL` at build time:

```bash
docker build --build-arg VITE_API_URL=https://api.meridian.example.com/api .
```

Or update `docker-compose.yml`:

```yaml
environment:
  VITE_API_URL: http://backend:8080/api
```

## Troubleshooting

### API Connection Issues

**Symptom:** "Failed to fetch" errors

**Solutions:**
1. Check backend is running: `curl http://localhost:5000/health`
2. Verify `VITE_API_URL` in `.env`
3. Check CORS configuration in backend
4. Inspect browser network tab for errors

### Build Errors

**Symptom:** TypeScript or build errors

**Solutions:**
1. Delete `node_modules` and reinstall: `rm -rf node_modules && npm install`
2. Clear Vite cache: `rm -rf .vite`
3. Check Node.js version: `node --version` (should be 18+)

### Docker Issues

**Symptom:** Container won't start or crashes

**Solutions:**
1. Check logs: `docker logs voice-concierge-admin`
2. Verify nginx configuration
3. Ensure dist/ folder was created during build
4. Check port conflicts

## Future Enhancements

- [ ] Real-time LiveKit playground with actual voice testing
- [ ] Analytics dashboard (most asked questions, response times)
- [ ] Bulk FAQ import/export (CSV)
- [ ] FAQ versioning and history
- [ ] Multi-language support
- [ ] User authentication and role-based access
- [ ] Advanced filtering and sorting
- [ ] FAQ templates
- [ ] Response time monitoring

## License

Part of The Meridian Voice Concierge project.
