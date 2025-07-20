using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
    public class ReadFinancialGoalRepository : ReadRepository<FinancialGoal>, IReadFinancialGoalRepository
    {
        private readonly ReadDbContext _context;
        public ReadFinancialGoalRepository(ReadDbContext context): base(context)
        {
            _context = context;
        }

         public override async Task<FinancialGoal?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(g => g.Contributions)
                .FirstOrDefaultAsync(g => g.Id == id);
        }
 public async Task<IEnumerable<FinancialGoal>> GetUserGoalsAsync(Guid userId)
        {
            return await _dbSet
                .Where(g => g.UserId == userId)
                .OrderBy(g => g.TargetDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FinancialGoal>> GetUserGoalsByStatusAsync(Guid userId, string status)
        {
            return await _dbSet
                .Include(g => g.Contributions)
                .Where(g => g.UserId == userId && g.Status == status)
                .OrderByDescending(g => g.StartDate)
                .ToListAsync();
        }

        public async Task UpdateGoalProgressAsync(Guid goalId, decimal amount)
        {
            var goal = await _dbSet.FindAsync(goalId);
            if (goal != null)
            {
                goal.CurrentAmount += amount;
                if (goal.CurrentAmount >= goal.TargetAmount)
                {
                    goal.Status = "Completed";
                }
                await _context.SaveChangesAsync();
            }
        }

     
        // ...other read-only methods...
    }
}
