using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoiceConcierge.Core.Domain.Entities;
using VoiceConcierge.Core.DTOs;
using VoiceConcierge.Core.Services;

namespace VoiceConcierge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FAQController : ControllerBase
{
    private readonly IFAQService _faqService;
    private readonly ILogger<FAQController> _logger;

    public FAQController(IFAQService faqService, ILogger<FAQController> logger)
    {
        _faqService = faqService;
        _logger = logger;
    }

    /// <summary>
    /// Get all FAQs (Public - for voice agent and guests)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FAQDto>>> GetAll()
    {
        var faqs = await _faqService.GetAllAsync();
        return Ok(faqs);
    }

    /// <summary>
    /// Get FAQ by ID (Public - for voice agent and guests)
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FAQDto>> GetById(Guid id)
    {
        var faq = await _faqService.GetByIdAsync(id);
        if (faq == null)
        {
            return NotFound();
        }
        return Ok(faq);
    }

    /// <summary>
    /// Search FAQs using semantic similarity (Public - for voice agent and guests)
    /// </summary>
    [HttpPost("search")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FAQSearchResult>>> Search([FromBody] FAQSearchRequest request)
    {
        _logger.LogInformation("Searching FAQs for query: {Query}", request.Query);
        
        var results = await _faqService.SearchAsync(
            request.Query,
            request.Limit,
            request.Threshold
        );
        
        _logger.LogInformation("Found {Count} results for query: {Query}", results.Count, request.Query);
        
        return Ok(results);
    }

    /// <summary>
    /// Create a new FAQ (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<FAQDto>> Create([FromBody] CreateFAQDto dto)
    {
        _logger.LogInformation("Creating new FAQ: {Question}", dto.Question);
        
        var faq = await _faqService.CreateAsync(dto);
        
        return CreatedAtAction(nameof(GetById), new { id = faq.Id }, faq);
    }

    /// <summary>
    /// Update an existing FAQ (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<FAQDto>> Update(Guid id, [FromBody] UpdateFAQDto dto)
    {
        try
        {
            _logger.LogInformation("Updating FAQ {Id}", id);
            
            var faq = await _faqService.UpdateAsync(id, dto);
            
            return Ok(faq);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete an FAQ (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deleting FAQ {Id}", id);
        
        await _faqService.DeleteAsync(id);
        
        return NoContent();
    }
}
