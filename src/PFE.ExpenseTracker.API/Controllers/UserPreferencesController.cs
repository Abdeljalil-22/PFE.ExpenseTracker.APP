using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFE.ExpenseTracker.Application.Features.Users.Commands;
using System.Security.Claims;

namespace PFE.ExpenseTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UserPreferencesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserPreferencesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("preferences")]
        public async Task<IActionResult> UpdatePreferences([FromBody] UpdateUserPreferencesCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            command.UserId = Guid.Parse(userId);
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return Ok(result.Data);
        }

        [HttpGet("preferences")]
        public async Task<IActionResult> GetPreferences()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var query = new GetUserPreferencesQuery { UserId = Guid.Parse(userId) };
            
            var result = await _mediator.Send(query);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return Ok(result.Data);
        }
    }
}
