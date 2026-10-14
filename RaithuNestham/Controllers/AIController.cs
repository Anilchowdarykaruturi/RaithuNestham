
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaithuNestham.DTOs.AI;
using RaithuNestham.Services.Interfaces;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;
    private readonly ILogger<AIController> _logger;

    public AIController(
        IAIService aiService,
        ILogger<AIController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(AIChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(new
            {
                message = "Question is required."
            });
        }

        try
        {
            var answer = await _aiService.AskAsync(
                request.FarmerId,
                request.Question);

            return Ok(new
            {
                question = request.Question,
                answer
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "AI chat failed for farmer {FarmerId}",
                request.FarmerId);

            return StatusCode(500, new
            {
                message = "AI chat failed. Check the backend logs."
            });
        }
    }
}
