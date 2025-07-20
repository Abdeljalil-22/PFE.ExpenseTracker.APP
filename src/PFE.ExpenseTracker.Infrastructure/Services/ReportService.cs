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
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;

namespace PFE.ExpenseTracker.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly IReadExpenseRepository _expenseRepository;
        private readonly IReadBudgetRepository _budgetRepository;
        private readonly IReadFinancialGoalRepository _goalRepository;
        private readonly IReadCategoryRepository _categoryRepository;
        private readonly ILogger<ReportService> _logger;

        public ReportService(
            IReadExpenseRepository expenseRepository,
            IReadBudgetRepository budgetRepository,
            IReadFinancialGoalRepository goalRepository,
            IReadCategoryRepository categoryRepository,
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
                        // worksheet.Cell(row, 4).Formula = $"=B{row}-C{row}";
                        // worksheet.Cell(row, 5).Formula = $"=C{row}/B{row}";
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


        public async Task<byte[]> GenerateFinancialGoalsReportAsync(Guid userId, string format)
        {
            var goals = await _goalRepository.GetUserGoalsAsync(userId);

            if (format.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
                using (var memoryStream = new MemoryStream())
                {
                    var writer = new PdfWriter(memoryStream);
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);

                    document.Add(new Paragraph("Financial Goals Report").SetFontSize(20));
                    document.Add(new Paragraph(""));

                    var table = new Table(4).UseAllAvailableWidth();
                    table.AddHeaderCell("Goal Name");
                    table.AddHeaderCell("Target Amount");
                    table.AddHeaderCell("Current Amount");
                    table.AddHeaderCell("Status");

                    foreach (var goal in goals)
                    {
                        table.AddCell(goal.Name);
                        table.AddCell(goal.TargetAmount.ToString("C"));
                        table.AddCell(goal.CurrentAmount.ToString("C"));
                        table.AddCell(goal.Status);
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
                    var worksheet = workbook.Worksheets.Add("Financial Goals");

                    worksheet.Cell(1, 1).Value = "Goal Name";
                    worksheet.Cell(1, 2).Value = "Target Amount";
                    worksheet.Cell(1, 3).Value = "Current Amount";
                    worksheet.Cell(1, 4).Value = "Status";

                    var row = 2;
                    foreach (var goal in goals)
                    {
                        worksheet.Cell(row, 1).Value = goal.Name;
                        worksheet.Cell(row, 2).Value = goal.TargetAmount;
                        worksheet.Cell(row, 3).Value = goal.CurrentAmount;
                        worksheet.Cell(row, 4).Value = goal.Status;
                        row++;
                    }

                    // Format
                    worksheet.Column(2).Style.NumberFormat.Format = "$#,##0.00";
                    worksheet.Column(3).Style.NumberFormat.Format = "$#,##0.00";

                    using (var memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
            }
        }

        public async Task<byte[]> GenerateAnnualSummaryReportAsync(Guid userId, int year, string format)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);

            var expenses = await _expenseRepository.GetUserExpensesAsync(userId);
            expenses = expenses.Where(e => e.Date >= startDate && e.Date <= endDate).ToList();

            var budgets = await _budgetRepository.GetUserBudgetsAsync(userId);
            var goals = await _goalRepository.GetUserGoalsAsync(userId);

            if (format.Equals("PDF", StringComparison.OrdinalIgnoreCase))
            {
            using (var memoryStream = new MemoryStream())
            {
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                document.Add(new Paragraph($"Annual Summary Report - {year}").SetFontSize(20));
                document.Add(new Paragraph(""));

                // Expenses Section
                document.Add(new Paragraph("Expenses").SetFontSize(16));
                var expenseTable = new Table(4).UseAllAvailableWidth();
                expenseTable.AddHeaderCell("Date");
                expenseTable.AddHeaderCell("Category");
                expenseTable.AddHeaderCell("Description");
                expenseTable.AddHeaderCell("Amount");
                foreach (var expense in expenses)
                {
                expenseTable.AddCell(expense.Date.ToShortDateString());
                expenseTable.AddCell(expense.Category?.Name ?? "N/A");
                expenseTable.AddCell(expense.Description);
                expenseTable.AddCell(expense.Amount.ToString("C"));
                }
                document.Add(expenseTable);
                document.Add(new Paragraph($"Total Expenses: {expenses.Sum(e => e.Amount):C}"));
                document.Add(new Paragraph(""));

                // Budgets Section
                document.Add(new Paragraph("Budgets").SetFontSize(16));
                var budgetTable = new Table(5).UseAllAvailableWidth();
                budgetTable.AddHeaderCell("Category");
                budgetTable.AddHeaderCell("Budget Amount");
                budgetTable.AddHeaderCell("Spent Amount");
                budgetTable.AddHeaderCell("Remaining");
                budgetTable.AddHeaderCell("% Used");
                foreach (var budget in budgets)
                {
                budgetTable.AddCell(budget.Category?.Name ?? "N/A");
                budgetTable.AddCell(budget.Amount.ToString("C"));
                budgetTable.AddCell(budget.SpentAmount.ToString("C"));
                budgetTable.AddCell((budget.Amount - budget.SpentAmount).ToString("C"));
                budgetTable.AddCell($"{(budget.Amount != 0 ? (budget.SpentAmount / budget.Amount * 100) : 0):F1}%");
                }
                document.Add(budgetTable);
                document.Add(new Paragraph(""));

                // Financial Goals Section
                document.Add(new Paragraph("Financial Goals").SetFontSize(16));
                var goalTable = new Table(4).UseAllAvailableWidth();
                goalTable.AddHeaderCell("Goal Name");
                goalTable.AddHeaderCell("Target Amount");
                goalTable.AddHeaderCell("Current Amount");
                goalTable.AddHeaderCell("Status");
                foreach (var goal in goals)
                {
                goalTable.AddCell(goal.Name);
                goalTable.AddCell(goal.TargetAmount.ToString("C"));
                goalTable.AddCell(goal.CurrentAmount.ToString("C"));
                goalTable.AddCell(goal.Status);
                }
                document.Add(goalTable);

                document.Close();
                return memoryStream.ToArray();
            }
            }
            else // Excel
            {
            using (var workbook = new XLWorkbook())
            {
                // Expenses Sheet
                var expenseSheet = workbook.Worksheets.Add("Expenses");
                expenseSheet.Cell(1, 1).Value = "Date";
                expenseSheet.Cell(1, 2).Value = "Category";
                expenseSheet.Cell(1, 3).Value = "Description";
                expenseSheet.Cell(1, 4).Value = "Amount";
                var row = 2;
                foreach (var expense in expenses)
                {
                expenseSheet.Cell(row, 1).Value = expense.Date;
                expenseSheet.Cell(row, 2).Value = expense.Category?.Name ?? "N/A";
                expenseSheet.Cell(row, 3).Value = expense.Description;
                expenseSheet.Cell(row, 4).Value = expense.Amount;
                row++;
                }
                expenseSheet.Column(1).Width = 15;
                expenseSheet.Column(2).Width = 20;
                expenseSheet.Column(3).Width = 40;
                expenseSheet.Column(4).Width = 15;
                expenseSheet.Column(4).Style.NumberFormat.Format = "$#,##0.00";
                expenseSheet.Cell(row, 3).Value = "Total:";
                expenseSheet.Cell(row, 4).Value = expenses.Sum(e => e.Amount);
                expenseSheet.Cell(row, 4).Style.NumberFormat.Format = "$#,##0.00";

                // Budgets Sheet
                var budgetSheet = workbook.Worksheets.Add("Budgets");
                budgetSheet.Cell(1, 1).Value = "Category";
                budgetSheet.Cell(1, 2).Value = "Budget Amount";
                budgetSheet.Cell(1, 3).Value = "Spent Amount";
                budgetSheet.Cell(1, 4).Value = "Remaining";
                budgetSheet.Cell(1, 5).Value = "% Used";
                row = 2;
                foreach (var budget in budgets)
                {
                budgetSheet.Cell(row, 1).Value = budget.Category?.Name ?? "N/A";
                budgetSheet.Cell(row, 2).Value = budget.Amount;
                budgetSheet.Cell(row, 3).Value = budget.SpentAmount;
                budgetSheet.Cell(row, 4).Value = budget.Amount - budget.SpentAmount;
                budgetSheet.Cell(row, 5).Value = budget.Amount != 0 ? budget.SpentAmount / budget.Amount : 0;
                row++;
                }
                var budgetRange = budgetSheet.Range(1, 1, row - 1, 5);
                budgetRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                budgetRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                budgetSheet.Column(2).Style.NumberFormat.Format = "$#,##0.00";
                budgetSheet.Column(3).Style.NumberFormat.Format = "$#,##0.00";
                budgetSheet.Column(4).Style.NumberFormat.Format = "$#,##0.00";
                budgetSheet.Column(5).Style.NumberFormat.Format = "0.0%";

                // Financial Goals Sheet
                var goalSheet = workbook.Worksheets.Add("Financial Goals");
                goalSheet.Cell(1, 1).Value = "Goal Name";
                goalSheet.Cell(1, 2).Value = "Target Amount";
                goalSheet.Cell(1, 3).Value = "Current Amount";
                goalSheet.Cell(1, 4).Value = "Status";
                row = 2;
                foreach (var goal in goals)
                {
                goalSheet.Cell(row, 1).Value = goal.Name;
                goalSheet.Cell(row, 2).Value = goal.TargetAmount;
                goalSheet.Cell(row, 3).Value = goal.CurrentAmount;
                goalSheet.Cell(row, 4).Value = goal.Status;
                row++;
                }
                goalSheet.Column(2).Style.NumberFormat.Format = "$#,##0.00";
                goalSheet.Column(3).Style.NumberFormat.Format = "$#,##0.00";

                using (var memoryStream = new MemoryStream())
                {
                workbook.SaveAs(memoryStream);
                return memoryStream.ToArray();
                }
            }
            }
        }
    }
}
