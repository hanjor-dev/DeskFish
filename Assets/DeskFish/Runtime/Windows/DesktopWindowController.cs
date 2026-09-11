using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace DeskFish.Runtime.Windows
{
    public enum DesktopInteractionMode
    {
        Interactive,
        PassThrough
    }

    /// <summary>Optional seam for a future native tray implementation.</summary>
    public interface IDesktopTrayAdapter : IDisposable
    {
        void SetInteractionMode(DesktopInteractionMode mode);
    }

    public sealed class DesktopWindowController : MonoBehaviour
    {
        private const string PositionSavedKey = "DeskFish.Windows.WindowPosition.Saved";
        private const string PositionXKey = "DeskFish.Windows.WindowPosition.X";
        private const string PositionYKey = "DeskFish.Windows.WindowPosition.Y";
        private const float PositionSaveIntervalSeconds = 2f;

        [SerializeField] private KeyCode toggleKey = KeyCode.F12;
        [SerializeField] private bool startPassThrough = true;
        [SerializeField] private bool restoreWindowPosition = true;

        private IDesktopTrayAdapter trayAdapter;
        private DesktopInteractionMode interactionMode;
        private float nextPositionSaveTime;

        public DesktopInteractionMode InteractionMode => interactionMode;
        public bool IsPassThrough => interactionMode == DesktopInteractionMode.PassThrough;
        public event Action<DesktopInteractionMode> InteractionModeChanged;

        private void Awake()
        {
            interactionMode = startPassThrough ? DesktopInteractionMode.PassThrough : DesktopInteractionMode.Interactive;
        }

        private void Start()
        {
            RestorePositionIfAvailable();
            ApplyWindowMode(interactionMode);
            nextPositionSaveTime = Time.unscaledTime + PositionSaveIntervalSeconds;
        }

        private void Update()
        {
            if (IsToggleHotkeyPressed()) ToggleInteractionMode();
            if (Time.unscaledTime >= nextPositionSaveTime)
            {
                SavePositionIfAvailable();
                nextPositionSaveTime = Time.unscaledTime + PositionSaveIntervalSeconds;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus) SavePositionIfAvailable();
        }

        private void OnApplicationQuit() => DisposeTrayAdapter();
        private void OnDestroy() => DisposeTrayAdapter();

        public void SetTrayAdapter(IDesktopTrayAdapter adapter)
        {
            DisposeTrayAdapter();
            trayAdapter = adapter;
            trayAdapter?.SetInteractionMode(interactionMode);
        }

        public void ToggleInteractionMode()
        {
            SetInteractionMode(IsPassThrough ? DesktopInteractionMode.Interactive : DesktopInteractionMode.PassThrough);
        }

        public void SetInteractionMode(DesktopInteractionMode mode)
        {
            if (interactionMode == mode) return;
            interactionMode = mode;
            ApplyWindowMode(mode);
            trayAdapter?.SetInteractionMode(mode);
            InteractionModeChanged?.Invoke(mode);
        }

        private void DisposeTrayAdapter()
        {
            trayAdapter?.Dispose();
            trayAdapter = null;
        }

        private bool IsToggleHotkeyPressed()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            return Win32Window.IsToggleHotkeyPressed();
#else
            return Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(toggleKey);
#endif
        }

        private static void ApplyWindowMode(DesktopInteractionMode mode)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (!Win32Window.SetDesktopWindow(mode == DesktopInteractionMode.PassThrough))
                Debug.LogWarning("DeskFish could not apply the Windows desktop window mode.");
#endif
        }

        private void RestorePositionIfAvailable()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (!restoreWindowPosition || !PlayerPrefs.HasKey(PositionSavedKey)) return;
            var position = new WindowPosition(PlayerPrefs.GetInt(PositionXKey), PlayerPrefs.GetInt(PositionYKey));
            if (!Win32Window.TryRestorePosition(position)) Debug.LogWarning("DeskFish could not restore the saved window position.");
#endif
        }

        private void SavePositionIfAvailable()
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (!restoreWindowPosition || !Win32Window.TryGetPosition(out var position)) return;
            PlayerPrefs.SetInt(PositionSavedKey, 1);
            PlayerPrefs.SetInt(PositionXKey, position.X);
            PlayerPrefs.SetInt(PositionYKey, position.Y);
            PlayerPrefs.Save();
