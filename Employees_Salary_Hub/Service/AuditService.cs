using Employees_Salary_Hub.Data;
using Employees_Salary_Hub.Models;
using Employees_Salary_Hub.Service.Interfaces;

namespace Employees_Salary_Hub.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context) => _context = context;

        public async Task LogAsync(
            string? userId, string action, string? ipAddress, string? details = null)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                Details = details,
                IPAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };
            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}
