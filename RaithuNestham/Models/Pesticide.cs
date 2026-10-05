namespace RaithuNestham.Models;

public class Pesticide
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string TargetPest { get; set; } = string.Empty;

    public string RecommendedCrop { get; set; } = string.Empty;

    public string UsageInstructions { get; set; } = string.Empty;
}