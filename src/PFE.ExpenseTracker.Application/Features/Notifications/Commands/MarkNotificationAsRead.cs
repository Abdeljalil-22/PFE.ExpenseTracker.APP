using MediatR;
using PFE.ExpenseTracker.Application.Common.Exceptions;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Notifications.Commands;

    public class MarkNotificationAsReadCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, Result>
    {  private readonly IReadNotificationRepository _readNotificationRepository;
    private readonly IWriteNotificationRepository _writeNotificationRepository;
        public MarkNotificationAsReadCommandHandler(
            IReadNotificationRepository readNotificationRepository,
            IWriteNotificationRepository writeNotificationRepository)
        {
            _readNotificationRepository = readNotificationRepository;
            _writeNotificationRepository = writeNotificationRepository;
        }

        public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _readNotificationRepository.GetByIdAsync(request.Id);
            
            if (notification == null)
                throw new NotFoundException("Notification", request.Id);

            if (notification.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            await _writeNotificationRepository.MarkAsReadAsync(request.Id);
            return Result.Success();
        }
    }

