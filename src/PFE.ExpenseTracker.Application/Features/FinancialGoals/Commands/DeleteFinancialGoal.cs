using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Application.Common.Exceptions;

namespace PFE.ExpenseTracker.Application.Features.FinancialGoals.Commands
{
    public class DeleteFinancialGoalCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteFinancialGoalCommandHandler : IRequestHandler<DeleteFinancialGoalCommand, Result>
    {
        private readonly IFinancialGoalRepository _goalRepository;

        public DeleteFinancialGoalCommandHandler(IFinancialGoalRepository goalRepository)
        {
            _goalRepository = goalRepository;
        }

        public async Task<Result> Handle(DeleteFinancialGoalCommand request, CancellationToken cancellationToken)
        {
            var goal = await _goalRepository.GetByIdAsync(request.Id);
            
            if (goal == null)
                throw new NotFoundException(nameof(goal), request.Id);

            if (goal.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            if (goal.CurrentAmount > 0)
                return Result.Failure("Cannot delete goal with existing contributions");

            await _goalRepository.DeleteAsync(goal);
            await _goalRepository.SaveChangesAsync();

            return Result.Success();
        }
    }
}
