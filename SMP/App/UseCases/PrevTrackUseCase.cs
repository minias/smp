// /App/UseCases/PrevTrackUseCase.cs

using SMP.App.Service;

namespace SMP.App.UseCases;

public class PrevTrackUseCase(PlayerState state, PlayUseCase playUseCase)
{
    public async Task ExecuteAsync()
    {
        if (state.Playlist.Count == 0)
            return;

        state.CurrentIndex =
            (state.CurrentIndex - 1 + state.Playlist.Count)
            % state.Playlist.Count;

        await playUseCase.ExecuteAsync(state.CurrentIndex);
    }
}