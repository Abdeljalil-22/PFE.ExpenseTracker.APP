using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
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

        private readonly IWriteExpenseRepository _writeExpenseRepository;
        private readonly IReadBudgetRepository _readBudgetRepository;
        private readonly IWriteBudgetRepository _writeBudgetRepository;
        private readonly IReadCategoryRepository _readCategoryRepository;

        public CreateExpenseCommandHandler(
            IReadExpenseRepository expenseRepository,
            IWriteExpenseRepository writeExpenseRepository,
            IReadBudgetRepository budgetRepository,
            IWriteBudgetRepository writeBudgetRepository,
            IReadCategoryRepository categoryRepository)
        {
            _writeExpenseRepository = writeExpenseRepository;
            _readBudgetRepository = budgetRepository;
            _writeBudgetRepository = writeBudgetRepository;
            _readCategoryRepository = categoryRepository;
        }

        public async Task<Result<ExpenseDto>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            var category = await _readCategoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return Result<ExpenseDto>.Failure("Category not found");

            var budget = await _readBudgetRepository.GetBudgetByCategoryAsync(request.UserId, request.CategoryId);
            if (budget != null)
            {
                await _writeBudgetRepository.UpdateBudgetSpentAmountAsync(budget.Id, request.Amount);
            }

            var expense = new Domain.Entities.Expense
            {
                Id = Guid.NewGuid(),
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
            await _writeExpenseRepository.AddAsync(expense);
            await _writeExpenseRepository.SaveChangesAsync();

            var dto = new ExpenseDto
            {
                Id = expense.Id,
                Description = expense.Description,
                Amount = expense.Amount,
                Date = expense.Date,
                Category= new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    
                },
                IsRecurring = expense.IsRecurring,
                RecurringFrequency = expense.RecurringFrequency,
                IsShared = expense.IsShared,
                Notes = expense.Notes
            };
            return Result<ExpenseDto>.Success(dto);
        }
    }
}
