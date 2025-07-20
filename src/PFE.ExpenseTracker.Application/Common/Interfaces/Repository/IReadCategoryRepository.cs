using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository
{
    public interface IReadCategoryRepository : IReadRepository<Category>
    {

        Task<IEnumerable<Category>> GetUserCategoriesAsync(Guid userId);
                Task<IEnumerable<Category>> GetDefaultCategoriesAsync();
        Task<Category?> GetByNameAsync(Guid userId, string name);
        // ...other read-only methods...
    }
}
