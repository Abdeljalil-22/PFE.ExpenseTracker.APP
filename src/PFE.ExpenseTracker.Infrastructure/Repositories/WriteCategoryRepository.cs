using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
    public class WriteCategoryRepository :WriteRepository<Category>, IWriteCategoryRepository
    {
        private readonly WriteDbContext _context;
        public WriteCategoryRepository(WriteDbContext context): base(context)
        {
            _context = context;
        }

       
        // ...other write-only methods...
    }
}
