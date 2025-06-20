

using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Notifications.Commands;


public class DeleteNotificationCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}


public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, Result>
{
    private readonly INotificationRepository _notificationRepository;

    public DeleteNotificationCommandHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Result> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _notificationRepository.GetByIdAsync(request.Id);

        if (notification == null)
            return Result.Failure("Notification not found");

        if (notification.UserId != request.UserId)
            return Result.Failure("Unauthorized access");

        await _notificationRepository.DeleteAsync(notification);
        await _notificationRepository.SaveChangesAsync();

        return Result.Success();
    }
}
