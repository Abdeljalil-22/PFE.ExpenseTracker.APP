using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Exceptions;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Notifications.Commands
{
    public class MarkNotificationAsReadCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.GetByIdAsync(request.Id);
            
            if (notification == null)
                throw new NotFoundException("Notification", request.Id);

            if (notification.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            await _notificationRepository.MarkAsReadAsync(request.Id);
            return Result.Success();
        }
    }
}

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
