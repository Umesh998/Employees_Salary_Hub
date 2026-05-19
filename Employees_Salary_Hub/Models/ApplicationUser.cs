// Models/ApplicationUser.cs


using Microsoft.AspNetCore.Identity;
using Employees_Salary_Hub.Models;

namespace Employees_Salary_Hub.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public bool IsFirstLogin { get; set; } = true;
        public bool IsActive { get; set; } = true;

        public DateTime? DateOfJoining { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<SalarySlip> SalarySlips { get; set; } = new List<SalarySlip>();
        public ICollection<OtpRecord> OtpRecords { get; set; } = new List<OtpRecord>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    }
}
