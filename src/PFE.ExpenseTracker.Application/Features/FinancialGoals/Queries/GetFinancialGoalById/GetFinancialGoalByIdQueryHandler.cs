using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.FinancialGoals.Queries.GetFinancialGoalById;


    public class GetFinancialGoalByIdQueryHandler : IRequestHandler<GetFinancialGoalByIdQuery, Result<FinancialGoalDto>>
    {
           private readonly IReadFinancialGoalRepository _readFinancialGoalRepository;
        private readonly IMapper _mapper;

        public GetFinancialGoalByIdQueryHandler(
            IReadFinancialGoalRepository readFinancialGoalRepository,
            IMapper mapper)
        {
            _readFinancialGoalRepository = readFinancialGoalRepository;
            _mapper = mapper;
        }

        public async Task<Result<FinancialGoalDto>> Handle(GetFinancialGoalByIdQuery request, CancellationToken cancellationToken)
        {
            var goal = await _readFinancialGoalRepository.GetByIdAsync(request.Id);
            if (goal == null)
                return Result<FinancialGoalDto>.Failure("Financial goal not found");

            var goalDto = _mapper.Map<FinancialGoalDto>(goal);
            return Result<FinancialGoalDto>.Success(goalDto);
        }
    }

