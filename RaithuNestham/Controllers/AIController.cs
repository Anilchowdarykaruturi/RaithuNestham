using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaithuNestham.DTOs.AI;
using RaithuNestham.Services.Interfaces;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIController : ControllerBase
{
    private readonly IAIService _aiService;

    public AIController(IAIService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        AIChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest(new
            {
                message = "Question is required."
            });
        }

        var answer = await _aiService.AskAsync(
            request.FarmerId,
            request.Question);

        return Ok(new
        {
            question = request.Question,
            answer
        });
    }
}