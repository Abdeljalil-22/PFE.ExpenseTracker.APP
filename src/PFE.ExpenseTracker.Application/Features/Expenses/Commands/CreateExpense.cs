using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Features.Expenses.Commands
{
    public class CreateExpenseCommand : IRequest<Result<ExpenseDto>>
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsRecurring { get; set; }
        public string RecurringFrequency { get; set; }
        public bool IsShared { get; set; }
        public string Notes { get; set; }
    }

    public class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
    {
        public CreateExpenseCommandValidator()
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.Amount)
                .GreaterThan(0);

            RuleFor(x => x.Date)
                .NotEmpty();

            RuleFor(x => x.CategoryId)
                .NotEmpty();
        }
    }

    public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, Result<ExpenseDto>>
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBudgetRepository _budgetRepository;

        public CreateExpenseCommandHandler(
            IExpenseRepository expenseRepository,
            ICategoryRepository categoryRepository,
            IBudgetRepository budgetRepository)
        {
            _expenseRepository = expenseRepository;
            _categoryRepository = categoryRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<Result<ExpenseDto>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return Result<ExpenseDto>.Failure("Category not found");

            var expense = new Expense
            {
                UserId = request.UserId,
                Description = request.Description,
                Amount = request.Amount,
                Date = request.Date,
                CategoryId = request.CategoryId,
                IsRecurring = request.IsRecurring,
                RecurringFrequency = request.RecurringFrequency,
                IsShared = request.IsShared,
                Notes = request.Notes
            };

            await _expenseRepository.AddAsync(expense);

            // Update budget if exists
            var budget = await _budgetRepository.GetBudgetByCategoryAsync(request.UserId, request.CategoryId);
            if (budget != null)
            {
                await _budgetRepository.UpdateBudgetSpentAmountAsync(budget.Id, request.Amount);
            }

            await _expenseRepository.SaveChangesAsync();

            return Result<ExpenseDto>.Success(new ExpenseDto
            {
                Id = expense.Id,
                Description = expense.Description,
                Amount = expense.Amount,
                Date = expense.Date,
                IsRecurring = expense.IsRecurring,
                RecurringFrequency = expense.RecurringFrequency,
                IsShared = expense.IsShared,
                Notes = expense.Notes,
                Category = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Icon = category.Icon,
                    Color = category.Color,
                    IsDefault = category.IsDefault
                }
            });
        }
    }
}
