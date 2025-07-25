using System;
using System.Collections.Generic;

namespace PFE.ExpenseTracker.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool IsDefault { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Expense>? Expenses { get; set; }
        public virtual ICollection<Budget>? Budgets { get; set; }
    }
}
