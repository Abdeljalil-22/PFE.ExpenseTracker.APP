using System;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository
{
    public interface IWriteCategoryRepository: IWriteRepository<Category>
    {
       
        // ...other write-only methods...
    }
}
