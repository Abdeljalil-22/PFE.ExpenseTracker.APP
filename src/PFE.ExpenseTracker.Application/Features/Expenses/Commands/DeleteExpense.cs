using MediatR;
using PFE.ExpenseTracker.Application.Common.Exceptions;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
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

private readonly IReadExpenseRepository _readExpenseRepository;
        private readonly IWriteExpenseRepository _writeExpenseRepository;
        private readonly IReadBudgetRepository _readBudgetRepository;
        private readonly IWriteBudgetRepository _writeBudgetRepository;



        public DeleteExpenseCommandHandler(
            IReadExpenseRepository expenseRepository,
            IWriteExpenseRepository writeExpenseRepository,
            IReadBudgetRepository budgetRepository,
            IWriteBudgetRepository writeBudgetRepository)
        {
        
            _readExpenseRepository = expenseRepository;
            _writeExpenseRepository = writeExpenseRepository;
            _readBudgetRepository = budgetRepository;
            _writeBudgetRepository = writeBudgetRepository;
        }

        public async Task<Result> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var expense = await _readExpenseRepository.GetByIdAsync(request.Id);
            
            if (expense == null)
                throw new NotFoundException(nameof(Expense), request.Id);

            if (expense.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            // Update budget
            var budget = await _readBudgetRepository.GetBudgetByCategoryAsync(request.UserId, expense.CategoryId);
            if (budget != null)
            {
                await _writeBudgetRepository.UpdateBudgetSpentAmountAsync(budget.Id, -expense.Amount);
            }

            await _writeExpenseRepository.DeleteAsync(expense);
            await _writeExpenseRepository.SaveChangesAsync();

            return Result.Success();
        }
    }
}
