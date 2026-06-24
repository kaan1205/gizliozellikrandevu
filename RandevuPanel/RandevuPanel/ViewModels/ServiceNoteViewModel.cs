using Microsoft.AspNetCore.Http;

namespace RandevuPanel.ViewModels;

public class ServiceNoteViewModel
{
    public int Id { get; set; }
    public string? VehicleBrand { get; set; }
    public string? VehicleModel { get; set; }
    public IFormFile? Photo { get; set; }
    public string? ExistingPhotoPath { get; set; }
    public string? PossibleOperations { get; set; }
    public decimal? TotalCost { get; set; }
    public string? OperationNote { get; set; }
    public bool OperationNotPossible { get; set; }
    public DateTime CreatedAt { get; set; }
}
