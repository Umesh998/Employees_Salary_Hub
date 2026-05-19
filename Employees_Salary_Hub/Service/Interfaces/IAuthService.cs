//using Employees_Salary_Hub.Models;
//using Employees_Salary_Hub.ViewModels;
//using Employees_Salary_Hub.ViewModels.Auth;

//namespace Employees_Salary_Hub.Services.Interfaces
//{
//    public interface IAuthService
//    {
//        Task<AuthResult> LoginAsync(LoginViewModel model, string ipAddress);
//        Task<AuthResult> VerifyOtpAsync(OtpVerifyViewModel model, string ipAddress);
//        Task<AuthResult> SetFirstPasswordAsync(string userId, SetPasswordViewModel model);
//        Task LogoutAsync(string userId);
//        string GenerateJwtToken(ApplicationUser user, IList<string> roles);
//    }

//    public record AuthResult(bool Success, string? Message, string? Token = null,
//                              string? UserId = null, bool IsFirstLogin = false);
//}







using Employees_Salary_Hub.Models;
using Employees_Salary_Hub.ViewModels.Auth;

namespace Employees_Salary_Hub.Service.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(LoginViewModel model, string ipAddress);
        Task<AuthResult> VerifyOtpAsync(OtpVerifyViewModel model, string ipAddress);
        Task<AuthResult> SetFirstPasswordAsync(string userId, SetPasswordViewModel model);
        Task LogoutAsync(string userId);
        string GenerateJwtToken(ApplicationUser user, IList<string> roles);
    }

    // ❌ Removed duplicate AuthResult — it lives in Models/AuthResult.cs
}
