using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;

namespace PFE.ExpenseTracker.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IFinancialGoalRepository _goalRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            IExpenseRepository expenseRepository,
            IBudgetRepository budgetRepository,
            IFinancialGoalRepository goalRepository,
            ICategoryRepository categoryRepository,
            ILogger<ReportService> logger)
        {
            _expenseRepository = expenseRepository;
            _budgetRepository = budgetRepository;
            _goalRepository = goalRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<byte[]> GenerateExpenseReportAsync(Guid userId, DateTime startDate, DateTime endDate, string format)
        {
            var expenses = await _expenseRepository.GetUserExpensesAsync(userId);
            expenses = expenses.Where(e => e.Date >= startDate && e.Date <= endDate).ToList();

            if (format.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                using (var memoryStream = new MemoryStream())
                {
                    var writer = new PdfWriter(memoryStream);
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);

                    document.Add(new Paragraph($"Expense Report ({startDate:d} - {endDate:d})").SetFontSize(20));
                    document.Add(new Paragraph(""));

                    var table = new Table(4).UseAllAvailableWidth();
                    table.AddHeaderCell("Date");
                    table.AddHeaderCell("Category");
                    table.AddHeaderCell("Description");
                    table.AddHeaderCell("Amount");

                    foreach (var expense in expenses)
                    {
                        table.AddCell(expense.Date.ToShortDateString());
                        table.AddCell(expense.Category?.Name ?? "N/A");
                        table.AddCell(expense.Description);
                        table.AddCell(expense.Amount.ToString("C"));
                    }

                    document.Add(table);
                    document.Add(new Paragraph($"Total: {expenses.Sum(e => e.Amount):C}"));
                    document.Close();

                    return memoryStream.ToArray();
                }
            }
            else // Excel
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Expenses");
                    
                    // Headers
                    worksheet.Cell(1, 1).Value = "Date";
                    worksheet.Cell(1, 2).Value = "Category";
                    worksheet.Cell(1, 3).Value = "Description";
                    worksheet.Cell(1, 4).Value = "Amount";

                    // Data
                    var row = 2;
                    foreach (var expense in expenses)
                    {
                        worksheet.Cell(row, 1).Value = expense.Date;
                        worksheet.Cell(row, 2).Value = expense.Category?.Name ?? "N/A";
                        worksheet.Cell(row, 3).Value = expense.Description;
                        worksheet.Cell(row, 4).Value = expense.Amount;
                        row++;
                    }

                    // Format
                    worksheet.Column(1).Width = 15;
                    worksheet.Column(2).Width = 20;
                    worksheet.Column(3).Width = 40;
                    worksheet.Column(4).Width = 15;
                    worksheet.Column(4).Style.NumberFormat.Format = "$#,##0.00";

                    using (var memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        public async Task<byte[]> GenerateBudgetReportAsync(Guid userId, DateTime startDate, DateTime endDate, string format)
        {
            var budgets = await _budgetRepository.GetUserBudgetsAsync(userId);
            
            if (format.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                using (var memoryStream = new MemoryStream())
                {
                    var writer = new PdfWriter(memoryStream);
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);

                    document.Add(new Paragraph("Budget Report").SetFontSize(20));
                    document.Add(new Paragraph(""));

                    var table = new Table(5).UseAllAvailableWidth();
                    table.AddHeaderCell("Category");
                    table.AddHeaderCell("Budget Amount");
                    table.AddHeaderCell("Spent Amount");
                    table.AddHeaderCell("Remaining");
                    table.AddHeaderCell("% Used");

                    foreach (var budget in budgets)
                    {
                        table.AddCell(budget.Category?.Name ?? "N/A");
                        table.AddCell(budget.Amount.ToString("C"));
                        table.AddCell(budget.SpentAmount.ToString("C"));
                        table.AddCell((budget.Amount - budget.SpentAmount).ToString("C"));
                        table.AddCell($"{(budget.SpentAmount / budget.Amount * 100):F1}%");
                    }

                    document.Add(table);
                    document.Close();

                    return memoryStream.ToArray();
                }
            }
            else // Excel
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Budgets");
                    
                    worksheet.Cell(1, 1).Value = "Category";
                    worksheet.Cell(1, 2).Value = "Budget Amount";
                    worksheet.Cell(1, 3).Value = "Spent Amount";
                    worksheet.Cell(1, 4).Value = "Remaining";
                    worksheet.Cell(1, 5).Value = "% Used";

                    var row = 2;
                    foreach (var budget in budgets)
                    {
                        worksheet.Cell(row, 1).Value = budget.Category?.Name ?? "N/A";
                        worksheet.Cell(row, 2).Value = budget.Amount;
                        worksheet.Cell(row, 3).Value = budget.SpentAmount;
                        worksheet.Cell(row, 4).Formula = $"=B{row}-C{row}";
                        worksheet.Cell(row, 5).Formula = $"=C{row}/B{row}";
                        row++;
                    }

                    // Format
                    var range = worksheet.Range(1, 1, row - 1, 5);
                    range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                    worksheet.Column(5).Style.NumberFormat.Format = "0.0%";

                    using (var memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        // Implement other report generation methods similarly
        public Task<byte[]> GenerateFinancialGoalsReportAsync(Guid userId, string format)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> GenerateAnnualSummaryReportAsync(Guid userId, int year, string format)
        {
            throw new NotImplementedException();
        }
    }
}
