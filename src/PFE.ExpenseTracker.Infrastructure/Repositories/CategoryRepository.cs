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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetUserCategoriesAsync(Guid userId)
        {
            return await _dbSet
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetDefaultCategoriesAsync()
        {
            return await _dbSet
                .Where(c => c.IsDefault)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public override async Task<Category> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Expenses)
                .Include(c => c.Budgets)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
