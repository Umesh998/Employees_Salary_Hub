namespace Employees_Salary_Hub.ViewModels.Admin
{
    public class SalarySlipDto
    {
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public DateOnly PayPeriod { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal HouseAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal OtherAllowances { get; set; }

        public decimal TaxDeduction { get; set; }
        public decimal ProvidentFund { get; set; }
        public decimal OtherDeductions { get; set; }

        public decimal GrossSalary => BasicSalary + HouseAllowance + TransportAllowance + OtherAllowances;
        public decimal TotalDeductions => TaxDeduction + ProvidentFund + OtherDeductions;
        public decimal NetSalary => GrossSalary - TotalDeductions;
    }
}
