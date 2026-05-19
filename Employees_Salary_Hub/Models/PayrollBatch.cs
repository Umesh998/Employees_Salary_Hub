using Employees_Salary_Hub.Models;

namespace Employees_Salary_Hub.Models
{
    public class PayrollBatch
    {
        public int Id { get; set; }
        public int Month { get; set; }   // 1-12
        public int Year { get; set; }
        public DateTime ReleaseDate { get; set; }   // e.g. 8 May for April payroll
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;

        public ICollection<SalarySlip> SalarySlips { get; set; } = new List<SalarySlip>();
    }
}
