using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RandevuPanel.Services;

namespace RandevuPanel.Controllers;

[Authorize]
public abstract class BaseController : Controller
{
    protected int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private readonly IAppointmentService _appointmentService;

    protected BaseController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var dashboard = await _appointmentService.GetDashboardDataAsync(CurrentUserId);
            ViewBag.TodayCount = dashboard.TodayCount;
            ViewBag.ApproachingAppointments = dashboard.ApproachingAppointments;
        }

        await next();
    }
}
