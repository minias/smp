// UI/MainForm.cs
using SMP.App;
using SMP.App.Interfaces;
using SMP.App.Service;
using SMP.Domain;
using SMP.Infrastructure.Storage;
using SMP.Infrastructure.Tray;
using SMP.Infrastructure.Youtube;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SMP.UI;

public partial class MainForm : Form
{
    private readonly PlayerService _player;
    private readonly YtDlpService _yt;
    private readonly TrayService _tray;
    private readonly PlaylistRepository _repo;

    // ✅ optional (DI 실패 대비)
    private readonly IUpdateService? _update;
    /// <summary>
    /// 루프 상태
    /// </summary>
    private LoopMode _loopMode = LoopMode.None;

    public MainForm(
        PlayerService player,
        YtDlpService yt,
        TrayService tray,
        PlaylistRepository repo,
        IUpdateService? update = null)
    {
        InitializeComponent();
        InitForm();

        _player = player;
        _yt = yt;
        _tray = tray;
        _repo = repo;
        _update = update;

        InitTray();
        BindEvents();

        InitVolume();

        // ✅ 초기화 통합
        _ = InitializeAsync();
    }

    /// <summary>
    /// 초기화 (메인폼)
    /// </summary>
    private void InitForm()
    {
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = true;

        MinimumSize = Size;
        MaximumSize = Size;

        // ✅ 기본값: 1회 재생
        _loopMode = LoopMode.None;
    }

    /// <summary>
    /// 초기화 (플레이리스트 + 업데이트)
    /// </summary>
    private async Task InitializeAsync()
    {
        await SafeLoadAsync();
        await CheckUpdateAsync();
    }

    /// <summary>
    /// 트레이 초기화
    /// </summary>
    private void InitTray()
    {
        var menu = new ContextMenuStrip();

        // ✅ 현재 선택된 인덱스 기준 재생
        menu.Items.Add("▶️ 재생", null, async (s, e) =>
        {
            int index = GetSelectedIndexOrDefault();
            await _player.PlayAsync(index);
        });

        // 다음곡
        menu.Items.Add("⏭️ 다음곡", null, async (s, e) =>
        {
            await _player.NextAsync();
        });

        menu.Items.Add("⏹️ 정지", null, (s, e) => _player.Stop());
        menu.Items.Add("🪟 열기", null, (s, e) => ShowMainWindow());
        menu.Items.Add("❌ 종료", null, (s, e) => Application.Exit());

        _tray.SetMenu(menu);

        // ✅ 더블 클릭 버그 수정
        _tray.OnDoubleClick(() => ShowMainWindow());
    }
 
    /// <summary>
    /// Player 이벤트 바인딩
    /// </summary>
    private void BindEvents()
    {
        _player.OnTrackChanged += item =>
        {
            if (IsDisposed) return;

            BeginInvoke(new Action(() =>
            {
                this.Text = $"재생중: {item.Title}";
                _tray.SetTitle(item.Title);
                _tray.Notify(item.Title);

                // ✅ 현재 재생 인덱스 가져오기
                int index = _player.GetCurrentIndex();

                // ✅ ListBox 선택 동기화
                if (index >= 0 && index < LstPlaylist.Items.Count)
                {
                    LstPlaylist.SelectedIndex = index;
                    LstPlaylist.TopIndex = index;
                }
            }));
        };
        // TODO 향후버전에서 수정
        //_player.OnStateChanged += state =>
        //{
        //    if (IsDisposed) return;

        //    BeginInvoke(new Action(() =>
        //    {
        //        BtnPlay.Enabled = state != PlaybackState.Playing;
        //        BtnStop.Enabled = state == PlaybackState.Playing;
        //    }));
        //};
    }

