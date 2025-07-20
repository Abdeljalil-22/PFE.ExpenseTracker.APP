using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
    public class WriteUserRepository : WriteRepository<User>,IWriteUserRepository
    {
        private readonly WriteDbContext _context;
        public WriteUserRepository(WriteDbContext context): base(context)
        {
            _context = context;
        }

       
        // ...other write-only methods...
    }
}
