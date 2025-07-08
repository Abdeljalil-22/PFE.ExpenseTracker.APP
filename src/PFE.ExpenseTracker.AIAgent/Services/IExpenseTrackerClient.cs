using  PFE.ExpenseTracker.AIAgent.Models;

namespace PFE.ExpenseTracker.AIAgent.Services;

public interface IExpenseTrackerClient
{
    Task<ActionResult> ExecuteActionAsync(McpAction action, string userId);
}

public class ActionResult
{
    public bool Success { get; set; }
    public string Error { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
}
