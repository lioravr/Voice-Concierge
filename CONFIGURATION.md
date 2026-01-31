# Configuration Guide

## Best Practices

This project follows .NET configuration best practices with a layered approach:

### Configuration Hierarchy

1. **appsettings.json** - Base configuration with empty values (no secrets)
2. **appsettings.Development.json** - Local development defaults (not deployed)
3. **Environment Variables** - Runtime configuration (Docker, Production)
4. **.env file** - Local environment variables (not committed to git)

### Configuration Sources Priority (lowest to highest)

```
appsettings.json → appsettings.Development.json → Environment Variables → Command Line Args
```

## Running Locally

### Option 1: Docker Compose (Recommended)

Docker Compose automatically uses environment variables from your `.env` file:

```bash
# Copy example file
cp .env.example .env

# Edit .env with your actual values
nano .env

# Run
docker compose up -d
```

**Configuration**: Environment variables override everything (defined in `docker-compose.yml`)

### Option 2: Local Development (dotnet run)

For running the backend directly with `dotnet run`:

```bash
cd backend/src/VoiceConcierge.API
dotnet run
```

**Configuration**: `appsettings.Development.json` provides defaults for local development

## Configuration Files

### appsettings.json (Production Base)

- **Purpose**: Base configuration for all environments
- **Content**: Empty values, no secrets, no hardcoded hosts
- **Version Control**: ✅ Committed to git

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "OpenAI": {
    "ApiKey": ""
  }
}
```

### appsettings.Development.json (Local Development)

- **Purpose**: Local development defaults when running outside Docker
- **Content**: Localhost connection strings, placeholder API keys
- **Version Control**: ✅ Committed to git (no real secrets)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=voice_concierge;Username=postgres;Password=postgres"
  }
}
```

### .env (Environment Variables)

- **Purpose**: Actual secrets and configuration
- **Content**: Real API keys, passwords, URLs
- **Version Control**: ❌ NOT committed (in .gitignore)

```bash
DB_PASSWORD=your_secure_password
OPENAI_API_KEY=sk-real-key-here
LIVEKIT_API_SECRET=real-secret-here
```

## Docker Configuration

Docker Compose uses environment variables and overrides the connection string:

```yaml
environment:
  ConnectionStrings__DefaultConnection: Host=postgres;Port=5432;Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}
  OpenAI__ApiKey: ${OPENAI_API_KEY}
  LiveKit__ApiKey: ${LIVEKIT_API_KEY}
```

**Note**: Double underscore `__` is used for nested configuration in .NET

## Security Best Practices

✅ **DO:**
- Use environment variables for secrets
- Keep `.env` in `.gitignore`
- Use empty values in `appsettings.json`
- Document required variables in `.env.example`

❌ **DON'T:**
- Commit real secrets to git
- Hardcode connection strings in appsettings.json
- Use production credentials in development files

## Environment Variable Format

### .NET Configuration

Use double underscore `__` for nested sections:

```bash
ConnectionStrings__DefaultConnection="Host=db;Port=5432;..."
OpenAI__ApiKey="sk-..."
LiveKit__Url="wss://..."
```

### Docker Compose

Can use both formats:

```yaml
# Method 1: Nested
ConnectionStrings__DefaultConnection: "Host=..."

# Method 2: Direct (for simpler values)
OPENAI_API_KEY: ${OPENAI_API_KEY}
```

## Required Configuration

See `.env.example` for a complete list of required variables:

- Database: `DB_NAME`, `DB_USER`, `DB_PASSWORD`
- OpenAI: `OPENAI_API_KEY`
- LiveKit: `LIVEKIT_URL`, `LIVEKIT_API_KEY`, `LIVEKIT_API_SECRET`

## Troubleshooting

### "Connection string is null or empty"

Check that environment variables are set:

```bash
# Docker: Check container env
docker compose exec backend env | grep ConnectionStrings

# Local: Check your .env file exists
cat .env
```

### Configuration not loading

Check the order of precedence:
1. Environment variables always win
2. Development settings override base settings
3. Base settings are the fallback

### Docker not reading .env

Ensure `.env` is in the same directory as `docker-compose.yml`:

```bash
ls -la .env docker-compose.yml
```
