using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using AutoMapper;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Features.FinancialGoals.Commands
{
    public class AddGoalContributionCommand : IRequest<Result<FinancialGoalDto>>
    {
        public Guid FinancialGoalId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
    }

    public class AddGoalContributionCommandValidator : AbstractValidator<AddGoalContributionCommand>
    {
        public AddGoalContributionCommandValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0);

            RuleFor(x => x.Date)
                .NotEmpty();

            RuleFor(x => x.Notes)
                .MaximumLength(500);
        }
    }

    public class AddGoalContributionCommandHandler : IRequestHandler<AddGoalContributionCommand, Result<FinancialGoalDto>>
    {
        private readonly IFinancialGoalRepository _goalRepository;
        private readonly IMapper _mapper;

        public AddGoalContributionCommandHandler(IFinancialGoalRepository goalRepository, IMapper mapper)
        {
            _goalRepository = goalRepository;
            _mapper = mapper;
        }

        public async Task<Result<FinancialGoalDto>> Handle(AddGoalContributionCommand request, CancellationToken cancellationToken)
        {
            var goal = await _goalRepository.GetByIdAsync(request.FinancialGoalId);
            if (goal == null)
                return Result<FinancialGoalDto>.Failure("Financial goal not found");

            if (goal.UserId != request.UserId)
                return Result<FinancialGoalDto>.Failure("Unauthorized access to financial goal");

            if (goal.Status == "Completed")
                return Result<FinancialGoalDto>.Failure("Cannot add contribution to completed goal");

            var contribution = new GoalContribution
            {
                FinancialGoalId = request.FinancialGoalId,
                Amount = request.Amount,
                Date = request.Date,
                Notes = request.Notes
            };

            goal.Contributions.Add(contribution);
            goal.CurrentAmount += request.Amount;

            if (goal.CurrentAmount >= goal.TargetAmount)
                goal.Status = "Completed";

            await _goalRepository.UpdateAsync(goal);
            await _goalRepository.SaveChangesAsync();

            var goalDto = _mapper.Map<FinancialGoalDto>(goal);
            return Result<FinancialGoalDto>.Success(goalDto);
        }
    }
}
