using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Users.Queries;


    public class GetUserPreferencesQuery : IRequest<Result<UserPreferencesDto>>
    {
        public Guid UserId { get; set; }
    }

    public class GetUserPreferencesQueryHandler : IRequestHandler<GetUserPreferencesQuery, Result<UserPreferencesDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserPreferencesQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserPreferencesDto>> Handle(GetUserPreferencesQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return Result<UserPreferencesDto>.Failure("Preferences not found");

            var PreferencesDto = _mapper.Map<UserPreferencesDto>(user.Preferences);
            return Result<UserPreferencesDto>.Success(PreferencesDto);
        }
    }
