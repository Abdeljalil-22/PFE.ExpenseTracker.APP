using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Application.Common.Exceptions;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;

namespace PFE.ExpenseTracker.Application.Features.Budgets.Commands.DeleteBudget;

    public class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand, Result>
    {
        private readonly IReadBudgetRepository _readbudgetRepository;
        private readonly IWriteBudgetRepository _writebudgetRepository;

        public DeleteBudgetCommandHandler(IReadBudgetRepository budgetRepository , IWriteBudgetRepository writeBudgetRepository)
        {
            _readbudgetRepository = budgetRepository;
            _writebudgetRepository = writeBudgetRepository;
        
        }

        public async Task<Result> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
        {
            var budget = await _readbudgetRepository.GetByIdAsync(request.Id);
            
            if (budget == null)
                throw new NotFoundException(nameof(budget), request.Id);

            if (budget.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            if (budget.SpentAmount > 0)
                return Result.Failure("Cannot delete budget with tracked expenses");

            await _writebudgetRepository.DeleteAsync(budget);
            await _writebudgetRepository.SaveChangesAsync();

            return Result.Success();
        }
   
}
