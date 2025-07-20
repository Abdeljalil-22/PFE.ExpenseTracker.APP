using FluentValidation;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Authentication;

namespace PFE.ExpenseTracker.Application.Features.Auth.Commands.RegisterCommand;

   
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

   