using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;
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
    private readonly IReadUserRepository _readUserRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IReadUserRepository userRepository, IMapper mapper)
        {
            _readUserRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _readUserRepository.GetByIdAsync(request.Id);
            if (user == null)
                return Result<UserDto>.Failure("User not found");

            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(userDto);
        }
    }

