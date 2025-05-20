using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using AutoMapper;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Features.Categories.Commands
{
    public class CreateCategoryCommand : IRequest<Result<CategoryDto>>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
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
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var existingCategory = await _categoryRepository.GetByNameAsync(request.UserId, request.Name);
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

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Result<CategoryDto>.Success(categoryDto);
        }
    }
}
