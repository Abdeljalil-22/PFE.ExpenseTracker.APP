namespace PFE.ExpenseTracker.Application.Common.Models;


public class FinancialGoalDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime TargetDate { get; set; }
    public string Status { get; set; }
    public List<GoalContributionDto> Contributions { get; set; }
}

   