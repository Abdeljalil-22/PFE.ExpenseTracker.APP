using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Infrastructure.Authentication
{
    public interface IJwtAuthenticationService
    {
        string GenerateJwtToken(User user);
        bool ValidateToken(string token);
    }
}
