

using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Expenses.Queries;

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
            var dto = _mapper.Map<ExpenseDto>(expense);
            return Result<ExpenseDto>.Success(dto);
        }
    }
