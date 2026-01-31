using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace VoiceConcierge.Core.Helpers;

/// <summary>
/// Helper class for generating LiveKit access tokens
/// </summary>
public static class LiveKitTokenHelper
{
    /// <summary>
    /// Generates a LiveKit JWT token with the specified parameters
    /// </summary>
    /// <param name="signingCredentials">Pre-computed signing credentials for JWT</param>
    /// <param name="apiKey">LiveKit API key (used as issuer)</param>
    /// <param name="identity">User identity for the session</param>
    /// <param name="roomName">LiveKit room name</param>
    /// <param name="tokenExpirationHours">Token expiration time in hours</param>
    /// <param name="voicePreference">Optional voice configuration preference ID</param>
    /// <returns>Encoded JWT token string</returns>
    public static string GenerateToken(
        SigningCredentials signingCredentials,
        string apiKey,
        string identity,
        string roomName,
        int tokenExpirationHours,
        int? voicePreference = null)
    {
        var now = DateTime.UtcNow;
        
        // Use JwtHeader and JwtPayload for proper nested claims
        var headers = new JwtHeader(signingCredentials);
        var payload = new JwtPayload();
        
        // Add standard JWT claims
        payload.Add("exp", new DateTimeOffset(now.AddHours(tokenExpirationHours)).ToUnixTimeSeconds());
        payload.Add("iss", apiKey);
        payload.Add("nbf", new DateTimeOffset(now).ToUnixTimeSeconds());
        payload.Add("sub", identity);
        payload.Add("name", identity);
        
        // Add video grants as Dictionary (not serialized string!)
        var videoGrants = new Dictionary<string, object>
        {
            { "canPublish", true },
            { "canPublishData", true },
            { "canSubscribe", true },
            { "room", roomName },
            { "roomJoin", true }
        };
        payload.Add("video", videoGrants);
        
        // Add metadata if voice preference is set
        if (voicePreference.HasValue)
        {
            var metadata = new Dictionary<string, object>
            {
                { "voice_preference", voicePreference.Value }
            };
            payload.Add("metadata", JsonSerializer.Serialize(metadata));
        }
        
        var token = new JwtSecurityToken(headers, payload);
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(token);
    }
}
