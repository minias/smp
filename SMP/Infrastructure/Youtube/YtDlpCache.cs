// File: Infrastructure/Youtube/YtDlpCache.cs

using System.Collections.Concurrent;

namespace SMP.Infrastructure.Youtube;

/// <summary>
/// yt-dlp 스트림 URL 캐시 (TTL 포함)
/// </summary>
public class YtDlpCache
{
    private class CacheEntry
    {
        public string StreamUrl { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

    // TTL 설정
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(30);

    /// <summary>
    /// 캐시 조회 (TTL 검사 포함)
    /// </summary>
    public bool TryGet(string key, out string streamUrl)
    {
        streamUrl = string.Empty;

        if (!_cache.TryGetValue(key, out var entry))
            return false;

        // TTL 만료 체크
        if (DateTime.UtcNow - entry.CreatedAt > _ttl)
        {
            _cache.TryRemove(key, out _);
            return false;
        }

        streamUrl = entry.StreamUrl;
        return true;
    }

    /// <summary>
    /// 캐시 저장
    /// </summary>
    public void Set(string key, string streamUrl)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        if (string.IsNullOrWhiteSpace(streamUrl)) return;

        _cache[key] = new CacheEntry
        {
            StreamUrl = streamUrl,
            CreatedAt = DateTime.UtcNow
        };
    }
}