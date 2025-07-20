using System;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository
{
    public interface IWriteBudgetRepository: IWriteRepository<Budget>
    {
  Task UpdateBudgetSpentAmountAsync(Guid budgetId, decimal amount);
        // ...other write-only methods...
    }
}
