using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PFE.ExpenseTracker.Application.Common.Interfaces;

namespace PFE.ExpenseTracker.Infrastructure.Services
{
    public class BudgetAlertService : BackgroundService
    {
        private readonly ILogger<BudgetAlertService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BudgetAlertService(
            ILogger<BudgetAlertService> logger,
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
                    await CheckBudgetThresholds();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error checking budget thresholds");
                }

                // Run every hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task CheckBudgetThresholds()
        {
            using var scope = _serviceProvider.CreateScope();
            var budgetRepository = scope.ServiceProvider.GetRequiredService<IBudgetRepository>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var budgets = await budgetRepository.GetAllAsync();
            foreach (var budget in budgets)
            {
                if (budget.AlertEnabled)
                {
                    var percentageUsed = (budget.SpentAmount / budget.Amount) * 100;
                    if (percentageUsed >= budget.AlertThresholdPercentage)
                    {
                        await notificationService.CreateBudgetAlertAsync(budget);
                    }
                }
            }
        }
    }
}
