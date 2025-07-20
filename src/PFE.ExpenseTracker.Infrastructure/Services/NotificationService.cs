using System;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IWriteNotificationRepository _writenotificationRepository;
        

        public NotificationService(IWriteNotificationRepository writenotificationRepository)
        {
            _writenotificationRepository = writenotificationRepository;
        }
        public async Task CreateBudgetAlertAsync(Budget budget)
        {
            var percentageUsed = (budget.SpentAmount / budget.Amount) * 100;
            if (budget.AlertEnabled && percentageUsed >= budget.AlertThresholdPercentage)
            {
                var notification = new Notification
                {
                    UserId = budget.UserId,
                    Title = "Budget Alert",
                    Message = $"You have used {percentageUsed:F1}% of your budget for {budget.Category.Name}",
                    Type = "BudgetAlert",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _writenotificationRepository.AddAsync(notification);
                await _writenotificationRepository.SaveChangesAsync();
            }
        }

        public async Task CreateRecurringExpenseReminderAsync(Expense expense)
        {
            if (expense.IsRecurring && expense.NextRecurringDate.HasValue)
            {
                var notification = new Notification
                {
                    UserId = expense.UserId,
                    Title = "Recurring Expense Reminder",
                    Message = $"Reminder: Your recurring expense '{expense.Description}' of {expense.Amount:C} is due on {expense.NextRecurringDate:d}",
                    Type = "RecurringExpenseReminder",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _writenotificationRepository.AddAsync(notification);
                await _writenotificationRepository.SaveChangesAsync();
            }
        }

        public async Task CreateGoalAchievedNotificationAsync(FinancialGoal goal)
        {
            if (goal.CurrentAmount >= goal.TargetAmount)
            {
                var notification = new Notification
                {
                    UserId = goal.UserId,
                    Title = "Goal Achieved!",
                    Message = $"Congratulations! You have reached your goal: {goal.Name}",
                    Type = "GoalAchieved",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _writenotificationRepository.AddAsync(notification);
                await _writenotificationRepository.SaveChangesAsync();
            }
        }
    }
}
