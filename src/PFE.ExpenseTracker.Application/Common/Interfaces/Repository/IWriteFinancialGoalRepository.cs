using System;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository
{
    public interface IWriteFinancialGoalRepository: IWriteRepository<FinancialGoal>
    {
        Task UpdateGoalProgressAsync(Guid goalId, decimal amount);
        // ...other write-only methods...
    }
}
