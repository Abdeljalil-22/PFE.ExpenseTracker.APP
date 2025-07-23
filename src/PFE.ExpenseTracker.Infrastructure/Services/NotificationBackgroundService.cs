using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;

namespace PFE.ExpenseTracker.Infrastructure.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly ILogger<NotificationBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public NotificationBackgroundService(
            ILogger<NotificationBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessNotifications(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing notifications");
                }

                // Check every 15 minutes
                await Task.Delay(TimeSpan.FromMinutes(3), stoppingToken);
            }
        }

        private async Task ProcessNotifications(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var budgetRepository = scope.ServiceProvider.GetRequiredService<IReadBudgetRepository>();
            var expenseRepository = scope.ServiceProvider.GetRequiredService<IReadExpenseRepository>();
            var goalRepository = scope.ServiceProvider.GetRequiredService<IReadFinancialGoalRepository>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            // Check budgets
            var budgets = await budgetRepository.GetAllAsync();
            foreach (var budget in budgets)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    await notificationService.CreateBudgetAlertAsync(budget);
                }
            }

            // Check recurring expenses
            var expenses = await expenseRepository.GetAllAsync();
            foreach (var expense in expenses)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    await notificationService.CreateRecurringExpenseReminderAsync(expense);
                }
            }

            // Check financial goals
            var goals = await goalRepository.GetAllAsync();
            foreach (var goal in goals)
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    await notificationService.CreateGoalAchievedNotificationAsync(goal);
                }
            }
        }
    }
}
