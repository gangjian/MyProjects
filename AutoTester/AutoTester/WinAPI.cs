using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Drawing;
using System.ComponentModel;

namespace AutoTester
{
    public class WinAPI
    {
        //需要调用的API
        //找到窗口（进程名称 可空，窗口名称）
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        //把窗口放到最前面
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        //模拟键盘事件
        [DllImport("User32.dll")]
        public static extern void keybd_event(Byte bVk, Byte bScan, Int32 dwFlags, Int32 dwExtraInfo);
        //释放按键的常量
        private const int KEYEVENTF_KEYUP = 2;
        //发送消息
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]//获取窗口大小
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]//获取窗口坐标
        public struct RECT
        {
            public int Left;    //最左坐标
            public int Top;     //最上坐标
            public int Right;   //最右坐标
            public int Bottom;  //最下坐标
        }

        #region SendMessage 参数
        private const int WM_KEYDOWN = 0X100;
        private const int WM_KEYUP = 0X101;
        private const int WM_SYSCHAR = 0X106;
        private const int WM_SYSKEYUP = 0X105;
        private const int WM_SYSKEYDOWN = 0X104;
        private const int WM_CHAR = 0X102;
        #endregion

        //鼠标事件
        // private readonly int MOUSEEVENTF_LEFTDOWN = 0x2;
        // private readonly int MOUSEEVENTF_LEFTUP = 0x4;
        [DllImport("user32")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        /// <summary>
        /// 发送一个字符串
        /// </summary>
        /// <param name="myIntPtr">窗口句柄</param> 
        /// <param name="Input">字符串</param>
        public static void InputStr(IntPtr k, string Input)
        {
            //不能发送汉字，只能发送键盘上有的内容 也可以模拟shift+！等
            byte[] ch = (ASCIIEncoding.ASCII.GetBytes(Input));
            for (int i = 0; i < ch.Length; i++)
            {
                SendMessage(k, WM_CHAR, ch[i], 0);
                System.Threading.Thread.Sleep(10);
            }
        }

        public static void SendCommand(IntPtr wnd, string cmdStr)
        {
            WinAPI.SetForegroundWindow(wnd);
            string a = cmdStr + "\n";
            WinAPI.InputStr(wnd, a);
        }
    }
}
