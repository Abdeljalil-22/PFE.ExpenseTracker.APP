using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Infrastructure.Persistence
{
    public class ApplicationDbInitializer
    {
        private readonly ILogger<ApplicationDbInitializer> _logger;
        private readonly WriteDbContext _context;

        public ApplicationDbInitializer(
            ILogger<ApplicationDbInitializer> logger,
            WriteDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task TrySeedAsync()
        {
            // Ensure database is up to date
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }

            // Default User
            var defaultUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin@expensetracker.com");
            if (defaultUser == null)
            {
                defaultUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@expensetracker.com",
                    UserName = "admin",
                    FirstName = "Admin",
                    LastName = "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    PasswordHash = "$2a$11$zdJ4tWuvNuReN1ksiBow1u.4t3ie9gqfe6k73/mvh2UcVF9lSIaae" // admin
                };
                await _context.Users.AddAsync(defaultUser);
                await _context.SaveChangesAsync();
            }

            // Default Categories
            if (!_context.Categories.Any(c => c.IsDefault))
            {
                var defaultCategories = new[]
                {
                    new Category { Name = "Food & Dining", Description = "Restaurants, groceries, and dining", Icon = "🍽️", Color = "#FF4081", IsDefault = true, UserId = defaultUser.Id },
                    new Category { Name = "Transportation", Description = "Public transport, fuel, and vehicle expenses", Icon = "🚗", Color = "#2196F3", IsDefault = true, UserId = defaultUser.Id },
                    new Category { Name = "Housing", Description = "Rent, utilities, and maintenance", Icon = "🏠", Color = "#4CAF50", IsDefault = true, UserId = defaultUser.Id },
                    new Category { Name = "Entertainment", Description = "Movies, games, and leisure activities", Icon = "🎮", Color = "#9C27B0", IsDefault = true, UserId = defaultUser.Id },
                    new Category { Name = "Shopping", Description = "Clothing, accessories, and personal items", Icon = "🛍️", Color = "#FF9800", IsDefault = true, UserId = defaultUser.Id },
                    new Category { Name = "Healthcare", Description = "Medical expenses and healthcare", Icon = "⚕️", Color = "#F44336", IsDefault = true, UserId = defaultUser.Id },
                    new Category { Name = "Education", Description = "Courses, books, and learning materials", Icon = "📚", Color = "#795548", IsDefault = true, UserId = defaultUser.Id },
                    new Category { Name = "Bills & Utilities", Description = "Regular bills and utility payments", Icon = "📱", Color = "#607D8B", IsDefault = true, UserId = defaultUser.Id },
                };

                foreach (var category in defaultCategories)
                {
                    category.Id = Guid.NewGuid();
                }

                await _context.Categories.AddRangeAsync(defaultCategories);
                await _context.SaveChangesAsync();
            }
        }
    }
}
