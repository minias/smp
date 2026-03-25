// App/Interfaces/IAudioPlayer.cs

using NAudio.Wave;

namespace SMP.App.Interfaces;

/// <summary>
/// 오디오 재생 포트 (Application → Infrastructure)
/// </summary>
/// <remarks>
/// PlayerService에서 사용하는 모든 기능을 정의해야 함
/// (출처: Clean Architecture - Interface Segregation Principle)
/// </remarks>
public interface IAudioPlayer : IDisposable
{
    /// <summary>
    /// 재생 종료 이벤트
    /// </summary>
    event EventHandler<StoppedEventArgs>? OnPlaybackStopped;

    /// <summary>
    /// 비동기 재생
    /// </summary>
    Task PlayAsync(string source);

    /// <summary>
    /// 정지
    /// </summary>
    void Stop();

    /// <summary>
    /// 일시정지
    /// </summary>
    void Pause();

    /// <summary>
    /// 볼륨 설정 (0.0 ~ 1.0)
    /// </summary>
    void SetVolume(float volume);
}