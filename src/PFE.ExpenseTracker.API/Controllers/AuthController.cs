using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFE.ExpenseTracker.Application.Features.Auth.Commands;
using MediatR;
using System.Security.Claims;
using PFE.ExpenseTracker.Application.Features.Users.Queries;
using PFE.ExpenseTracker.Application.Features.Auth.Commands.RegisterCommand;
using PFE.ExpenseTracker.Application.Features.Auth.Queries.Login;
using PFE.ExpenseTracker.Application.Features.Auth.Commands.GoogleAuthCommand;

namespace PFE.ExpenseTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginQuerie command)
        {
            var result = await _mediator.Send(command);
            
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        [HttpPost("google")]
        public async Task<IActionResult> AuthenticateWithGoogle([FromBody] GoogleAuthCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var query = new GetUserByIdQuery { Id = Guid.Parse(userId) };
            var result = await _mediator.Send(query);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }
    }
}
