using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Application.Common.Exceptions;

namespace PFE.ExpenseTracker.Application.Features.Budgets.Commands
{
    public class DeleteBudgetCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand, Result>
    {
        private readonly IBudgetRepository _budgetRepository;

        public DeleteBudgetCommandHandler(IBudgetRepository budgetRepository)
        {
            _budgetRepository = budgetRepository;
        }

        public async Task<Result> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetByIdAsync(request.Id);
            
            if (budget == null)
                throw new NotFoundException(nameof(budget), request.Id);

            if (budget.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            if (budget.SpentAmount > 0)
                return Result.Failure("Cannot delete budget with tracked expenses");

            await _budgetRepository.DeleteAsync(budget);
            await _budgetRepository.SaveChangesAsync();

            return Result.Success();
        }
    }
}
