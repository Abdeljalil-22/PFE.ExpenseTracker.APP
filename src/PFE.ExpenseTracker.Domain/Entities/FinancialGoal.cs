using System;
using System.Collections.Generic;

namespace PFE.ExpenseTracker.Domain.Entities
{
    public class FinancialGoal
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public string Status { get; set; }
        public User User { get; set; }
        public ICollection<GoalContribution> Contributions { get; set; }
    }
}
