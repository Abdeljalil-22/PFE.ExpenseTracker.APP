using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Application.Common.Exceptions;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;

namespace PFE.ExpenseTracker.Application.Features.FinancialGoals.Commands
{
    public class DeleteFinancialGoalCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteFinancialGoalCommandHandler : IRequestHandler<DeleteFinancialGoalCommand, Result>
    {
       
        private readonly IReadFinancialGoalRepository _readFinancialGoalRepository;
        private readonly IWriteFinancialGoalRepository _writeFinancialGoalRepository;

        public DeleteFinancialGoalCommandHandler(
            IReadFinancialGoalRepository readFinancialGoalRepository,
            IWriteFinancialGoalRepository writeFinancialGoalRepository)
        {
            _readFinancialGoalRepository = readFinancialGoalRepository;
            _writeFinancialGoalRepository = writeFinancialGoalRepository;
        }

        public async Task<Result> Handle(DeleteFinancialGoalCommand request, CancellationToken cancellationToken)
        {
            var goal = await _readFinancialGoalRepository.GetByIdAsync(request.Id);
            
            if (goal == null)
                throw new NotFoundException(nameof(goal), request.Id);

            if (goal.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            if (goal.CurrentAmount > 0)
                return Result.Failure("Cannot delete goal with existing contributions");

            await _writeFinancialGoalRepository.DeleteAsync(goal);
            await _writeFinancialGoalRepository.SaveChangesAsync();

            return Result.Success();
        }
    }
}
