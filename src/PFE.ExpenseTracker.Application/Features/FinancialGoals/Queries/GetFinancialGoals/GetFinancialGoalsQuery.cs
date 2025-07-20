using MediatR;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.FinancialGoals.Queries.GetFinancialGoals;



    public class GetFinancialGoalsQuery : IRequest<Result<List<FinancialGoalDto>>>
    {
        public Guid UserId { get; set; }
        public string Status { get; set; }
    }

 