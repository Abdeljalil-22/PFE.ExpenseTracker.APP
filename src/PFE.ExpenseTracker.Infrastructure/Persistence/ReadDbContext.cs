using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Infrastructure.Persistence
{
    public class ReadDbContext : DbContext
    {
        public ReadDbContext(DbContextOptions<ReadDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserPreferences> UserPreferences { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<FinancialGoal> FinancialGoals { get; set; }
        public DbSet<GoalContribution> GoalContributions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // ...existing entity configuration...
        }
    }
}
