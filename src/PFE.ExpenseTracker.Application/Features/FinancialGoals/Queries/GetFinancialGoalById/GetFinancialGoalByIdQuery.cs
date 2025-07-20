using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.FinancialGoals.Queries.GetFinancialGoalById;



    public class GetFinancialGoalByIdQuery : IRequest<Result<FinancialGoalDto>>
    {
        public Guid Id { get; set; }
    }

   
