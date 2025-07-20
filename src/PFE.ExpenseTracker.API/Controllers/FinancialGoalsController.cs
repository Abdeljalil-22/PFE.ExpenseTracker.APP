using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFE.ExpenseTracker.Application.Features.FinancialGoals.Commands;
using PFE.ExpenseTracker.Application.Features.FinancialGoals.Queries;
using System.Security.Claims;
using MediatR;
using PFE.ExpenseTracker.Application.Features.FinancialGoals.Queries.GetFinancialGoals;
using PFE.ExpenseTracker.Application.Features.FinancialGoals.Queries.GetFinancialGoalById;

namespace PFE.ExpenseTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialGoalsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FinancialGoalsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var query = new GetFinancialGoalsQuery { UserId = Guid.Parse(userId) };
            var result = await _mediator.Send(query);
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetFinancialGoalByIdQuery { Id = id });
            if (!result.Succeeded)
                return NotFound();
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFinancialGoalCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            command.UserId = Guid.Parse(userId);
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFinancialGoalCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            command.UserId = Guid.Parse(userId);
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return Ok(result.Data);
        }

        [HttpPost("{id}/contributions")]
        public async Task<IActionResult> AddContribution(Guid id, [FromBody] AddGoalContributionCommand command)
        {
            command.FinancialGoalId = id;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            command.UserId = Guid.Parse(userId);
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var command = new DeleteFinancialGoalCommand { Id = id, UserId = Guid.Parse(userId) };
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return NoContent();
        }
    }
}
