using System;

namespace PFE.ExpenseTracker.Domain.Entities
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public Guid ExpenseId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
        public Expense Expense { get; set; }
    }
}
