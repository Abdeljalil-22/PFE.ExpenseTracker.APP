using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PFE.ExpenseTracker.Application.Features.Categories.Commands;
using PFE.ExpenseTracker.Application.Features.Categories.Queries;
using MediatR;
using System.Security.Claims;

namespace PFE.ExpenseTracker.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var query = new GetCategoriesQuery { UserId = Guid.Parse(userId) };
            var result = await _mediator.Send(query);
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery { Id = id });
            if (!result.Succeeded)
                return NotFound();
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            command.UserId = Guid.Parse(userId);
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
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
            var command = new DeleteCategoryCommand { Id = id, UserId = Guid.Parse(userId) };
            
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            
            return NoContent();
        }
    }
}
