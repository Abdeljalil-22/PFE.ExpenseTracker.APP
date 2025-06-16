using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Budgets.Queries
{
    public class GetBudgetByCategoryQuery : IRequest<Result<BudgetDto>>
    {
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class GetBudgetByCategoryQueryHandler : IRequestHandler<GetBudgetByCategoryQuery, Result<BudgetDto>>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;

        public GetBudgetByCategoryQueryHandler(IBudgetRepository budgetRepository, IMapper mapper)
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
}