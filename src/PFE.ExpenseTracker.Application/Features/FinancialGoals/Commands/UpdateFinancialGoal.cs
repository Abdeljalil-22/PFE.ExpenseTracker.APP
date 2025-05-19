using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Features.FinancialGoals.Commands
{
    public class UpdateFinancialGoalCommand : IRequest<Result<FinancialGoalDto>>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TargetAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
    }

    public class UpdateFinancialGoalCommandValidator : AbstractValidator<UpdateFinancialGoalCommand>
    {
        public UpdateFinancialGoalCommandValidator()
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

    public class UpdateFinancialGoalCommandHandler : IRequestHandler<UpdateFinancialGoalCommand, Result<FinancialGoalDto>>
    {
        private readonly IFinancialGoalRepository _goalRepository;
        private readonly IMapper _mapper;

        public UpdateFinancialGoalCommandHandler(IFinancialGoalRepository goalRepository, IMapper mapper)
        {
            _goalRepository = goalRepository;
            _mapper = mapper;
        }

        public async Task<Result<FinancialGoalDto>> Handle(UpdateFinancialGoalCommand request, CancellationToken cancellationToken)
        {
            var goal = await _goalRepository.GetByIdAsync(request.Id);
            if (goal == null)
                return Result<FinancialGoalDto>.Failure("Financial goal not found");

            if (goal.UserId != request.UserId)
                return Result<FinancialGoalDto>.Failure("Unauthorized access to financial goal");

            if (goal.Status == "Completed")
                return Result<FinancialGoalDto>.Failure("Cannot modify completed goals");

            goal.Name = request.Name;
            goal.Description = request.Description;
            goal.TargetAmount = request.TargetAmount;
            goal.StartDate = request.StartDate;
            goal.TargetDate = request.TargetDate;

            if (goal.CurrentAmount >= goal.TargetAmount)
                goal.Status = "Completed";

            await _goalRepository.UpdateAsync(goal);
            await _goalRepository.SaveChangesAsync();

            var goalDto = _mapper.Map<FinancialGoalDto>(goal);
            return Result<FinancialGoalDto>.Success(goalDto);
        }
    }
}
