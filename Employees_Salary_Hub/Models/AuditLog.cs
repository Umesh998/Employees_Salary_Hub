using Employees_Salary_Hub.Models;

namespace Employees_Salary_Hub.Models
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string? IPAddress { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public ApplicationUser? User { get; set; }
    }
}



