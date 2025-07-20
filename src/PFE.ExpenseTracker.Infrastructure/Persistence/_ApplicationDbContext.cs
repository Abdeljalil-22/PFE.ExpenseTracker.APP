// using Microsoft.EntityFrameworkCore;
// using PFE.ExpenseTracker.Domain.Entities;

// namespace PFE.ExpenseTracker.Infrastructure.Persistence
// {
//     public class ApplicationDbContext : DbContext
//     {
//         public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//             : base(options)
//         {
//         }

//         public DbSet<User> Users { get; set; }
//         public DbSet<UserPreferences> UserPreferences { get; set; }
//         public DbSet<Expense> Expenses { get; set; }
//         public DbSet<Category> Categories { get; set; }
//         public DbSet<Budget> Budgets { get; set; }
//         public DbSet<FinancialGoal> FinancialGoals { get; set; }
//         public DbSet<GoalContribution> GoalContributions { get; set; }
//         public DbSet<Notification> Notifications { get; set; }
//         public DbSet<Attachment> Attachments { get; set; }
//         public DbSet<ChatHistory> ChatHistories { get; set; }

//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             base.OnModelCreating(modelBuilder);

//             modelBuilder.Entity<User>(entity =>
//             {
//                 entity.HasKey(e => e.Id);
//                 entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
//                 entity.Property(e => e.UserName).IsRequired().HasMaxLength(256);
//                 entity.Property(e => e.PasswordHash).IsRequired();
                
//                 entity.HasOne(e => e.Preferences)
//                     .WithOne(e => e.User)
//                     .HasForeignKey<UserPreferences>(e => e.UserId);
//             });

//             modelBuilder.Entity<UserPreferences>(entity =>
//             {
//                 entity.HasKey(e => e.Id);
//                 entity.Property(e => e.Currency).IsRequired().HasMaxLength(10);
//                 entity.Property(e => e.Language).IsRequired().HasMaxLength(10);
//             });

//             modelBuilder.Entity<Expense>(entity =>
//             {
//                 entity.HasKey(e => e.Id);
//                 entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
//                 entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
                
//                 entity.HasOne(e => e.User)
//                     .WithMany(e => e.Expenses)
//                     .HasForeignKey(e => e.UserId)
//                     .OnDelete(DeleteBehavior.Restrict);

//                 entity.HasOne(e => e.Category)
//                     .WithMany(e => e.Expenses)
//                     .HasForeignKey(e => e.CategoryId)
//                     .OnDelete(DeleteBehavior.Restrict);
//             });

//             modelBuilder.Entity<Category>(entity =>
//             {
//                 entity.HasKey(e => e.Id);
//                 entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                
//                 entity.HasOne(e => e.User)
//                     .WithMany(e => e.Categories)
//                     .HasForeignKey(e => e.UserId)
//                     .OnDelete(DeleteBehavior.Restrict);
//             });

//             modelBuilder.Entity<Budget>(entity =>
//             {
//                 entity.HasKey(e => e.Id);
//                 entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
//                 entity.Property(e => e.SpentAmount).HasColumnType("decimal(18,2)");
                
//                 entity.HasOne(e => e.User)
//                     .WithMany(e => e.Budgets)
//                     .HasForeignKey(e => e.UserId)
//                     .OnDelete(DeleteBehavior.Restrict);

//                 entity.HasOne(e => e.Category)
//                     .WithMany(e => e.Budgets)
//                     .HasForeignKey(e => e.CategoryId)
//                     .OnDelete(DeleteBehavior.Restrict);
//             });

//             modelBuilder.Entity<FinancialGoal>(entity =>
//             {
//                 entity.HasKey(e => e.Id);
//                 entity.Property(e => e.TargetAmount).HasColumnType("decimal(18,2)");
//                 entity.Property(e => e.CurrentAmount).HasColumnType("decimal(18,2)");
//                 entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                
//                 entity.HasOne(e => e.User)
//                     .WithMany(e => e.FinancialGoals)
//                     .HasForeignKey(e => e.UserId)
//                     .OnDelete(DeleteBehavior.Restrict);
//             });

//             modelBuilder.Entity<Notification>(entity =>
//             {
//                 entity.HasKey(e => e.Id);
//                 entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
//                 entity.Property(e => e.Message).IsRequired();
                
//                 entity.HasOne(e => e.User)
//                     .WithMany(e => e.Notifications)
//                     .HasForeignKey(e => e.UserId)
//                     .OnDelete(DeleteBehavior.Cascade);
//             });

//             modelBuilder.Entity<Attachment>(entity =>
//             {
//                 entity.HasKey(e => e.Id);
//                 entity.Property(e => e.FileName).IsRequired().HasMaxLength(256);
//                 entity.Property(e => e.FilePath).IsRequired();
                
//                 entity.HasOne(e => e.Expense)
//                     .WithMany(e => e.Attachments)
//                     .HasForeignKey(e => e.ExpenseId)
//                     .OnDelete(DeleteBehavior.Cascade);
//             });

//             modelBuilder.Entity<GoalContribution>(entity =>
//             {
//                 entity.HasKey(e => e.Id);
//                 entity.Property(e => e.Amount).HasPrecision(18, 2);
                
//                 entity.HasOne(e => e.FinancialGoal)
//                     .WithMany(e => e.Contributions)
//                     .HasForeignKey(e => e.FinancialGoalId)
//                     .OnDelete(DeleteBehavior.Cascade);
//             });
//         }
//     }
// }
