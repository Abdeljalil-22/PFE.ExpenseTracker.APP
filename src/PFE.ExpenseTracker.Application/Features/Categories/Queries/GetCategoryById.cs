
using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Categories.Queries;

    public class GetCategoryByIdQuery : IRequest<Result<CategoryDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
    {
          private readonly IReadCategoryRepository _readCategoryRepository;
        private readonly IMapper _mapper;

        public GetCategoryByIdQueryHandler(IReadCategoryRepository categoryRepository, IMapper mapper)
        {
            _readCategoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _readCategoryRepository.GetByIdAsync(request.Id);
            if (category == null)
                return Result<CategoryDto>.Failure("Category not found");

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Result<CategoryDto>.Success(categoryDto);
        }
    }
