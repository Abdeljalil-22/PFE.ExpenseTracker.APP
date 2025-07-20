using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Notifications.Commands;


public class MarkAllNotificationsAsReadCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
}

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, Result>
{
   private readonly IReadNotificationRepository _readNotificationRepository;
    private readonly IWriteNotificationRepository _writeNotificationRepository;

    public MarkAllNotificationsAsReadCommandHandler(
        IReadNotificationRepository readNotificationRepository,
        IWriteNotificationRepository writeNotificationRepository)
    {
        _readNotificationRepository = readNotificationRepository;
        _writeNotificationRepository = writeNotificationRepository;
    }
    public async Task<Result> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var notifications = await _readNotificationRepository.GetUnreadNotificationsAsync(request.UserId);
        foreach (var notification in notifications)
        {
            await _writeNotificationRepository.MarkAsReadAsync(notification.Id);
        }
        
        return Result.Success();
    }
}
