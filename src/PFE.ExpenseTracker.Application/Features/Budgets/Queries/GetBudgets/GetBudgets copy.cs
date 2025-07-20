
using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;


namespace PFE.ExpenseTracker.Application.Features.Budgets.Queries.GetBudgets;
  

   

    public class GetBudgetsQueryHandler : IRequestHandler<GetBudgetsQuery, Result<List<BudgetDto>>>
    {
        private readonly IReadBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;

        public GetBudgetsQueryHandler(IReadBudgetRepository budgetRepository, IMapper mapper)
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

   