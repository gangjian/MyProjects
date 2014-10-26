using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace TomatoClock2
{
    public partial class Form1 : Form
    {
        private ClassTomatoClock m_TomatoClock;
        private ClassTimeClock m_TimeClock;
        private bool m_bPauseFlag = false;

        public Form1()
        {
            m_TomatoClock = new ClassTomatoClock();
            m_TimeClock = new ClassTimeClock();
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            //m_TomatoClock.Paint(e.Graphics);
            m_TimeClock.Paint(e.Graphics);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_bPauseFlag)
            {
                return;
            }
            m_TomatoClock.TimerTick();
            this.Invalidate();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            m_TomatoClock.Reset();
            this.Invalidate();
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (m_bPauseFlag)
            {
                m_bPauseFlag = false;
                buttonPause.Text = "Pause";
            }
            else
            {
                m_bPauseFlag = true;
                buttonPause.Text = "Continue";
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            m_TomatoClock.FormResize(this.ClientSize);
            m_TimeClock.FormResize(this.ClientSize);
        }

        private bool _formMove = false;     // 窗体是否移动
        private Point _formPoint;           // 记录窗体的位置
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            _formPoint = new Point();
            int xOffset;
            int yOffset;
            if (e.Button == MouseButtons.Left)
            {
                xOffset = -e.X - SystemInformation.FrameBorderSize.Width;
                yOffset = -e.Y - SystemInformation.CaptionHeight - SystemInformation.FrameBorderSize.Height;
                _formPoint = new Point(xOffset, yOffset);
                _formMove = true;           // 开始移动
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_formMove == true)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(_formPoint.X, _formPoint.Y);
                Location = mousePos;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)  //按下的是鼠标左键
            {
                _formMove = false;              //停止移动
            }
        }
    }
}
