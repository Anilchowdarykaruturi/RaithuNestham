
using Microsoft.EntityFrameworkCore;
using RaithuNestham.Data;
using RaithuNestham.Models;

namespace RaithuNestham.Services;

public class MarketPriceService
{
    private readonly ApplicationDbContext _dbContext;

    public MarketPriceService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<MarketPrice>> GetPricesAsync(
        string? crop,
        string? district,
        DateTime? reportedDate,
        CancellationToken cancellationToken = default)
    {
        IQueryable<MarketPrice> query =
            _dbContext.MarketPrices.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(crop))
        {
            query = query.Where(p => p.CropName == crop);
        }

        if (!string.IsNullOrWhiteSpace(district))
        {
            query = query.Where(p => p.District == district);
        }

        if (reportedDate.HasValue)
        {
            var date = reportedDate.Value.Date;
            var nextDate = date.AddDays(1);

            query = query.Where(p =>
                p.ReportedDate >= date &&
                p.ReportedDate < nextDate);
        }

        return await query
            .OrderByDescending(p => p.ReportedDate)
            .ThenBy(p => p.CropName)
            .ToListAsync(cancellationToken);
    }
}
