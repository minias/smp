// /App/Service/PlayerState.cs

using SMP.Domain;

namespace SMP.App.Service;

/// <summary>
/// 플레이어 상태 관리 (순수 상태)
/// </summary>
public class PlayerState
{
    public List<PlaylistItem> Playlist { get; set; } = [];
    public int CurrentIndex { get; set; } = -1;
    public PlaybackState State { get; set; } = PlaybackState.Stopped;
    public LoopMode LoopMode { get; set; } = LoopMode.None;

    public float Volume { get; set; } = 0.5f;
}