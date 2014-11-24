﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using SuperKeys;

namespace GDIPlusTest.GameRobots.Robot2
{
    public partial class FormRobot2 : Form
    {
        public FormRobot2()
        {
            InitializeComponent();
        }

        WinIOApi m_winio_api;

        private void FormRobot2_Load(object sender, EventArgs e)
        {
            m_winio_api = new WinIOApi();
        }

        private void FormRobot2_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_winio_api.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread.Sleep(3000);
            for (int i = 0; i < 100; i++)
            {
                m_winio_api.KeyPress(WinIoSys.Key.VK_CONTROL, 100);
                m_winio_api.KeyPress(WinIoSys.Key.VK_SHIFT, 100);
            }
        }
    }
}