using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using SuperKeys;

namespace DEMO
{
    public partial class MainForm : Form
    {
        private int m_Second = 4;
        WinIoSys m_IoSys = new WinIoSys();

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            m_Second--;
            labelSecond.Text = m_Second.ToString();
            if (m_Second == 0)
            {
                //m_IoSys.KeyDown(WinIoSys.Key.VK_S);
                //Thread.Sleep(100);
                //m_IoSys.KeyDown(WinIoSys.Key.VK_D);
                //Thread.Sleep(100);
                //m_IoSys.KeyUp(WinIoSys.Key.VK_S);
                //Thread.Sleep(100);
                //m_IoSys.KeyUp(WinIoSys.Key.VK_D);
                //Thread.Sleep(10);
                //m_IoSys.KeyPress(WinIoSys.Key.VK_J, 200);

                for (int i = 0; i < 100; i++)
                {
                    m_IoSys.KeyPress(WinIoSys.Key.VK_CONTROL, 100);
                    m_IoSys.KeyPress(WinIoSys.Key.VK_SHIFT, 100);
                    //m_IoSys.KeyDown(WinIoSys.Key.VK_SHIFT);
                    //Thread.Sleep(10);
                    //m_IoSys.KeyUp(WinIoSys.Key.VK_SHIFT);
                }
                
                timer1.Stop();
                m_Second = 4;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            m_IoSys.InitSuperKeys();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_IoSys.CloseSuperKeys();
        }
    }
}
