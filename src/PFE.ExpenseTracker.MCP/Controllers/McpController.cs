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

            var actionJsonRaw = await _geminiService.GetActionFromPromptAsync(request.Prompt);
            // Remove markdown code block markers and trim whitespace
            _logger.LogInformation("Received Gemini response: {ActionJsonRaw}", actionJsonRaw);
            var actionJson = actionJsonRaw
                .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
                .Replace("```", "", StringComparison.OrdinalIgnoreCase)
                .Trim();
            _logger.LogInformation("Parsed Gemini response: {ActionJson}", actionJson);
            McpAction? action = null;
            try
            {
                action = JsonSerializer.Deserialize<McpAction>(actionJson);
                _logger.LogInformation("Deserialized action: {@Action}", action);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse Gemini response: {Raw}", actionJsonRaw);
                return BadRequest(new McpResponse
                {
                    Success = false,
                    Error = "Could not interpret the request (invalid JSON from Gemini)",
                    Data = new Dictionary<string, object> { ["raw"] = actionJsonRaw }
                });
            }

            if (action == null)
            {
                return BadRequest(new McpResponse
                {
                    Success = false,
                    Error = "Could not interpret the request (empty action)",
                    Data = new Dictionary<string, object> { ["raw"] = actionJsonRaw }
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
