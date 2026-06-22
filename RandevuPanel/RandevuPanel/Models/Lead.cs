namespace RandevuPanel.Models;

public enum LeadStatus
{
    Yeni,
    Arandı,
    RandevuyaDonustu,
    İlgilenmedi
}

public class Lead
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? VehicleBrand { get; set; }
    public string? VehicleModel { get; set; }
    public int? VehicleYear { get; set; }
    public ContactPlatform ContactPlatform { get; set; }
    public string ProcessDescription { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public LeadStatus Status { get; set; } = LeadStatus.Yeni;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public int CreatedByUserId { get; set; }
    public AppUser CreatedByUser { get; set; } = null!;
}
