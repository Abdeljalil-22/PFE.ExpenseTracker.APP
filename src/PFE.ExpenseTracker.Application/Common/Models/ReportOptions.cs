 namespace PFE.ExpenseTracker.Application.Common.Models;
public class ReportOptions
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Format { get; set; } = "PDF";
        public bool IncludeCharts { get; set; } = true;
        public string[] Categories { get; set; }
        public string Currency { get; set; }
    }