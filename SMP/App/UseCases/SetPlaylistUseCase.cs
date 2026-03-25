// /App/UseCases/SetPlaylistUseCase.cs

using SMP.App.Service;
using SMP.Domain;

namespace SMP.App.UseCases;

public class SetPlaylistUseCase(PlayerState state)
{
    public void Execute(List<PlaylistItem> items)
    {
        state.Playlist = items ?? [];
        state.CurrentIndex = -1;
    }
}