namespace RaithuNestham.Models;

public class Fertilizer
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string RecommendedFor { get; set; } = string.Empty;

    public string UsageInstructions { get; set; } = string.Empty;

    public bool IsOrganic { get; set; }
}