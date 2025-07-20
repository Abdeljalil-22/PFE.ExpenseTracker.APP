using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository
{
    public interface IReadNotificationRepository: IReadRepository<Notification>
    {  Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(Guid userId);
        // ...other read-only methods...
    }
}
