namespace RaithuNestham.Models;

public class Field
{
    public int Id { get; set; }

    public int FarmerId { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public decimal AreaInAcres { get; set; }

    public string SoilType { get; set; } = string.Empty;

    public string IrrigationType { get; set; } = string.Empty;

    public Farmer? Farmer { get; set; }

    public ICollection<CropRecord> CropRecords { get; set; }
        = new List<CropRecord>();
}
