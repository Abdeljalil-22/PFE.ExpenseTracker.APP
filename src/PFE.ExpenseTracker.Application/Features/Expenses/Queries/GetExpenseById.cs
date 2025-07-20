

using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Expenses.Queries;

    public class GetExpenseByIdQuery : IRequest<Result<ExpenseDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetExpenseByIdQueryHandler : IRequestHandler<GetExpenseByIdQuery, Result<ExpenseDto>>
    {
            private readonly IReadExpenseRepository _readExpenseRepository;
        private readonly IMapper _mapper;

        public GetExpenseByIdQueryHandler(IReadExpenseRepository readExpenseRepository, IMapper mapper)
        {
            _readExpenseRepository = readExpenseRepository;
            _mapper = mapper;
        }

        public async Task<Result<ExpenseDto>> Handle(GetExpenseByIdQuery request, CancellationToken cancellationToken)
        {
            var expense = await _readExpenseRepository.GetByIdAsync(request.Id);
            if (expense == null)
                return Result<ExpenseDto>.Failure("Expense not found");
            var dto = _mapper.Map<ExpenseDto>(expense);
            return Result<ExpenseDto>.Success(dto);
        }
    }
