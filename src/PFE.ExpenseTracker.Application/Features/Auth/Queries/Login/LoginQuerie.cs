using MediatR;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Auth.Queries.Login;

    public class LoginQuerie : IRequest<Result<AuthenticationResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

   