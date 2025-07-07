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
            // Prepare chat history
            var history = request.History ?? new List<string>();
            history.Add($"User: {request.Prompt}");

            // Send full history to Gemini
            var fullPrompt = string.Join("\n", history);
            var actionJsonRaw = await _geminiService.GetActionFromPromptAsync(fullPrompt);
            _logger.LogInformation("Received Gemini response: {ActionJsonRaw}", actionJsonRaw);
            var actionJson = actionJsonRaw
                .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
                .Replace("```", "", StringComparison.OrdinalIgnoreCase)
                .Trim();
            _logger.LogInformation("Parsed Gemini response: {ActionJson}", actionJson);
            McpAction? action = null;
            string aiResponse = null;
            try
            {
                action = JsonSerializer.Deserialize<McpAction>(actionJson);
                _logger.LogInformation("Deserialized action: {@Action}", action);
            }
            catch
            {
                // If not valid JSON, treat as AI follow-up question
                aiResponse = actionJsonRaw;
            }

            if (action == null && aiResponse == null)
            {
                return BadRequest(new McpResponse
                {
                    Success = false,
                    Error = "Could not interpret the request (empty action)",
                    Data = new Dictionary<string, object> { ["raw"] = actionJsonRaw },
                    History = history
                });
            }

            if (aiResponse != null)
            {
                history.Add($"AI: {aiResponse}");
                return Ok(new McpResponse
                {
                    Success = false,
                    Response = aiResponse,
                    History = history
                });
            }

            // Execute the action
            var result = await _expenseTrackerClient.ExecuteActionAsync(action, request.UserId);
            var serverResponse = result.Success
                ? $"Successfully {action.Type.ToLower()}d {action.Entity.ToLower()}"
                : result.Error;
            history.Add($"Server: {serverResponse}");

            return Ok(new McpResponse
            {
                Success = result.Success,
                Response = serverResponse,
                Action = action.Type,
                Data = result.Data,
                History = history
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing MCP request");
            return StatusCode(500, new McpResponse
            {
                Success = false,
                Error = ex.Message
            });
        }
    }
}