    /// <summary>
    /// 안전한 플레이리스트 로드
    /// </summary>
    private async Task SafeLoadAsync()
    {
        try
        {
            var items = await _repo.LoadAsync();

            LstPlaylist.Items.Clear();

            foreach (var item in items)
            {
                LstPlaylist.Items.Add(item);
            }

            // ✅ PlayerService에 전체 리스트 동기화
            _player.SetPlaylist(items);
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
            if (_update == null)
                return;

            var update = await _update.CheckForUpdateAsync();

            if (update == null)
                return;

            if (IsDisposed)
                return;

            BeginInvoke(new Action(() =>
            {
                var result = MessageBox.Show(
                    $"새 버전 {update.Version} 발견\n업데이트하시겠습니까?",
                    "SMP 업데이트",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _update.DownloadAndInstallAsync(update);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[Update Install Error] {ex}");
                        }
                    });
                }
            }));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Update Error] {ex}");
        }
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
    /// 트레이에서 창 열기
    /// </summary>
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

        // 🔥 항상 복원
        WindowState = FormWindowState.Normal;

        // 🔥 사이즈 복원
        Size = MaximumSize;

        BringToFront();
        Activate();
    }

    /// <summary>
    /// URL 추가
    /// </summary>
    private async void BtnAdd_Click(object sender, EventArgs e)
    {
        var url = TxtUrl.Text.Trim();
        if (string.IsNullOrWhiteSpace(url)) return;

        try
        {
            // 중복 방지
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

            // ✅ 리스트 전체를 PlayerService와 동기화
            var items = LstPlaylist.Items.Cast<PlaylistItem>().ToList();
            _player.SetPlaylist(items);

            TxtUrl.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"추가 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 재생
    /// </summary>
    private async void BtnPlay_Click(object sender, EventArgs e)
    {
        try
        {
            ///@TODO 향후 다시 
            ///BtnPlay.Enabled = false;
            int index = GetSelectedIndexOrDefault();
            await _player.PlayAsync(index);
            //BtnPlay.Enabled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"재생 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 다음곡
    /// </summary>
    private async void BtnNext_Click(object sender, EventArgs e)
    {
        try
        {
            await _player.NextAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"다음곡 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 정지
    /// </summary>
    private void BtnStop_Click(object sender, EventArgs e)
    {
        _player.Stop();
        this.Text = "SMP Player";
    }

    /// <summary>
    /// 일시 정지
    /// </summary>
    public void Pause()
    {
        _player.Stop(); // 현재 구조상 Stop 기반
        this.Text = "SMP Player";
    }

    /// <summary>
    /// 삭제
    /// </summary>
    private void BtnDel_Click(object sender, EventArgs e)
    {
        if (LstPlaylist.SelectedItem is PlaylistItem item)
        {
            LstPlaylist.Items.Remove(item);

            // ✅ 삭제 후 PlayerService 동기화
            var items = LstPlaylist.Items.Cast<PlaylistItem>().ToList();
            _player.SetPlaylist(items);
        }
    }

    /// <summary>
    /// 선택 인덱스 반환 (없으면 0)
    /// </summary>
    private int GetSelectedIndexOrDefault()
    {
        if (LstPlaylist.SelectedIndex >= 0)
            return LstPlaylist.SelectedIndex;

        return 0;
    }

    /// <summary>
    /// 볼륨 초기화
    /// </summary>
    private void InitVolume()
    {
        TrackVolume.Value = 50;
        _player.SetVolume(0.5f);
        LblVolume.Text = "볼륨: 50%";
    }

    /// <summary>
    /// 볼륨 변경
    /// </summary>
    private void TrackVolume_Scroll(object sender, EventArgs e)
    {
        float volume = TrackVolume.Value / 100f;

        _player.SetVolume(volume);
        LblVolume.Text = $"볼륨: {TrackVolume.Value}%";
    }

    /// <summary>
    /// 종료 시 저장
    /// </summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        try
        {
            var items = LstPlaylist.Items
                .Cast<PlaylistItem>()
                .ToList();

            _ = _repo.SaveAsync(items);
            _tray.Dispose();
        }
        catch
        {
            // 종료 중 예외 무시
        }

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

        _player.SetLoopMode(_loopMode);
        UpdateLoopButtonUI();
    }

    /// <summary>
    /// 루프 버튼 UI 갱신
    /// </summary>
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

        if (index == ListBox.NoMatches)
            return;
        // 🔹 선택만 변경
        LstPlaylist.SelectedIndex = index;

        // 🔹 기존 재생 중이면 정지
        _player.Stop();
        this.Text = "SMP Player";
    }

    private async void LstPlaylist_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        try
        {

            int index = LstPlaylist.IndexFromPoint(e.Location);

            if (index == ListBox.NoMatches)
                return;

            // 🔥 더블클릭 → 선택 + 재생
            LstPlaylist.SelectedIndex = index;

            // 더블클릭 → 재생        
            await _player.PlayAsync(index);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"더블클릭 재생 실패: {ex.Message}");
        }
     
    }
}