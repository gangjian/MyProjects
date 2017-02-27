using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace HMI_Controler
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			this.ExePath = tbxExePath.Text;
		}

		string ExePath = "C:\\Users\\GangJian\\03_work\\github\\MyProjects\\HMI_Client_Demo\\Debug\\HMI_Client_Demo.exe";

		private void button1_Click(object sender, EventArgs e)
		{
			SendCmdStr(textBox1.Text);
		}

		void SendCmdStr(string cmd_str)
		{
			if (string.IsNullOrEmpty(cmd_str))
			{
				return;
			}
			Process clientProcess = new Process();
			clientProcess.StartInfo = new ProcessStartInfo(this.ExePath, cmd_str);
			clientProcess.StartInfo.UseShellExecute = false;
			clientProcess.StartInfo.CreateNoWindow = true;
			clientProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			clientProcess.Start();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			SendCmdStr(textBox2.Text);
		}

		private void button3_Click(object sender, EventArgs e)
		{
			SendCmdStr(textBox3.Text);
		}

		private void button4_Click(object sender, EventArgs e)
		{
			SendCmdStr(textBox4.Text);
		}

		private void button5_Click(object sender, EventArgs e)
		{
			SendCmdStr(textBox5.Text);
		}

		private void button6_Click(object sender, EventArgs e)
		{
			SendCmdStr(textBox6.Text);
		}

		private void button7_Click(object sender, EventArgs e)
		{
			SendCmdStr(textBox7.Text);
		}

		private void button8_Click(object sender, EventArgs e)
		{
			SendCmdStr(textBox8.Text);
		}
	}
}
