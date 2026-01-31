using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VoiceConcierge.Core.Constants;
using VoiceConcierge.Core.DTOs;
using VoiceConcierge.Core.Helpers;

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
        var apiSecret = configuration[ConfigurationKeys.LiveKit.ApiSecret] 
            ?? throw new InvalidOperationException("LiveKit:ApiSecret not configured");
        _liveKitApiKey = configuration[ConfigurationKeys.LiveKit.ApiKey] 
            ?? throw new InvalidOperationException("LiveKit:ApiKey not configured");
        _liveKitUrl = configuration[ConfigurationKeys.LiveKit.Url] 
            ?? throw new InvalidOperationException("LiveKit:Url not configured");
        _defaultRoomName = configuration[ConfigurationKeys.LiveKit.DefaultRoomName] ?? "voice-concierge";
        _tokenExpirationHours = int.Parse(configuration[ConfigurationKeys.LiveKit.TokenExpirationHours] ?? "6");
        
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

            var token = LiveKitTokenHelper.GenerateToken(
                _signingCredentials,
                _liveKitApiKey,
                identity,
                roomName,
                _tokenExpirationHours,
                request.VoicePreference);

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
}
