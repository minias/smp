// Infrastructure/Audio/StreamingAudioPlayer.cs

using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SMP.App.Interfaces;

namespace SMP.Infrastructure.Audio;

/// <summary>
/// 스트리밍 오디오 플레이어 (NAudio 기반)
/// - URL 기반 스트리밍 재생
/// - 볼륨 제어 지원
/// - Pause / Resume 지원
/// - 재생 종료 이벤트 제공
/// </summary>
public class StreamingAudioPlayer(IWavePlayer output) : IAudioPlayer
{
    /// <summary>
    /// 오디오 출력 장치 (DI)
    /// </summary>
    private readonly IWavePlayer _output = output ?? throw new ArgumentNullException(nameof(output));

    /// <summary>
    /// 미디어 디코더 (URL → PCM)
    /// </summary>
    private MediaFoundationReader? _reader;

    /// <summary>
    /// 볼륨 조절 Provider
    /// </summary>
    private VolumeSampleProvider? _volumeProvider;

    /// <summary>
    /// 현재 볼륨 상태 (트랙 변경 시 유지)
    /// </summary>
    private float _currentVolume = 0.5f;

    /// <summary>
    /// 재생 종료 이벤트
    /// </summary>
    public event EventHandler<StoppedEventArgs>? OnPlaybackStopped;

    private bool _disposed;

    /// <summary>
    /// 비동기 재생
    /// </summary>
    public Task PlayAsync(string url)
    {
        PlayInternal(url);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 내부 재생 로직
    /// </summary>
    private void PlayInternal(string url)
    {
        Stop();

        // 1. Reader 생성
        _reader = new MediaFoundationReader(url);

        // 2. SampleProvider 변환
        var sampleProvider = _reader.ToSampleProvider();

        // 3. Volume 적용
        _volumeProvider = new VolumeSampleProvider(sampleProvider)
        {
            Volume = _currentVolume
        };

        // 4. 이벤트 핸들러 (단일 구조)
        _output.PlaybackStopped -= HandlePlaybackStopped;
        _output.PlaybackStopped += HandlePlaybackStopped;

        // 5. 초기화 및 재생
        _output.Init(_volumeProvider);
        _output.Play();
    }

    /// <summary>
    /// Pause
    /// </summary>
    public void Pause()
    {
        if (_output.PlaybackState == PlaybackState.Playing)
        {
            _output.Pause();
        }
    }

    /// <summary>
    /// Resume
    /// </summary>
    public void Resume()
    {
        if (_output.PlaybackState == PlaybackState.Paused)
        {
            _output.Play();
        }
    }

    /// <summary>
    /// 현재 상태 반환
    /// </summary>
    public PlaybackState GetPlaybackState()
    {
        return _output.PlaybackState;
    }

    /// <summary>
    /// 볼륨 설정 (0.0 ~ 1.0)
    /// </summary>
    public void SetVolume(float volume)
    {
        _currentVolume = Math.Clamp(volume, 0f, 1f);

        _volumeProvider?.Volume = _currentVolume;
    }

    /// <summary>
    /// 정지 및 리소스 해제
    /// </summary>
    public void Stop()
    {
        try
        {
            _output.Stop();
        }
        catch
        {
            // NAudio 내부 예외 무시
        }

        _reader?.Dispose();
        _reader = null;

        _volumeProvider = null;
    }

    /// <summary>
    /// 내부 이벤트 처리
    /// </summary>
    private void HandlePlaybackStopped(object? sender, StoppedEventArgs e)
    {
        OnPlaybackStopped?.Invoke(this, e);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 실제 Dispose 로직
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _output.PlaybackStopped -= HandlePlaybackStopped;

            Stop();
            _output.Dispose();
        }

        _disposed = true;
    }
}