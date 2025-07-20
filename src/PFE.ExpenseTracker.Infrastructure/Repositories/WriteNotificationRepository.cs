using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
    public class WriteNotificationRepository : WriteRepository<Notification>, IWriteNotificationRepository
    {
        private readonly WriteDbContext _context;
        public WriteNotificationRepository(WriteDbContext context): base(context)
        {
            _context = context;
        }
public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _dbSet.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
        
        // ...other write-only methods...
    }
}
