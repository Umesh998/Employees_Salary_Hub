using Employees_Salary_Hub.Models;                  // ← AuthResult lives here
using Employees_Salary_Hub.Service.Interfaces;      // ← IAuthService, IAuditService
using Employees_Salary_Hub.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IAuditService _auditService;

    public AuthController(IAuthService authService, IAuditService auditService)
    {
        _authService = authService;
        _auditService = auditService;
    }

    [HttpGet("login")]
    public IActionResult Login() => View();

    [HttpPost("login"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.LoginAsync(model, ip ?? "");

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message!);
            return View(model);
        }

        TempData["UserId"] = result.UserId;
        TempData["IsFirstLogin"] = result.IsFirstLogin;
        return RedirectToAction("VerifyOtp");
    }

    [HttpGet("verify-otp")]
    public IActionResult VerifyOtp()
    {
        var userId = TempData["UserId"]?.ToString();
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login");

        return View(new OtpVerifyViewModel { UserId = userId });
    }

    [HttpPost("verify-otp"), ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyOtp(OtpVerifyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["UserId"] = model.UserId;
            return View(model);
        }

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.VerifyOtpAsync(model, ip ?? "");

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message!);
            TempData["UserId"] = model.UserId;
            return View(model);
        }

        // Store JWT in secure HttpOnly cookie
        Response.Cookies.Append("jwt", result.Token!, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        if (result.IsFirstLogin)
        {
            TempData["UserId"] = result.UserId;
            return RedirectToAction("SetPassword");
        }

        // Redirect based on role
        if (result.Role == "Admin")
            return RedirectToAction("Index", "Admin");

        return RedirectToAction("Index", "Employee");
    }

    [HttpGet("set-password")]
    public IActionResult SetPassword()
    {
        var userId = TempData["UserId"]?.ToString();
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login");

        return View(new SetPasswordViewModel { UserId = userId });
    }

    [HttpPost("set-password"), ValidateAntiForgeryToken]
    public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _authService.SetFirstPasswordAsync(model.UserId, model);

        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message!);
            return View(model);
        }

        return RedirectToAction("Index", "Employee");
    }

    [HttpPost("logout"), Authorize, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
            await _authService.LogoutAsync(userId);

        Response.Cookies.Delete("jwt");
        return RedirectToAction("Login");
    }
}
