using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PFE.ExpenseTracker.MCP.Models;

namespace PFE.ExpenseTracker.MCP.Services;

public class ExpenseTrackerClient : IExpenseTrackerClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExpenseTrackerClient> _logger;
    private readonly string _baseUrl;

    public ExpenseTrackerClient(IConfiguration configuration, ILogger<ExpenseTrackerClient> logger)
    {
        _httpClient = new HttpClient();
        _logger = logger;
        _baseUrl = configuration["ExpenseTracker:ApiUrl"] ?? "http://localhost:5000/api/";
    }

    public async Task<ActionResult> ExecuteActionAsync(McpAction action, string userId)
    {
        try
        {
            // Prepare request based on action type
            var endpoint = GetEndpoint(action);
            var method = GetHttpMethod(action.Type);
            var content = action.Type != "DELETE" && action.Type != "READ"
                ? CreateRequestContent(action, userId)
                : null;

            var request = new HttpRequestMessage(method, $"{_baseUrl}{endpoint}")
            {
                Content = content
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Get JWT token for the user (simulate or fetch from auth service)
            string jwt = await GetJwtForUser(userId);
            if (!string.IsNullOrEmpty(jwt))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await _httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
                    return new ActionResult { Success = true, Data = data };
                }
                catch
                {
                    return new ActionResult { Success = true, Data = new Dictionary<string, object> { ["raw"] = responseContent } };
                }
            }
            else
            {
                _logger.LogWarning("API error: {Status} - {Content}", response.StatusCode, responseContent);
                return new ActionResult { Success = false, Error = responseContent };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing action {Type} for {Entity}", action.Type, action.Entity);
            return new ActionResult { Success = false, Error = ex.Message };
        }
    }

    // Simulate JWT retrieval (replace with real auth integration)
    private async Task<string> GetJwtForUser(string userId)
    {
        // TODO: Call Auth API or cache token per user
        // For demo, return a static token or fetch from config
        await Task.CompletedTask;
        return "demo-jwt-token";
    }

    private string GetEndpoint(McpAction action)
    {
        var entityPlural = action.Entity switch
        {
            "Expense" => "expenses",
            "Budget" => "budgets",
            "Category" => "categories",
            "FinancialGoal" => "financialgoals",
            _ => throw new ArgumentException($"Unknown entity: {action.Entity}")
        };

        if (action.Type == "CREATE")
            return entityPlural;
        
        if (action.Parameters.TryGetValue("id", out var id))
            return $"{entityPlural}/{id}";
        
        return entityPlural;
    }

    private HttpMethod GetHttpMethod(string actionType) => actionType switch
    {
        "CREATE" => HttpMethod.Post,
        "READ" => HttpMethod.Get,
        "UPDATE" => HttpMethod.Put,
        "DELETE" => HttpMethod.Delete,
        _ => throw new ArgumentException($"Unknown action type: {actionType}")
    };

    private HttpContent CreateRequestContent(McpAction action, string userId)
    {
        var parameters = new Dictionary<string, object>(action.Parameters)
        {
            ["userId"] = userId
        };

        var json = JsonSerializer.Serialize(parameters);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
