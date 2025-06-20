using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFE.ExpenseTracker.Application.Features.Notifications.Commands;
using PFE.ExpenseTracker.Application.Features.Notifications.Queries;
using System.Security.Claims;
using AutoMapper;
using MediatR;

namespace PFE.ExpenseTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool unreadOnly = false)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var query = new GetNotificationsQuery 
            { 
                UserId = Guid.Parse(userId),
                UnreadOnly = unreadOnly 
            };
            var result = await _mediator.Send(query);
            return Ok(result.Data);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var command = new MarkNotificationAsReadCommand 
            { 
                Id = id,
                UserId = Guid.Parse(userId)
            };
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return Ok();
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var command = new MarkAllNotificationsAsReadCommand 
            { 
                UserId = Guid.Parse(userId)
            };
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var command = new DeleteNotificationCommand 
            { 
                Id = id,
                UserId = Guid.Parse(userId)
            };
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return NoContent();
        }
    }
}
