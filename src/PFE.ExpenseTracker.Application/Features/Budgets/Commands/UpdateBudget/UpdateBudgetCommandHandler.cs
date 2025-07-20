
using FluentValidation;
using MediatR;
using AutoMapper;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;

namespace PFE.ExpenseTracker.Application.Features.Budgets.Commands.UpdateBudget;
    public class UpdateBudgetCommandHandler : IRequestHandler<UpdateBudgetCommand, Result<BudgetDto>>
    {
     
        private readonly IWriteBudgetRepository _budgetRepository;
        private readonly IReadBudgetRepository _readbudgetRepository;
        private readonly IReadCategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public UpdateBudgetCommandHandler(
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
        public async Task<Result<BudgetDto>> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
        {
            var budget = await _readbudgetRepository.GetByIdAsync(request.Id);
            if (budget == null)
                return Result<BudgetDto>.Failure("Budget not found");

            if (budget.UserId != request.UserId)
                return Result<BudgetDto>.Failure("Unauthorized access to budget");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return Result<BudgetDto>.Failure("Category not found");

            if (budget.CategoryId != request.CategoryId)
            {
                var existingBudget = await _readbudgetRepository.GetBudgetByCategoryAsync(request.UserId, request.CategoryId);
                if (existingBudget != null)
                    return Result<BudgetDto>.Failure("A budget for this category already exists");
            }

            budget.CategoryId = request.CategoryId;
            budget.Amount = request.Amount;
            budget.StartDate = request.StartDate;
            budget.EndDate = request.EndDate;
            

            await _budgetRepository.UpdateAsync(budget);
            await _budgetRepository.SaveChangesAsync();

            var budgetDto = _mapper.Map<BudgetDto>(budget);
            budgetDto.Category = _mapper.Map<CategoryDto>(category);

            return Result<BudgetDto>.Success(budgetDto);
        }
    }

