using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFE.ExpenseTracker.Application.Features.Expenses.Commands;
using PFE.ExpenseTracker.Application.Features.Expenses.Queries;

namespace PFE.ExpenseTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Tags("Expenses")]
    public class ExpensesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExpensesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all expenses for the authenticated user
        /// </summary>
        /// <param name="query">Query parameters for filtering expenses</param>
        /// <returns>A list of expenses</returns>
        /// <response code="200">Returns the list of expenses</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<ExpenseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetExpensesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result.Data);
        }

        /// <summary>
        /// Get an expense by ID
        /// </summary>
        /// <param name="id">The ID of the expense to retrieve</param>
        /// <returns>The expense details</returns>
        /// <response code="200">Returns the expense</response>
        /// <response code="404">If the expense is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetExpenseByIdQuery { Id = id });
            if (!result.Succeeded)
                return NotFound();
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            command.UserId = Guid.Parse(userId);
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExpenseCommand command)
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var command = new DeleteExpenseCommand { Id = id, UserId = Guid.Parse(userId) };
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return NoContent();
        }

        [HttpPost("{id}/attachments")]
        public async Task<IActionResult> AddAttachment(Guid id, [FromForm] AddExpenseAttachmentCommand command)
        {
            command.ExpenseId = id;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            command.UserId = Guid.Parse(userId);
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return Ok(result.Data);
        }
    }
}
