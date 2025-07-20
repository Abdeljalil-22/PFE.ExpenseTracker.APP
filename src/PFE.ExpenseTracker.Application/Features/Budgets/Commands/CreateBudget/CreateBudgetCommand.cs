using MediatR;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Budgets.Commands.CreateBudget;

public class CreateBudgetCommand : IRequest<Result<BudgetDto>>
{
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Period { get; set; }
}