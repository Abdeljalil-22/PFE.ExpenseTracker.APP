using MediatR;
using PFE.ExpenseTracker.Application.Common.Exceptions;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Features.Expenses.Commands
{
    public class DeleteExpenseCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteExpenseCommandHandler : IRequestHandler<DeleteExpenseCommand, Result>
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IBudgetRepository _budgetRepository;

        public DeleteExpenseCommandHandler(
            IExpenseRepository expenseRepository,
            IBudgetRepository budgetRepository)
        {
            _expenseRepository = expenseRepository;
            _budgetRepository = budgetRepository;
        }

        public async Task<Result> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _expenseRepository.GetByIdAsync(request.Id);
            
            if (expense == null)
                throw new NotFoundException(nameof(Expense), request.Id);

            if (expense.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            // Update budget
            var budget = await _budgetRepository.GetBudgetByCategoryAsync(request.UserId, expense.CategoryId);
            if (budget != null)
            {
                await _budgetRepository.UpdateBudgetSpentAmountAsync(budget.Id, -expense.Amount);
            }

            await _expenseRepository.DeleteAsync(expense);
            await _expenseRepository.SaveChangesAsync();

            return Result.Success();
        }
    }
}
