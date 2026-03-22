using SMP.Applications;
using SMP.Domain;
using SMP.Infrastructure.Storage;
using SMP.Infrastructure.Tray;
using SMP.Infrastructure.Youtube;
using System.Windows.Forms;

namespace SMP.UI;

public partial class MainForm : Form
{
    private readonly PlayerService _player;
    private readonly YtDlpService _yt;
    private readonly TrayService _tray;
    private readonly PlaylistRepository _repo;
    private readonly IUpdateService _update; // ✅ 추가

    public MainForm(
        PlayerService player,
        YtDlpService yt,
        TrayService tray,
        PlaylistRepository repo,
        IUpdateService update) // ✅ DI 추가
    {
        InitializeComponent();

        _player = player;
        _yt = yt;
        _tray = tray;
        _repo = repo;

        InitTray();
        BindEvents();

        InitVolume();         // ✅ 볼륨 초기화        
        _ = InitializeAsync(); // ✅ 통합 초기화
    }

    /// <summary>
    /// Player 이벤트 바인딩
    /// </summary>
    private void BindEvents()
    {
        _player.OnTrackChanged += item =>
        {
            if (IsDisposed) return;

            BeginInvoke(() =>
            {
                this.Text = $"재생중: {item.Title}";
                _tray.SetTitle(item.Title);
                _tray.Notify(item.Title);
            });
        };
    }

    /// <summary>
    /// 초기화 (플레이리스트 + 업데이트 체크)
    /// </summary>
    private async Task InitializeAsync()
    {
        await SafeLoadAsync();
        await CheckUpdateAsync(); // ✅ 추가
    }


    /// <summary>
    /// 안전한 플레이리스트 로드
    /// </summary>
    private async Task SafeLoadAsync()
    {
        try
        {
            var items = await _repo.LoadAsync();

            foreach (var item in items)
            {
                lstPlaylist.Items.Add(item);
                _player.Add(item);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"플레이리스트 로드 실패: {ex.Message}");
        }
    }
    /// <summary>
    /// 업데이트 체크
    /// </summary>
    private async Task CheckUpdateAsync()
    {
        try
        {
            var update = await _update.CheckForUpdateAsync();

            if (update == null) return;

            if (IsDisposed) return;

            BeginInvoke(async () =>
            {
                var result = MessageBox.Show(
                    $"새 버전 {update.Version} 발견\n업데이트하시겠습니까?",
                    "SMP 업데이트",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    await _update.DownloadAndInstallAsync(update);
                }
            });
        }
        catch (Exception ex)
        {
            // 업데이트 실패는 무시 (UX 방해 금지)
            Console.WriteLine($"[Update] 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 트레이 초기화
    /// </summary>
    private void InitTray()
    {
        var menu = new ContextMenuStrip();

        menu.Items.Add("▶️ 재생", null, async (s, e) => await _player.PlayAsync());
        menu.Items.Add("⏭️ 다음곡", null, (s, e) => _player.Skip());
        menu.Items.Add("⏹️ 정지", null, (s, e) => _player.Stop());
        menu.Items.Add("🪟 열기", null, (s, e) => ShowMainWindow());        
        menu.Items.Add("❌ 종료", null, (s, e) => Application.Exit());

        _tray.SetMenu(menu);
        _tray.OnDoubleClick(() => Show());
    }

    /// <summary>
    /// 최소화 시 트레이
    /// </summary>
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        if (WindowState == FormWindowState.Minimized)
            Hide();
    }

    /// <summary>
    /// 트레이 열기 시 창 열기
    /// </summary>
    private void ShowMainWindow()
    {
        if (InvokeRequired)
        {
            Invoke(ShowMainWindow);
            return;
        }

        Show();
        WindowState = FormWindowState.Normal;
        StartPosition = FormStartPosition.CenterScreen;
        BringToFront();
        Activate();
    }

    /// <summary>
    /// URL 추가
    /// </summary>
    private async void btnAdd_Click(object sender, EventArgs e)
    {
        var url = txtUrl.Text.Trim();
        if (string.IsNullOrWhiteSpace(url)) return;

        try
        {
            // 중복 방지
            if (lstPlaylist.Items.Cast<PlaylistItem>().Any(x => x.Url == url))
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

            lstPlaylist.Items.Add(item);
            _player.Add(item);

            txtUrl.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"추가 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 재생
    /// </summary>
    private async void btnPlay_Click(object sender, EventArgs e)
    {
        await _player.PlayAsync();
    }

    /// <summary>
    /// 다음곡
    /// </summary>
    private void btnNext_Click(object sender, EventArgs e)
    {
        _player.Skip();
    }

    /// <summary>
    /// 정지
    /// </summary>
    private void btnStop_Click(object sender, EventArgs e)
    {
        _player.Stop();
        this.Text = "SMP Player";
    }

    /// <summary>
    /// 삭제
    /// </summary>
    private void btnDel_Click(object sender, EventArgs e)
    {
        if (lstPlaylist.SelectedItem is PlaylistItem item)
        {
            lstPlaylist.Items.Remove(item);
        }
    }

    /// <summary>
    /// 볼륨 초기화
    /// </summary>
    private void InitVolume()
    {
        trackVolume.Value = 50;
        _player.SetVolume(0.5f);
        lblVolume.Text = "볼륨: 50%";
    }

    /// <summary>
    /// 볼륨 변경
    /// </summary>
    private void trackVolume_Scroll(object sender, EventArgs e)
    {
        float volume = trackVolume.Value / 100f;

        _player.SetVolume(volume);
        lblVolume.Text = $"볼륨: {trackVolume.Value}%";
    }

    /// <summary>
    /// 종료 시 저장
    /// </summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        try
        {
            var items = lstPlaylist.Items
                .Cast<PlaylistItem>()
                .ToList();

            // fire-and-forget (UI 종료 방해 X)
            _ = _repo.SaveAsync(items);
        }
        catch
        {
            // 무시 (종료 중)
        }

        base.OnFormClosing(e);
    }
}