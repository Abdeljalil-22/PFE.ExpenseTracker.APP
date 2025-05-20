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
    public class CreateFinancialGoalCommand : IRequest<Result<FinancialGoalDto>>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TargetAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
    }

    public class CreateFinancialGoalCommandValidator : AbstractValidator<CreateFinancialGoalCommand>
    {
        public CreateFinancialGoalCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.TargetAmount)
                .GreaterThan(0);

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .LessThan(x => x.TargetDate);

            RuleFor(x => x.TargetDate)
                .NotEmpty()
                .GreaterThan(x => x.StartDate);
        }
    }

    public class CreateFinancialGoalCommandHandler : IRequestHandler<CreateFinancialGoalCommand, Result<FinancialGoalDto>>
    {
        private readonly IFinancialGoalRepository _goalRepository;

        public CreateFinancialGoalCommandHandler(IFinancialGoalRepository goalRepository)
        {
            _goalRepository = goalRepository;
        }

        public async Task<Result<FinancialGoalDto>> Handle(CreateFinancialGoalCommand request, CancellationToken cancellationToken)
        {
            var goal = new FinancialGoal
            {
                UserId = request.UserId,
                Name = request.Name,
                Description = request.Description,
                TargetAmount = request.TargetAmount,
                CurrentAmount = 0,
                StartDate = request.StartDate,
                TargetDate = request.TargetDate,
                Status = "In Progress"
            };

            await _goalRepository.AddAsync(goal);
            await _goalRepository.SaveChangesAsync();

            return Result<FinancialGoalDto>.Success(new FinancialGoalDto
            {
                Id = goal.Id,
                Name = goal.Name,
                Description = goal.Description,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                StartDate = goal.StartDate,
                TargetDate = goal.TargetDate,
                Status = goal.Status,
                Contributions = new List<GoalContributionDto>()
            });
        }
    }
}
