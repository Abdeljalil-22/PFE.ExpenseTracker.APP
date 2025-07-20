using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Application.Common.Exceptions;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;

namespace PFE.ExpenseTracker.Application.Features.Categories.Commands
{
    public class DeleteCategoryCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
    
         private readonly IReadCategoryRepository _readCategoryRepository;
        private readonly IWriteCategoryRepository _writeCategoryRepository;
        private readonly IReadExpenseRepository _readExpenseRepository;

        public DeleteCategoryCommandHandler(
            IReadCategoryRepository readCategoryRepository,
            IWriteCategoryRepository writeCategoryRepository,
            IReadExpenseRepository readExpenseRepository )
        {
            _readCategoryRepository  = readCategoryRepository;
            _writeCategoryRepository = writeCategoryRepository;
            _readExpenseRepository = readExpenseRepository;
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _readCategoryRepository.GetByIdAsync(request.Id);
            
            if (category == null)
                throw new NotFoundException(nameof(Category), request.Id);

            if (category.UserId != request.UserId)
                throw new UnauthorizedAccessException();

            if (category.IsDefault)
                return Result.Failure("Cannot delete default categories");

            var hasExpenses = await _readExpenseRepository.HasExpensesInCategoryAsync(category.Id);
            if (hasExpenses)
                return Result.Failure("Cannot delete category with existing expenses");

            await _writeCategoryRepository.DeleteAsync(category);
            await _writeCategoryRepository.SaveChangesAsync();

            return Result.Success();
        }
    }
}
