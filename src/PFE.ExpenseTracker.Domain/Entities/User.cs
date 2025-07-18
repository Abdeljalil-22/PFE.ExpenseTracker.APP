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
        public UserPreferences Preferences { get; set; }
        public ICollection<Expense> Expenses { get; set; }
        public ICollection<Category> Categories { get; set; }
        public ICollection<Budget> Budgets { get; set; }
        public ICollection<FinancialGoal> FinancialGoals { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
