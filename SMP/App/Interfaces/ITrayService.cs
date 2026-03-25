// /App/Interfaces/ITrayService.cs

using System.Windows.Forms;

namespace SMP.App.Interfaces;

/// <summary>
/// 트레이 서비스 Port
/// </summary>
public interface ITrayService : IDisposable
{
    void SetMenu(ContextMenuStrip menu);
    void SetTitle(string title);
    void Notify(string message);
    void OnDoubleClick(Action action);
}