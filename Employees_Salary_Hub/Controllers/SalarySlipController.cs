using Employees_Salary_Hub.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Employees_Salary_Hub.Service.Interfaces;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

[Authorize, Route("salary")]
public class SalarySlipController : Controller
{
    private readonly ISalarySlipService _slipService;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IAuditService _auditService;

    [HttpGet("download/{id:int}")]
    public async Task<IActionResult> Download(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var isAdmin = User.IsInRole("Admin");

        var slip = await _context.SalarySlips
            .Include(s => s.PayrollBatch)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (slip == null) return NotFound();

        // ── Ownership check (employees only) ──────────────────────
        if (!isAdmin && slip.UserId != userId)
            return Forbid();

        // ── Release date check ────────────────────────────────────
        if (!isAdmin && (!slip.PayrollBatch.IsPublished
            || slip.PayrollBatch.ReleaseDate > DateTime.UtcNow))
            return BadRequest("Salary slip not released yet.");

        // ── Serve file without exposing real path ─────────────────
        if (string.IsNullOrEmpty(slip.PdfPath))
            return NotFound("PDF not generated.");

        var physicalPath = Path.Combine(
            _env.WebRootPath, slip.PdfPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        if (!System.IO.File.Exists(physicalPath)) return NotFound();

        await _auditService.LogAsync(userId, $"SLIP_DOWNLOADED SlipId={id}",
            HttpContext.Connection.RemoteIpAddress?.ToString());

        var bytes = await System.IO.File.ReadAllBytesAsync(physicalPath);
        var fileName = $"SalarySlip_{slip.Id}.pdf";
        return File(bytes, "application/pdf", fileName);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var slips = await _slipService.GetEmployeeSlipsAsync(userId);
        return View(slips);
    }
}
