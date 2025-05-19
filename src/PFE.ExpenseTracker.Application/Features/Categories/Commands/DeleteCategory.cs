using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Application.Common.Exceptions;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IExpenseRepository _expenseRepository;

        public DeleteCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            IExpenseRepository expenseRepository)
        {
            _categoryRepository = categoryRepository;
            _expenseRepository = expenseRepository;
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id);
            
            if (category == null)
                throw new NotFoundException(nameof(Category), request.Id);

            if (category.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            if (category.IsDefault)
                return Result.Failure("Cannot delete default categories");

            var hasExpenses = await _expenseRepository.HasExpensesInCategoryAsync(category.Id);
            if (hasExpenses)
                return Result.Failure("Cannot delete category with existing expenses");

            await _categoryRepository.DeleteAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return Result.Success();
        }
    }
}
