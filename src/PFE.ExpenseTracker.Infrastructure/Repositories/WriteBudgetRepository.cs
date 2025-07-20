using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
    public class WriteBudgetRepository :WriteRepository<Budget>, IWriteBudgetRepository
    {
        private readonly WriteDbContext _context;
        public WriteBudgetRepository(WriteDbContext context): base(context)
        {
            _context = context;
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
        // ...other write-only methods...
    }
}