#endif
        }
    }

    public readonly struct WindowPosition : IEquatable<WindowPosition>
    {
        public WindowPosition(int x, int y) { X = x; Y = y; }
        public int X { get; }
        public int Y { get; }
        public bool Equals(WindowPosition other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is WindowPosition other && Equals(other);
        public override int GetHashCode() => (X * 397) ^ Y;
    }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    internal static class Win32Window
    {
        private const int GwlExStyle = -20;
        private const int GwlStyle = -16;
        private const int WsCaption = 0x00C00000;
        private const int WsThickFrame = 0x00040000;
        private const int WsMinimizeBox = 0x00020000;
        private const int WsMaximizeBox = 0x00010000;
        private const int WsSysMenu = 0x00080000;
        private const int WsExLayered = 0x00080000;
        private const int WsExTransparent = 0x00000020;
        private const int WsExToolWindow = 0x00000080;
        private const uint LwaColorkey = 0x00000001;
        private const uint SwpNosize = 0x0001;
        private const uint SwpNomove = 0x0002;
        private const uint SwpShowwindow = 0x0040;
        private const uint SwpFramechanged = 0x0020;
        private const int VkControl = 0x11;
        private const int VkShift = 0x10;
        private const int VkF12 = 0x7B;
        private static readonly IntPtr HwndTopmost = new IntPtr(-1);
        private static bool previousHotkeyState;

        [DllImport("user32.dll")] private static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int virtualKey);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)] private static extern IntPtr GetWindowLongPtr(IntPtr handle, int index);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)] private static extern IntPtr SetWindowLongPtr(IntPtr handle, int index, IntPtr value);
        [DllImport("user32.dll")] private static extern bool SetWindowPos(IntPtr handle, IntPtr insertAfter, int x, int y, int width, int height, uint flags);
        [DllImport("user32.dll")] private static extern bool SetLayeredWindowAttributes(IntPtr handle, uint colorKey, byte alpha, uint flags);
        [DllImport("user32.dll")] private static extern bool GetWindowRect(IntPtr handle, out RectNative rectangle);

        public static bool IsToggleHotkeyPressed()
        {
            var pressed = IsKeyDown(VkControl) && IsKeyDown(VkShift) && IsKeyDown(VkF12);
            var triggered = pressed && !previousHotkeyState;
            previousHotkeyState = pressed;
            return triggered;
        }

        public static bool SetDesktopWindow(bool passThrough)
        {
            var handle = GetActiveWindow();
            if (handle == IntPtr.Zero) return false;

            var style = GetWindowLongPtr(handle, GwlStyle).ToInt64();
            style &= ~(WsCaption | WsThickFrame | WsMinimizeBox | WsMaximizeBox | WsSysMenu);
            SetWindowLongPtr(handle, GwlStyle, new IntPtr(style));

            var extendedStyle = GetWindowLongPtr(handle, GwlExStyle).ToInt64();
            extendedStyle |= WsExLayered | WsExToolWindow;
            if (passThrough) extendedStyle |= WsExTransparent;
            else extendedStyle &= ~WsExTransparent;
            SetWindowLongPtr(handle, GwlExStyle, new IntPtr(extendedStyle));

            // The prototype uses magenta as its clear color; color-key transparency
            // keeps this adapter compatible with Unity's built-in player window.
            var transparent = SetLayeredWindowAttributes(handle, 0x00FF00FF, 0, LwaColorkey);
            var positioned = SetWindowPos(handle, HwndTopmost, 0, 0, 0, 0, SwpNomove | SwpNosize | SwpShowwindow | SwpFramechanged);
            return transparent && positioned;
        }

        public static bool TryGetPosition(out WindowPosition position)
        {
            position = default;
            var handle = GetActiveWindow();
            if (handle == IntPtr.Zero || !GetWindowRect(handle, out var rectangle)) return false;
            position = new WindowPosition(rectangle.Left, rectangle.Top);
            return true;
        }

        public static bool TryRestorePosition(WindowPosition position)
        {
            var handle = GetActiveWindow();
            return handle != IntPtr.Zero && SetWindowPos(handle, HwndTopmost, position.X, position.Y, 0, 0, SwpNosize | SwpShowwindow);
        }

        private static bool IsKeyDown(int virtualKey) => (GetAsyncKeyState(virtualKey) & 0x8000) != 0;

        [StructLayout(LayoutKind.Sequential)]
        private struct RectNative
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
#endif
}
