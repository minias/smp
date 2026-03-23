// App/Ports/IAudioPlayer.cs

namespace SMP.App.Ports;

/// <summary>
/// 오디오 재생 포트
/// </summary>
public interface IAudioPlayer
{
    /// <summary>
    /// 비동기 재생
    /// </summary>
    Task PlayAsync(string source);

    /// <summary>
    /// 정지
    /// </summary>
    void Stop();
}