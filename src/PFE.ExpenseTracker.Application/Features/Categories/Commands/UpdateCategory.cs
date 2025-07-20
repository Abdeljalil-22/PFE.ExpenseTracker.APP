
using FluentValidation;
using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;

namespace PFE.ExpenseTracker.Application.Features.Categories.Commands
{
    public class UpdateCategoryCommand : IRequest<Result<CategoryDto>>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }

    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.Icon)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Color)
                .NotEmpty()
                .MaximumLength(7)
                .Matches("^#[0-9A-Fa-f]{6}$")
                .WithMessage("Color must be a valid hex color code (e.g. #FF0000)");
        }
    }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
    {
         private readonly IReadCategoryRepository _readCategoryRepository;
        private readonly IWriteCategoryRepository _writeCategoryRepository;
        private readonly IMapper _mapper;

        public UpdateCategoryCommandHandler(IReadCategoryRepository readCategoryRepository, IWriteCategoryRepository writeCategoryRepository, IMapper mapper)
        {
            _mapper = mapper;
            _readCategoryRepository = readCategoryRepository;
            _writeCategoryRepository = writeCategoryRepository;
        }

        public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _readCategoryRepository.GetByIdAsync(request.Id);
            if (category == null)
                return Result<CategoryDto>.Failure("Category not found");

            if (category.UserId != request.UserId)
                return Result<CategoryDto>.Failure("Unauthorized access to category");

            if (category.IsDefault)
                return Result<CategoryDto>.Failure("Cannot modify default categories");

            var existingCategory = await _readCategoryRepository.GetByNameAsync(request.UserId, request.Name);
            if (existingCategory != null && existingCategory.Id != request.Id)
                return Result<CategoryDto>.Failure("A category with this name already exists");

            category.Name = request.Name;
            category.Description = request.Description;
            category.Icon = request.Icon;
            category.Color = request.Color;

            await _writeCategoryRepository.UpdateAsync(category);
            await _writeCategoryRepository.SaveChangesAsync();

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Result<CategoryDto>.Success(categoryDto);
        }
    }
}
