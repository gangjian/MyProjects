using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GDIPlusTest
{
    class Win32Api
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        public static IntPtr findWindow(string lpClassName, string lpWindowName)
        {
            return FindWindow(lpClassName, lpWindowName);
        }

        [DllImport("User32.dll")]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        const int MOUSEEVENTF_MOVE = 0x0001;
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        [DllImport("User32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        const uint KEYEVENTF_KEYDOWN = 0x0;
        const uint KEYEVENTF_KEYUP = 0x2;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        public const int VK_CONTROL = 0x11;
        public const int VK_UP      = 0x26;
        public const int VK_LEFT    = 0x25;
        public const int VK_DOWN    = 0x28;
        public const int VK_RIGHT   = 0x27;

        /// <summary>
        /// 鼠标单击
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void MouseClick(int x, int y)
        {
            int scrWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int scrHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int dx = x * 65536 / scrWidth;
            int dy = y * 65536 / scrHeight;
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, dx, dy, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// 鼠标双击
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="interval">双击间隔(默认100毫秒)</param>
        public static void MouseDoubleClick(int x, int y, int interval = 100)
        {
            MouseClick(x, y);
            System.Threading.Thread.Sleep(interval);
            MouseClick(x, y);
        }

        public static void MouseMove(int x, int y)
        {
            int scrWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int scrHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            int dx = x * 65536 / scrWidth;
            int dy = y * 65536 / scrHeight;
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, dx, dy, 0, 0);
        }

        public static void KeyboardKeyDown(int vk)
        {
            for (int i = 0; i < 500; i++)
            {
                keybd_event((byte)vk, 0, KEYEVENTF_KEYDOWN, 0);
                System.Threading.Thread.Sleep(10);
            }
        }

        public static void KeyboardKeyUp(int vk)
        {
            keybd_event((byte)vk, 0, KEYEVENTF_KEYUP, 0);
        }
    }
}
