using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RandevuPanel.Data;
using RandevuPanel.Models;
using RandevuPanel.Services;
using RandevuPanel.ViewModels;

namespace RandevuPanel.Controllers;

public class CompletedOperationsController : BaseController
{
    private readonly AppDbContext _db;

    public CompletedOperationsController(IAppointmentService appointmentService, AppDbContext db)
        : base(appointmentService)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? search, DateOnly? dateFrom, DateOnly? dateTo)
    {
        var query = _db.CompletedOperations.Where(o => o.CreatedByUserId == CurrentUserId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(o =>
                (o.CustomerName != null && o.CustomerName.Contains(search)) ||
                (o.VehicleBrand != null && o.VehicleBrand.Contains(search)) ||
                (o.VehicleModel != null && o.VehicleModel.Contains(search)) ||
                (o.OperationDescription != null && o.OperationDescription.Contains(search)));

        if (dateFrom.HasValue)
            query = query.Where(o => o.OperationDate >= dateFrom.Value);
        if (dateTo.HasValue)
            query = query.Where(o => o.OperationDate <= dateTo.Value);

        var list = await query.OrderByDescending(o => o.OperationDate).ThenByDescending(o => o.CreatedAt).ToListAsync();

        ViewBag.Search = search;
        ViewBag.DateFrom = dateFrom?.ToString("yyyy-MM-dd");
        ViewBag.DateTo = dateTo?.ToString("yyyy-MM-dd");
        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CompletedOperationViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CompletedOperationViewModel model)
    {
        var op = new CompletedOperation
        {
            VehicleBrand = model.VehicleBrand,
            VehicleModel = model.VehicleModel,
            VehicleYear = model.VehicleYear,
            CustomerName = model.CustomerName,
            OperationDescription = model.OperationDescription,
            OperationDate = model.OperationDate,
            TotalCost = model.TotalCost,
            Notes = model.Notes,
            CreatedByUserId = CurrentUserId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _db.CompletedOperations.Add(op);
        await _db.SaveChangesAsync();
        TempData["Success"] = "İşlem kaydedildi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var op = await _db.CompletedOperations.FirstOrDefaultAsync(o => o.Id == id && o.CreatedByUserId == CurrentUserId);
        if (op == null) return NotFound();

        return View(new CompletedOperationViewModel
        {
            Id = op.Id,
            VehicleBrand = op.VehicleBrand,
            VehicleModel = op.VehicleModel,
            VehicleYear = op.VehicleYear,
            CustomerName = op.CustomerName,
            OperationDescription = op.OperationDescription,
            OperationDate = op.OperationDate,
            TotalCost = op.TotalCost,
            Notes = op.Notes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CompletedOperationViewModel model)
    {
        var op = await _db.CompletedOperations.FirstOrDefaultAsync(o => o.Id == id && o.CreatedByUserId == CurrentUserId);
        if (op == null) return NotFound();

        op.VehicleBrand = model.VehicleBrand;
        op.VehicleModel = model.VehicleModel;
        op.VehicleYear = model.VehicleYear;
        op.CustomerName = model.CustomerName;
        op.OperationDescription = model.OperationDescription;
        op.OperationDate = model.OperationDate;
        op.TotalCost = model.TotalCost;
        op.Notes = model.Notes;
        op.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        TempData["Success"] = "İşlem güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var op = await _db.CompletedOperations.FirstOrDefaultAsync(o => o.Id == id && o.CreatedByUserId == CurrentUserId);
        if (op != null)
        {
            _db.CompletedOperations.Remove(op);
            await _db.SaveChangesAsync();
        }
        TempData["Success"] = "Kayıt silindi.";
        return RedirectToAction(nameof(Index));
    }
}
