using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;
using BC = BCrypt.Net.BCrypt;

namespace PFE.ExpenseTracker.Infrastructure.Authentication
{
    public class IdentityService : IIdentityService
    {
        private readonly ApplicationDbContext _context;

        public IdentityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Preferences)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> CreateUserAsync(string email, string password, string name, bool isEmailVerified = false)
        {
            var hashedPassword = BC.HashPassword(password);
            
            var user = new User
            {
                Email = email,
                PasswordHash = hashedPassword,
                UserName = name,
                EmailVerified = isEmailVerified,
                CreatedAt = DateTime.UtcNow,
                Preferences = new UserPreferences
                {
                    Currency = "USD",
                    Language = "en",
                    DarkMode = false,
                    EmailNotifications = true,
                    PushNotifications = true
                }
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
