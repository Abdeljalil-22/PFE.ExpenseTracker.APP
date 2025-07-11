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
            var expenses = await _expenseRepository.GetUserExpensesFilteredAsync(
                request.UserId,
                request.CategoryId,
                request.StartDate,
                request.EndDate,
                request.IsRecurring
            );
            var expenseDtos = _mapper.Map<List<ExpenseDto>>(expenses);
            return Result<List<ExpenseDto>>.Success(expenseDtos);
        }
    }

}
