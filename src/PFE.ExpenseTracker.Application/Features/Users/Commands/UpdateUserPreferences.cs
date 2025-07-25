using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Features.Users.Commands
{
    public class UpdateUserPreferencesCommand : IRequest<Result<UserPreferencesDto>>
    {
        public Guid UserId { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public bool DarkMode { get; set; }
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
    }

    public class UpdateUserPreferencesCommandValidator : AbstractValidator<UpdateUserPreferencesCommand>
    {
        public UpdateUserPreferencesCommandValidator()
        {
            RuleFor(x => x.Currency)
                .NotEmpty()
                .MaximumLength(10);

            RuleFor(x => x.Language)
                .NotEmpty()
                .MaximumLength(10);
        }
    }

    public class UpdateUserPreferencesCommandHandler : IRequestHandler<UpdateUserPreferencesCommand, Result<UserPreferencesDto>>
    {
        private readonly IReadUserRepository _readUserRepository;
        private readonly IWriteUserRepository _writeUserRepository;
        private readonly IMapper _mapper;

        public UpdateUserPreferencesCommandHandler(
            IReadUserRepository readUserRepository,
            IWriteUserRepository userRepository,
            IMapper mapper)
        {
            _readUserRepository = readUserRepository;
            _writeUserRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserPreferencesDto>> Handle(UpdateUserPreferencesCommand request, CancellationToken cancellationToken)
        {
            var user = await _readUserRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return Result<UserPreferencesDto>.Failure("User not found");

            if (user.Preferences == null)
            {
                user.Preferences = new UserPreferences();
            }
            user.Preferences.Currency = request.Currency;
            user.Preferences.Language = request.Language;
            user.Preferences.DarkMode = request.DarkMode;
            user.Preferences.EmailNotifications = request.EmailNotifications;
            user.Preferences.PushNotifications = request.PushNotifications;

            await _writeUserRepository.UpdateAsync(user);
            await _writeUserRepository.SaveChangesAsync();
            var PreferencesDto = _mapper.Map<UserPreferencesDto>(user.Preferences);

            return Result<UserPreferencesDto>.Success(
         
                PreferencesDto
                );
        }
    }
}
