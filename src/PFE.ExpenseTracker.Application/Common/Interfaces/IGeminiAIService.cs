
namespace PFE.ExpenseTracker.Application.Common.Interfaces;

public interface IGeminiAIService
{
    Task<string> GetActionFromPromptAsync(string prompt);
}