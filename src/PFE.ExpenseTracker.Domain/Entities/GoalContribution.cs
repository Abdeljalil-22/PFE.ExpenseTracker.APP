using System;

namespace PFE.ExpenseTracker.Domain.Entities
{
    public class GoalContribution
    {
        public Guid Id { get; set; }
        public Guid FinancialGoalId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public FinancialGoal FinancialGoal { get; set; }
    }
}
