namespace RandevuPanel.Models;

public class ServiceNote
{
    public int Id { get; set; }
    public string? VehicleBrand { get; set; }
    public string? VehicleModel { get; set; }
    public string? PhotoPath { get; set; }
    public string? PossibleOperations { get; set; }
    public decimal? TotalCost { get; set; }
    public string? OperationNote { get; set; }
    public bool OperationNotPossible { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public int CreatedByUserId { get; set; }
    public AppUser CreatedByUser { get; set; } = null!;
}
