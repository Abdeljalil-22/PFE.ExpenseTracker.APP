using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
    public class ReadUserRepository :ReadRepository<User>, IReadUserRepository
    {
        private readonly ReadDbContext _context;
        public ReadUserRepository(ReadDbContext context): base(context)
        {
            _context = context;
        }


        public override async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Preferences)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await  _context.Users
                .Include(u => u.Preferences)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await  _context.Users
                .Include(u => u.Preferences)
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await  _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await  _context.Users.AnyAsync(u => u.UserName == username);
        }
        // ...other read-only methods...
    }
}
