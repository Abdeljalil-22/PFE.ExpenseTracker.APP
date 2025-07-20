using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.FinancialGoals.Queries.GetFinancialGoals;



    public class GetFinancialGoalsQueryHandler : IRequestHandler<GetFinancialGoalsQuery, Result<List<FinancialGoalDto>>>
    {
     
           private readonly IReadFinancialGoalRepository _readFinancialGoalRepository;

        private readonly IMapper _mapper;

        public GetFinancialGoalsQueryHandler(
            IReadFinancialGoalRepository readFinancialGoalRepository,
            IMapper mapper)
        {
            _readFinancialGoalRepository = readFinancialGoalRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<FinancialGoalDto>>> Handle(GetFinancialGoalsQuery request, CancellationToken cancellationToken)
        {
            var goals = string.IsNullOrEmpty(request.Status)
                ? await _readFinancialGoalRepository.GetUserGoalsAsync(request.UserId)
                : await _readFinancialGoalRepository.GetUserGoalsByStatusAsync(request.UserId, request.Status);

            var goalDtos = _mapper.Map<List<FinancialGoalDto>>(goals);
            return Result<List<FinancialGoalDto>>.Success(goalDtos);
        }
    }

 