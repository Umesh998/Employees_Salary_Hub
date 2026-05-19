namespace Employees_Salary_Hub.Models
{
    public record AuthResult(
        bool Success,
        string? Message = null,
        string? Token = null,
        string? UserId = null,
        bool IsFirstLogin = false,
        string? Role = null
    );
}