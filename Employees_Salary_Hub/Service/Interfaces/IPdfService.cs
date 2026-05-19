using Employees_Salary_Hub.Models;
using Employees_Salary_Hub.ViewModels.Admin;

namespace Employees_Salary_Hub.Service.Interfaces
{
    public interface IPdfService
    {
        Task<string> GenerateSalarySlipAsync(SalarySlip slip, ApplicationUser employee, PayrollBatch batch);
    }
}
