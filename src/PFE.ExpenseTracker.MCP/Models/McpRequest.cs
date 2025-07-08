namespace PFE.ExpenseTracker.MCP.Models;

public class McpRequest
{
    public string? Prompt { get; set; }
    public string? UserId { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();
    public List<string>? History { get; set; } = new();
    public bool IgnoreHistory { get; set; } // Allow client to optionally start a fresh conversation
}




