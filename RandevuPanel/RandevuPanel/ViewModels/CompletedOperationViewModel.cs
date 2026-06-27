namespace RandevuPanel.ViewModels;

public class CompletedOperationViewModel
{
    public int Id { get; set; }
    public string? VehicleBrand { get; set; }
    public string? VehicleModel { get; set; }
    public int? VehicleYear { get; set; }
    public string? CustomerName { get; set; }
    public string? LicensePlate { get; set; }
    public string? OperationDescription { get; set; }
    public DateOnly OperationDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public string? Notes { get; set; }
}
