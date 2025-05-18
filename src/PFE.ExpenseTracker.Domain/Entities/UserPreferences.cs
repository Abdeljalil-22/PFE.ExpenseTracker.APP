using System;

namespace PFE.ExpenseTracker.Domain.Entities
{
    public class UserPreferences
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Currency { get; set; }
        public string Language { get; set; }
        public bool DarkMode { get; set; }
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public User User { get; set; }
    }
}
