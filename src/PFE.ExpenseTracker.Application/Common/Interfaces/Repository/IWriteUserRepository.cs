using System;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository
{
    public interface IWriteUserRepository: IWriteRepository<User>
    {

        // ...other write-only methods...
    }
}
