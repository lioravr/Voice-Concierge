using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.DTOs;
using VoiceConcierge.Core.Services;

namespace VoiceConcierge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnansweredQuestionsController : ControllerBase
{
    private readonly IUnansweredQuestionService _questionService;
    private readonly ILogger<UnansweredQuestionsController> _logger;

    public UnansweredQuestionsController(
        IUnansweredQuestionService questionService,
        ILogger<UnansweredQuestionsController> logger)
    {
        _questionService = questionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all pending unanswered questions (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<List<UnansweredQuestionDto>>> GetAllPending()
    {
        var questions = await _questionService.GetAllPendingAsync();
        return Ok(questions);
    }

    /// <summary>
    /// Get unanswered question by ID (Admin only)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UnansweredQuestionDto>> GetById(Guid id)
    {
        var question = await _questionService.GetByIdAsync(id);
        if (question == null)
        {
            return NotFound();
        }
        return Ok(question);
    }

    /// <summary>
    /// Record a new unanswered question (Public - for voice agent)
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<UnansweredQuestionDto>> Record([FromBody] RecordQuestionRequest request)
    {
        _logger.LogInformation("Recording unanswered question: {Question}", request.Question);
        
        var question = await _questionService.RecordAsync(request.Question);
        
        return CreatedAtAction(nameof(GetById), new { id = question.Id }, question);
    }

    /// <summary>
    /// Convert an unanswered question to an FAQ (Admin only)
    /// </summary>
    [HttpPost("{id}/convert")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<FAQDto>> ConvertToFAQ(Guid id, [FromBody] ConvertToFAQRequest request)
    {
        try
        {
            _logger.LogInformation("Converting unanswered question {Id} to FAQ", id);
            
            var faq = await _questionService.ConvertToFAQAsync(id, request.Answer, request.Category);
            
            return Ok(faq);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Dismiss an unanswered question (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Dismiss(Guid id)
    {
        _logger.LogInformation("Dismissing unanswered question {Id}", id);
        
        await _questionService.DismissAsync(id);
        
        return NoContent();
    }
}
