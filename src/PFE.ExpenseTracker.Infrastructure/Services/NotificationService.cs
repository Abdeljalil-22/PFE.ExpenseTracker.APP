using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IWriteNotificationRepository _writeNotificationRepository;
         private readonly IReadNotificationRepository _readNotificationRepository;
        private readonly IReadBudgetRepository _budgetRepository;
        private readonly ILogger<NotificationService> _logger;
        private readonly IEmailService _emailService;
        private readonly IReadUserRepository _userRepository;

        public NotificationService(
            IWriteNotificationRepository writeNotificationRepository,
            IReadNotificationRepository readNotificationRepository,
            IReadBudgetRepository budgetRepository,
            ILogger<NotificationService> logger,
            IEmailService emailService,
            IReadUserRepository userRepository
            )
        {
            _writeNotificationRepository = writeNotificationRepository;
            _readNotificationRepository = readNotificationRepository;
            _budgetRepository = budgetRepository;
            _logger = logger;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        public async Task CreateBudgetAlertAsync(Budget budget)
        {
            try
            {
                if (budget == null)
                {
                    _logger.LogWarning("CreateBudgetAlertAsync called with null budget");
                    return;
                }

                if (budget.Amount <= 0)
                {
                    _logger.LogWarning("Budget amount is zero or negative for budget {BudgetId}", budget.Id);
                    return;
                }

                var percentageUsed = (budget.SpentAmount / budget.Amount) * 100;
                
                if (budget.AlertEnabled && percentageUsed >= budget.AlertThresholdPercentage)
                {
                var categoryName = budget.Category?.Name ?? "Unknown Category";
                // Prevent duplicate notifications for the same event
                var existing = await _readNotificationRepository.ExistsAsync(
                    budget.UserId,
                    "BudgetAlert",
                    $"You have used {percentageUsed:F1}% of your budget for {categoryName}");
                if (!existing)
                {
                    var notification = new Notification
                    {
                        UserId = budget.UserId,
                        Title = "Budget Alert",
                        Message = $"You have used {percentageUsed:F1}% of your budget for {categoryName}",
                        Type = "BudgetAlert",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _writeNotificationRepository.AddAsync(notification);
                    await _writeNotificationRepository.SaveChangesAsync();
var  user= await _userRepository.GetByIdAsync(budget.UserId);
                    if (user is not null)
                    {
                        // Send email alert
                          await _emailService.SendBudgetAlertAsync(
                       user.Email,
                       user.UserName,
                       
                        categoryName,
                        budget.SpentAmount,
                        budget.Amount);
                    }
                    
                 
                    _logger.LogInformation("Budget alert notification created for user {UserId} for category {Category}", budget.UserId, categoryName);
                }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating budget alert notification for budget {BudgetId}", budget?.Id);
                throw;
            }
        }

        public async Task CreateRecurringExpenseReminderAsync(Expense expense)
        {
            try
            {
                if (expense == null)
                {
                    _logger.LogWarning("CreateRecurringExpenseReminderAsync called with null expense");
                    return;
                }

                if (expense.IsRecurring && expense.NextRecurringDate.HasValue)
                {
                    var description = string.IsNullOrEmpty(expense.Description) ? "Unnamed expense" : expense.Description;
                var message = $"Reminder: Your recurring expense '{description}' of {expense.Amount:C} is due on {expense.NextRecurringDate:d}";
                var existing = await _readNotificationRepository.ExistsAsync(
                    expense.UserId,
                    "RecurringExpenseReminder",
                    message);
                if (!existing)
                {
                    var notification = new Notification
                    {
                        UserId = expense.UserId,
                        Title = "Recurring Expense Reminder",
                        Message = message,
                        Type = "RecurringExpenseReminder",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _writeNotificationRepository.AddAsync(notification);
                    await _writeNotificationRepository.SaveChangesAsync();

                    var  user= await _userRepository.GetByIdAsync(expense.UserId);
                    if (user is not null)
                    {
                        // Send email alert
                          await _emailService.SendExpenseAlertAsync(
                       user.Email,
                       user.UserName,
                       expense.Amount,
                        expense.Category.Name.IsNullOrEmpty()? "Uncategorized" : expense.Category.Name
                        );
                    }
                    _logger.LogInformation("Recurring expense reminder created for user {UserId} for expense {ExpenseId}", expense.UserId, expense.Id);
                }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating recurring expense reminder for expense {ExpenseId}", expense?.Id);
                throw;
            }
        }

        public async Task CreateGoalAchievedNotificationAsync(FinancialGoal goal)
        {
            try
            {
                if (goal == null)
                {
                    _logger.LogWarning("CreateGoalAchievedNotificationAsync called with null goal");
                    return;
                }

                if (goal.CurrentAmount >= goal.TargetAmount)
                {
                    var goalName = string.IsNullOrEmpty(goal.Name) ? "Unnamed goal" : goal.Name;
                var message = $"Congratulations! You have reached your goal: {goalName}";
                var existing = await _readNotificationRepository.ExistsAsync(
                    goal.UserId,
                    "GoalAchieved",
                    message);
                if (!existing)
                {
                    var notification = new Notification
                    {
                        UserId = goal.UserId,
                        Title = "Goal Achieved!",
                        Message = message,
                        Type = "GoalAchieved",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _writeNotificationRepository.AddAsync(notification);
                    await _writeNotificationRepository.SaveChangesAsync();
 var  user= await _userRepository.GetByIdAsync(notification.UserId);
                    if (user is not null)
                    {
                        // Send email alert
                          await _emailService.SendGoalAchievedAsync(
                       user.Email,
                       user.UserName,
                       goalName
                        );
                    }

                    _logger.LogInformation("Goal achieved notification created for user {UserId} for goal {GoalId}", goal.UserId, goal.Id);
                }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating goal achieved notification for goal {GoalId}", goal?.Id);
                throw;
            }
        }
    }
}
