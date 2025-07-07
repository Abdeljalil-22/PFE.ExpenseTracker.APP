using System.Text.Json;
using StackExchange.Redis;

namespace PFE.ExpenseTracker.Infrastructure.Services;

public interface IChatHistoryService
{
    Task<List<string>> GetChatHistoryAsync(string userId);
    Task SaveChatHistoryAsync(string userId, List<string> history);
}

public class ChatHistoryService : IChatHistoryService
{
    private readonly IConnectionMultiplexer _redis;
    private const string CHAT_HISTORY_KEY_PREFIX = "chat:history:";
    private const int CHAT_HISTORY_EXPIRY_DAYS = 30;

    public ChatHistoryService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    private string GetChatHistoryKey(string userId) => $"{CHAT_HISTORY_KEY_PREFIX}{userId}";

    public async Task<List<string>> GetChatHistoryAsync(string userId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        
        var db = _redis.GetDatabase();
        var key = GetChatHistoryKey(userId);
        
        var historyJson = await db.StringGetAsync(key);
        if (!historyJson.HasValue)
            return new List<string>();

        return JsonSerializer.Deserialize<List<string>>(historyJson!) ?? new List<string>();
    }

    public async Task SaveChatHistoryAsync(string userId, List<string> history)
    {
        ArgumentNullException.ThrowIfNull(userId);
        ArgumentNullException.ThrowIfNull(history);
        
        var db = _redis.GetDatabase();
        var key = GetChatHistoryKey(userId);
        
        var historyJson = JsonSerializer.Serialize(history);
        await db.StringSetAsync(
            key,
            historyJson,
            expiry: TimeSpan.FromDays(CHAT_HISTORY_EXPIRY_DAYS)
        );
    }
}
