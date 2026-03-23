using SMP.Domain;
using SMP.Infrastructure.Audio;
using SMP.Infrastructure.Youtube;
using NAudio.Wave;

namespace SMP.App.Service;

/// <summary>
/// 플레이어 상태 정의
/// </summary>
public enum PlaybackState
{
    Stopped,
    Playing,
    Paused
}

/// <summary>
/// 플레이어 서비스
/// - 플레이리스트 관리 (Index 기반)
/// - 유튜브 스트림 변환
/// - 오디오 재생 제어
/// - 상태 머신 기반 관리
/// </summary>
public class PlayerService
{
    private readonly StreamingAudioPlayer _player;
    private readonly YtDlpService _yt;

    /// <summary>
    /// 플레이리스트
    /// </summary>
    private List<PlaylistItem> _playlist = [];

    /// <summary>
    /// 현재 재생 인덱스
    /// </summary>
    private int _currentIndex = -1;

    /// <summary>
    /// 재생 상태
    /// </summary>
    private PlaybackState _state = PlaybackState.Stopped;

    /// <summary>
    /// 상태 변경 이벤트 (UI 동기화용)
    /// </summary>
    public event Action<PlaybackState>? OnStateChanged;

    /// <summary>
    /// 현재 재생 곡 변경 이벤트
    /// </summary>
    public event Action<PlaylistItem>? OnTrackChanged;

    /// <summary>
    /// 루프 상태
    /// </summary>
    private LoopMode _loopMode = LoopMode.None;

    /// <summary>
    /// 생성자
    /// </summary>
    public PlayerService(YtDlpService yt)
    {
        var waveOut = new WaveOutEvent();
        _player = new StreamingAudioPlayer(waveOut);

        _yt = yt;

        // 재생 종료 시 자동 다음 곡
        _player.OnPlaybackStopped += HandleNext;
    }

    /// <summary>
    /// 상태 변경 공통 처리
    /// </summary>
    private void SetState(PlaybackState state)
    {
        if (_state == state) return;

        _state = state;
        OnStateChanged?.Invoke(_state);
    }

    /// <summary>
    /// 플레이리스트 설정
    /// </summary>
    public void SetPlaylist(List<PlaylistItem> items)
    {
        _playlist = items ?? [];
        _currentIndex = -1;
        _state = PlaybackState.Stopped;

        OnStateChanged?.Invoke(_state);
    }

    /// <summary>
    /// 이전 곡
    /// </summary>
    public async Task PrevAsync()
    {
        if (_playlist.Count == 0) return;

        _currentIndex--;

        if (_currentIndex < 0)
            _currentIndex = _playlist.Count - 1;

        await PlayCurrent();
    }

    /// <summary>
    /// 특정 인덱스 재생
    /// </summary>
    public async Task PlayAsync(int index)
    {
        if (_playlist.Count == 0) return;

        if (index < 0 || index >= _playlist.Count)
            index = 0;

        _currentIndex = index;

        await PlayCurrent();
    }

    /// <summary>
    /// 현재 인덱스 기준 재생
    /// </summary>
    private async Task PlayCurrent()
    {
        if (_currentIndex < 0 || _currentIndex >= _playlist.Count)
            return;

        var item = _playlist[_currentIndex];

        // 기존 재생 완전 정지
        _player.Stop();

        // UI 트랙 변경 알림
        OnTrackChanged?.Invoke(item);

        // 상태 변경
        SetState(PlaybackState.Playing);

        // 스트림 URL 조회
        var streamUrl = await _yt.GetStreamUrlAsync(item.Url);

        if (string.IsNullOrWhiteSpace(streamUrl))
        {
            SetState(PlaybackState.Stopped);
            return;
        }

        // 실제 재생
        await _player.PlayAsync(streamUrl);

        // 재생 보장 (중요)
        SetState(PlaybackState.Playing);
    }

    /// <summary>
    /// 다음 곡 (루프 정책 반영)
    /// </summary>
    public async Task NextAsync()
    {
        if (_playlist.Count == 0)
            return;

        switch (_loopMode)
        {
            case LoopMode.Single:
                await PlayCurrent();
                return;

            case LoopMode.All:
                _currentIndex++;

                if (_currentIndex >= _playlist.Count)
                    _currentIndex = 0;

                break;

            case LoopMode.None:
            default:
                if (_currentIndex + 1 >= _playlist.Count)
                {
                    Stop();
                    return;
                }

                _currentIndex++;
                break;
        }

        await PlayCurrent();
    }

    /// <summary>
    /// 재생 종료 이벤트 → 자동 다음 곡
    /// </summary>
    private async void HandleNext(object? sender, StoppedEventArgs e)
    {
        try
        {
            if (_state != PlaybackState.Playing)
                return;

            await NextAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// 정지
    /// </summary>
    public void Stop()
    {
        _player.Stop();
        SetState(PlaybackState.Stopped);
    }

    /// <summary>
    /// 일시정지 (향후 확장용)
    /// </summary>
    public void Pause()
    {
        _player.Stop();
        SetState(PlaybackState.Paused);
    }

    /// <summary>
    /// 현재 재생 중 곡 반환
    /// </summary>
    public PlaylistItem? GetCurrent()
    {
        if (_currentIndex < 0 || _currentIndex >= _playlist.Count)
            return null;

        return _playlist[_currentIndex];
    }

    /// <summary>
    /// 현재 인덱스 반환
    /// </summary>
    public int GetCurrentIndex()
    {
        return _currentIndex;
    }

    /// <summary>
    /// 현재 상태 반환
    /// </summary>
    public PlaybackState GetState()
    {
        return _state;
    }

    /// <summary>
    /// 볼륨 조정
    /// </summary>
    public void SetVolume(float volume)
    {
        _player.SetVolume(volume);
    }

    /// <summary>
    /// 루프 모드 설정
    /// </summary>
    public void SetLoopMode(LoopMode mode)
    {
        _loopMode = mode;
    }
}