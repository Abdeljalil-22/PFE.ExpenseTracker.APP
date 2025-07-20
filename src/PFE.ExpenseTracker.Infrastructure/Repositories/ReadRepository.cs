using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Infrastructure.Persistence;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
    public class ReadRepository<TEntity> : IReadRepository<TEntity> where TEntity : class
    {
        protected readonly DbSet<TEntity> _dbSet;

        public ReadRepository(ReadDbContext context)
        {
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }}
}
