using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Infrastructure.Authentication;
using PFE.ExpenseTracker.Infrastructure.Persistence;
using PFE.ExpenseTracker.Infrastructure.Repositories;
using PFE.ExpenseTracker.Infrastructure.Services;
using StackExchange.Redis;

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

            // Add Redis and chat history service
            var redisConnection = configuration.GetConnectionString("Redis");
            services.AddSingleton<IConnectionMultiplexer>(sp => 
                ConnectionMultiplexer.Connect(redisConnection ?? "localhost:6379"));
            services.AddScoped<IChatHistoryService, ChatHistoryService>();

            // Existing services
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBudgetRepository, BudgetRepository>();
            services.AddScoped<IFinancialGoalRepository, FinancialGoalRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IJwtAuthenticationService, JwtAuthenticationService>();
            services.AddScoped<INotificationService, NotificationService>();
            // services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IChatHistoryRepository, ChatHistoryRepository>();
            services.AddScoped<ApplicationDbInitializer>();

            // Register background services
            services.AddHostedService<RecurringExpenseService>();
            services.AddHostedService<BudgetAlertService>();

            // Register GeminiAIService
            services.AddScoped<IGeminiAIService,GeminiAIService>();

            // Register AIAgent service
            services.AddScoped<PFE.ExpenseTracker.AIAgent.Services.AIAgent>();
            services.AddScoped<PFE.ExpenseTracker.AIAgent.Services.IExpenseTrackerClient, PFE.ExpenseTracker.AIAgent.Services.ExpenseTrackerClient>();



            return services;
        }
    }
}
