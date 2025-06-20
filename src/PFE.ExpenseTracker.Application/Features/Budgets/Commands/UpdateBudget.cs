
using FluentValidation;
using MediatR;
using AutoMapper;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Budgets.Commands
{
    public class UpdateBudgetCommand : IRequest<Result<BudgetDto>>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Period { get; set; }
    }

    public class UpdateBudgetCommandValidator : AbstractValidator<UpdateBudgetCommand>
    {
        public UpdateBudgetCommandValidator()
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

    public class UpdateBudgetCommandHandler : IRequestHandler<UpdateBudgetCommand, Result<BudgetDto>>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public UpdateBudgetCommandHandler(
            IBudgetRepository budgetRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Result<BudgetDto>> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetByIdAsync(request.Id);
            if (budget == null)
                return Result<BudgetDto>.Failure("Budget not found");

            if (budget.UserId != request.UserId)
                return Result<BudgetDto>.Failure("Unauthorized access to budget");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return Result<BudgetDto>.Failure("Category not found");

            if (budget.CategoryId != request.CategoryId)
            {
                var existingBudget = await _budgetRepository.GetBudgetByCategoryAsync(request.UserId, request.CategoryId);
                if (existingBudget != null)
                    return Result<BudgetDto>.Failure("A budget for this category already exists");
            }

            budget.CategoryId = request.CategoryId;
            budget.Amount = request.Amount;
            budget.StartDate = request.StartDate;
            budget.EndDate = request.EndDate;
            

            await _budgetRepository.UpdateAsync(budget);
            await _budgetRepository.SaveChangesAsync();

            var budgetDto = _mapper.Map<BudgetDto>(budget);
            budgetDto.Category = _mapper.Map<CategoryDto>(category);

            return Result<BudgetDto>.Success(budgetDto);
        }
    }
}
