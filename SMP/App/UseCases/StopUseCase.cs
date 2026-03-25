// /App/UseCases/StopUseCase.cs

using SMP.App.Interfaces;
using SMP.App.Service;
using SMP.Domain;

namespace SMP.App.UseCases;

public class StopUseCase
{
    private readonly IAudioPlayer _player;
    private readonly PlayerState _state;

    public StopUseCase(IAudioPlayer player, PlayerState state)
    {
        _player = player;
        _state = state;
    }

    public void Execute()
    {
        _player.Stop();
        _state.State = PlaybackState.Stopped;
    }
}