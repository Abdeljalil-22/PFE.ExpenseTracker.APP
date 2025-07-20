using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
    public class ReadExpenseRepository :ReadRepository<Expense>, IReadExpenseRepository
    {
        private readonly ReadDbContext _context;
        public ReadExpenseRepository(ReadDbContext context): base(context)
        {
            _context = context;
        }




        public async Task<IEnumerable<Expense>> GetUserExpensesFilteredAsync(
             Guid userId,
             Guid? categoryId = null,
             DateTime? startDate = null,
             DateTime? endDate = null,
             bool? isRecurring = null)
        {
            var query = _dbSet
                .Include(e => e.Category)
                .Include(e => e.Attachments)
                .Where(e => e.UserId == userId);

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);
            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value);
            if (isRecurring.HasValue)
                query = query.Where(e => e.IsRecurring == isRecurring.Value);

            return await query.OrderByDescending(e => e.Date).ToListAsync();
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

    public async Task<bool> HasExpensesInCategoryAsync(Guid categoryId)
    {
        return await _dbSet.AnyAsync(e => e.CategoryId == categoryId);
    }

    public override async Task<Expense?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .Include(e => e.Category)
            .Include(e => e.Attachments)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Expense>> GetRecurringExpensesAsync()
    {
        return await _dbSet
            .Include(e => e.Category)
            .Where(e => e.IsRecurring)
            .ToListAsync();
    }

      
        // ...other read-only methods...
    }
}
