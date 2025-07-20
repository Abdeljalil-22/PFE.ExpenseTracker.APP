using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetByCategory;

    


    public class GetBudgetByCategoryQueryHandler : IRequestHandler<GetBudgetByCategoryQuery, Result<BudgetDto>>
    {
        private readonly IReadBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;

        public GetBudgetByCategoryQueryHandler(IReadBudgetRepository budgetRepository, IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _mapper = mapper;
        }

        public async Task<Result<BudgetDto>> Handle(GetBudgetByCategoryQuery request, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudgetByCategoryAsync(request.UserId, request.CategoryId);
            if (budget == null)
                return Result<BudgetDto>.Failure("Budget not found");

            var budgetDto = _mapper.Map<BudgetDto>(budget);
            return Result<BudgetDto>.Success(budgetDto);
        }
    }
