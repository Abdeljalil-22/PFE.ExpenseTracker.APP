using System;

namespace PFE.ExpenseTracker.Domain.Entities
{
    public class Budget
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
        public decimal SpentAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AlertEnabled { get; set; } = true;
        public int AlertThresholdPercentage { get; set; } 
        public User User { get; set; }
        public Category Category { get; set; }
    }
}
