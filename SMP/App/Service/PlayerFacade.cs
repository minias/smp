// /App/Service/PlayerFacade.cs

using SMP.App.UseCases;

namespace SMP.App.Service;

/// <summary>
/// 플레이어 오케스트레이션 Facade
/// - UI / MediaKey 입력을 UseCase로 라우팅
/// </summary>
public class PlayerFacade
{
    private readonly PlayUseCase _playUseCase;
    private readonly NextTrackUseCase _nextUseCase;
    private readonly PrevTrackUseCase _prevUseCase;
    private readonly StopUseCase _stopUseCase;
    private readonly SetVolumeUseCase _volumeUseCase;
    private readonly PlayPauseUseCase _playPauseUseCase;

    public PlayerFacade(
        PlayUseCase playUseCase,
        NextTrackUseCase nextUseCase,
        PrevTrackUseCase prevUseCase,
        StopUseCase stopUseCase,
        SetVolumeUseCase volumeUseCase,
        PlayPauseUseCase playPauseUseCase)
    {
        _playUseCase = playUseCase;
        _nextUseCase = nextUseCase;
        _prevUseCase = prevUseCase;
        _stopUseCase = stopUseCase;
        _volumeUseCase = volumeUseCase;
        _playPauseUseCase = playPauseUseCase;
    }

    /// <summary>
    /// 특정 트랙 재생
    /// </summary>
    public Task Play(int index)
    {
        return _playUseCase.ExecuteAsync(index);
    }

    /// <summary>
    /// 다음 곡
    /// </summary>
    public Task Next()
    {
        return _nextUseCase.ExecuteAsync();
    }

    /// <summary>
    /// 이전 곡
    /// </summary>
    public Task Previous()
    {
        return _prevUseCase.ExecuteAsync();
    }

    /// <summary>
    /// 재생/일시정지 토글
    /// </summary>
    public Task PlayPause()
    {
        return _playPauseUseCase.ExecuteAsync();
    }

    /// <summary>
    /// 정지
    /// </summary>
    public void Stop()
    {
        _stopUseCase.Execute();
    }

    /// <summary>
    /// 볼륨 설정
    /// </summary>
    public void SetVolume(float volume)
    {
        _volumeUseCase.Execute(volume);
    }
}