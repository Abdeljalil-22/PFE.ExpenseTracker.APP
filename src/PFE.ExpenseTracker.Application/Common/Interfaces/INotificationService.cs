using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task CreateBudgetAlertAsync(Budget budget);
        Task CreateRecurringExpenseReminderAsync(Expense expense);
        Task CreateGoalAchievedNotificationAsync(FinancialGoal goal);
    }
}
