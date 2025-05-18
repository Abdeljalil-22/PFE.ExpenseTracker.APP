using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Common.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateExpenseReportAsync(Guid userId, DateTime startDate, DateTime endDate, string format);
        Task<byte[]> GenerateBudgetReportAsync(Guid userId, DateTime startDate, DateTime endDate, string format);
        Task<byte[]> GenerateFinancialGoalsReportAsync(Guid userId, string format);
        Task<byte[]> GenerateAnnualSummaryReportAsync(Guid userId, int year, string format);
    }

    public class ReportOptions
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Format { get; set; } = "PDF";
        public bool IncludeCharts { get; set; } = true;
        public string[] Categories { get; set; }
        public string Currency { get; set; }
    }
}
