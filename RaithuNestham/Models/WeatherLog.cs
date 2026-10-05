namespace RaithuNestham.Models;

public class WeatherLog
{
    public int Id { get; set; }

    public string Village { get; set; } = string.Empty;

    public decimal Temperature { get; set; }

    public decimal Humidity { get; set; }

    public decimal Rainfall { get; set; }

    public string WeatherCondition { get; set; } = string.Empty;

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}