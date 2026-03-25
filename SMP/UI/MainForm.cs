// /UI/MainForm.cs

using SMP.App.Interfaces;
using SMP.App.Service;
using SMP.App.UseCases;
using SMP.Domain;
using SMP.Infrastructure.Input;
using System.Windows.Forms;

namespace SMP.UI;

public partial class MainForm : Form
{
    private readonly IYoutubeService _yt;
    private readonly ITrayService _tray;
    private readonly IPlaylistRepository _repo;
    private readonly IUpdateService? _update;

    private readonly PlayerState _state;

    private readonly PlayUseCase _playUseCase;
    private readonly NextTrackUseCase _nextUseCase;
    private readonly StopUseCase _stopUseCase;
    private readonly SetPlaylistUseCase _setPlaylistUseCase;
    private readonly SetVolumeUseCase _setVolumeUseCase;
    private readonly SetLoopModeUseCase _setLoopModeUseCase;
    private readonly PrevTrackUseCase _prevUseCase;
    private readonly PlayPauseUseCase _playPauseUseCase;

    // ✅ MediaKey Detector 주입
    private readonly MediaKeyListener _listener;
    private readonly MediaKeyPatternDetector _media;

    /// <summary>
    /// 루프 상태
    /// </summary>
    private LoopMode _loopMode = LoopMode.None;

    public MainForm(
        PlayerState state,
        PlayUseCase playUseCase,
        NextTrackUseCase nextUseCase,
        StopUseCase stopUseCase,
        PrevTrackUseCase prevUseCase,
        SetPlaylistUseCase setPlaylistUseCase,
        SetVolumeUseCase setVolumeUseCase,
        SetLoopModeUseCase setLoopModeUseCase,
        PlayPauseUseCase playPauseUseCase,
        MediaKeyPatternDetector media,
        MediaKeyListener listener,
        IYoutubeService yt,
        ITrayService tray,
        IPlaylistRepository repo,
        IUpdateService? update = null)
    {
        InitializeComponent();
        InitForm();

        _state = state;

        _playUseCase = playUseCase;
        _nextUseCase = nextUseCase;
        _prevUseCase = prevUseCase;
        _stopUseCase = stopUseCase;

        _setPlaylistUseCase = setPlaylistUseCase;
        _setVolumeUseCase = setVolumeUseCase;
        _setLoopModeUseCase = setLoopModeUseCase;

        _playPauseUseCase = playPauseUseCase;

        _media = media;
        _listener = listener;

        _yt = yt;
        _tray = tray;
        _repo = repo;
        _update = update;

        BindMediaKeys(); // ✅ MediaKey 연결
        InitTray();
        BindEvents();
        InitVolume();

        _ = InitializeAsync();
    }

    private void InitForm()
    {
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = true;

        MinimumSize = Size;
        MaximumSize = Size;

        _loopMode = LoopMode.None;
    }

    private async Task InitializeAsync()
    {
        await SafeLoadAsync();
    }

    private void InitTray()
    {
        var menu = new ContextMenuStrip();

        menu.Items.Add("▶️ 재생", null, async (s, e) =>
        {
            int index = GetSelectedIndexOrDefault();
            await _playUseCase.ExecuteAsync(index);
        });

        menu.Items.Add("⏭️ 다음곡", null, async (s, e) =>
        {
            await _nextUseCase.ExecuteAsync();
        });

        menu.Items.Add("⏹️ 정지", null, (s, e) => _stopUseCase.Execute());
        menu.Items.Add("🪟 열기", null, (s, e) => ShowMainWindow());
        menu.Items.Add("❌ 종료", null, (s, e) => Application.Exit());

        _tray.SetMenu(menu);
        _tray.OnDoubleClick(() => ShowMainWindow());
    }

    private void BindEvents()
    {
        _playUseCase.OnTrackChanged += item =>
        {
            if (IsDisposed) return;

            BeginInvoke(new Action(() =>
            {
                this.Text = $"재생중: {item.Title}";
                _tray.SetTitle(item.Title);
                _tray.Notify(item.Title);

                int index = _state.CurrentIndex;

                if (index >= 0 && index < LstPlaylist.Items.Count)
                {
                    LstPlaylist.SelectedIndex = index;
                    LstPlaylist.TopIndex = index;
                }
            }));
        };
    }

    private async Task SafeLoadAsync()
    {
        try
        {
            var items = await _repo.LoadAsync();

            LstPlaylist.Items.Clear();

            foreach (var item in items)
                LstPlaylist.Items.Add(item);

            _setPlaylistUseCase.Execute(items);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"플레이리스트 로드 실패: {ex.Message}");
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        if (WindowState == FormWindowState.Minimized)
            Hide();
    }

    private void ShowMainWindow()
    {
        if (IsDisposed) return;

        if (InvokeRequired)
        {
            Invoke(ShowMainWindow);
            return;
        }

        if (!Visible)
            Show();

        WindowState = FormWindowState.Normal;
        Size = MaximumSize;

        BringToFront();
        Activate();
    }

