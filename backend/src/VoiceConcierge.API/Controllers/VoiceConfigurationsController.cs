using Microsoft.AspNetCore.Mvc;
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
    /// Get all voice configurations
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VoiceConfigurationDto>>> GetAll()
    {
        var voices = await _voiceService.GetAllAsync();
        return Ok(voices);
    }

    /// <summary>
    /// Get the currently active voice configuration
    /// </summary>
    [HttpGet("active")]
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
    /// Set a voice as active (deactivates all others)
    /// </summary>
    [HttpPut("{voiceId:int}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetActive(int voiceId)
    {
        _logger.LogInformation("Setting voice {VoiceId} as active", voiceId);
        
        await _voiceService.SetActiveAsync(voiceId);
        
        return NoContent();
    }
}
