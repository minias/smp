// /App/UseCases/SetVolumeUseCase.cs

using SMP.App.Interfaces;
using SMP.App.Service;

namespace SMP.App.UseCases;

public class SetVolumeUseCase(
    IAudioPlayer player,
    PlayerState state)
{
    public void Execute(float volume)
    {
        state.Volume = volume;
        player.SetVolume(volume);
    }
}