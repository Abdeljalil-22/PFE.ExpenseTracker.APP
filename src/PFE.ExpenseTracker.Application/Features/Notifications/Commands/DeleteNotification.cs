

using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Notifications.Commands;


public class DeleteNotificationCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}


public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, Result>
{
    private readonly IReadNotificationRepository _readNotificationRepository;
    private readonly IWriteNotificationRepository _writeNotificationRepository;

    public DeleteNotificationCommandHandler(
        IReadNotificationRepository readNotificationRepository,
        IWriteNotificationRepository writeNotificationRepository)
    {
        _readNotificationRepository = readNotificationRepository;
        _writeNotificationRepository = writeNotificationRepository;
    }

    public async Task<Result> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _readNotificationRepository.GetByIdAsync(request.Id);

        if (notification == null)
            return Result.Failure("Notification not found");

        if (notification.UserId != request.UserId)
            return Result.Failure("Unauthorized access");

        await _writeNotificationRepository.DeleteAsync(notification);
        await _writeNotificationRepository.SaveChangesAsync();

        return Result.Success();
    }
}
