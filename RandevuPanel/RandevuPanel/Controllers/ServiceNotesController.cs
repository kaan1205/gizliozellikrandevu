using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RandevuPanel.Data;
using RandevuPanel.Models;
using RandevuPanel.Services;
using RandevuPanel.ViewModels;

namespace RandevuPanel.Controllers;

public class ServiceNotesController : BaseController
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ServiceNotesController(IAppointmentService appointmentService, AppDbContext db, IWebHostEnvironment env)
        : base(appointmentService)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var notes = await _db.ServiceNotes
            .Where(n => n.CreatedByUserId == CurrentUserId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return View(notes);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ServiceNoteViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceNoteViewModel model)
    {
        string? photoPath = null;
        if (model.Photo != null && model.Photo.Length > 0)
            photoPath = await SavePhoto(model.Photo);

        var note = new ServiceNote
        {
            VehicleBrand = model.OperationNotPossible ? null : model.VehicleBrand,
            VehicleModel = model.OperationNotPossible ? null : model.VehicleModel,
            VehicleYear = model.OperationNotPossible ? null : model.VehicleYear,
            PhotoPath = model.OperationNotPossible ? null : photoPath,
            PossibleOperations = model.OperationNotPossible ? null : model.PossibleOperations,
            TotalCost = model.OperationNotPossible ? null : model.TotalCost,
            OperationNote = model.OperationNotPossible ? null : model.OperationNote,
            OperationNotPossible = model.OperationNotPossible,
            CreatedByUserId = CurrentUserId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _db.ServiceNotes.Add(note);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Not eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var note = await _db.ServiceNotes.FirstOrDefaultAsync(n => n.Id == id && n.CreatedByUserId == CurrentUserId);
        if (note == null) return NotFound();

        var model = new ServiceNoteViewModel
        {
            Id = note.Id,
            VehicleBrand = note.VehicleBrand,
            VehicleModel = note.VehicleModel,
            VehicleYear = note.VehicleYear,
            ExistingPhotoPath = note.PhotoPath,
            PossibleOperations = note.PossibleOperations,
            TotalCost = note.TotalCost,
            OperationNote = note.OperationNote,
            OperationNotPossible = note.OperationNotPossible,
            CreatedAt = note.CreatedAt
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceNoteViewModel model)
    {
        var note = await _db.ServiceNotes.FirstOrDefaultAsync(n => n.Id == id && n.CreatedByUserId == CurrentUserId);
        if (note == null) return NotFound();

        string? photoPath = note.PhotoPath;
        if (!model.OperationNotPossible && model.Photo != null && model.Photo.Length > 0)
        {
            if (photoPath != null)
                DeletePhoto(photoPath);
            photoPath = await SavePhoto(model.Photo);
        }

        note.OperationNotPossible = model.OperationNotPossible;
        if (model.OperationNotPossible)
        {
            note.VehicleBrand = null;
            note.VehicleModel = null;
            note.PhotoPath = null;
            note.PossibleOperations = null;
            note.TotalCost = null;
            note.OperationNote = null;
        }
        else
        {
            note.VehicleBrand = model.VehicleBrand;
            note.VehicleModel = model.VehicleModel;
            note.VehicleYear = model.VehicleYear;
            note.PhotoPath = photoPath;
            note.PossibleOperations = model.PossibleOperations;
            note.TotalCost = model.TotalCost;
            note.OperationNote = model.OperationNote;
        }
        note.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Not güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var note = await _db.ServiceNotes.FirstOrDefaultAsync(n => n.Id == id && n.CreatedByUserId == CurrentUserId);
        if (note != null)
        {
            if (note.PhotoPath != null)
                DeletePhoto(note.PhotoPath);
            _db.ServiceNotes.Remove(note);
            await _db.SaveChangesAsync();
        }
        TempData["Success"] = "Not silindi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<string> SavePhoto(IFormFile file)
    {
        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "notes");
        Directory.CreateDirectory(uploadsDir);
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsDir, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return $"/uploads/notes/{fileName}";
    }

    private void DeletePhoto(string path)
    {
        var fullPath = Path.Combine(_env.WebRootPath, path.TrimStart('/'));
        if (System.IO.File.Exists(fullPath))
            System.IO.File.Delete(fullPath);
    }
}
