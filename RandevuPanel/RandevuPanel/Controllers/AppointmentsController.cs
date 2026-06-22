using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RandevuPanel.Data;
using RandevuPanel.Models;
using RandevuPanel.Services;
using RandevuPanel.ViewModels.Appointments;

namespace RandevuPanel.Controllers;

public class AppointmentsController : BaseController
{
    private readonly IAppointmentService _appointmentService;
    private readonly AppDbContext _db;

    public AppointmentsController(IAppointmentService appointmentService, AppDbContext db) : base(appointmentService)
    {
        _appointmentService = appointmentService;
        _db = db;
    }

    public async Task<IActionResult> Index(AppointmentFilterViewModel filter)
    {
        var appointments = await _appointmentService.GetFilteredAsync(filter, CurrentUserId);
        var vm = new AppointmentListViewModel
        {
            Appointments = appointments,
            Filter = filter
        };
        return View(vm);
    }

    public IActionResult Calendar()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> CalendarEvents(string start, string end)
    {
        var from = DateOnly.Parse(start[..10]);
        var to = DateOnly.Parse(end[..10]);

        var appointments = await _appointmentService.GetCalendarEventsAsync(CurrentUserId, from, to);

        var events = appointments.Select(a => new
        {
            id = a.Id,
            title = $"{a.AppointmentTime:HH:mm} - {a.CustomerName} ({a.VehicleBrand} {a.VehicleModel})",
            start = $"{a.AppointmentDate:yyyy-MM-dd}T{a.AppointmentTime:HH:mm:ss}",
            color = a.Status switch
            {
                AppointmentStatus.Bekliyor => "#0d6efd",
                AppointmentStatus.Geldi => "#ffc107",
                AppointmentStatus.Tamamlandi => "#198754",
                AppointmentStatus.IptalEdildi => "#dc3545",
                _ => "#6c757d"
            },
            extendedProps = new
            {
                status = a.Status.ToString(),
                customerPhone = a.CustomerPhone,
                process = a.ProcessDescription,
                appointmentId = a.Id
            }
        });

        return Json(events);
    }

    [HttpGet]
    public IActionResult Create(string? customerName, string? vehicleBrand, string? vehicleModel,
        int? vehicleYear, int? contactPlatform, string? processDescription, string? notes, int? fromLeadId)
    {
        var vm = new AppointmentCreateViewModel
        {
            CustomerName = customerName,
            VehicleBrand = vehicleBrand ?? string.Empty,
            VehicleModel = vehicleModel ?? string.Empty,
            VehicleYear = vehicleYear,
            ProcessDescription = processDescription ?? string.Empty,
            Notes = notes
        };
        if (contactPlatform.HasValue && Enum.IsDefined(typeof(RandevuPanel.Models.ContactPlatform), contactPlatform.Value))
            vm.ContactPlatform = (RandevuPanel.Models.ContactPlatform)contactPlatform.Value;
        if (fromLeadId.HasValue)
            ViewBag.FromLeadId = fromLeadId.Value;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AppointmentCreateViewModel model, int? fromLeadId)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _appointmentService.CreateAsync(model, CurrentUserId);

        if (fromLeadId.HasValue)
        {
            var lead = await _db.Leads.FirstOrDefaultAsync(l => l.Id == fromLeadId.Value && l.CreatedByUserId == CurrentUserId);
            if (lead != null)
            {
                lead.Status = LeadStatus.RandevuyaDonustu;
                lead.UpdatedAt = DateTime.Now;
                await _db.SaveChangesAsync();
            }
        }

        TempData["Success"] = "Randevu başarıyla eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id, CurrentUserId);
        if (appointment == null) return NotFound();

        var vm = new AppointmentEditViewModel
        {
            Id = appointment.Id,
            AppointmentDate = appointment.AppointmentDate,
            AppointmentTime = appointment.AppointmentTime,
            VehicleBrand = appointment.VehicleBrand,
            VehicleModel = appointment.VehicleModel,
            VehicleYear = appointment.VehicleYear,
            VinNumber = appointment.VinNumber,
            ProcessDescription = appointment.ProcessDescription,
            ContactPlatform = appointment.ContactPlatform,
            CustomerName = appointment.CustomerName,
            CustomerPhone = appointment.CustomerPhone,
            Notes = appointment.Notes,
            Status = appointment.Status
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AppointmentEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _appointmentService.UpdateAsync(id, model);
        TempData["Success"] = "Randevu başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Detail(int id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id, CurrentUserId);
        if (appointment == null) return NotFound();
        return View(appointment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _appointmentService.DeleteAsync(id, CurrentUserId);
        TempData["Success"] = "Randevu silindi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, AppointmentStatus status)
    {
        await _appointmentService.UpdateStatusAsync(id, status, CurrentUserId);
        return Json(new { success = true });
    }
}
