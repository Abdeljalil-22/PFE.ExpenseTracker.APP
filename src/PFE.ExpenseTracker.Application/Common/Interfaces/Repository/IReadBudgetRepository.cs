using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository
{
    public interface IReadBudgetRepository:IReadRepository<Budget>
    {
        // Task<Budget?> GetByIdAsync(Guid id);
        // Task<IEnumerable<Budget>> GetAllAsync();
        Task<IEnumerable<Budget>> GetUserBudgetsAsync(Guid userId);
        Task<Budget?> GetBudgetByCategoryAsync(Guid userId, Guid categoryId);
        // ...other read-only methods...
    }
}
