

namespace PFE.ExpenseTracker.AIAgent.Models;
public class McpProcessRequest
{
    public string? Prompt { get; set; }
    public string? UserId { get; set; }
    public Dictionary<string, object>? Context { get; set; }
    public List<string>? History { get; set; }
    public bool IgnoreHistory { get; set; }
}
