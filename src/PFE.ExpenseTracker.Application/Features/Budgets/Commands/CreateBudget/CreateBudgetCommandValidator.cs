using FluentValidation;


namespace PFE.ExpenseTracker.Application.Features.Budgets.Commands.CreateBudget;



    public class CreateBudgetCommandValidator : AbstractValidator<CreateBudgetCommand>
    {
        public CreateBudgetCommandValidator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty();

            RuleFor(x => x.Amount)
                .GreaterThan(0);

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .LessThan(x => x.EndDate);

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .GreaterThan(x => x.StartDate);

            RuleFor(x => x.Period)
                .NotEmpty()
                .Must(x => x is "Monthly" or "Yearly" or "Weekly")
                .WithMessage("Period must be either Weekly, Monthly, or Yearly");
        }
    }
