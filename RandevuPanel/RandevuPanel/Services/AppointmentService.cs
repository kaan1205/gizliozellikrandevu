using Microsoft.EntityFrameworkCore;
using RandevuPanel.Data;
using RandevuPanel.Models;
using RandevuPanel.ViewModels.Appointments;
using RandevuPanel.ViewModels.Dashboard;

namespace RandevuPanel.Services;

public class AppointmentService : IAppointmentService
{
    private readonly AppDbContext _db;

    public AppointmentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Appointment>> GetFilteredAsync(AppointmentFilterViewModel filter, int userId)
    {
        var query = _db.Appointments
            .Where(a => a.CreatedByUserId == userId)
            .AsQueryable();

        if (filter.DateFrom.HasValue)
            query = query.Where(a => a.AppointmentDate >= DateOnly.FromDateTime(filter.DateFrom.Value));

        if (filter.DateTo.HasValue)
            query = query.Where(a => a.AppointmentDate <= DateOnly.FromDateTime(filter.DateTo.Value));

        if (!string.IsNullOrWhiteSpace(filter.VehicleBrand))
            query = query.Where(a => a.VehicleBrand.Contains(filter.VehicleBrand));

        if (!string.IsNullOrWhiteSpace(filter.CustomerName))
            query = query.Where(a => a.CustomerName.Contains(filter.CustomerName));

        if (!string.IsNullOrWhiteSpace(filter.CustomerPhone))
            query = query.Where(a => a.CustomerPhone.Contains(filter.CustomerPhone));

        if (filter.ContactPlatform.HasValue)
            query = query.Where(a => a.ContactPlatform == filter.ContactPlatform.Value);

        if (filter.Status.HasValue)
            query = query.Where(a => a.Status == filter.Status.Value);

        return await query
            .OrderByDescending(a => a.AppointmentDate)
            .ThenByDescending(a => a.AppointmentTime)
            .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAsync(int id, int userId)
    {
        return await _db.Appointments
            .Include(a => a.CreatedByUser)
            .FirstOrDefaultAsync(a => a.Id == id && a.CreatedByUserId == userId);
    }

    public async Task CreateAsync(AppointmentCreateViewModel vm, int userId)
    {
        var appointment = new Appointment
        {
            AppointmentDate = vm.AppointmentDate,
            AppointmentTime = vm.AppointmentTime,
            VehicleBrand = vm.VehicleBrand,
            VehicleModel = vm.VehicleModel,
            VinNumber = vm.VinNumber,
            ProcessDescription = vm.ProcessDescription,
            ContactPlatform = vm.ContactPlatform,
            CustomerName = vm.CustomerName,
            CustomerPhone = vm.CustomerPhone,
            Notes = vm.Notes,
            Status = AppointmentStatus.Bekliyor,
            CreatedByUserId = userId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, AppointmentEditViewModel vm)
    {
        var appointment = await _db.Appointments.FindAsync(id);
        if (appointment == null) return;

        appointment.AppointmentDate = vm.AppointmentDate;
        appointment.AppointmentTime = vm.AppointmentTime;
        appointment.VehicleBrand = vm.VehicleBrand;
        appointment.VehicleModel = vm.VehicleModel;
        appointment.VinNumber = vm.VinNumber;
        appointment.ProcessDescription = vm.ProcessDescription;
        appointment.ContactPlatform = vm.ContactPlatform;
        appointment.CustomerName = vm.CustomerName;
        appointment.CustomerPhone = vm.CustomerPhone;
        appointment.Notes = vm.Notes;
        appointment.Status = vm.Status;
        appointment.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, int userId)
    {
        var appointment = await _db.Appointments
            .FirstOrDefaultAsync(a => a.Id == id && a.CreatedByUserId == userId);

        if (appointment != null)
        {
            _db.Appointments.Remove(appointment);
            await _db.SaveChangesAsync();
        }
    }

    public async Task UpdateStatusAsync(int id, AppointmentStatus status, int userId)
    {
        var appointment = await _db.Appointments
            .FirstOrDefaultAsync(a => a.Id == id && a.CreatedByUserId == userId);

        if (appointment != null)
        {
            appointment.Status = status;
            appointment.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync(int userId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var now = TimeOnly.FromDateTime(DateTime.Now);
        var in7Days = today.AddDays(7);

        var allAppointments = await _db.Appointments
            .Where(a => a.CreatedByUserId == userId)
            .ToListAsync();

        var todayList = allAppointments
            .Where(a => a.AppointmentDate == today)
            .OrderBy(a => a.AppointmentTime)
            .ToList();

        var approaching = todayList
            .Where(a => a.AppointmentTime > now && a.AppointmentTime <= now.AddHours(1)
                     && a.Status == AppointmentStatus.Bekliyor)
            .ToList();

        return new DashboardViewModel
        {
            TodayCount = todayList.Count,
            UpcomingCount = allAppointments.Count(a => a.AppointmentDate > today && a.AppointmentDate <= in7Days),
            CompletedCount = allAppointments.Count(a => a.Status == AppointmentStatus.Tamamlandi),
            CancelledCount = allAppointments.Count(a => a.Status == AppointmentStatus.IptalEdildi),
            TodayAppointments = todayList,
            ApproachingAppointments = approaching
        };
    }

    public async Task<List<Appointment>> GetByDateAsync(DateOnly date, int userId)
    {
        return await _db.Appointments
            .Where(a => a.CreatedByUserId == userId && a.AppointmentDate == date)
            .OrderBy(a => a.AppointmentTime)
            .ToListAsync();
    }

    public async Task<List<Appointment>> GetCalendarEventsAsync(int userId, DateOnly from, DateOnly to)
    {
        return await _db.Appointments
            .Where(a => a.CreatedByUserId == userId
                     && a.AppointmentDate >= from
                     && a.AppointmentDate <= to)
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.AppointmentTime)
            .ToListAsync();
    }
}
