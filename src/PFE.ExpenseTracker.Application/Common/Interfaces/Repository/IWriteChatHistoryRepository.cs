namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;

public interface IWriteChatHistoryRepository
{
  
    /// <summary>
    /// Saves the chat history for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose chat history is to be saved.</param>
    /// <param name="history">The list of chat messages to save.</param>
    Task SaveOrUpdateAsync(string userId, List<string> history);


  
}