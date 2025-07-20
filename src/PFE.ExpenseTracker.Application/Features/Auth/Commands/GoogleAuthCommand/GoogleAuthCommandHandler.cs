using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Authentication;

namespace PFE.ExpenseTracker.Application.Features.Auth.Commands.GoogleAuthCommand;



    public class GoogleAuthCommandHandler : IRequestHandler<GoogleAuthCommand, Result<AuthenticationResponse>>
    {
        
        private readonly IReadUserRepository _readuserRepository;
        private readonly IWriteUserRepository _writeuserRepository;
        private readonly IJwtAuthenticationService _jwtService;
        private readonly IGoogleAuthService _googleAuthService;

        public GoogleAuthCommandHandler(
            IReadUserRepository readuserRepository,
            IWriteUserRepository writeuserRepository,
            IJwtAuthenticationService jwtService,
            IGoogleAuthService googleAuthService)
        {
           _readuserRepository= readuserRepository;
            _writeuserRepository = writeuserRepository;
            _jwtService = jwtService;
            _googleAuthService = googleAuthService;
        }

        public async Task<Result<AuthenticationResponse>> Handle(GoogleAuthCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verify the Google ID token
                var googleUser = await _googleAuthService.VerifyGoogleTokenAsync(request.IdToken);
                if (googleUser == null)
                {
                    return Result<AuthenticationResponse>.Failure(new[] { "Invalid Google token" });
                }

                // Check if user exists, if not create a new one
                var user = await _readuserRepository.GetByEmailAsync(googleUser.Email);
                if (user == null)
                {
                    // Create new user


                     var addUser = new User
            {
                Id = Guid.NewGuid(),
                Email =  googleUser.Email,
                UserName = googleUser.Name,
                // PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                // FirstName = request.FirstName,
                // LastName = request.LastName,
                IsActive = true
            };

            await _writeuserRepository.AddAsync(addUser);
            await _writeuserRepository.SaveChangesAsync();
                    // user = await _userRepository.(
                    //     googleUser.Email,
                    //     Guid.NewGuid().ToString(), // Generate random password for Google users
                    //     googleUser.Name,
                    //     true); // Email is verified as it's from Google

                    if (user == null)
                    {
                        return Result<AuthenticationResponse>.Failure(new[] { "Failed to create user account" });
                    }
                }

                // Generate JWT token
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
                            Currency = user.Preferences?.Currency ?? "USD",
                            Language = user.Preferences?.Language ?? "en",
                            DarkMode = user.Preferences?.DarkMode ?? false,
                            EmailNotifications = user.Preferences?.EmailNotifications ?? true,
                            PushNotifications = user.Preferences?.PushNotifications ?? true
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return Result<AuthenticationResponse>.Failure(new[] { ex.Message });
            }
        }
    }

