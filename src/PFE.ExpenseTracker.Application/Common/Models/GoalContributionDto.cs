namespace PFE.ExpenseTracker.Application.Common.Models;


 public class GoalContributionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
    }