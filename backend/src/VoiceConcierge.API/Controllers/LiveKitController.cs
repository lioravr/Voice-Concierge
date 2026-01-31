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
    private readonly ILogger<LiveKitController> _logger;
    private readonly string _liveKitUrl;
    private readonly string _liveKitApiKey;
    private readonly string _defaultRoomName;
    private readonly int _tokenExpirationHours;
    private readonly SigningCredentials _signingCredentials;

    public LiveKitController(IConfiguration configuration, ILogger<LiveKitController> logger)
    {
        _logger = logger;
        
        // Read LiveKit configuration once in constructor for better performance
        var apiSecret = configuration["LiveKit:ApiSecret"] 
            ?? throw new InvalidOperationException("LiveKit:ApiSecret not configured");
        _liveKitApiKey = configuration["LiveKit:ApiKey"] 
            ?? throw new InvalidOperationException("LiveKit:ApiKey not configured");
        _liveKitUrl = configuration["LiveKit:Url"] 
            ?? throw new InvalidOperationException("LiveKit:Url not configured");
        _defaultRoomName = configuration["LiveKit:DefaultRoomName"] ?? "voice-concierge";
        _tokenExpirationHours = int.Parse(configuration["LiveKit:TokenExpirationHours"] ?? "6");
        
        // Pre-compute signing credentials (reused for all tokens)
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiSecret));
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
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
            var identity = string.IsNullOrEmpty(request.Identity) ? Guid.NewGuid().ToString() : request.Identity;
            var roomName = string.IsNullOrEmpty(request.RoomName) ? _defaultRoomName : request.RoomName;

            var token = GenerateLiveKitToken(identity, roomName, request.VoicePreference);

            _logger.LogInformation("Generated LiveKit token for identity: {Identity}, room: {Room}, voicePreference: {Voice}", 
                identity, roomName, request.VoicePreference);

            return Ok(new
            {
                token,
                identity,
                roomName,
                url = _liveKitUrl
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating LiveKit token");
            return StatusCode(500, new { error = "Failed to generate token" });
        }
    }

    private string GenerateLiveKitToken(string identity, string roomName, int? voicePreference)
    {
        var now = DateTime.UtcNow;
        
        // Use JwtHeader and JwtPayload for proper nested claims
        var headers = new JwtHeader(_signingCredentials);
        var payload = new JwtPayload();
        
        // Add standard JWT claims
        payload.Add("exp", new DateTimeOffset(now.AddHours(_tokenExpirationHours)).ToUnixTimeSeconds());
        payload.Add("iss", _liveKitApiKey);
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
