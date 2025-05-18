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
        private readonly ApplicationDbContext _context;

        public ApplicationDbInitializer(
            ILogger<ApplicationDbInitializer> logger,
            ApplicationDbContext context)
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
            // Default Categories
            if (!_context.Categories.Any(c => c.IsDefault))
            {
                var defaultCategories = new[]
                {
                    new Category { Name = "Food & Dining", Description = "Restaurants, groceries, and dining", Icon = "🍽️", Color = "#FF4081", IsDefault = true },
                    new Category { Name = "Transportation", Description = "Public transport, fuel, and vehicle expenses", Icon = "🚗", Color = "#2196F3", IsDefault = true },
                    new Category { Name = "Housing", Description = "Rent, utilities, and maintenance", Icon = "🏠", Color = "#4CAF50", IsDefault = true },
                    new Category { Name = "Entertainment", Description = "Movies, games, and leisure activities", Icon = "🎮", Color = "#9C27B0", IsDefault = true },
                    new Category { Name = "Shopping", Description = "Clothing, accessories, and personal items", Icon = "🛍️", Color = "#FF9800", IsDefault = true },
                    new Category { Name = "Healthcare", Description = "Medical expenses and healthcare", Icon = "⚕️", Color = "#F44336", IsDefault = true },
                    new Category { Name = "Education", Description = "Courses, books, and learning materials", Icon = "📚", Color = "#795548", IsDefault = true },
                    new Category { Name = "Bills & Utilities", Description = "Regular bills and utility payments", Icon = "📱", Color = "#607D8B", IsDefault = true },
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
