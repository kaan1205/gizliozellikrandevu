using Microsoft.AspNetCore.Mvc;
using RandevuPanel.Services;

namespace RandevuPanel.Controllers;

public class DashboardController : BaseController
{
    private readonly IAppointmentService _appointmentService;

    public DashboardController(IAppointmentService appointmentService) : base(appointmentService)
    {
        _appointmentService = appointmentService;
    }

    public async Task<IActionResult> Index()
    {
        var vm = await _appointmentService.GetDashboardDataAsync(CurrentUserId);
        return View(vm);
    }
}
