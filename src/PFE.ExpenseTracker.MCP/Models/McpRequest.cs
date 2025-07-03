namespace PFE.ExpenseTracker.MCP.Models;

public class McpRequest
{
    public string Prompt { get; set; }
    public string UserId { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();
}

public class McpResponse
{
    public string Response { get; set; }
    public string Action { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public bool Success { get; set; }
    public string Error { get; set; }
}

public class McpAction
{
    public string Type { get; set; } // CREATE, READ, UPDATE, DELETE
    public string Entity { get; set; } // Expense, Budget, Category, etc.
    public Dictionary<string, object> Parameters { get; set; } = new();
}
