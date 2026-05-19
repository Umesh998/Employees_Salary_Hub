using Employees_Salary_Hub.Models;
using Employees_Salary_Hub.Service.Interfaces;
using Employees_Salary_Hub.ViewModels.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Employees_Salary_Hub.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOtpService _otpService;
        private readonly IAuditService _auditService;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOtpService otpService, IAuditService auditService,
            IConfiguration config, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _otpService = otpService;
            _auditService = auditService;
            _config = config;
            _logger = logger;
        }

        public async Task<AuthResult> LoginAsync(LoginViewModel model, string ipAddress)
        {
            var user = await _userManager.FindByNameAsync(model.EmployeeCode);
            if (user == null || !user.IsActive)
                return new AuthResult(false, "Invalid credentials.");

            var result = await _signInManager.CheckPasswordSignInAsync(
                user, model.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
                return new AuthResult(false, "Account locked. Try again in 15 minutes.");

            if (!result.Succeeded)
            {
                await _auditService.LogAsync(user.Id, "LOGIN_FAILED", ipAddress);
                return new AuthResult(false, "Invalid credentials.");
            }

            await _otpService.GenerateAndSendAsync(user.Id, user.Email!);
            await _auditService.LogAsync(user.Id, "OTP_SENT", ipAddress);

            return new AuthResult(true, "OTP sent to registered email.",
                UserId: user.Id, IsFirstLogin: user.IsFirstLogin);
        }

        public async Task<AuthResult> VerifyOtpAsync(OtpVerifyViewModel model, string ipAddress)
        {
            var valid = await _otpService.VerifyAsync(model.UserId, model.OtpCode);
            if (!valid) return new AuthResult(false, "Invalid or expired OTP.");

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return new AuthResult(false, "User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
            var role = roles.FirstOrDefault();

            await _auditService.LogAsync(user.Id, "LOGIN_SUCCESS", ipAddress);

            return new AuthResult(true, "Login successful.",
                Token: token,
                UserId: user.Id,
                IsFirstLogin: user.IsFirstLogin,
                Role: role);
        }

        public async Task<AuthResult> SetFirstPasswordAsync(
            string userId, SetPasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new AuthResult(false, "User not found.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
                return new AuthResult(false,
                    string.Join(", ", result.Errors.Select(e => e.Description)));

            user.IsFirstLogin = false;
            await _userManager.UpdateAsync(user);
            return new AuthResult(true, "Password set successfully.");
        }

        public string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name,           user.EmployeeCode),
                new("FullName",                user.FullName),
                new("EmployeeCode",            user.EmployeeCode),
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:ExpiryMinutes"] ?? "60"));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: exp,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task LogoutAsync(string userId) =>
            await _auditService.LogAsync(userId, "LOGOUT", null);
    }
}
