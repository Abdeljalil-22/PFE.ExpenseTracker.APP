using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Notifications.Commands;


public class MarkAllNotificationsAsReadCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
}

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Result>
{
    private readonly INotificationRepository _notificationRepository;

    public MarkAllNotificationsAsReadCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Result> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository.GetUnreadNotificationsAsync(request.UserId);
        foreach (var notification in notifications)
        {
            await _notificationRepository.MarkAsReadAsync(notification.Id);
        }
        
        return Result.Success();
    }
}
