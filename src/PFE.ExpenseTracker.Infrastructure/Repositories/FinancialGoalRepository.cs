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
    public class FinancialGoalRepository : Repository<FinancialGoal>, IFinancialGoalRepository
    {
        public FinancialGoalRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FinancialGoal>> GetUserGoalsAsync(Guid userId)
        {
            return await _dbSet
                .Include(g => g.Contributions)
                .Where(g => g.UserId == userId)
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

        public override async Task<FinancialGoal> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(g => g.Contributions)
                .FirstOrDefaultAsync(g => g.Id == id);
        }
    }
}
