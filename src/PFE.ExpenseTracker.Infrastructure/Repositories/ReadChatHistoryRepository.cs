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
   

    public class ReadChatHistoryRepository : IReadChatHistoryRepository
    {
        private readonly ReadDbContext _dbContext;
        public ReadChatHistoryRepository(ReadDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ChatHistory?> GetByUserIdAsync(string userId)
        {
            return await _dbContext.ChatHistories.FirstOrDefaultAsync(x => x.UserId == userId);
        }

      
    }
}
