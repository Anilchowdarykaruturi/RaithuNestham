namespace RaithuNestham.Services.Interfaces;

public interface IAIService
{
    Task<string> AskAsync(
        int farmerId,
        string question);
}
