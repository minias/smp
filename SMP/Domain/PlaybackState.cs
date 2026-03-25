// File: SMP.Domain/PlaybackState.cs

namespace SMP.Domain;

/// <summary>
/// 플레이어 재생 상태
/// </summary>
public enum PlaybackState
{
    /// <summary>
    /// 정지
    /// </summary>
    Stopped,

    /// <summary>
    /// 재생 중
    /// </summary>
    Playing,

    /// <summary>
    /// 일시정지
    /// </summary>
    Paused
}