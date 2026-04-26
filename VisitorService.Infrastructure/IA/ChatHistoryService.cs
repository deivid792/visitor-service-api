using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;

public class ChatHistoryService
{
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _expiry = TimeSpan.FromDays(7);

    public ChatHistoryService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<List<ChatMessage>?> GetHistoryAsync(string userId)
    {
        var json = await _cache.GetStringAsync(userId);
        if (string.IsNullOrEmpty(json)) return null;

        return JsonSerializer.Deserialize<List<ChatMessage>>(json);
    }

    public async Task SaveHistoryAsync(string userId, List<ChatMessage> messages)
    {
        var historyToSave = messages.TakeLast(10).ToList();
        var json = JsonSerializer.Serialize(historyToSave);
        
        await _cache.SetStringAsync(userId, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _expiry
        });
    }
}