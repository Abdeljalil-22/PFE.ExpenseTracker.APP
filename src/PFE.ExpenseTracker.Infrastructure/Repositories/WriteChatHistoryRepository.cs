using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Infrastructure.Persistence;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;

namespace PFE.ExpenseTracker.Infrastructure.Repositories
{
   

    public class WriteChatHistoryRepository : IWriteChatHistoryRepository
    {
        private readonly WriteDbContext _dbContext;
        public WriteChatHistoryRepository(WriteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task SaveOrUpdateAsync(string userId, List<string> history)
        {
            var entity = await _dbContext.ChatHistories.FirstOrDefaultAsync(x => x.UserId == userId);
            var historyJson = System.Text.Json.JsonSerializer.Serialize(history);
            if (entity == null)
            {
                entity = new ChatHistory
                {
                    UserId = userId,
                    HistoryJson = historyJson,
                    LastUpdated = DateTime.UtcNow
                };
                _dbContext.ChatHistories.Add(entity);
            }
            else
            {
                entity.HistoryJson = historyJson;
                entity.LastUpdated = DateTime.UtcNow;
                _dbContext.ChatHistories.Update(entity);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
