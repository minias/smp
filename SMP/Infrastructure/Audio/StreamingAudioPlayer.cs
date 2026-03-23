// Infrastructure/Audio/StreamingAudioPlayer.cs

using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SMP.App.Ports;

namespace SMP.Infrastructure.Audio;

/// <summary>
/// 스트리밍 오디오 플레이어 (NAudio 기반)
/// - URL 기반 스트리밍 재생
/// - 볼륨 제어 지원
/// - 재생 종료 이벤트 제공
/// </summary>
/// <remarks>
/// 생성자 (DI)
/// </remarks>
public class StreamingAudioPlayer(IWavePlayer output) : IAudioPlayer, IDisposable
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
    /// 이벤트 핸들러 참조 (해제용)
    /// </summary>
    private EventHandler<StoppedEventArgs>? _playbackStoppedHandler;

    /// <summary>
    /// 재생 종료 이벤트
    /// </summary>
    public event EventHandler<StoppedEventArgs>? OnPlaybackStopped;

    /// <summary>
    /// 비동기 재생 (인터페이스 구현)
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
        // 기존 리소스 정리
        Stop();

        // 1. Reader 생성 (디코딩)
        _reader = new MediaFoundationReader(url);

        // 2. SampleProvider 변환
        var sampleProvider = _reader.ToSampleProvider();

        // 3. Volume 적용
        _volumeProvider = new VolumeSampleProvider(sampleProvider)
        {
            Volume = _currentVolume
        };

        // 4. 이벤트 핸들러 등록
        _playbackStoppedHandler = (s, e) =>
        {
            OnPlaybackStopped?.Invoke(this, e);
        };

        _output.PlaybackStopped += _playbackStoppedHandler;

        // 5. 초기화 및 재생
        _output.Init(_volumeProvider);
        _output.Play();
    }

    /// <summary>
    /// 볼륨 설정 (0.0 ~ 1.0)
    /// </summary>
    public void SetVolume(float volume)
    {
        _currentVolume = Math.Clamp(volume, 0f, 1f);

        // 현재 재생 중이면 즉시 반영
        _volumeProvider?.Volume = _currentVolume;
    }

    /// <summary>
    /// 정지 및 리소스 해제
    /// </summary>
    public void Stop()
    {
        try
        {
            // 이벤트 핸들러 제거
            if (_playbackStoppedHandler != null)
            {
                _output.PlaybackStopped -= _playbackStoppedHandler;
                _playbackStoppedHandler = null;
            }

            _output.Stop();
        }
        catch
        {
            // 종료 중 예외 무시 (NAudio 특성)
        }

        // 리소스 해제
        _reader?.Dispose();
        _reader = null;

        _volumeProvider = null;
    }

    /// <summary>
    /// Dispose 패턴
    /// </summary>
    public void Dispose()
    {
        Stop();

        // 출력 장치 해제
        _output.Dispose();

        GC.SuppressFinalize(this);
    }
}