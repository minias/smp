// Infrastructure/Tray/TrayService.cs

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SMP.App.Interfaces;

namespace SMP.Infrastructure.Tray;

/// <summary>
/// 시스템 트레이 아이콘 관리 서비스
/// </summary>
public class TrayService : ITrayService, IDisposable
{
    private readonly NotifyIcon _tray;
    private bool _disposed;

    public TrayService()
    {
        _tray = new NotifyIcon();

        InitializeIcon();
        InitializeMenu();
    }

    private void InitializeIcon()
    {
        try
        {
            var iconPath = Path.Combine(AppContext.BaseDirectory, "smp.ico");

            _tray.Icon = File.Exists(iconPath)
                ? new Icon(iconPath)
                : SystemIcons.Application;

            _tray.Text = "SMP";
            _tray.Visible = true;
        }
        catch
        {
            _tray.Icon = SystemIcons.Application;
            _tray.Visible = true;
        }
    }

    private void InitializeMenu()
    {
        var contextMenu = new ContextMenuStrip();

        var exitItem = new ToolStripMenuItem("Exit", null, (_, __) =>
        {
            Dispose();
            Application.Exit();
        });

        contextMenu.Items.Add(exitItem);

        _tray.ContextMenuStrip = contextMenu;
    }

    public void SetMenu(ContextMenuStrip menu)
    {
        ThrowIfDisposed();
        _tray.ContextMenuStrip = menu;
    }

    public void Show()
    {
        ThrowIfDisposed();
        _tray.Visible = true;
    }

    public void Hide()
    {
        ThrowIfDisposed();
        _tray.Visible = false;
    }

    public void SetTitle(string title)
    {
        ThrowIfDisposed();

        try
        {
            _tray.Text = string.IsNullOrWhiteSpace(title)
                ? "SMP"
                : (title.Length > 60 ? title[..60] : title);
        }
        catch
        {
            _tray.Text = "SMP";
        }
    }

    public void Notify(string message)
    {
        ThrowIfDisposed();

        try
        {
            _tray.ShowBalloonTip(2000, "SMP", message, ToolTipIcon.Info);
        }
        catch
        {
            // ignore
        }
    }

    public void OnDoubleClick(Action action)
    {
        ThrowIfDisposed();

        _tray.DoubleClick += (s, e) =>
        {
            try { action(); } catch { }
        };
    }

    // =========================
    // IDisposable 패턴 (핵심)
    // =========================

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // 🔥 핵심
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // managed resources
            try
            {
                _tray.Visible = false;
                _tray.Dispose();
            }
            catch
            {
                // ignore
            }
        }

        // unmanaged resources (없지만 패턴 유지)

        _disposed = true;
    }

    ~TrayService()
    {
        Dispose(false);
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}