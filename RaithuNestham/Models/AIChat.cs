namespace RaithuNestham.Models;

public class AIChat
{
    public int Id { get; set; }

    public int FarmerId { get; set; }

    public string Question { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Farmer? Farmer { get; set; }
}