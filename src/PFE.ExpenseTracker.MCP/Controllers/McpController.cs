using Microsoft.AspNetCore.Mvc;
using PFE.ExpenseTracker.MCP.Models;
using PFE.ExpenseTracker.MCP.Services;
using System.Text.Json;

namespace PFE.ExpenseTracker.MCP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class McpController : ControllerBase
{
    private readonly GeminiService _geminiService;
    private readonly IExpenseTrackerClient _expenseTrackerClient;
    private readonly ILogger<McpController> _logger;

    public McpController(
        GeminiService geminiService,
        IExpenseTrackerClient expenseTrackerClient,
        ILogger<McpController> logger)
    {
        _geminiService = geminiService;
        _expenseTrackerClient = expenseTrackerClient;
        _logger = logger;
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessRequest([FromBody] McpRequest request)
    {
        // Basic validation
        var errors = new List<ValidationError>();
        if (string.IsNullOrWhiteSpace(request.Prompt))
            errors.Add(new ValidationError { Field = "Prompt", Error = "Prompt is required." });
        if (string.IsNullOrWhiteSpace(request.UserId))
            errors.Add(new ValidationError { Field = "UserId", Error = "UserId is required." });
        if (errors.Count > 0)
            return BadRequest(new { Success = false, Errors = errors });

        try
        {
            // Get structured action from Gemini
            var actionJson = await _geminiService.GetActionFromPromptAsync(request.Prompt);
            var action = JsonSerializer.Deserialize<McpAction>(actionJson);

            if (action == null)
            {
                return BadRequest(new McpResponse
                {
                    Success = false,
                    Error = "Could not interpret the request"
                });
            }

            // Execute the action
            var result = await _expenseTrackerClient.ExecuteActionAsync(action, request.UserId);

            return Ok(new McpResponse
            {
                Success = result.Success,
                Response = result.Success
                    ? $"Successfully {action.Type.ToLower()}d {action.Entity.ToLower()}"
                    : result.Error,
                Action = action.Type,
                Data = result.Data
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MCP request");
            return StatusCode(500, new McpResponse
            {
                Success = false,
                Error = "An error occurred while processing your request"
            });
        }
    }
}
