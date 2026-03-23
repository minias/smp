// File: Infrastructure/Youtube/YtDlpService.cs

using System.Diagnostics;
using System.Collections.Concurrent;

namespace SMP.Infrastructure.Youtube;

/// <summary>
/// 유튜브 URL → 실제 스트림 URL / 제목 추출 서비스
/// - yt-dlp.exe 기반
/// - TTL 캐시
/// - 중복 요청 방지 (async lock)
/// </summary>
public class YtDlpService(string exePath)
{
    private readonly string _exePath = exePath;

    /// <summary>
    /// 캐시 엔트리 (값 + 생성 시간)
    /// </summary>
    private class CacheEntry
    {
        public string StreamUrl { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

    /// <summary>
    /// 스트림 URL 캐시
    /// </summary>
    private readonly ConcurrentDictionary<string, CacheEntry> _streamCache = new();

    /// <summary>
    /// URL별 Lock (중복 요청 방지)
    /// </summary>
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    /// <summary>
    /// 캐시 TTL
    /// </summary>
    private readonly TimeSpan _ttl = TimeSpan.FromMinutes(30);

    /// <summary>
    /// URL별 lock 획득
    /// </summary>
    private async Task<IDisposable> AcquireLockAsync(string key)
    {
        var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();

        return new Releaser(() =>
        {
            semaphore.Release();

            // cleanup (optional)
            if (semaphore.CurrentCount == 1)
                _locks.TryRemove(key, out _);
        });
    }

    /// <summary>
    /// Lock 해제용 IDisposable
    /// </summary>
    private class Releaser(Action release) : IDisposable
    {
        private readonly Action _release = release;

        public void Dispose() => _release();
    }

    /// <summary>
    /// 유튜브 URL → 실제 스트림 URL 추출 (캐싱 + TTL + Lock 포함)
    /// </summary>
    public async Task<string?> GetStreamUrlAsync(string youtubeUrl)
    {
        // ✅ 1. 캐시 확인
        if (TryGetFromCache(youtubeUrl, out var cached))
        {
            Console.WriteLine("[yt-dlp] cache hit");
            return cached;
        }

        // ✅ 2. 중복 요청 방지 Lock
        using (await AcquireLockAsync(youtubeUrl))
        {
            // Lock 대기 중 캐시가 채워졌을 수 있음 → 재확인
            if (TryGetFromCache(youtubeUrl, out cached))
            {
                Console.WriteLine("[yt-dlp] cache hit (after lock)");
                return cached;
            }

            // ✅ 3. yt-dlp 실행
            var psi = new ProcessStartInfo
            {
                FileName = _exePath,
                Arguments = $"-f bestaudio --no-playlist -g {youtubeUrl}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null) return null;

            string? output = await process.StandardOutput.ReadLineAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (!string.IsNullOrWhiteSpace(error))
                Console.WriteLine($"[yt-dlp-error] {error}");

            if (string.IsNullOrWhiteSpace(output))
                return null;

            var streamUrl = output.Trim();

            // ✅ 4. 캐시 저장
            SetCache(youtubeUrl, streamUrl);

            return streamUrl;
        }
    }

    /// <summary>
    /// 제목 가져오기
    /// </summary>
    public async Task<string?> GetTitleAsync(string youtubeUrl)
    {
        var psi = new ProcessStartInfo
        {
            FileName = _exePath,
            Arguments = $"--get-title {youtubeUrl}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null) return null;

        string title = await process.StandardOutput.ReadLineAsync() ?? "";
        await process.WaitForExitAsync();

        return string.IsNullOrWhiteSpace(title) ? null : title.Trim();
    }

    /// <summary>
    /// 캐시 조회 (TTL 체크 포함)
    /// </summary>
    private bool TryGetFromCache(string key, out string streamUrl)
    {
        streamUrl = string.Empty;

        if (!_streamCache.TryGetValue(key, out var entry))
            return false;

        // TTL 만료 체크
        if (DateTime.UtcNow - entry.CreatedAt > _ttl)
        {
            _streamCache.TryRemove(key, out _);
            return false;
        }

        streamUrl = entry.StreamUrl;
        return true;
    }

    /// <summary>
    /// 캐시 저장
    /// </summary>
    private void SetCache(string key, string streamUrl)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        if (string.IsNullOrWhiteSpace(streamUrl)) return;

        _streamCache[key] = new CacheEntry
        {
            StreamUrl = streamUrl,
            CreatedAt = DateTime.UtcNow
        };
    }
}