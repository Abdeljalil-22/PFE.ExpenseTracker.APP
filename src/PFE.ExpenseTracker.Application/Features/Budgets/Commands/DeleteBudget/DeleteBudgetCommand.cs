using MediatR;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Budgets.Commands.DeleteBudget;

    public class DeleteBudgetCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

   