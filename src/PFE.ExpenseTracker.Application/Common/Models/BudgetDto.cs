using System;
using System.Collections.Generic;

namespace PFE.ExpenseTracker.Application.Common.Models
{

      public class BudgetDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
        public decimal SpentAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Period { get; set; }
        public CategoryDto Category { get; set; }
    }
    // public class BudgetDto
    // {
    //     public Guid Id { get; set; }
    //     public decimal Amount { get; set; }
    //     public decimal SpentAmount { get; set; }
    //     public DateTime StartDate { get; set; }
    //     public DateTime EndDate { get; set; }
    //     public bool AlertEnabled { get; set; }
    //     public int AlertThresholdPercentage { get; set; }
    //     public CategoryDto Category { get; set; }
    // }

    public class BudgetCreateDto
    {
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AlertEnabled { get; set; }
        public int AlertThresholdPercentage { get; set; }
    }

    public class BudgetUpdateDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool AlertEnabled { get; set; }
        public int AlertThresholdPercentage { get; set; }
    }

    public class FinancialGoalDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
        public string Status { get; set; }
        public List<GoalContributionDto> Contributions { get; set; }
    }

    public class GoalContributionDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
    }

    public class FinancialGoalCreateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal TargetAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime TargetDate { get; set; }
    }

    public class GoalContributionCreateDto
    {
        public Guid FinancialGoalId { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
    }
}
