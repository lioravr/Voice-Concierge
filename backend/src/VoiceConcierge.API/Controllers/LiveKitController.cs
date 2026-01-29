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

        var videoGrants = new
        {
            room = roomName,
            roomJoin = true,
            canPublish = true,
            canSubscribe = true,
            canPublishData = true
        };

        var metadata = new Dictionary<string, object>();
        if (voicePreference.HasValue)
        {
            metadata["voice_preference"] = voicePreference.Value;
        }

        var claims = new List<Claim>
        {
            new Claim("sub", identity),
            new Claim("name", identity),
            new Claim("video", JsonSerializer.Serialize(videoGrants), JsonClaimValueTypes.Json)
        };

        // Add metadata claim if we have voice preference
        if (metadata.Count > 0)
        {
            claims.Add(new Claim("metadata", JsonSerializer.Serialize(metadata), JsonClaimValueTypes.Json));
        }

        var tokenDescriptor = new JwtSecurityToken(
            issuer: apiKey,
            audience: apiKey,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: credentials
        );

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(tokenDescriptor);
    }
}

public record TokenRequest
{
    public string? Identity { get; init; }
    public string? RoomName { get; init; }
    public int? VoicePreference { get; init; }
}
