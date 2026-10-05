namespace RaithuNestham.Models;

public class GovernmentScheme
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Eligibility { get; set; } = string.Empty;

    public string Benefits { get; set; } = string.Empty;

    public string ApplicationProcess { get; set; } = string.Empty;

    public string OfficialWebsite { get; set; } = string.Empty;
}