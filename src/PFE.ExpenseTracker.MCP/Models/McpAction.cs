using System.Text.Json.Serialization;

namespace PFE.ExpenseTracker.MCP.Models;
public class McpAction
{
    [JsonPropertyName("type")]
    public string? Type { get; set; } // CREATE, READ, UPDATE, DELETE

    [JsonPropertyName("entity")]
    public string? Entity { get; set; } // Expense, Budget, Category, etc.

    [JsonPropertyName("parameters")]
    public Dictionary<string, object> Parameters { get; set; } = new();
}