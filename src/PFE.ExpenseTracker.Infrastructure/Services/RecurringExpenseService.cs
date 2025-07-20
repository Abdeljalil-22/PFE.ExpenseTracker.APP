using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Infrastructure.Services
{
    public class RecurringExpenseService : BackgroundService
    {
        private readonly ILogger<RecurringExpenseService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public RecurringExpenseService(
            ILogger<RecurringExpenseService> logger,
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
                    await ProcessRecurringExpenses();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing recurring expenses");
                }

                // Run once per day at midnight
                var now = DateTime.UtcNow;
                var tomorrow = now.Date.AddDays(1);
                var delay = tomorrow - now;
                await Task.Delay(delay, stoppingToken);
            }
        }

        private async Task ProcessRecurringExpenses()
        {
            using var scope = _serviceProvider.CreateScope();
            var readExpenseRepository  = scope.ServiceProvider.GetRequiredService<IReadExpenseRepository>();
            var writeExpenseRepository  = scope.ServiceProvider.GetRequiredService<IWriteExpenseRepository>();

            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var today = DateTime.UtcNow.Date;
            var recurringExpenses = await readExpenseRepository.GetRecurringExpensesAsync();

            foreach (var expense in recurringExpenses)
            {
                if (expense.NextRecurringDate?.Date == today)
                {
                    // Create new expense instance
                    var newExpense = new Expense
                    {
                        UserId = expense.UserId,
                        CategoryId = expense.CategoryId,
                        Description = expense.Description,
                        Amount = expense.Amount,
                        Date = today,
                        IsRecurring = true,
                        RecurringFrequency = expense.RecurringFrequency,
                        Notes = expense.Notes
                    };

                    // Set next recurring date based on frequency
                    expense.NextRecurringDate = expense.RecurringFrequency.ToLower() switch
                    {
                        "daily" => today.AddDays(1),
                        "weekly" => today.AddDays(7),
                        "monthly" => today.AddMonths(1),
                        "yearly" => today.AddYears(1),
                        _ => null
                    };

                    await writeExpenseRepository.AddAsync(newExpense);
                    await writeExpenseRepository.UpdateAsync(expense);
                    await writeExpenseRepository.SaveChangesAsync();

                    // Create notification for the next occurrence
                    if (expense.NextRecurringDate.HasValue)
                    {
                        await notificationService.CreateRecurringExpenseReminderAsync(expense);
                    }
                }
            }
        }
    }
}
