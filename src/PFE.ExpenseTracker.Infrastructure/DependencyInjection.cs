using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Infrastructure.Authentication;
using PFE.ExpenseTracker.Infrastructure.Persistence;
using PFE.ExpenseTracker.Infrastructure.Repositories;

namespace PFE.ExpenseTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBudgetRepository, BudgetRepository>();
            services.AddScoped<IFinancialGoalRepository, FinancialGoalRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IJwtAuthenticationService, JwtAuthenticationService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<ApplicationDbInitializer>();

            // Register background services
            services.AddHostedService<RecurringExpenseService>();
            services.AddHostedService<BudgetAlertService>();

            return services;
        }
    }
}
