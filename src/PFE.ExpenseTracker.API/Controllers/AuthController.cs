using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFE.ExpenseTracker.Application.Features.Auth.Commands;
using MediatR;
using System.Security.Claims;

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
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result.Data);
        }

        // [Authorize]
        // [HttpGet("me")]
        // public async Task<IActionResult> GetCurrentUser()
        // {
        //     var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //     // var query = new GetUserById { Id = Guid.Parse(userId) };
        //     // var result = await _mediator.Send(query);

        //     // if (!result.Succeeded)
        //     //     return BadRequest(result.Errors);

        //     // return Ok(result.Data);
        // }
    }
}
