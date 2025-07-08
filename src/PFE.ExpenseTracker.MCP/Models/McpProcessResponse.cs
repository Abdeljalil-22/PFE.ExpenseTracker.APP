
namespace PFE.ExpenseTracker.MCP.Models;
public class McpProcessResponse
{
    public bool Success { get; set; }
    public string? Response { get; set; }
    public string? Action { get; set; }
    public object? Data { get; set; }
    public string? Error { get; set; }
    public List<string>? History { get; set; }
}