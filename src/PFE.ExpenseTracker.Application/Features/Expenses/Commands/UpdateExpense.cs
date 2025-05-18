using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Exceptions;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Expenses.Commands
{
    public class UpdateExpenseCommand : IRequest<Result<ExpenseDto>>
    {
        public Guid Id { get; set; }
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

    public class UpdateExpenseCommandValidator : AbstractValidator<UpdateExpenseCommand>
    {
        public UpdateExpenseCommandValidator()
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

    public class UpdateExpenseCommandHandler : IRequestHandler<UpdateExpenseCommand, Result<ExpenseDto>>
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IBudgetRepository _budgetRepository;

        public UpdateExpenseCommandHandler(
            IExpenseRepository expenseRepository,
            ICategoryRepository categoryRepository,
            IBudgetRepository budgetRepository)
        {
            _expenseRepository = expenseRepository;
            _categoryRepository = categoryRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<Result<ExpenseDto>> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _expenseRepository.GetByIdAsync(request.Id);
            if (expense == null)
                throw new NotFoundException(nameof(Expense), request.Id);

            if (expense.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return Result<ExpenseDto>.Failure("Category not found");

            // If category changed, update both old and new budgets
            if (expense.CategoryId != request.CategoryId)
            {
                var oldBudget = await _budgetRepository.GetBudgetByCategoryAsync(request.UserId, expense.CategoryId);
                if (oldBudget != null)
                {
                    await _budgetRepository.UpdateBudgetSpentAmountAsync(oldBudget.Id, -expense.Amount);
                }

                var newBudget = await _budgetRepository.GetBudgetByCategoryAsync(request.UserId, request.CategoryId);
                if (newBudget != null)
                {
                    await _budgetRepository.UpdateBudgetSpentAmountAsync(newBudget.Id, request.Amount);
                }
            }
            // If only amount changed, update current budget
            else if (expense.Amount != request.Amount)
            {
                var budget = await _budgetRepository.GetBudgetByCategoryAsync(request.UserId, request.CategoryId);
                if (budget != null)
                {
                    var difference = request.Amount - expense.Amount;
                    await _budgetRepository.UpdateBudgetSpentAmountAsync(budget.Id, difference);
                }
            }

            expense.Description = request.Description;
            expense.Amount = request.Amount;
            expense.Date = request.Date;
            expense.CategoryId = request.CategoryId;
            expense.IsRecurring = request.IsRecurring;
            expense.RecurringFrequency = request.RecurringFrequency;
            expense.IsShared = request.IsShared;
            expense.Notes = request.Notes;

            await _expenseRepository.UpdateAsync(expense);
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
