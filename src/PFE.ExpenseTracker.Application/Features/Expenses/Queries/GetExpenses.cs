using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Expenses.Queries
{
    public class GetExpensesQuery : IRequest<Result<List<ExpenseDto>>>
    {
        public Guid UserId { get; set; }
        public Guid? CategoryId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsRecurring { get; set; }
    }

    public class GetExpensesQueryHandler : IRequestHandler<GetExpensesQuery, Result<List<ExpenseDto>>>
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;

        public GetExpensesQueryHandler(IExpenseRepository expenseRepository, IMapper mapper)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<ExpenseDto>>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _expenseRepository.GetUserExpensesAsync(request.UserId);
            var expenseDtos = _mapper.Map<List<ExpenseDto>>(expenses);
            return Result<List<ExpenseDto>>.Success(expenseDtos);
        }
    }

    public class GetExpenseByIdQuery : IRequest<Result<ExpenseDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetExpenseByIdQueryHandler : IRequestHandler<GetExpenseByIdQuery, Result<ExpenseDto>>
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;

        public GetExpenseByIdQueryHandler(IExpenseRepository expenseRepository, IMapper mapper)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }

        public async Task<Result<ExpenseDto>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
        {
            var expense = await _expenseRepository.GetByIdAsync(request.Id);
            if (expense == null)
                return Result<ExpenseDto>.Failure("Expense not found");

            var expenseDto = _mapper.Map<ExpenseDto>(expense);
            return Result<ExpenseDto>.Success(expenseDto);
        }
    }
}
