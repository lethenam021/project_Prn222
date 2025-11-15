using System.Security.Claims;
using System.Text.Json;
using BusinessLogic.DTOs.Request.Report;
using BusinessLogic.Interface;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Seller")]
public class ReportController : Controller
{
    private readonly IReportService _reportService;
    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<IActionResult> ManageReport(
        ReportTypeEnum reportType = ReportTypeEnum.Order,
        ReportTimeEnum reportTime = ReportTimeEnum.Weekly,
        int month = 0,
        int? week = null)
    {
        var req = new ReportRequest
        {
            SellerId =  int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            ReportType = reportType,
            ReportTime = reportTime,
            Month = month,
            Week = week
        };

        var data = await _reportService.GetReportAsync(req);

        var currentMonth = month == 0 ? DateTime.Today.Month : month;
        var currentWeek = week ?? 1;

        ViewBag.ReportType = reportType;
        ViewBag.ReportTime = reportTime;
        ViewBag.Month = currentMonth;
        ViewBag.Week = currentWeek;

        return View(data);
    }
}
