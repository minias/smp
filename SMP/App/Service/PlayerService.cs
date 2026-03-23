// App/PlayerService.cs

using SMP.Domain;
using SMP.Infrastructure.Audio;
using SMP.Infrastructure.Youtube;
using NAudio.Wave;

namespace SMP.App.Service;

/// <summary>
/// 플레이어 서비스
/// - 플레이리스트 관리
/// - 유튜브 스트림 변환
/// - 오디오 재생 제어
/// </summary>
public class PlayerService
{
    private readonly StreamingAudioPlayer _player;
    private readonly YtDlpService _yt;

    public event Action<PlaylistItem>? OnTrackChanged;

    // 플레이리스트 (FIFO)
    private readonly Queue<PlaylistItem> _playlist = new();

    // 현재 재생중 아이템
    private PlaylistItem? _current;

    private bool _isPlaying = false;

    /// <summary>
    /// 생성자
    /// </summary>
    public PlayerService(YtDlpService yt)
    {
        // ✅ 수정: IWavePlayer 전달
        var waveOut = new WaveOutEvent();
        _player = new StreamingAudioPlayer(waveOut);

        _yt = yt;

        // 재생 종료 시 다음 곡 자동 실행
        _player.OnPlaybackStopped += HandleNext;
    }

    /// <summary>
    /// 플레이리스트 추가
    /// </summary>
    public void Add(PlaylistItem item)
    {
        _playlist.Enqueue(item);
    }

    /// <summary>
    /// 재생 시작
    /// </summary>
    public async Task PlayAsync()
    {
        if (_isPlaying) return;

        await PlayNext();
    }

    /// <summary>
    /// 다음 곡 재생 (핵심 로직)
    /// </summary>
    private async Task PlayNext()
    {
        if (_playlist.Count == 0)
        {
            _isPlaying = false;
            _current = null;
            return;
        }

        _isPlaying = true;
        _current = _playlist.Dequeue();

        OnTrackChanged?.Invoke(_current);

        for (int i = 0; i < 2; i++)
        {
            var streamUrl = await _yt.GetStreamUrlAsync(_current.Url);

            if (!string.IsNullOrWhiteSpace(streamUrl))
            {
                // ✅ 수정: Play → PlayAsync
                await _player.PlayAsync(streamUrl);
                return;
            }
        }

        await PlayNext();
    }

    /// <summary>
    /// 재생 종료 이벤트 → 자동 다음 곡
    /// </summary>
    private async void HandleNext(object? sender, StoppedEventArgs e)
    {
        await PlayNext();
    }

    /// <summary>
    /// 전체 정지
    /// </summary>
    public void Stop()
    {
        _playlist.Clear();
        _player.Stop();

        _isPlaying = false;
        _current = null;
    }

    /// <summary>
    /// 다음 곡 스킵
    /// </summary>
    public void Skip()
    {
        _player.Stop(); // PlaybackStopped 이벤트 → PlayNext()
    }

    /// <summary>
    /// 볼륨 조정
    /// </summary>
    public void SetVolume(float volume)
    {
        _player.SetVolume(volume);
    }

    /// <summary>
    /// 현재 재생중 정보
    /// </summary>
    public PlaylistItem? GetCurrent()
    {
        return _current;
    }
}