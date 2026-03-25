// /App/UseCases/NextTrackUseCase.cs

using SMP.App.Service;
using SMP.Domain;

namespace SMP.App.UseCases;

public class NextTrackUseCase
{
    private readonly PlayerState _state;
    private readonly PlayUseCase _play;

    public NextTrackUseCase(PlayerState state, PlayUseCase play)
    {
        _state = state;
        _play = play;
    }

    public async Task ExecuteAsync()
    {
        if (_state.Playlist.Count == 0)
            return;

        int nextIndex;

        switch (_state.LoopMode)
        {
            case LoopMode.Single:
                nextIndex = _state.CurrentIndex;
                break;

            case LoopMode.All:
                nextIndex = (_state.CurrentIndex + 1) % _state.Playlist.Count;
                break;

            default:
                if (_state.CurrentIndex + 1 >= _state.Playlist.Count)
                {
                    _state.State = PlaybackState.Stopped;
                    return;
                }
                nextIndex = _state.CurrentIndex + 1;
                break;
        }

        await _play.ExecuteAsync(nextIndex);
    }
}