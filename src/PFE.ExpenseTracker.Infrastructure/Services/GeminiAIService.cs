using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PFE.ExpenseTracker.Infrastructure.Services;

public class GeminiAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public GeminiAIService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _apiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("Gemini:ApiKey not configured.");
        _model = configuration["Gemini:Model"] ?? "gemini-pro";
    }

    public async Task<string> GetActionFromPromptAsync(string prompt)
    {

var systemInstruction = """
You are an advanced AI assistant for a personal finance app. Your job is to extract the user's intent from natural language and return a single, valid JSON object describing the action to perform.
Supported actions: CREATE, READ, UPDATE, DELETE. Supported entities: Expense, Budget, Category, FinancialGoal.
For each entity, map user input to the following parameters (fill as many as possible):

Expense: amount (decimal), description (string), date (ISO 8601), categoryId (GUID), notes (string), isRecurring (bool), recurringFrequency (string), isShared (bool), id (GUID, for update/delete)
Budget: name (string), amount (decimal), startDate (ISO 8601), endDate (ISO 8601), categoryId (GUID), alertEnabled (bool), alertThresholdPercentage (int), id (GUID, for update/delete)
Category: name (string), description (string), color (string), icon (string), id (GUID, for update/delete)
FinancialGoal: name (string), description (string), targetAmount (decimal), targetDate (ISO 8601), id (GUID, for update/delete)

Always use ISO 8601 for dates. If the user asks for a list, use type: READ. If the user asks to update or delete, require an 'id' parameter.

If you cannot infer a required parameter (such as amount, name, or id) from the user's prompt, respond with a clear, conversational question asking the user to provide the missing information. For example: "What is the amount for the expense?" or "Which budget do you want to update? Please provide the budget id."

Return ONLY the JSON object if all required information is present. If you need to ask the user for missing info, return ONLY the question as plain text (no JSON, no extra explanation).

Example: For 'add a new expense of 50$ for lunch today', return:
{
  "type": "CREATE",
  "entity": "Expense",
  "parameters": {
    "amount": 50,
    "description": "lunch",
    "date": "2025-07-03"
  }
}

Example: For 'update my budget', return:
Which budget do you want to update? Please provide the budget id.
""";
    

      
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_model}:generateContent?key={_apiKey}";
        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    role = "user",
                    parts = new[]
                    {
                        new { text = $"{systemInstruction}\n\nUser request: {prompt}" }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.1,
                maxOutputTokens = 1000
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Gemini API error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        var root = doc.RootElement;

        if (root.TryGetProperty("candidates", out var candidates) &&
            candidates.GetArrayLength() > 0 &&
            candidates[0].TryGetProperty("content", out var contentProp) &&
            contentProp.TryGetProperty("parts", out var parts) &&
            parts.GetArrayLength() > 0 &&
            parts[0].TryGetProperty("text", out var textProp))
        {
            return textProp.GetString();
        }

        throw new Exception("Failed to get a valid response from Gemini API");
    }
}