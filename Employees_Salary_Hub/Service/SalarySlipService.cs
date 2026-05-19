using Employees_Salary_Hub.Data;
using Employees_Salary_Hub.Models;
using Employees_Salary_Hub.Service.Interfaces;
using Employees_Salary_Hub.ViewModels.Admin;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Employees_Salary_Hub.Services
{
    public class SalarySlipService : ISalarySlipService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;
        private readonly UserManager<ApplicationUser> _userManager;

        public SalarySlipService(ApplicationDbContext ctx,
            IPdfService pdfService, UserManager<ApplicationUser> um)
        {
            _context = ctx;
            _pdfService = pdfService;
            _userManager = um;
        }

        public async Task<(int Created, int Skipped)> ProcessPayrollUploadAsync(
            List<ExcelRowDto> rows, int month, int year,
            DateTime releaseDate, string adminId)
        {
            // Create or fetch payroll batch
            var batch = await _context.PayrollBatches
                .FirstOrDefaultAsync(b => b.Month == month && b.Year == year);

            if (batch == null)
            {
                batch = new PayrollBatch
                {
                    Month = month,
                    Year = year,
                    ReleaseDate = releaseDate,
                    CreatedBy = adminId
                };
                await _context.PayrollBatches.AddAsync(batch);
                await _context.SaveChangesAsync();
            }

            int created = 0, skipped = 0;

            foreach (var row in rows)
            {
                // Upsert employee account
                var user = await _userManager.FindByNameAsync(row.EmployeeCode)
                    ?? await CreateEmployeeAsync(row);

                // Check for duplicate slip
                var exists = await _context.SalarySlips
                    .AnyAsync(s => s.UserId == user.Id
                               && s.PayrollBatchId == batch.Id);
                if (exists) { skipped++; continue; }

                var slip = new SalarySlip
                {
                    PayrollBatchId = batch.Id,
                    UserId = user.Id,
                    BasicSalary = row.BasicSalary,
                    HRA = row.HRA,
                    Bonus = row.Bonus,
                    PF = row.PF,
                    Tax = row.Tax,
                    NetSalary = row.NetSalary
                };

                await _context.SalarySlips.AddAsync(slip);
                await _context.SaveChangesAsync(); // get slip.Id

                // Generate PDF
                slip.PdfPath = await _pdfService
                    .GenerateSalarySlipAsync(slip, user, batch);
                await _context.SaveChangesAsync();
                created++;
            }

            return (created, skipped);
        }

        public async Task<List<SalarySlip>> GetEmployeeSlipsAsync(string userId)
        {
            return await _context.SalarySlips
                .Include(s => s.PayrollBatch)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.PayrollBatch.Year)
                .ThenByDescending(s => s.PayrollBatch.Month)
                .ToListAsync();
        }

        // FIX 1: Removed the wrong IEnumerable<SalarySlip> overload entirely.
        // Publishing is done by batchId — this is the only PublishBatchAsync needed.
        public async Task<bool> PublishBatchAsync(int batchId)
        {
            var batch = await _context.PayrollBatches.FindAsync(batchId);
            if (batch == null) return false;

            batch.IsPublished = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // FIX 2: Added missing GetSlipsByBatchAsync
        public async Task<IEnumerable<SalarySlip>> GetSlipsByBatchAsync(int batchId)
        {
            return await _context.SalarySlips
                .Include(s => s.PayrollBatch)
                .Where(s => s.PayrollBatchId == batchId)
                .ToListAsync();
        }

        private async Task<ApplicationUser> CreateEmployeeAsync(ExcelRowDto row)
        {
            var user = new ApplicationUser
            {
                UserName = row.EmployeeCode,
                EmployeeCode = row.EmployeeCode,
                FullName = row.EmployeeName,
                MobileNumber = row.MobileNumber,
                Email = row.Email ?? $"{row.EmployeeCode}@company.local",
                Department = row.Department,
                Designation = row.Designation,
                IsFirstLogin = true,
                IsActive = true
            };

            // Temporary password — employee must change on first login
            var result = await _userManager.CreateAsync(user, $"Pay@{row.EmployeeCode}1!");
            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, "Employee");

            return user;
        }
    }
}
