using RandevuPanel.Models;

namespace RandevuPanel.ViewModels.Appointments;

public class AppointmentFilterViewModel
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? VehicleBrand { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public ContactPlatform? ContactPlatform { get; set; }
    public AppointmentStatus? Status { get; set; }
}
