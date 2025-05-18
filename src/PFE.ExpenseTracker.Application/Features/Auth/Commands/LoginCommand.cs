using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Infrastructure.Authentication;
// using BCrypt.Net;

namespace PFE.ExpenseTracker.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(v => v.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(v => v.Password)
                .NotEmpty();
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtAuthenticationService _jwtService;

        public LoginCommandHandler(IUserRepository userRepository, IJwtAuthenticationService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthenticationResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            // if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            // {
            //     return Result<AuthenticationResponse>.Failure("Invalid email or password");
            // }

            if (!user.IsActive)
            {
                return Result<AuthenticationResponse>.Failure("Account is disabled");
            }

            var token = _jwtService.GenerateJwtToken(user);

            return Result<AuthenticationResponse>.Success(new AuthenticationResponse
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Preferences = new UserPreferencesDto
                    {
                        Currency = user.Preferences.Currency,
                        Language = user.Preferences.Language,
                        DarkMode = user.Preferences.DarkMode,
                        EmailNotifications = user.Preferences.EmailNotifications,
                        PushNotifications = user.Preferences.PushNotifications
                    }
                }
            });
        }
    }
}
