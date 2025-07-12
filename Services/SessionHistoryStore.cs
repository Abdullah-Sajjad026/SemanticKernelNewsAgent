using System.Collections.Concurrent;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SemanticKernalNewsAPI.Services;

public class SessionHistoryStore
{
    private readonly ILogger<SessionHistoryStore> _log;
    private readonly ConcurrentDictionary<string, Entry> _sessions = new();
    private readonly TimeSpan _sessionTimeout = TimeSpan.FromMinutes(1);

    public SessionHistoryStore(ILogger<SessionHistoryStore> log)
    {
        _log = log;
        _ = Task.Run(CleanupLoop);
    }

    public ChatHistory GetChatHistory(string sessionId)
    {
        var entry = _sessions.GetOrAdd(sessionId, _ =>
        {
            _log.LogInformation("Starting new session {SessionId}", sessionId);
            return new Entry { History = new ChatHistory(), LastSeen = DateTime.UtcNow };
        });

        entry.LastSeen = DateTime.UtcNow;
        return entry.History;
    }

    private async Task CleanupLoop()
    {
        while (true)
        {
            var now = DateTime.UtcNow;
            var expired = _sessions.Where(kvp => now - kvp.Value.LastSeen > _sessionTimeout)
                .Select(kvp => kvp.Key).ToList();

            foreach (var key in expired)
            {
                if (_sessions.TryRemove(key, out _))
                    _log.LogInformation("Removed expired session {SessionId}", key);
            }

            await Task.Delay(TimeSpan.FromSeconds(30));
        }
    }

    private class Entry
    {
        public ChatHistory History { get; set; } = null!;
        public DateTime LastSeen { get; set; }
    }
}