    private async void BtnAdd_Click(object sender, EventArgs e)
    {
        var url = TxtUrl.Text.Trim();
        if (string.IsNullOrWhiteSpace(url)) return;

        try
        {
            if (LstPlaylist.Items.Cast<PlaylistItem>().Any(x => x.Url == url))
            {
                MessageBox.Show("이미 추가된 URL입니다.");
                return;
            }

            var title = await _yt.GetTitleAsync(url);

            var item = new PlaylistItem
            {
                Title = title ?? url,
                Url = url
            };

            LstPlaylist.Items.Add(item);

            var items = LstPlaylist.Items.Cast<PlaylistItem>().ToList();
            _setPlaylistUseCase.Execute(items);

            TxtUrl.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"추가 실패: {ex.Message}");
        }
    }

    private async void BtnPlay_Click(object sender, EventArgs e)
    {
        int index = GetSelectedIndexOrDefault();
        await _playUseCase.ExecuteAsync(index);
    }

    private async void BtnNext_Click(object sender, EventArgs e)
    {
        await _nextUseCase.ExecuteAsync();
    }

    private void BtnStop_Click(object sender, EventArgs e)
    {
        _stopUseCase.Execute();
        this.Text = "SMP Player";
    }

    public void Pause()
    {
        _stopUseCase.Execute();
        this.Text = "SMP Player";
    }

    private void BtnDel_Click(object sender, EventArgs e)
    {
        if (LstPlaylist.SelectedItem is PlaylistItem item)
        {
            LstPlaylist.Items.Remove(item);

            var items = LstPlaylist.Items.Cast<PlaylistItem>().ToList();
            _setPlaylistUseCase.Execute(items);
        }
    }

    private int GetSelectedIndexOrDefault()
    {
        if (LstPlaylist.SelectedIndex >= 0)
            return LstPlaylist.SelectedIndex;

        return 0;
    }

    private void InitVolume()
    {
        TrackVolume.Value = 50;
        _setVolumeUseCase.Execute(0.5f);
        LblVolume.Text = "볼륨: 50%";
    }

    private void TrackVolume_Scroll(object sender, EventArgs e)
    {
        float volume = TrackVolume.Value / 100f;
        _setVolumeUseCase.Execute(volume);
        LblVolume.Text = $"볼륨: {TrackVolume.Value}%";
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        try
        {
            var items = LstPlaylist.Items.Cast<PlaylistItem>().ToList();
            _ = _repo.SaveAsync(items);
            _tray.Dispose();
        }
        catch { }

        base.OnFormClosing(e);
    }

    private void BtnLoop_Click(object sender, EventArgs e)
    {
        _loopMode = _loopMode switch
        {
            LoopMode.None => LoopMode.All,
            LoopMode.All => LoopMode.Single,
            LoopMode.Single => LoopMode.None,
            _ => LoopMode.None
        };

        _setLoopModeUseCase.Execute(_loopMode);
        UpdateLoopButtonUI();
    }

    private void UpdateLoopButtonUI()
    {
        BtnLoop.Text = _loopMode switch
        {
            LoopMode.None => "🔁 1회",
            LoopMode.All => "🔂 전체",
            LoopMode.Single => "🔂 1곡",
            _ => "🔁"
        };
    }

    private void LstPlaylist_MouseClick(object sender, MouseEventArgs e)
    {
        int index = LstPlaylist.IndexFromPoint(e.Location);
        if (index == ListBox.NoMatches) return;

        LstPlaylist.SelectedIndex = index;
        _stopUseCase.Execute();
        this.Text = "SMP Player";
    }

    private async void LstPlaylist_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        int index = LstPlaylist.IndexFromPoint(e.Location);
        if (index == ListBox.NoMatches) return;

        LstPlaylist.SelectedIndex = index;
        await _playUseCase.ExecuteAsync(index);
    }

    /// <summary>
    /// MediaKey 바인딩
    /// </summary>
    private void BindMediaKeys()
    {
        _media.OnNext -= HandleNext;
        _media.OnPrevious -= HandlePrevious;
        _media.OnPlayPause -= HandlePlayPause;
        _media.OnVolumeUp -= HandleVolumeUp;
        _media.OnVolumeDown -= HandleVolumeDown;

        _media.OnNext += HandleNext;
        _media.OnPrevious += HandlePrevious;
        _media.OnPlayPause += HandlePlayPause;
        _media.OnVolumeUp -= HandleVolumeUp;
        _media.OnVolumeDown -= HandleVolumeDown;
    }

    /// <summary>
    /// Next Track
    /// </summary>
    private async void HandleNext()
    {
        await _nextUseCase.ExecuteAsync();
    }

    /// <summary>
    /// Previous Track
    /// </summary>
    private async void HandlePrevious()
    {
        await _prevUseCase.ExecuteAsync();
    }

    /// <summary>
    /// Play / Pause Toggle
    /// </summary>
    private async void HandlePlayPause()
    {
        await _playPauseUseCase.ExecuteAsync();
    }

    /// <summary>
    /// Volume Up
    /// </summary>
    private void HandleVolumeUp()
    {
        int value = TrackVolume.Value;
        value = Math.Min(100, value + 5);

        TrackVolume.Value = value;
        _setVolumeUseCase.Execute(value / 100f);

        LblVolume.Text = $"볼륨: {value}%";
    }

    /// <summary>
    /// Volume Down
    /// </summary>
    private void HandleVolumeDown()
    {
        int value = TrackVolume.Value;
        value = Math.Max(0, value - 5);

        TrackVolume.Value = value;
        _setVolumeUseCase.Execute(value / 100f);

        LblVolume.Text = $"볼륨: {value}%";
    }
}