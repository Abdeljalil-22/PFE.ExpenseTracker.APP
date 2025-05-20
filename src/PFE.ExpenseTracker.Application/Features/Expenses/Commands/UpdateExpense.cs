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
                return Result<ExpenseDto>.Failure("Expense not found");
            if (expense.UserId != request.UserId)
                return Result<ExpenseDto>.Failure("Unauthorized");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return Result<ExpenseDto>.Failure("Category not found");

            // Optionally update budget spent amount if amount or category changed
            if (expense.Amount != request.Amount || expense.CategoryId != request.CategoryId)
            {
                var oldBudget = await _budgetRepository.GetBudgetByCategoryAsync(request.UserId, expense.CategoryId);
                if (oldBudget != null)
                    await _budgetRepository.UpdateBudgetSpentAmountAsync(oldBudget.Id, -expense.Amount);
                var newBudget = await _budgetRepository.GetBudgetByCategoryAsync(request.UserId, request.CategoryId);
                if (newBudget != null)
                    await _budgetRepository.UpdateBudgetSpentAmountAsync(newBudget.Id, request.Amount);
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

            var dto = new ExpenseDto
            {
                Id = expense.Id,
                // UserId = expense.UserId,
                Description = expense.Description,
                Amount = expense.Amount,
                Date = expense.Date,
                // CategoryId = expense.CategoryId,
                IsRecurring = expense.IsRecurring,
                RecurringFrequency = expense.RecurringFrequency,
                IsShared = expense.IsShared,
                Notes = expense.Notes
            };
            return Result<ExpenseDto>.Success(dto);
        }
    }
}
