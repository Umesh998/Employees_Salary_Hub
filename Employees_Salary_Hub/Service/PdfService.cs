using Employees_Salary_Hub.Models;
using Employees_Salary_Hub.Service.Interfaces;
using Employees_Salary_Hub.ViewModels.Admin;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace Employees_Salary_Hub.Services
{
    public class PdfService : IPdfService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<PdfService> _logger;

        public PdfService(IWebHostEnvironment env, ILogger<PdfService> logger)
        {
            _env = env;
            _logger = logger;
        }

        // ── Single clean method, saves to disk and returns the relative URL ──
        public async Task<string> GenerateSalarySlipAsync(
            SalarySlip slip, ApplicationUser employee, PayrollBatch batch)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var dir = Path.Combine(_env.WebRootPath, "salaryslips");
            Directory.CreateDirectory(dir);

            var fileName = $"{employee.EmployeeCode}_{batch.Month:D2}_{batch.Year}.pdf";
            var filePath = Path.Combine(dir, fileName);

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(t => t.FontFamily("Arial").FontSize(10));

                    page.Header().Element(ComposeHeader(employee, batch));
                    page.Content().Element(ComposeContent(slip, employee, batch));
                    page.Footer().AlignCenter()
                        .Text(t =>
                        {
                            t.Span("Page ");
                            t.CurrentPageNumber();
                            t.Span(" of ");
                            t.TotalPages();
                        });
                });
            });

            doc.GeneratePdf(filePath);
            _logger.LogInformation("PDF generated: {Path}", filePath);
            return $"/salaryslips/{fileName}";
        }

        private static Action<IContainer> ComposeHeader(
            ApplicationUser emp, PayrollBatch batch)
        => header =>
        {
            header.Column(col =>
            {
                col.Item().Row(row =>
                {
                    // Company info on left
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("ACME CORPORATION")
                            .FontSize(16).Bold().FontColor(Colors.Blue.Medium);
                        c.Item().Text("123 Business Park, Mumbai 400001");
                        c.Item().Text("HR Department | hr@acme.com");
                    });

                    // Salary Slip title on right
                    // FIX 1: was malformed string interpolation with mismatched braces
                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().Text("SALARY SLIP")
                            .FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                        c.Item().Text(
                            $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(batch.Month)} {batch.Year}")
                            .FontSize(12).FontColor(Colors.Grey.Medium);
                    });
                });

                col.Item().PaddingVertical(4).LineHorizontal(1)
                    .LineColor(Colors.Blue.Medium);
            });
        };

        private static Action<IContainer> ComposeContent(
            SalarySlip slip, ApplicationUser emp, PayrollBatch batch)
        => content =>
        {
            content.Column(col =>
            {
                // Employee Details
                col.Item().PaddingBottom(8).Text("EMPLOYEE DETAILS")
                    .Bold().FontColor(Colors.Blue.Darken1);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn();
                        c.RelativeColumn();
                    });

                    void AddRow(string label, string value)
                    {
                        table.Cell().Text(label).Bold();
                        table.Cell().Text(value);
                    }

                    AddRow("Employee Code:", emp.EmployeeCode);
                    AddRow("Name:", emp.FullName);
                    AddRow("Department:", emp.Department ?? "-");
                    AddRow("Designation:", emp.Designation ?? "-");
                });

                col.Item().PaddingVertical(8).LineHorizontal(0.5f);

                // Earnings & Deductions
                col.Item().PaddingBottom(8).Text("EARNINGS & DEDUCTIONS")
                    .Bold().FontColor(Colors.Blue.Darken1);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(2); c.RelativeColumn();
                        c.RelativeColumn(2); c.RelativeColumn();
                    });

                    // Header row
                    table.Header(h =>
                    {
                        h.Cell().Background(Colors.Blue.Medium).Padding(4)
                            .Text("Earnings").FontColor(Colors.White).Bold();
                        h.Cell().Background(Colors.Blue.Medium).Padding(4)
                            .Text("Amount").FontColor(Colors.White).Bold();
                        h.Cell().Background(Colors.Red.Medium).Padding(4)
                            .Text("Deductions").FontColor(Colors.White).Bold();
                        h.Cell().Background(Colors.Red.Medium).Padding(4)
                            .Text("Amount").FontColor(Colors.White).Bold();
                    });

                    void Cell(string t, bool alt = false) =>
                        table.Cell()
                            .Background(alt ? Colors.Grey.Lighten3 : Colors.White)
                            .Padding(4).Text(t);

                    Cell("Basic Salary"); Cell(slip.BasicSalary.ToString("N2"));
                    Cell("PF"); Cell(slip.PF.ToString("N2"));
                    Cell("HRA", true); Cell(slip.HRA.ToString("N2"), true);
                    Cell("Tax", true); Cell(slip.Tax.ToString("N2"), true);
                    Cell("Bonus"); Cell(slip.Bonus.ToString("N2"));
                    Cell(""); Cell("");
                });

                col.Item().PaddingVertical(8).LineHorizontal(1)
                    .LineColor(Colors.Blue.Medium);

                // Net Salary
                col.Item().Background(Colors.Blue.Lighten4).Padding(10).Row(row =>
                {
                    row.RelativeItem().Text("NET SALARY").Bold().FontSize(14);
                    row.RelativeItem().AlignRight()
                        .Text($"INR {slip.NetSalary:N2}").Bold().FontSize(14)
                        .FontColor(Colors.Green.Darken2);
                });
            });
        };

        public Task<byte[]> GenerateSalarySlipAsync(SalarySlipDto salarySlip)
        {
            throw new NotImplementedException();
        }
    }
}
