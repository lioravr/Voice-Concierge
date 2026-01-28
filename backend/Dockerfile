# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY VoiceConcierge.sln ./
COPY src/VoiceConcierge.API/VoiceConcierge.API.csproj src/VoiceConcierge.API/
COPY src/VoiceConcierge.Core/VoiceConcierge.Core.csproj src/VoiceConcierge.Core/
COPY src/VoiceConcierge.Infrastructure/VoiceConcierge.Infrastructure.csproj src/VoiceConcierge.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build and publish
WORKDIR /src/src/VoiceConcierge.API
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "VoiceConcierge.API.dll"]
