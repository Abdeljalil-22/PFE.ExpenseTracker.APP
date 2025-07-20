using FluentValidation;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Authentication;

namespace PFE.ExpenseTracker.Application.Features.Auth.Commands.RegisterCommand;

    public class RegisterCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

   