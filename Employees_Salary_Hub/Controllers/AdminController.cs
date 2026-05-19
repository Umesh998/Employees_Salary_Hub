using Employees_Salary_Hub.Data;
using Employees_Salary_Hub.Models;
using Employees_Salary_Hub.Service.Interfaces;
using Employees_Salary_Hub.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollPro.Web.ViewModels.Admin;
using System.Security.Claims;

[Authorize(Policy = "AdminOnly"), Route("admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ISalarySlipService _slipService;
    private readonly IAuditService _auditService;

    // FIX 1: Constructor was missing entirely
    public AdminController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ISalarySlipService slipService,
        IAuditService auditService)
    {
        _context = context;
        _userManager = userManager;
        _slipService = slipService;
        _auditService = auditService;
    }

    // FIX 2: Index was commented out — restored and working
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var vm = new Employees_Salary_Hub.ViewModels.Admin.AdminDashboardViewModel
        {
            TotalEmployees = await _context.Users.CountAsync(),
            TotalBatches = await _context.PayrollBatches.CountAsync(),
            TotalSlips = await _context.SalarySlips.CountAsync(),
            RecentBatches = await _context.PayrollBatches
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .ToListAsync(),
            RecentAuditLogs = await _context.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .Take(10)
                .ToListAsync(),
        };
        return View(vm);
    }

    [HttpGet("employees")]
    public async Task<IActionResult> Employees()
    {
        var employees = await _context.Users
            .OrderBy(u => u.EmployeeCode)
            .ToListAsync();
        return View(employees);
    }

    [HttpPost("toggle-active/{id}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);
        TempData["Success"] = $"Employee {user.EmployeeCode} has been {(user.IsActive ? "activated" : "deactivated")}.";
        return RedirectToAction("Employees");
    }

    [HttpPost("reset-password/{id}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _userManager.ResetPasswordAsync(user, token, $"Pay@{user.EmployeeCode}1!");
        user.IsFirstLogin = true;
        await _userManager.UpdateAsync(user);

        TempData["Success"] = "Password reset. Employee must set new password on next login.";
        return RedirectToAction("Employees");
    }

    [HttpGet("batches")]
    public async Task<IActionResult> Batches()
    {
        var batches = await _context.PayrollBatches
            .Include(b => b.SalarySlips)
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.Month)
            .ToListAsync();
        return View(batches);
    }

    [HttpPost("publish/{id:int}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int id)
    {
        // FIX 3: Removed unused salarySlips variable — PublishBatchAsync takes int directly
        var success = await _slipService.PublishBatchAsync(id);
        if (!success)
        {
            TempData["Error"] = "Batch not found.";
            return RedirectToAction("Batches");
        }

        var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _auditService.LogAsync(adminId, "BATCH_PUBLISHED", null, $"BatchId: {id}");

        TempData["Success"] = "Batch published successfully.";
        return RedirectToAction("Batches");
    }

    [HttpGet("audit-logs")]
    public async Task<IActionResult> AuditLogs(int page = 1)
    {
        const int pageSize = 50;
        var logs = await _context.AuditLogs
            .Include(a => a.User)
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return View(logs);
    }
}




