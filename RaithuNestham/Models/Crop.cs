namespace RaithuNestham.Models;

public class Crop
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string TeluguName { get; set; } = string.Empty;

    public string Season { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<CropRecord> CropRecords { get; set; }
        = new List<CropRecord>();
}
