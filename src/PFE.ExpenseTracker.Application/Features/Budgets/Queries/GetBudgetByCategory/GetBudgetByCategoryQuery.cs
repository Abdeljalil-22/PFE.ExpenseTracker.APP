using MediatR;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetByCategory;

    public class GetBudgetByCategoryQuery : IRequest<Result<BudgetDto>>
    {
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
    }

