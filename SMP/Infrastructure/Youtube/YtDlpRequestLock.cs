// File: Infrastructure/Youtube/YtDlpRequestLock.cs

using System.Collections.Concurrent;

namespace SMP.Infrastructure.Youtube;

/// <summary>
/// 동일 URL 중복 요청 방지 (In-flight dedup)
/// </summary>
public class YtDlpRequestLock
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    /// <summary>
    /// URL별 Lock 획득
    /// </summary>
    public async Task<IDisposable> AcquireAsync(string key)
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

    private class Releaser(Action release) : IDisposable
    {
        private readonly Action _release = release;

        public void Dispose() => _release();
    }
}