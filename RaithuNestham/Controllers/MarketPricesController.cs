
using Microsoft.AspNetCore.Mvc;
using RaithuNestham.Services;

namespace RaithuNestham.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketPricesController : ControllerBase
{
    private readonly MarketPriceService _marketPriceService;

    public MarketPricesController(
        MarketPriceService marketPriceService)
    {
        _marketPriceService = marketPriceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPrices(
        [FromQuery] string? crop,
        [FromQuery] string? district,
        [FromQuery] DateTime? reportedDate,
        CancellationToken cancellationToken)
    {
        var prices = await _marketPriceService.GetPricesAsync(
            crop,
            district,
            reportedDate,
            cancellationToken);

        return Ok(new
        {
            State = "Andhra Pradesh",
            Count = prices.Count,
            Prices = prices
        });
    }
}
