using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
    public class BudgetRepository : Repository<Budget>, IBudgetRepository
    {
        public BudgetRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Budget>> GetUserBudgetsAsync(Guid userId)
        {
            return await _dbSet
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.StartDate)
                .ToListAsync();
        }

        public async Task<Budget> GetBudgetByCategoryAsync(Guid userId, Guid categoryId)
        {
            return await _dbSet
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == categoryId);
        }

        public async Task UpdateBudgetSpentAmountAsync(Guid budgetId, decimal amount)
        {
            var budget = await _dbSet.FindAsync(budgetId);
            if (budget != null)
            {
                budget.SpentAmount += amount;
                await _context.SaveChangesAsync();
            }
        }
    }
}
