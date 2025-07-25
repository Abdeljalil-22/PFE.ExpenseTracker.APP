using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
    public class ReadCategoryRepository :ReadRepository<Category>, IReadCategoryRepository
    {
        private readonly ReadDbContext _context;
        public ReadCategoryRepository(ReadDbContext context): base(context)
        {
            _context = context;
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

        public async Task<Category?> GetByNameAsync(Guid userId, string name)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Name.ToLower() == name.ToLower());
        }

        public override async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Expenses)
                .Include(c => c.Budgets)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
  
        // ...other read-only methods...
    }
}
