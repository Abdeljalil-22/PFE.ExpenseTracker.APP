using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
    }

    public interface IExpenseRepository : IRepository<Expense>
    {
        Task<IEnumerable<Expense>> GetUserExpensesAsync(Guid userId);
        Task<IEnumerable<Expense>> GetExpensesByCategoryAsync(Guid userId, Guid categoryId);
        Task<IEnumerable<Expense>> GetRecurringExpensesAsync(Guid userId);
        Task<bool> HasExpensesInCategoryAsync(Guid categoryId);
    }

    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetUserCategoriesAsync(Guid userId);
        Task<IEnumerable<Category>> GetDefaultCategoriesAsync();
        Task<Category?> GetByNameAsync(Guid userId,string name);
    }

    public interface IBudgetRepository : IRepository<Budget>
    {
        Task<IEnumerable<Budget>> GetUserBudgetsAsync(Guid userId);
        Task<Budget> GetBudgetByCategoryAsync(Guid userId, Guid categoryId);
        Task UpdateBudgetSpentAmountAsync(Guid budgetId, decimal amount);
    }

    public interface IFinancialGoalRepository : IRepository<FinancialGoal>
    {
        Task<IEnumerable<FinancialGoal>> GetUserGoalsAsync(Guid userId);
        Task<IEnumerable<FinancialGoal>> GetUserGoalsByStatusAsync(Guid userId, string status);

        Task UpdateGoalProgressAsync(Guid goalId, decimal amount);
        
    }

    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId);
    }
}
