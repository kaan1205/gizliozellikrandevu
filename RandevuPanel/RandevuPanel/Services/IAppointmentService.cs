using RandevuPanel.Models;
using RandevuPanel.ViewModels.Appointments;
using RandevuPanel.ViewModels.Dashboard;

namespace RandevuPanel.Services;

public interface IAppointmentService
{
    Task<List<Appointment>> GetFilteredAsync(AppointmentFilterViewModel filter, int userId);
    Task<Appointment?> GetByIdAsync(int id, int userId);
    Task CreateAsync(AppointmentCreateViewModel vm, int userId);
    Task UpdateAsync(int id, AppointmentEditViewModel vm);
    Task DeleteAsync(int id, int userId);
    Task UpdateStatusAsync(int id, AppointmentStatus status, int userId);
    Task<DashboardViewModel> GetDashboardDataAsync(int userId);
    Task<List<Appointment>> GetByDateAsync(DateOnly date, int userId);
    Task<List<Appointment>> GetCalendarEventsAsync(int userId, DateOnly from, DateOnly to);
}
