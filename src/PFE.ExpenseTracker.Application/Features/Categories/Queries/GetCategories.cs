using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
using PFE.ExpenseTracker.Application.Common.Models;

namespace PFE.ExpenseTracker.Application.Features.Categories.Queries
{
   

    public class GetCategoriesQuery : IRequest<Result<List<CategoryDto>>>
    {
        public Guid UserId { get; set; }
    }

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
    {
        private readonly IReadCategoryRepository _readCategoryRepository;
        private readonly IMapper _mapper;

        public GetCategoriesQueryHandler(IReadCategoryRepository categoryRepository, IMapper mapper)
        {
            _readCategoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _readCategoryRepository.GetUserCategoriesAsync(request.UserId);
            var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return Result<List<CategoryDto>>.Success(categoryDtos);
        }
    }

}
