using UnityEngine;

namespace DeskFish.Runtime.Windows
{
    public sealed class DesktopWindowController : MonoBehaviour
    {
        [SerializeField] private KeyCode toggleKey = KeyCode.F12;
        private bool clickThrough;

        private void Start()
        {
            SetWindowMode(false);
        }

        private void Update()
        {
            var hotkeyPressed = Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(toggleKey);
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            hotkeyPressed = Win32Window.IsToggleHotkeyPressed();
#endif
            if (hotkeyPressed)
            {
                clickThrough = !clickThrough;
                SetWindowMode(clickThrough);
            }
        }

        private static void SetWindowMode(bool passThrough)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            Win32Window.SetDesktopWindow(passThrough);
#endif
        }
    }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    internal static class Win32Window
    {
        private const int GwlExStyle = -20;
        private const int WsExLayered = 0x00080000;
        private const int WsExTransparent = 0x00000020;
        private const int WsExToolWindow = 0x00000080;
        private const uint LwaColorkey = 0x00000001;
        private const uint SwpNosize = 0x0001;
        private const uint SwpNomove = 0x0002;
        private const uint SwpShowwindow = 0x0040;
        private static readonly System.IntPtr HwndTopmost = new System.IntPtr(-1);
        private static bool previousHotkeyState;

        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern System.IntPtr GetActiveWindow();
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern short GetAsyncKeyState(int virtualKey);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern int GetWindowLong(System.IntPtr handle, int index);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern int SetWindowLong(System.IntPtr handle, int index, int value);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern bool SetWindowPos(System.IntPtr handle, System.IntPtr insertAfter, int x, int y, int width, int height, uint flags);
        [System.Runtime.InteropServices.DllImport("user32.dll")] private static extern bool SetLayeredWindowAttributes(System.IntPtr handle, uint colorKey, byte alpha, uint flags);

        public static bool IsToggleHotkeyPressed()
        {
            const int vkControl = 0x11;
            const int vkShift = 0x10;
            const int vkF12 = 0x7B;
            var pressed = (GetAsyncKeyState(vkControl) & 0x8000) != 0 &&
                          (GetAsyncKeyState(vkShift) & 0x8000) != 0 &&
                          (GetAsyncKeyState(vkF12) & 0x8000) != 0;
            var triggered = pressed && !previousHotkeyState;
            previousHotkeyState = pressed;
            return triggered;
        }

        public static void SetDesktopWindow(bool passThrough)
        {
            var handle = GetActiveWindow();
            var style = GetWindowLong(handle, GwlExStyle);
            style |= WsExLayered | WsExToolWindow;
            if (passThrough) style |= WsExTransparent;
            else style &= ~WsExTransparent;
            SetWindowLong(handle, GwlExStyle, style);
            SetLayeredWindowAttributes(handle, 0x00FF00FF, 0, LwaColorkey);
            SetWindowPos(handle, HwndTopmost, 0, 0, 0, 0, SwpNomove | SwpNosize | SwpShowwindow);
        }
    }
#endif
}
