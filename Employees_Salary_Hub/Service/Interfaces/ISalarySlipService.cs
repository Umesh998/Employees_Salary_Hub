using Employees_Salary_Hub.Models;
using Employees_Salary_Hub.ViewModels.Admin;

namespace Employees_Salary_Hub.Service.Interfaces
{
    public interface ISalarySlipService
    {
        Task<(int Created, int Skipped)> ProcessPayrollUploadAsync(
            List<ExcelRowDto> rows, int month, int year,
            DateTime releaseDate, string adminId);

        Task<List<SalarySlip>> GetEmployeeSlipsAsync(string userId);

        // FIX: was Task PublishBatchAsync(IEnumerable<SalarySlip>) — wrong signature
        // matches the service: returns bool, takes int batchId
        Task<bool> PublishBatchAsync(int batchId);

        Task<IEnumerable<SalarySlip>> GetSlipsByBatchAsync(int batchId);
    }
}
