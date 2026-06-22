using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RandevuPanel.Data;
using RandevuPanel.Models;
using RandevuPanel.Services;
using RandevuPanel.ViewModels.Leads;

namespace RandevuPanel.Controllers;

public class LeadsController : BaseController
{
    private readonly AppDbContext _db;

    public LeadsController(IAppointmentService appointmentService, AppDbContext db)
        : base(appointmentService)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(LeadStatus? status)
    {
        var query = _db.Leads.Where(l => l.CreatedByUserId == CurrentUserId);
        if (status.HasValue)
            query = query.Where(l => l.Status == status.Value);

        var leads = await query.OrderByDescending(l => l.CreatedAt).ToListAsync();
        ViewBag.StatusFilter = status;
        return View(leads);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new LeadCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeadCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var lead = new Lead
        {
            CustomerName = model.CustomerName,
            VehicleBrand = model.VehicleBrand,
            VehicleModel = model.VehicleModel,
            VehicleYear = model.VehicleYear,
            ContactPlatform = model.ContactPlatform,
            ProcessDescription = model.ProcessDescription,
            Notes = model.Notes,
            CreatedByUserId = CurrentUserId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _db.Leads.Add(lead);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Potansiyel müşteri eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, LeadStatus status)
    {
        var lead = await _db.Leads.FirstOrDefaultAsync(l => l.Id == id && l.CreatedByUserId == CurrentUserId);
        if (lead == null) return Json(new { success = false });

        lead.Status = status;
        lead.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return Json(new { success = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var lead = await _db.Leads.FirstOrDefaultAsync(l => l.Id == id && l.CreatedByUserId == CurrentUserId);
        if (lead != null)
        {
            _db.Leads.Remove(lead);
            await _db.SaveChangesAsync();
        }
        TempData["Success"] = "Kayıt silindi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ConvertToAppointment(int id)
    {
        var lead = await _db.Leads.FirstOrDefaultAsync(l => l.Id == id && l.CreatedByUserId == CurrentUserId);
        if (lead == null) return NotFound();

        return RedirectToAction("Create", "Appointments", new
        {
            customerName = lead.CustomerName,
            vehicleBrand = lead.VehicleBrand,
            vehicleModel = lead.VehicleModel,
            vehicleYear = lead.VehicleYear,
            contactPlatform = (int)lead.ContactPlatform,
            processDescription = lead.ProcessDescription,
            notes = lead.Notes,
            fromLeadId = lead.Id
        });
    }
}
