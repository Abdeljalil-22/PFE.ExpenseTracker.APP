using Microsoft.Extensions.Configuration;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using System.Net;
using System.Net.Mail;

namespace PFE.ExpenseTracker.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;
        private readonly IReadUserRepository _readUserRepository;
        
        private readonly string _fromEmail;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            var smtpSettings = _configuration.GetSection("EmailSettings");

            // Helper to get value from env or config
            string GetSetting(string key, string envVar, string fallback = "")
            {
                var value = smtpSettings[key];
                if (string.IsNullOrWhiteSpace(value) || value.StartsWith("${"))
                {
                    var envValue = Environment.GetEnvironmentVariable(envVar);
                    return !string.IsNullOrWhiteSpace(envValue) ? envValue : fallback;
                }
                return value ?? fallback;
            }

            _fromEmail = GetSetting("FromEmail", "FROM_EMAIL");
            var smtpServer = GetSetting("SmtpServer", "SMTP_SERVER");
            var smtpPortStr = GetSetting("SmtpPort", "SMTP_PORT", "587");
            var smtpUsername = GetSetting("SmtpUsername", "SMTP_USERNAME");
            var smtpPassword = GetSetting("SmtpPassword", "SMTP_PASSWORD", "");
            var enableSslStr = GetSetting("EnableSsl", "ENABLE_SSL", "true");

            int port = 587;
            if (!string.IsNullOrWhiteSpace(smtpPortStr))
            {
                if (!int.TryParse(smtpPortStr, out port))
                {
                    port = 587;
                }
            }

            bool enableSsl = true;
            if (!string.IsNullOrWhiteSpace(enableSslStr))
            {
                if (!bool.TryParse(enableSslStr, out enableSsl))
                {
                    enableSsl = true;
                }
            }

            _smtpClient = new SmtpClient(smtpServer)
            {
                Port = port,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = enableSsl
            };
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };
            mailMessage.To.Add(to);

            await _smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendWelcomeEmailAsync(string to, string userName)
        {
            string subject = "Welcome to Expense Tracker!";
            string body = $@"
                <h2>Welcome to Expense Tracker, {userName}!</h2>
                <p>We're excited to have you on board. With Expense Tracker, you can:</p>
                <ul>
                    <li>Track your daily expenses</li>
                    <li>Set and monitor budgets</li>
                    <li>Create financial goals</li>
                    <li>Get insights about your spending habits</li>
                </ul>
                <p>Start tracking your expenses today!</p>";

            await SendEmailAsync(to, subject, body, true);
        }

        public async Task SendPasswordResetEmailAsync(string to, string resetToken)
        {
            string subject = "Reset Your Password - Expense Tracker";
            string resetLink = $"{_configuration["ApplicationUrl"]}/reset-password?token={resetToken}";
            string body = $@"
                <h2>Reset Your Password</h2>
                <p>You've requested to reset your password. Click the link below to proceed:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>If you didn't request this, please ignore this email.</p>";

            await SendEmailAsync(to, subject, body, true);
        }

        public async Task SendExpenseAlertAsync(string to, string userName, decimal amount, string category)
        {
            string subject = "Expense Alert - High Spending Detected";
            string body = $@"
                <h2>Expense Alert</h2>
                <p>Hi {userName},</p>
                <p>We noticed a significant expense in your account:</p>
                <ul>
                    <li>Amount: {amount:C}</li>
                    <li>Category: {category}</li>
                </ul>
                <p>Log in to your account to review this expense.</p>";

            await SendEmailAsync(to, subject, body, true);
        }

        public async Task SendBudgetAlertAsync(string to, string userName, string budgetName, decimal currentSpending, decimal limit)
        {
            string subject = "Budget Alert - Approaching Limit";
            string body = $@"
                <h2>Budget Alert</h2>
                <p>Hi {userName},</p>
                <p>Your budget '{budgetName}' is approaching its limit:</p>
                <ul>
                    <li>Current Spending: {currentSpending:C}</li>
                    <li>Budget Limit: {limit:C}</li>
                </ul>
                <p>Log in to your account to review your budget.</p>";

            await SendEmailAsync(to, subject, body, true);
        }

        public async Task SendGoalAchievedAsync(string to, string userName, string goalName)
        {
            string subject = "Congratulations! Financial Goal Achieved";
            string body = $@"
                <h2>Goal Achieved! 🎉</h2>
                <p>Congratulations {userName}!</p>
                <p>You've achieved your financial goal: {goalName}</p>
                <p>Keep up the great work! Why not set a new goal?</p>";

            await SendEmailAsync(to, subject, body, true);
        }
    }
}
