using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository
{
    public interface IReadFinancialGoalRepository: IReadRepository<FinancialGoal>
    {
        Task<IEnumerable<FinancialGoal>> GetUserGoalsAsync(Guid userId);
        Task<IEnumerable<FinancialGoal>> GetUserGoalsByStatusAsync(Guid userId, string status);

        // // ...other read-only methods...
    }
}
