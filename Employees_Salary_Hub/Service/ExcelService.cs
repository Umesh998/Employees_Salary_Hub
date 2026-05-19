using Employees_Salary_Hub.Service.Interfaces;
using Employees_Salary_Hub.ViewModels.Admin;
using OfficeOpenXml;



namespace Employees_Salary_Hub.Services
{
    public class ExcelService : IExcelService
    {
        public ExcelService() =>
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        public async Task<(List<ExcelRowDto> Rows, List<string> Errors)>
            ParsePayrollExcelAsync(Stream fileStream)
        {
            var rows = new List<ExcelRowDto>();
            var errors = new List<string>();

            using var package = new ExcelPackage(fileStream);
            var sheet = package.Workbook.Worksheets.FirstOrDefault();
            if (sheet == null)
            { errors.Add("No worksheet found."); return (rows, errors); }

            // Validate header row
            var expectedHeaders = new[]
            { "EmployeeCode","EmployeeName","MobileNumber","Email",
              "Department","Designation","BasicSalary","HRA",
              "Bonus","PF","Tax","NetSalary" };

            for (int col = 1; col <= expectedHeaders.Length; col++)
            {
                var header = sheet.Cells[1, col].Text.Trim();
                if (!string.Equals(header, expectedHeaders[col - 1],
                    StringComparison.OrdinalIgnoreCase))
                {
                    errors.Add($"Column {col} expected '{expectedHeaders[col - 1]}', got '{header}'.");
                }
            }
            if (errors.Any()) return (rows, errors);

            var seenCodes = new HashSet<string>();

            for (int row = 2; row <= sheet.Dimension.End.Row; row++)
            {
                var code = sheet.Cells[row, 1].Text.Trim();
                if (string.IsNullOrEmpty(code)) continue;

                if (seenCodes.Contains(code))
                { errors.Add($"Row {row}: Duplicate EmployeeCode '{code}'."); continue; }

                seenCodes.Add(code);

                rows.Add(new ExcelRowDto
                {
                    EmployeeCode = code,
                    EmployeeName = sheet.Cells[row, 2].Text.Trim(),
                    MobileNumber = sheet.Cells[row, 3].Text.Trim(),
                    Email = sheet.Cells[row, 4].Text.Trim(),
                    Department = sheet.Cells[row, 5].Text.Trim(),
                    Designation = sheet.Cells[row, 6].Text.Trim(),
                    BasicSalary = ParseDecimal(sheet.Cells[row, 7].Text),
                    HRA = ParseDecimal(sheet.Cells[row, 8].Text),
                    Bonus = ParseDecimal(sheet.Cells[row, 9].Text),
                    PF = ParseDecimal(sheet.Cells[row, 10].Text),
                    Tax = ParseDecimal(sheet.Cells[row, 11].Text),
                    NetSalary = ParseDecimal(sheet.Cells[row, 12].Text),
                });
            }

            return (rows, errors);
        }

        private static decimal ParseDecimal(string value) =>
            decimal.TryParse(value, out var d) ? d : 0m;
    }
}
