
namespace PFE.ExpenseTracker.Application.Common.Interfaces
{
    public interface IReadRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        
    }
}
