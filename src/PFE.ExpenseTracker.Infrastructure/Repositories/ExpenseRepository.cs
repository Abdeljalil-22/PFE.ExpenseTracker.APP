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
    public class ExpenseRepository : Repository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Expense>> GetUserExpensesAsync(Guid userId)
        {
            return await _dbSet
                .Include(e => e.Category)
                .Include(e => e.Attachments)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetExpensesByCategoryAsync(Guid userId, Guid categoryId)
        {
            return await _dbSet
                .Include(e => e.Category)
                .Include(e => e.Attachments)
                .Where(e => e.UserId == userId && e.CategoryId == categoryId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetRecurringExpensesAsync(Guid userId)
        {
            return await _dbSet
                .Include(e => e.Category)
                .Where(e => e.UserId == userId && e.IsRecurring)
                .OrderBy(e => e.NextRecurringDate)
                .ToListAsync();
        }

        public override async Task<Expense> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(e => e.Category)
                .Include(e => e.Attachments)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
