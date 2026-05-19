using Employees_Salary_Hub.Data;
using Employees_Salary_Hub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Employees_Salary_Hub.Service.Interfaces;
using System.Security.Claims;
using Employees_Salary_Hub.ViewModels.Employee;


[Authorize(Policy = "EmployeeOnly"), Route("employee")]
public class EmployeeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ISalarySlipService _slipService;
    private readonly UserManager<ApplicationUser> _userManager;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        var slips = await _slipService.GetEmployeeSlipsAsync(userId);

        var vm = new EmployeeDashboardViewModel
        {
            Employee = user!,
            SalarySlips = slips,
            LatestSlip = slips.FirstOrDefault()
        };
        return View(vm);
    }

    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        return View(user);
    }

    [HttpGet("change-password")]
    public IActionResult ChangePassword() => View();

    [HttpPost("change-password"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        var result = await _userManager.ChangePasswordAsync(
            user!, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError("", err.Description);
            return View(model);
        }
        TempData["Success"] = "Password changed successfully.";
        return RedirectToAction("Index");
    }
}
