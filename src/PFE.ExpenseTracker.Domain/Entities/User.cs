using System;
using System.Collections.Generic;

namespace PFE.ExpenseTracker.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }
        public virtual UserPreferences Preferences { get; set; }
        public virtual ICollection<Expense> Expenses { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public virtual ICollection<FinancialGoal> FinancialGoals { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
