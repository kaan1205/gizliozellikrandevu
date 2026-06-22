using RandevuPanel.Models;

namespace RandevuPanel.ViewModels.Appointments;

public class AppointmentListViewModel
{
    public List<Appointment> Appointments { get; set; } = new();
    public AppointmentFilterViewModel Filter { get; set; } = new();
}
