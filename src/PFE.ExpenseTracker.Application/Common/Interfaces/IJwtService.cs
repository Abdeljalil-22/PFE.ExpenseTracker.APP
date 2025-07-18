using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
