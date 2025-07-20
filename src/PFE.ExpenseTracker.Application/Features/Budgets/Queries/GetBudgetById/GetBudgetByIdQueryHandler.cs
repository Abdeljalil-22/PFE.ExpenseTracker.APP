
using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;


namespace PFE.ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetById;



    public class GetBudgetByIdQueryHandler : IRequestHandler<GetBudgetByIdQuery, Result<BudgetDto>>
    {
        private readonly IReadBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;

        public GetBudgetByIdQueryHandler(IReadBudgetRepository budgetRepository, IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _mapper = mapper;
        }

        public async Task<Result<BudgetDto>> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetByIdAsync(request.Id);
            if (budget == null)
                return Result<BudgetDto>.Failure("Budget not found");

            var budgetDto = _mapper.Map<BudgetDto>(budget);
            return Result<BudgetDto>.Success(budgetDto);
        }
    }