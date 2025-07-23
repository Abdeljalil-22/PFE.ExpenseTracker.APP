using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
    public class ReadBudgetRepository : IReadBudgetRepository
    {
        private readonly ReadDbContext _context;
        public ReadBudgetRepository(ReadDbContext context)
        {
            _context = context;
        }

        public async Task<Budget?> GetByIdAsync(Guid id)
            => await _context.Budgets.FindAsync(id);

        public async Task<IEnumerable<Budget>> GetAllAsync()
            => await _context.Budgets.Include(i=>i.Category).ToListAsync();

        public async Task<IEnumerable<Budget>> GetUserBudgetsAsync(Guid userId)
            => await _context.Budgets.Include(i=>i.Category).Where(b => b.UserId == userId).ToListAsync();

        public async Task<Budget?> GetBudgetByCategoryAsync(Guid userId, Guid categoryId)
            => await _context.Budgets.FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == categoryId);
        // ...other read-only methods...
    }
}
