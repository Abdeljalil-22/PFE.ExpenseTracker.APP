

namespace PFE.ExpenseTracker.Application.Common.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GenerateExpenseReportAsync(Guid userId, DateTime startDate, DateTime endDate, string format);
        Task<byte[]> GenerateBudgetReportAsync(Guid userId, DateTime startDate, DateTime endDate, string format);
        Task<byte[]> GenerateFinancialGoalsReportAsync(Guid userId, string format);
        Task<byte[]> GenerateAnnualSummaryReportAsync(Guid userId, int year, string format);
    }

    
}
