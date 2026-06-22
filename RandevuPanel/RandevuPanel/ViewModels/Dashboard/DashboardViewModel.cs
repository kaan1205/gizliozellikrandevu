using RandevuPanel.Models;

namespace RandevuPanel.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TodayCount { get; set; }
    public int UpcomingCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }

    public List<Appointment> TodayAppointments { get; set; } = new();
    public List<Appointment> ApproachingAppointments { get; set; } = new();
}
