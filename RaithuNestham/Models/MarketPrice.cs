
namespace RaithuNestham.Models;

public class MarketPrice
{
    public int Id { get; set; }

    public string CropName { get; set; } = string.Empty;

    public string CropNameTelugu { get; set; } = string.Empty;

    public string State { get; set; } = "Andhra Pradesh";

    public string District { get; set; } = string.Empty;

    public string MarketName { get; set; } = string.Empty;

    public decimal MinPrice { get; set; }

    public decimal MaxPrice { get; set; }

    public decimal ModalPrice { get; set; }

    public string PriceUnit { get; set; } = "Quintal";

    public DateTime ReportedDate { get; set; }

    public string DataSource { get; set; } = string.Empty;

    public DateTime LastUpdatedUtc { get; set; }
}
