using Employees_Salary_Hub.ViewModels.Admin;

namespace Employees_Salary_Hub.Service.Interfaces
{
    public interface IExcelService
    {
        Task<(List<ExcelRowDto> Rows, List<string> Errors)> ParsePayrollExcelAsync(Stream fileStream);
    }
}
