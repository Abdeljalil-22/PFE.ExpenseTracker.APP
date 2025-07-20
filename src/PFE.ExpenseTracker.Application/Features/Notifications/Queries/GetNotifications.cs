using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Notifications.Queries
{
   

    public class GetNotificationsQuery : IRequest<Result<List<NotificationDto>>>
    {
        public Guid UserId { get; set; }
        public bool UnreadOnly { get; set; }
    }

    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, Result<List<NotificationDto>>>
    {
          private readonly IReadNotificationRepository _readNotificationRepository;
        private readonly IMapper _mapper;

        public GetNotificationsQueryHandler(
            IReadNotificationRepository readNotificationRepository,
            IMapper mapper)
        {
            _readNotificationRepository = readNotificationRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = request.UnreadOnly
                ? await _readNotificationRepository.GetUnreadNotificationsAsync(request.UserId)
                : await _readNotificationRepository.GetUserNotificationsAsync(request.UserId);

            var notificationDtos = _mapper.Map<List<NotificationDto>>(notifications);
            return Result<List<NotificationDto>>.Success(notificationDtos);
        }
    }
}
