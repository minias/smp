// /Infrastructure/Input/MediaKeyListener.cs
/*
Keyboard Hook
 → MediaKeyListener
   → OnKeyPressed(vkCode)
     → MediaKeyPatternDetector.ProcessKey(vkCode)
       → OnNext / OnPrevious / OnPlayPause
         → MainForm.BindMediaKeys()
           → UseCase
             → PlayerFacade
               → Domain / Service
 */
using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace SMP.Infrastructure.Input;

/// <summary>
/// Windows 글로벌 미디어 키 리스너 (Raw Input 수집 + Detector 위임)
/// </summary>
public partial class MediaKeyListener : IDisposable
{
    private IntPtr _hookId = IntPtr.Zero;
    private readonly LowLevelKeyboardProc _proc;
    private bool _disposed;

    private readonly MediaKeyPatternDetector _detector;

    public MediaKeyListener(MediaKeyPatternDetector detector)
    {
        _detector = detector;
        _proc = HookCallback;
        _hookId = SetHook(_proc);
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule!;

        return SetWindowsHookEx(
            13,
            proc,
            GetModuleHandle(curModule.ModuleName),
            0
        );
    }

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            // 미쳤냐? 미쳤어? 이코드부터 왜 달라지는건데? 기억력이 나빠? 메모리 더 박아줘?
            var data = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

            bool isKeyUp = (data.flags & 0x80) != 0;

            if (!isKeyUp)
            {
                Debug.WriteLine($"[Hook] vkCode={data.vkCode}");

                // ✅ Detector로 위임 (핵심)
                _detector.HandleKey(data.vkCode);
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    #region WinAPI

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(
        int idHook,
        LowLevelKeyboardProc lpfn,
        IntPtr hMod,
        uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(
        IntPtr hhk,
        int nCode,
        IntPtr wParam,
        IntPtr lParam);

    #endregion

    public void Dispose()
    {
        if (_disposed) return;

        if (_hookId != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}