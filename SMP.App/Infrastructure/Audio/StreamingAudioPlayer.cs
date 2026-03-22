// Infrastructure/Audio/StreamingAudioPlayer.cs
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using SMP.Domain;

namespace SMP.Infrastructure.Audio;

public class StreamingAudioPlayer : IAudioPlayer, IDisposable
{
    private IWavePlayer? _output;
    private MediaFoundationReader? _reader;
    private VolumeSampleProvider? _volumeProvider;

    // 현재 볼륨 상태 유지 (트랙 변경 시 유지)
    private float _currentVolume = 0.5f;

    // 이벤트 핸들러 참조 (해제용)
    private EventHandler<StoppedEventArgs>? _playbackStoppedHandler;

    public event EventHandler<StoppedEventArgs>? OnPlaybackStopped;

    /// <summary>
    /// 볼륨 설정 (0.0 ~ 1.0)
    /// </summary>
    public void SetVolume(float volume)
    {
        _currentVolume = Math.Clamp(volume, 0f, 1f);

        if (_volumeProvider != null)
        {
            _volumeProvider.Volume = _currentVolume;
        }
    }

    /// <summary>
    /// 스트리밍 재생
    /// </summary>
    public void Play(string url)
    {
        Stop(); // 기존 리소스 정리

        // 1. Reader (디코딩)
        _reader = new MediaFoundationReader(url);

        // 2. SampleProvider 변환
        var sampleProvider = _reader.ToSampleProvider();

        // 3. Volume 적용
        _volumeProvider = new VolumeSampleProvider(sampleProvider)
        {
            Volume = _currentVolume
        };

        // 4. Output 장치
        _output = new WaveOutEvent();

        // 이벤트 핸들러 생성 (참조 유지)
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
    /// 정지 및 리소스 해제
    /// </summary>
    public void Stop()
    {
        if (_output != null)
        {
            try
            {
                // 이벤트 해제 (중요)
                if (_playbackStoppedHandler != null)
                {
                    _output.PlaybackStopped -= _playbackStoppedHandler;
                    _playbackStoppedHandler = null;
                }

                _output.Stop();
            }
            catch
            {
                // 종료 중 예외 무시
            }
        }

        _reader?.Dispose();
        _output?.Dispose();

        _reader = null;
        _output = null;
        _volumeProvider = null;
    }

    /// <summary>
    /// 자원 해제 (Dispose 패턴)
    /// </summary>
    public void Dispose()
    {
        Stop();
    }
}