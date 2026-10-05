namespace RaithuNestham.Models;

public class EquipmentSubsidy
{
    public int Id { get; set; }

    public string EquipmentName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string SchemeName { get; set; } = string.Empty;

    public string EligibleFarmers { get; set; } = string.Empty;

    public string SubsidyDetails { get; set; } = string.Empty;

    public string MaximumSubsidy { get; set; } = string.Empty;

    public string ApplicationProcess { get; set; } = string.Empty;

    public string OfficialWebsite { get; set; } = string.Empty;

    public string State { get; set; } = "Andhra Pradesh";

    public string LastVerified { get; set; } = string.Empty;
}
