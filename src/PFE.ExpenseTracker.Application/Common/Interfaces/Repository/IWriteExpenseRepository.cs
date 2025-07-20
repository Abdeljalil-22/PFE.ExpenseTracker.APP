using System;
using System.Threading.Tasks;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Common.Interfaces.Repository
{
    public interface IWriteExpenseRepository: IWriteRepository<Expense>
    {

        // ...other write-only methods...
    }
}
