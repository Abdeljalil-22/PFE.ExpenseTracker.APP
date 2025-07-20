using MediatR;
using AutoMapper;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;

namespace PFE.ExpenseTracker.Application.Features.Budgets.Commands.CreateBudget
{    public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, Result<BudgetDto>>
    {
        private readonly IWriteBudgetRepository _budgetRepository;
        private readonly IReadBudgetRepository _readbudgetRepository;
        private readonly IReadCategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CreateBudgetCommandHandler(
            IWriteBudgetRepository budgetRepository,
            IReadCategoryRepository categoryRepository,
            IReadBudgetRepository ReadbudgetRepository,
            IMapper mapper)
        {
            _budgetRepository = budgetRepository;
            _categoryRepository = categoryRepository;
            _readbudgetRepository = ReadbudgetRepository;
            _mapper = mapper;
        }

        public async Task<Result<BudgetDto>> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return Result<BudgetDto>.Failure("Category not found");

            var existingBudget = await _readbudgetRepository.GetBudgetByCategoryAsync(request.UserId, request.CategoryId);
            if (existingBudget != null)
                return Result<BudgetDto>.Failure("A budget for this category already exists");

            var budget = new Budget
            {
                UserId = request.UserId,
                CategoryId = request.CategoryId,
                Amount = request.Amount,
                SpentAmount = 0,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
            };

            await _budgetRepository.AddAsync(budget);
            await _budgetRepository.SaveChangesAsync();

            var budgetDto = _mapper.Map<BudgetDto>(budget);
            budgetDto.Category = _mapper.Map<CategoryDto>(category);
            
            return Result<BudgetDto>.Success(budgetDto);
        }
    }
}
