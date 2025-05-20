using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using AutoMapper;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Features.Budgets.Commands
{
    public class CreateBudgetCommand : IRequest<Result<BudgetDto>>
    {
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Period { get; set; }
    }

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

    public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, Result<BudgetDto>>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CreateBudgetCommandHandler(
            IBudgetRepository budgetRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Result<BudgetDto>> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return Result<BudgetDto>.Failure("Category not found");

            var existingBudget = await _budgetRepository.GetBudgetByCategoryAsync(request.UserId, request.CategoryId);
            if (existingBudget != null)
                return Result<BudgetDto>.Failure("A budget for this category already exists");

            var budget = new Budget
            {
                UserId = request.UserId,
                CategoryId = request.CategoryId,
                Amount = request.Amount,
                SpentAmount = 0,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                // Period = request.Period
            };

            await _budgetRepository.AddAsync(budget);
            await _budgetRepository.SaveChangesAsync();

            var budgetDto = _mapper.Map<BudgetDto>(budget);
            budgetDto.Category = _mapper.Map<CategoryDto>(category);
            
            return Result<BudgetDto>.Success(budgetDto);
        }
    }
}
