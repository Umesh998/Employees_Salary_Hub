using Employees_Salary_Hub.Models;
using Employees_Salary_Hub.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Employees_Salary_Hub.Service;
using System.Security.Claims;
using Employees_Salary_Hub.Service.Interfaces;

[Authorize(Policy = "AdminOnly"), Route("admin/upload")]
public class UploadController : Controller
{
    private readonly IExcelService _excelService;
    private readonly ISalarySlipService _slipService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditService _auditService;

    [HttpGet] public IActionResult Index() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(
        IFormFile file, int month, int year, DateTime releaseDate)
    {
        if (file == null || file.Length == 0)
        { ModelState.AddModelError("", "No file selected."); return View("Index"); }

        using var stream = file.OpenReadStream();
        var (rows, errors) = await _excelService.ParsePayrollExcelAsync(stream);

        if (errors.Any())
        { ViewBag.Errors = errors; return View("Index"); }

        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _slipService.ProcessPayrollUploadAsync(
            rows, month, year, releaseDate, adminId);

        await _auditService.LogAsync(adminId,
            $"PAYROLL_UPLOADED Month={month}/{year} Records={rows.Count}",
            HttpContext.Connection.RemoteIpAddress?.ToString());

        TempData["Success"] = $"Processed {result.Created} slips. {result.Skipped} skipped.";
        return RedirectToAction("Index", "Admin");
    }


}
