namespace RaithuNestham.Models;

public class Farmer
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Village { get; set; } = string.Empty;

    public string Mandal { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public string State { get; set; } = "Andhra Pradesh";

    public User? User { get; set; }

    public ICollection<Field> Fields { get; set; } = new List<Field>();
}
