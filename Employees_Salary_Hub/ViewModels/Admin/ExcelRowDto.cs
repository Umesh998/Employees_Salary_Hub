namespace Employees_Salary_Hub.ViewModels.Admin
{
    public class ExcelRowDto
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public string? Department { get; set; }
        public string? Designation { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HRA { get; set; }
        public decimal Bonus { get; set; }
        public decimal PF { get; set; }
        public decimal Tax { get; set; }
        public decimal NetSalary { get; set; }
    }
}
