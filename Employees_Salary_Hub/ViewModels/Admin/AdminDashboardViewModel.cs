using Employees_Salary_Hub.Models;

namespace Employees_Salary_Hub.ViewModels.Admin
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
