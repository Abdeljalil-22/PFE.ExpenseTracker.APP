
namespace PFE.ExpenseTracker.AIAgent.Services;

// using PFE.ExpenseTracker.Infrastructure.Services;
using PFE.ExpenseTracker.Application.Common.Interfaces;
using PFE.ExpenseTracker.AIAgent.Models;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PFE.ExpenseTracker.Application.Common.Interfaces.Repository;

public class AIAgent
{
    private readonly IExpenseTrackerClient _expenseTrackerClient;
    private readonly ILogger<AIAgent> _logger;
    private readonly  IGeminiAIService _geminiService;
    private readonly IChatHistoryService _chatHistoryService;
    private readonly IReadChatHistoryRepository _readChatHistoryRepository;
    private readonly IWriteChatHistoryRepository _writeChatHistoryRepository;

    public AIAgent(
        IExpenseTrackerClient expenseTrackerClient,
        ILogger<AIAgent> logger,
        IGeminiAIService geminiService,
        IChatHistoryService chatHistoryService,
        IReadChatHistoryRepository readChatHistoryRepository,
        IWriteChatHistoryRepository writeChatHistoryRepository)
    {
        _expenseTrackerClient = expenseTrackerClient;
        _logger = logger;
        _geminiService = geminiService;
        _chatHistoryService = chatHistoryService;
        _readChatHistoryRepository = readChatHistoryRepository;
        _writeChatHistoryRepository = writeChatHistoryRepository;
    }
   

    public async Task<McpProcessResponse> ProcessAsync(McpProcessRequest request)
    {
        try
        {
             // Load history from Redis
            var history = new List<string>();
            if (!request.IgnoreHistory) // Allow client to optionally start a fresh conversation
            {
                var savedHistory = await _chatHistoryService.GetChatHistoryAsync(request.UserId);
                history = savedHistory;
            }

            // Add current prompt to history
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
            string? aiResponse = null;
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
                return new McpProcessResponse
                {
                    Success = false,
                    Error = "Could not interpret the request (empty action)",
                    Data = new Dictionary<string, object> { ["raw"] = actionJsonRaw },
                    History = history
                };
            }

            if (aiResponse != null)
            {
                history.Add($"AI: {aiResponse}");
                await _chatHistoryService.SaveChatHistoryAsync(request.UserId, history);
                return new McpProcessResponse
                {
                    Success = false,
                    Response = aiResponse,
                    History = history
                };
            }

            if (action == null) // Already handled above but needed for null-safety
            {
                throw new InvalidOperationException("Unexpected null action after validation");
            }

            // Execute the action
            var result = await _expenseTrackerClient.ExecuteActionAsync(action, request.UserId);
            var serverResponse = result.Success
                ? $"Successfully {action.Type?.ToLower() ?? "executed"} {action.Entity?.ToLower() ?? "action"}"
                : result.Error ?? "Unknown error occurred";
            history.Add($"Server: {serverResponse}");

            // Save updated history to Redis
            await _chatHistoryService.SaveChatHistoryAsync(request.UserId, history);
            // Save updated history to database
            await _writeChatHistoryRepository.SaveOrUpdateAsync(request.UserId, history);

            return new McpProcessResponse
            {
                Success = result.Success,
                Response = serverResponse,
                Action = action.Type ?? "unknown",
                Data = result.Data,
                History = history
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing AI request");
            return new McpProcessResponse { Success = false, Error = ex.Message };
        }
    }
}