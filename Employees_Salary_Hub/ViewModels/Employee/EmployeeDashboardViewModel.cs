using Employees_Salary_Hub.Models;
using System.ComponentModel.DataAnnotations;

namespace Employees_Salary_Hub.ViewModels.Employee
{
    public class EmployeeDashboardViewModel
    {
        public ApplicationUser Employee { get; set; } = null!;
        public List<SalarySlip> SalarySlips { get; set; } = new();
        public SalarySlip? LatestSlip { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;
        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}

// ViewModels/Admin/AdminDashboardViewModel.cs
namespace PayrollPro.Web.ViewModels.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalBatches { get; set; }
        public int TotalSlips { get; set; }
        public List<PayrollBatch> RecentBatches { get; set; } = new();
        public List<AuditLog> RecentAuditLogs { get; set; } = new();
    }
}
