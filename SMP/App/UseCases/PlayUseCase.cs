// /App/UseCases/PlayUseCase.cs

using SMP.App.Interfaces;
using SMP.Domain;
using SMP.App.Service;

namespace SMP.App.UseCases;

/// <summary>
/// 재생 UseCase
/// </summary>
public class PlayUseCase
{
    private readonly IAudioPlayer _player;
    private readonly IYoutubeService _yt;
    private readonly PlayerState _state;

    public event Action<PlaylistItem>? OnTrackChanged;

    public PlayUseCase(
        IAudioPlayer player,
        IYoutubeService yt,
        PlayerState state)
    {
        _player = player;
        _yt = yt;
        _state = state;
    }

    public async Task ExecuteAsync(int index)
    {
        if (_state.Playlist.Count == 0)
            return;

        index = (index < 0 || index >= _state.Playlist.Count) ? 0 : index;
        _state.CurrentIndex = index;

        var item = _state.Playlist[index];

        var url = await _yt.GetStreamUrlAsync(item.Url);

        if (string.IsNullOrWhiteSpace(url))
        {
            _state.State = PlaybackState.Stopped;
            return;
        }

        _player.Stop();

        OnTrackChanged?.Invoke(item);

        await _player.PlayAsync(url);

        _state.State = PlaybackState.Playing;
    }
}