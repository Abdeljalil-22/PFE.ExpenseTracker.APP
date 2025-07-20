using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
    public class WriteFinancialGoalRepository : WriteRepository<FinancialGoal>, IWriteFinancialGoalRepository
    {
        private readonly WriteDbContext _context;
        public WriteFinancialGoalRepository(WriteDbContext context): base(context)
        {
            _context = context;
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

        // ...other write-only methods...
    }
}
