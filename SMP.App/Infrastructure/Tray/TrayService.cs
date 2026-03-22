using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SMP.Infrastructure.Tray;

/// <summary>
/// 시스템 트레이 아이콘 관리 서비스
/// </summary>
public class TrayService
{
    // ✅ 단일 NotifyIcon 인스턴스
    private readonly NotifyIcon _tray;

    public TrayService()
    {
        // NotifyIcon 초기화
        _tray = new NotifyIcon();

        InitializeIcon();
        InitializeMenu();
    }

    /// <summary>
    /// 트레이 아이콘 초기화
    /// </summary>
    private void InitializeIcon()
    {
        // 실행 파일 기준 경로
        var iconPath = Path.Combine(AppContext.BaseDirectory, "smp.ico");

        if (File.Exists(iconPath))
        {
            _tray.Icon = new Icon(iconPath);
        }
        else
        {
            // fallback 아이콘
            _tray.Icon = SystemIcons.Application;
        }

        _tray.Visible = true;
        _tray.Text = "SMP";
    }

    /// <summary>
    /// 트레이 우클릭 메뉴 구성
    /// </summary>
    private void InitializeMenu()
    {
        var contextMenu = new ContextMenuStrip();

        var exitItem = new ToolStripMenuItem("Exit", null, (_, __) =>
        {
            Application.Exit();
        });

        contextMenu.Items.Add(exitItem);

        _tray.ContextMenuStrip = contextMenu;
    }

    /// <summary>
    /// 외부에서 메뉴 설정
    /// </summary>
    public void SetMenu(ContextMenuStrip menu)
    {
        _tray.ContextMenuStrip = menu;
    }

    /// <summary>
    /// 트레이 표시
    /// </summary>
    public void Show()
    {
        _tray.Visible = true;
    }

    /// <summary>
    /// 트레이 툴팁 텍스트 설정
    /// </summary>
    public void SetTitle(string title)
    {
        _tray.Text = title.Length > 60 ? title[..60] : title;
    }

    /// <summary>
    /// 알림 메시지 표시
    /// </summary>
    public void Notify(string title)
    {
        _tray.ShowBalloonTip(2000, "SMP", title, ToolTipIcon.Info);
    }

    /// <summary>
    /// 더블클릭 이벤트 등록
    /// </summary>
    public void OnDoubleClick(Action action)
    {
        _tray.DoubleClick += (s, e) => action();
    }
}