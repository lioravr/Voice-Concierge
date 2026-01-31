using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceConcierge.Core.Constants;
using VoiceConcierge.Core.DTOs;
using VoiceConcierge.Core.Services;

namespace VoiceConcierge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VoiceConfigurationsController : ControllerBase
{
    private readonly IVoiceConfigurationService _voiceService;
    private readonly ILogger<VoiceConfigurationsController> _logger;

    public VoiceConfigurationsController(
        IVoiceConfigurationService voiceService,
        ILogger<VoiceConfigurationsController> logger)
    {
        _voiceService = voiceService;
        _logger = logger;
    }

    /// <summary>
    /// Get all voice configurations (Public - for voice agent)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VoiceConfigurationDto>>> GetAll()
    {
        var voices = await _voiceService.GetAllAsync();
        return Ok(voices);
    }

    /// <summary>
    /// Get the currently active voice configuration (Public - for voice agent)
    /// </summary>
    [HttpGet("active")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VoiceConfigurationDto>> GetActive()
    {
        var voice = await _voiceService.GetActiveAsync();
        if (voice == null)
        {
            return NotFound(new { message = "No active voice configuration found" });
        }
        return Ok(voice);
    }

    /// <summary>
    /// Generate a preview audio sample for a specific voice (Public - for admins to preview)
    /// </summary>
    [HttpGet("{voiceId:int}/preview")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVoicePreview(int voiceId)
    {
        _logger.LogInformation("Generating preview for voice {VoiceId}", voiceId);
        
        var audioData = await _voiceService.GeneratePreviewAsync(voiceId);
        if (audioData == null)
        {
            return NotFound(new { message = "Voice configuration not found" });
        }
        
        return File(audioData, "audio/mpeg", $"voice-preview-{voiceId}.mp3");
    }

    /// <summary>
    /// Get voice configuration by voice ID (1-4)
    /// </summary>
    [HttpGet("{voiceId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VoiceConfigurationDto>> GetByVoiceId(int voiceId)
    {
        var voice = await _voiceService.GetByVoiceIdAsync(voiceId);
        if (voice == null)
        {
            return NotFound();
        }
        return Ok(voice);
    }

    /// <summary>
    /// Set a voice as active (Admin only)
    /// </summary>
    [HttpPut("{voiceId:int}/activate")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SetActive(int voiceId)
    {
        _logger.LogInformation("Setting voice {VoiceId} as active", voiceId);
        
        await _voiceService.SetActiveAsync(voiceId);
        
        return NoContent();
    }
}
