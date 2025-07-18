namespace PFE.ExpenseTracker.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = false);
        Task SendWelcomeEmailAsync(string to, string userName);
        Task SendPasswordResetEmailAsync(string to, string resetToken);
        Task SendExpenseAlertAsync(string to, string userName, decimal amount, string category);
        Task SendBudgetAlertAsync(string to, string userName, string budgetName, decimal currentSpending, decimal limit);
        Task SendGoalAchievedAsync(string to, string userName, string goalName);
    }
}
