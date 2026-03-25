// /Infrastructure/Input/MediaKeyPatternDetector.cs

using System;
using System.Timers;
using System.Diagnostics;
using Timer = System.Timers.Timer;

namespace SMP.Infrastructure.Input;

/// <summary>
/// 미디어 키 패턴 감지기
/// - 단일 키: Play/Pause, Next, Previous
/// - 패턴 키: Volume Up/Down (더블 입력 기반 Next/Previous)
/// </summary>
public class MediaKeyPatternDetector
{
    private readonly Timer _timer;

    private int _volumeUpCount = 0;
    private int _volumeDownCount = 0;

    private const int ThresholdMs = 300;

    // ✅ 이벤트
    public event Action? OnNext;
    public event Action? OnPrevious;
    public event Action? OnVolumeUp;
    public event Action? OnVolumeDown;
    public event Action? OnPlayPause;

    public MediaKeyPatternDetector()
    {
        _timer = new Timer(ThresholdMs)
        {
            AutoReset = false
        };
        _timer.Elapsed += OnTimeout;
    }

    public void HandleKey(uint vkCode)
    {
        switch (vkCode)
        {
            // 🎯 미디어 키 (즉시 반응)
            case 0xB3: // VK_MEDIA_PLAY_PAUSE
                Debug.WriteLine("[Key] PlayPause");
                OnPlayPause?.Invoke();
                break;

            case 0xB0: // VK_MEDIA_NEXT_TRACK
                Debug.WriteLine("[Key] Next");
                OnNext?.Invoke();
                break;

            case 0xB1: // VK_MEDIA_PREV_TRACK
                Debug.WriteLine("[Key] Previous");
                OnPrevious?.Invoke();
                break;

            // 🎯 볼륨 키 (패턴 기반)
            case 0xAF: // VK_VOLUME_UP
                _volumeUpCount++;
                RestartTimer();
                break;

            case 0xAE: // VK_VOLUME_DOWN
                _volumeDownCount++;
                RestartTimer();
                break;
        }
    }

    private void RestartTimer()
    {
        _timer.Stop();
        _timer.Start();
    }

    private void OnTimeout(object? sender, ElapsedEventArgs e)
    {
        Debug.WriteLine($"[Pattern] Up={_volumeUpCount}, Down={_volumeDownCount}");

        // Volume Up 패턴
        if (_volumeUpCount > 0)
        {
            if (_volumeUpCount >= 2)
                OnNext?.Invoke();   // 더블 → Next
            else
                OnVolumeUp?.Invoke();
        }

        // Volume Down 패턴
        if (_volumeDownCount > 0)
        {
            if (_volumeDownCount >= 2)
                OnPrevious?.Invoke(); // 더블 → Previous
            else
                OnVolumeDown?.Invoke();
        }

        _volumeUpCount = 0;
        _volumeDownCount = 0;
    }
}