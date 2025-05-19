using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Features.Budgets.Queries
{
    public class BudgetDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public decimal Amount { get; set; }
        public decimal SpentAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Period { get; set; }
        public CategoryDto Category { get; set; }
    }

    public class GetBudgetsQuery : IRequest<Result<List<BudgetDto>>>
    {
        public Guid UserId { get; set; }
    }

    public class GetBudgetsQueryHandler : IRequestHandler<GetBudgetsQuery, Result<List<BudgetDto>>>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;

        public GetBudgetsQueryHandler(IBudgetRepository budgetRepository, IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<BudgetDto>>> Handle(GetBudgetsQuery request, CancellationToken cancellationToken)
        {
            var budgets = await _budgetRepository.GetUserBudgetsAsync(request.UserId);
            var budgetDtos = _mapper.Map<List<BudgetDto>>(budgets);
            return Result<List<BudgetDto>>.Success(budgetDtos);
        }
    }

    public class GetBudgetByIdQuery : IRequest<Result<BudgetDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetBudgetByIdQueryHandler : IRequestHandler<GetBudgetByIdQuery, Result<BudgetDto>>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;

        public GetBudgetByIdQueryHandler(IBudgetRepository budgetRepository, IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _mapper = mapper;
        }

        public async Task<Result<BudgetDto>> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetByIdAsync(request.Id);
            if (budget == null)
                return Result<BudgetDto>.Failure("Budget not found");

            var budgetDto = _mapper.Map<BudgetDto>(budget);
            return Result<BudgetDto>.Success(budgetDto);
        }
    }

    public class GetBudgetByCategoryQuery : IRequest<Result<BudgetDto>>
    {
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
    }

    public class GetBudgetByCategoryQueryHandler : IRequestHandler<GetBudgetByCategoryQuery, Result<BudgetDto>>
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly IMapper _mapper;

        public GetBudgetByCategoryQueryHandler(IBudgetRepository budgetRepository, IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _mapper = mapper;
        }

        public async Task<Result<BudgetDto>> Handle(GetBudgetByCategoryQuery request, CancellationToken cancellationToken)
        {
            var budget = await _budgetRepository.GetBudgetByCategoryAsync(request.UserId, request.CategoryId);
            if (budget == null)
                return Result<BudgetDto>.Failure("Budget not found");

            var budgetDto = _mapper.Map<BudgetDto>(budget);
            return Result<BudgetDto>.Success(budgetDto);
        }
    }
}
