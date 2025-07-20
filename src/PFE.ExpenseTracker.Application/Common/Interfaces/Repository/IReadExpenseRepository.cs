using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository
{
    public interface IReadExpenseRepository: IReadRepository<Expense>
    {

         Task<IEnumerable<Expense>> GetUserExpensesAsync(Guid userId);
        Task<IEnumerable<Expense>> GetExpensesByCategoryAsync(Guid userId, Guid categoryId);
        Task<IEnumerable<Expense>> GetRecurringExpensesAsync(Guid userId);
        Task<bool> HasExpensesInCategoryAsync(Guid categoryId);

        Task<IEnumerable<Expense>> GetUserExpensesFilteredAsync(
            Guid userId,
            Guid? categoryId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? isRecurring = null
        );
        Task<IEnumerable<Expense>> GetRecurringExpensesAsync();
        // ...other read-only methods...
    }
}
