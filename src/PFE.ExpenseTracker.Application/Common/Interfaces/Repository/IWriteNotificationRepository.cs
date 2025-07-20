using System;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository
{
    public interface IWriteNotificationRepository : IWriteRepository<Notification>
    {
        Task MarkAsReadAsync(Guid notificationId);
        
        // ...other write-only methods...
    }
}
