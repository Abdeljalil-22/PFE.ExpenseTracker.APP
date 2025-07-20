using FluentValidation;
using MediatR;
using AutoMapper;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;

namespace PFE.ExpenseTracker.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<Result<CategoryDto>>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
    }

    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
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

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
    {
        private readonly IReadCategoryRepository _readCategoryRepository;
        private readonly IWriteCategoryRepository _writeCategoryRepository;

        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(IReadCategoryRepository categoryRepository,IWriteCategoryRepository writeCategoryRepository, IMapper mapper)
        {
            _readCategoryRepository = categoryRepository;
            _writeCategoryRepository = writeCategoryRepository;
            _mapper = mapper;
        }

        public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var existingCategory = await _readCategoryRepository.GetByNameAsync(request.UserId, request.Name);
            if (existingCategory != null)
                return Result<CategoryDto>.Failure("A category with this name already exists");

            var category = new Category
            {
                UserId = request.UserId,
                Name = request.Name,
                Description = request.Description,
                Icon = request.Icon,
                Color = request.Color,
                IsDefault = false
            };

            await _writeCategoryRepository.AddAsync(category);
            await _writeCategoryRepository.SaveChangesAsync();

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Result<CategoryDto>.Success(categoryDto);
        }
    }
}
