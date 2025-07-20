using MediatR;
using PFE.ExpenseTracker.Application.Common.Models;


namespace PFE.ExpenseTracker.Application.Features.Budgets.Queries.GetBudgetById;



public class GetBudgetByIdQuery : IRequest<Result<BudgetDto>>
    {
        public Guid Id { get; set; }
    }

  