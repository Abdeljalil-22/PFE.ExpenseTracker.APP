
using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;


namespace PFE.ExpenseTracker.Application.Features.Budgets.Queries;
  

    public class GetBudgetsQuery : IRequest<Result<List<BudgetDto>>>
    {
        public Guid UserId { get; set; }
    }

    public class GetBudgetsQueryHandler : IRequestHandler<GetBudgetsQuery, Result<List<BudgetDto>>>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;

        public GetBudgetsQueryHandler(IBudgetRepository budgetRepository, IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<BudgetDto>>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetUserBudgetsAsync(request.UserId);
            var budgetDtos = _mapper.Map<List<BudgetDto>>(budgets);
            return Result<List<BudgetDto>>.Success(budgetDtos);
        }
    }

   