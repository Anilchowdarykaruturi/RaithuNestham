namespace RaithuNestham.Models;

public class CropRecord
{
    public int Id { get; set; }

    public int FieldId { get; set; }

    public int CropId { get; set; }

    public DateTime PlantingDate { get; set; }

    public DateTime? HarvestDate { get; set; }

    public decimal? ExpectedYield { get; set; }

    public Field? Field { get; set; }

    public Crop? Crop { get; set; }
}
