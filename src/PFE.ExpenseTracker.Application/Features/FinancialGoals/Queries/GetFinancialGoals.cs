using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.FinancialGoals.Queries
{
    public class GoalContributionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
    }

    public class FinancialGoalDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public string Status { get; set; }
        public List<GoalContributionDto> Contributions { get; set; }
    }

    public class GetFinancialGoalsQuery : IRequest<Result<List<FinancialGoalDto>>>
    {
        public Guid UserId { get; set; }
        public string Status { get; set; }
    }

    public class GetFinancialGoalsQueryHandler : IRequestHandler<GetFinancialGoalsQuery, Result<List<FinancialGoalDto>>>
    {
        private readonly IFinancialGoalRepository _goalRepository;
        private readonly IMapper _mapper;

        public GetFinancialGoalsQueryHandler(IFinancialGoalRepository goalRepository, IMapper mapper)
        {
            _goalRepository = goalRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<FinancialGoalDto>>> Handle(GetFinancialGoalsQuery request, CancellationToken cancellationToken)
        {
            var goals = string.IsNullOrEmpty(request.Status)
                ? await _goalRepository.GetUserGoalsAsync(request.UserId)
                : await _goalRepository.GetUserGoalsByStatusAsync(request.UserId, request.Status);

            var goalDtos = _mapper.Map<List<FinancialGoalDto>>(goals);
            return Result<List<FinancialGoalDto>>.Success(goalDtos);
        }
    }

    public class GetFinancialGoalByIdQuery : IRequest<Result<FinancialGoalDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetFinancialGoalByIdQueryHandler : IRequestHandler<GetFinancialGoalByIdQuery, Result<FinancialGoalDto>>
    {
        private readonly IFinancialGoalRepository _goalRepository;
        private readonly IMapper _mapper;

        public GetFinancialGoalByIdQueryHandler(IFinancialGoalRepository goalRepository, IMapper mapper)
        {
            _goalRepository = goalRepository;
            _mapper = mapper;
        }

        public async Task<Result<FinancialGoalDto>> Handle(GetFinancialGoalByIdQuery request, CancellationToken cancellationToken)
        {
            var goal = await _goalRepository.GetByIdAsync(request.Id);
            if (goal == null)
                return Result<FinancialGoalDto>.Failure("Financial goal not found");

            var goalDto = _mapper.Map<FinancialGoalDto>(goal);
            return Result<FinancialGoalDto>.Success(goalDto);
        }
    }
}
