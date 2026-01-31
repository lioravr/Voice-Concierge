using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace VoiceConcierge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LiveKitController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LiveKitController> _logger;

    public LiveKitController(IConfiguration configuration, ILogger<LiveKitController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Generate LiveKit token (Public - for voice clients)
    /// </summary>
    [HttpPost("token")]
    [AllowAnonymous]
    public IActionResult GetToken([FromBody] TokenRequest request)
    {
        try
        {
            var apiKey = _configuration["LiveKit:ApiKey"];
            var apiSecret = _configuration["LiveKit:ApiSecret"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                _logger.LogError("LiveKit credentials not configured");
                return StatusCode(500, new { error = "LiveKit credentials not configured" });
            }

            var identity = string.IsNullOrEmpty(request.Identity) ? Guid.NewGuid().ToString() : request.Identity;
            var roomName = string.IsNullOrEmpty(request.RoomName) ? "voice-concierge" : request.RoomName;

            var token = GenerateLiveKitToken(apiKey, apiSecret, identity, roomName, request.VoicePreference);

            _logger.LogInformation("Generated LiveKit token for identity: {Identity}, room: {Room}, voicePreference: {Voice}", 
                identity, roomName, request.VoicePreference);

            return Ok(new
            {
                token,
                identity,
                roomName,
                url = _configuration["LiveKit:Url"]
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating LiveKit token");
            return StatusCode(500, new { error = "Failed to generate token" });
        }
    }

    private string GenerateLiveKitToken(string apiKey, string apiSecret, string identity, string roomName, int? voicePreference)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var now = DateTime.UtcNow;
        
        // Use JwtHeader and JwtPayload for proper nested claims
        var headers = new JwtHeader(credentials);
        var payload = new JwtPayload();
        
        // Add standard JWT claims
        payload.Add("exp", new DateTimeOffset(now.AddHours(6)).ToUnixTimeSeconds());
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

public record TokenRequest
{
    public string? Identity { get; init; }
    public string? RoomName { get; init; }
    public int? VoicePreference { get; init; }
}
