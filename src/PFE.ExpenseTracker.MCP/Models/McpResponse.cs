
namespace PFE.ExpenseTracker.MCP.Models;

public class McpResponse
{
    public string? Response { get; set; }
    public string? Action { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public bool Success { get; set; }
    public string? Error { get; set; }
    public List<string>? History { get; set; } = new();
}