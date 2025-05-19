using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Authentication;

namespace PFE.ExpenseTracker.Application.Features.Auth.Commands
{
    public class RegisterCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(v => v.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(256);

            RuleFor(v => v.UserName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(256);

            RuleFor(v => v.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(v => v.FirstName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(v => v.LastName)
                .NotEmpty()
                .MaximumLength(100);
        }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthenticationResponse>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtAuthenticationService _jwtService;

        public RegisterCommandHandler(IUserRepository userRepository, IJwtAuthenticationService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthenticationResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                return Result<AuthenticationResponse>.Failure("Email already exists");
            }

            if (await _userRepository.UsernameExistsAsync(request.UserName))
            {
                return Result<AuthenticationResponse>.Failure("Username already exists");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                UserName = request.UserName,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            var token = _jwtService.GenerateJwtToken(user);
            var response = new AuthenticationResponse
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Token = token
            };
            return Result<AuthenticationResponse>.Success(response);
        }
    }
}
