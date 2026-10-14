namespace RaithuNestham.DTOs.AI;

public class AIChatRequest
{
    public int FarmerId { get; set; }

    public string Question { get; set; } = string.Empty;
}