using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Models;
using PFE.ExpenseTracker.Application.Features.Users.Queries;
using PFE.ExpenseTracker.Domain.Entities;

namespace PFE.ExpenseTracker.Application.Features.Users.Queries;


    public class GetUserByIdQuery : IRequest<Result<UserDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
                return Result<UserDto>.Failure("User not found");

            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(userDto);
        }
    }



    // public class GetUserQueryHandler : IRequestHandler<GetUserByIdQuery, Result<List<UserDto>>>
    // {
    //     private readonly IUserRepository _userRepository;
    //     private readonly IMapper _mapper;

    //     public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
    //     {
    //         _userRepository = userRepository;
    //         _mapper = mapper;
    //     }

    //     public async Task<Result<List<UserDto>>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    //     {
    //         var user = await _userRepository.GetByIdAsync(request.Id);
    //         if (user == null)
    //             return Result<List<UserDto>>.Failure("User not found");

    //         var userDto = _mapper.Map<UserDto>(user);
    //         return Result<List<UserDto>>.Success(new List<UserDto> { userDto });
    //     }
    // }

   
    // public class GetBudgetByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<BudgetDto>>
    // {
    //     private readonly IBudgetRepository _budgetRepository;
    //     private readonly IMapper _mapper;

    //     public GetBudgetByIdQueryHandler(IBudgetRepository budgetRepository, IMapper mapper)
    //     {
    //         _budgetRepository = budgetRepository;
    //         _mapper = mapper;
    //     }

    //     // public async Task<Result<BudgetDto>> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    //     // {
    //     //     var budget = await _budgetRepository.GetByIdAsync(request.Id);
    //     //     if (budget == null)
    //     //         return Result<BudgetDto>.Failure("Budget not found");

    //     //     var budgetDto = _mapper.Map<BudgetDto>(budget);
    //     //     return Result<BudgetDto>.Success(budgetDto);
    //     // }
    // }

