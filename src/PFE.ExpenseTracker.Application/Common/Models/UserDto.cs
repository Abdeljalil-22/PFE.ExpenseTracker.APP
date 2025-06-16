using System;

namespace PFE.ExpenseTracker.Application.Common.Models
{
    
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserPreferencesDto Preferences { get; set; }
    }

    public class UserPreferencesDto
    {
        public string Currency { get; set; }
        public string Language { get; set; }
        public bool DarkMode { get; set; }
        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }
    }

    public class AuthenticationResponse
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }
}
