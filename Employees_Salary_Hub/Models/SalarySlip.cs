using Employees_Salary_Hub.Models;
using System.ComponentModel.DataAnnotations;

namespace Employees_Salary_Hub.Models
{
    public class SalarySlip
    {
        public int Id { get; set; }
        public int PayrollBatchId { get; set; }
        public string UserId { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal BasicSalary { get; set; }
        public decimal HRA { get; set; }
        public decimal Bonus { get; set; }
        public decimal PF { get; set; }
        public decimal Tax { get; set; }
        public decimal NetSalary { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public decimal TotalDeductions { get; set; }

        public decimal  TotalAllowances { get; set; }
        public string? PdfPath { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public PayrollBatch PayrollBatch { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
