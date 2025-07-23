using FluentValidation;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Authentication;

namespace PFE.ExpenseTracker.Application.Features.Auth.Commands.RegisterCommand;


    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthenticationResponse>>
    {
        private readonly IReadUserRepository _readuserRepository;
        private readonly IWriteUserRepository _writeuserRepository;
        private readonly IJwtAuthenticationService _jwtService;
        private readonly IEmailService _emailService;

        public RegisterCommandHandler(
            IReadUserRepository readuserRepository,
            IWriteUserRepository writeuserRepository,
            IJwtAuthenticationService jwtService,
            IEmailService emailService)
        {
            _readuserRepository = readuserRepository;
            _writeuserRepository = writeuserRepository;
            _jwtService = jwtService;
            _emailService = emailService;
        }
        public async Task<Result<AuthenticationResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _readuserRepository.EmailExistsAsync(request.Email))
            {
                return Result<AuthenticationResponse>.Failure("Email already exists");
            }

            if (await _readuserRepository.UsernameExistsAsync(request.UserName))
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

            await _writeuserRepository.AddAsync(user);
            await _writeuserRepository.SaveChangesAsync();

            var token = _jwtService.GenerateJwtToken(user);
            var response = new AuthenticationResponse
            {
                Token = token,
                User= new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }
            };
             await _emailService.SendWelcomeEmailAsync(user.Email, user.UserName);

            return Result<AuthenticationResponse>.Success(response);
        }
    }

