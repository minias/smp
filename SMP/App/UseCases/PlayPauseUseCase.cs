// /App/UseCases/PlayPauseUseCase.cs

using SMP.App.Interfaces;
using SMP.App.Service;
using SMP.Domain;

namespace SMP.App.UseCases;

/// <summary>
/// 재생/일시정지 토글 UseCase
/// </summary>
public class PlayPauseUseCase
{
    private readonly IAudioPlayer _player;
    private readonly PlayerState _state;
    private readonly PlayUseCase _playUseCase;

    public PlayPauseUseCase(
        IAudioPlayer player,
        PlayerState state,
        PlayUseCase playUseCase)
    {
        _player = player;
        _state = state;
        _playUseCase = playUseCase;
    }

    /// <summary>
    /// 토글 실행
    /// </summary>
    public async Task ExecuteAsync()
    {
        if (_state.State == PlaybackState.Playing)
        {
            _player.Pause();
            _state.State = PlaybackState.Paused;
        }
        else
        {
            await _playUseCase.ExecuteAsync(_state.CurrentIndex);
            _state.State = PlaybackState.Playing;
        }
    }
}