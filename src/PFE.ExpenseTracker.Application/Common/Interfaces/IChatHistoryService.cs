
namespace PFE.ExpenseTracker.Application.Common.Interfaces;

public interface IChatHistoryService
{
    Task<List<string>> GetChatHistoryAsync(string userId);
    Task SaveChatHistoryAsync(string userId, List<string> history);
}