using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Infrastructure.Authentication;
using PFE.ExpenseTracker.Infrastructure.Persistence;
using PFE.ExpenseTracker.Infrastructure.Repositories;
using PFE.ExpenseTracker.Infrastructure.Services;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using StackExchange.Redis;

namespace PFE.ExpenseTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
             {
            // Register WriteDbContext for commands
            services.AddDbContext<WriteDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(WriteDbContext).Assembly.FullName)));

            // Register ReadDbContext for queries (can use a read replica connection string if available)
            services.AddDbContext<ReadDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("ReadConnection") ?? configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ReadDbContext).Assembly.FullName)));

            // Add Redis and chat history service
            var redisConnection = configuration.GetConnectionString("Redis");
            services.AddSingleton<IConnectionMultiplexer>(sp => 
                ConnectionMultiplexer.Connect(redisConnection ?? "localhost:6379"));
            services.AddScoped<IChatHistoryService, ChatHistoryService>();

            // Existing services
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddScoped<IJwtService, JwtService>();
            // services.AddScoped<IIdentityService, IdentityService>();

              services.AddScoped<IJwtAuthenticationService, JwtAuthenticationService>();
            services.AddScoped<INotificationService, NotificationService>();
            // services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IReportService, ReportService>();

            // Register CQRS repositories

            services.AddScoped<IReadExpenseRepository, ReadExpenseRepository>();
            services.AddScoped<IWriteExpenseRepository, WriteExpenseRepository>();

            services.AddScoped<IReadCategoryRepository, ReadCategoryRepository>();
            services.AddScoped<IWriteCategoryRepository, WriteCategoryRepository>();

            services.AddScoped<IReadUserRepository, ReadUserRepository>();
            services.AddScoped<IWriteUserRepository, WriteUserRepository>();

            services.AddScoped<IReadFinancialGoalRepository, ReadFinancialGoalRepository>();
            services.AddScoped<IWriteFinancialGoalRepository, WriteFinancialGoalRepository>();

            services.AddScoped<IReadNotificationRepository, ReadNotificationRepository>();
            services.AddScoped<IWriteNotificationRepository, WriteNotificationRepository>();
       
            services.AddScoped<IReadBudgetRepository, ReadBudgetRepository>();
            services.AddScoped<IWriteBudgetRepository, WriteBudgetRepository>();

          
            services.AddScoped<IReadChatHistoryRepository, ReadChatHistoryRepository>();
            services.AddScoped<IWriteChatHistoryRepository, WriteChatHistoryRepository>();
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
