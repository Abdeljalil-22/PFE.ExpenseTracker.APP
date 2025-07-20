using FluentValidation;

namespace PFE.ExpenseTracker.Application.Features.Auth.Queries.Login;


    public class LoginQueriesValidator : AbstractValidator<LoginQuerie>
    {
        public LoginQueriesValidator()
        {
            RuleFor(v => v.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(v => v.Password)
                .NotEmpty();
        }
    }

