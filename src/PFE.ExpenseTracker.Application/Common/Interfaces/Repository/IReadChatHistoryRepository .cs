namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Domain.Entities;

public interface IReadChatHistoryRepository
{
    /// <summary>
    /// Retrieves the chat history for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose chat history is to be retrieved.</param>
    /// <returns>A list of chat messages.</returns>
    Task<ChatHistory?> GetByUserIdAsync(string userId);

    
  
}