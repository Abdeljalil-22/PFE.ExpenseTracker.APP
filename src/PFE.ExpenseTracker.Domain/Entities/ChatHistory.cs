using System;
using System.Collections.Generic;

namespace PFE.ExpenseTracker.Domain.Entities
{
    public class ChatHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = null!;
        public string HistoryJson { get; set; } = null!;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
