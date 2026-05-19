namespace Employees_Salary_Hub.Service.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string? userId, string action, string? ipAddress, string? details = null);
    }
}
