

namespace PFE.ExpenseTracker.Domain.Entities;

    public class Expense
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public bool IsRecurring { get; set; }
        public string RecurringFrequency { get; set; }
        public DateTime? NextRecurringDate { get; set; }
        public bool IsShared { get; set; }
        public string Notes { get; set; }
        public virtual User User { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
    }

