using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Infrastructure.Authentication;

namespace PFE.ExpenseTracker.Application.Features.Auth.Queries.Login;

  
    public class LoginQuerieHandler : IRequestHandler<LoginQuerie, Result<AuthenticationResponse>>
    {
        private readonly IReadUserRepository _userRepository;
        private readonly IJwtAuthenticationService _jwtService;

        public LoginQuerieHandler(IReadUserRepository userRepository, IJwtAuthenticationService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<AuthenticationResponse>> Handle(LoginQuerie request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Result<AuthenticationResponse>.Failure("Invalid email or password");
            }

            if (!user.IsActive)
            {
                return Result<AuthenticationResponse>.Failure("Account is disabled");
            }

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
            return Result<AuthenticationResponse>.Success(response);
        }
    }

