using System;
using System.Collections.Generic;

namespace PFE.ExpenseTracker.Application.Common.Models
{
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public bool IsRecurring { get; set; }
        public string RecurringFrequency { get; set; }
        public DateTime? NextRecurringDate { get; set; }
        public bool IsShared { get; set; }
        public string Notes { get; set; }
        public CategoryDto Category { get; set; }
        public List<AttachmentDto> Attachments { get; set; }
    }

    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool IsDefault { get; set; }
    }

    public class AttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
    }

    public class ExpenseCreateDto
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsRecurring { get; set; }
        public string RecurringFrequency { get; set; }
        public bool IsShared { get; set; }
        public string Notes { get; set; }
    }

    public class ExpenseUpdateDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsRecurring { get; set; }
        public string RecurringFrequency { get; set; }
        public bool IsShared { get; set; }
        public string Notes { get; set; }
    }
}
