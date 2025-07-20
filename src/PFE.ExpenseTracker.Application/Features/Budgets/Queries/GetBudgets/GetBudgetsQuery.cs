using MediatR;
using PFE.ExpenseTracker.Application.Common.Models;


namespace PFE.ExpenseTracker.Application.Features.Budgets.Queries.GetBudgets;
  

    public class GetBudgetsQuery : IRequest<Result<List<BudgetDto>>>
    {
        public Guid UserId { get; set; }
    }

    