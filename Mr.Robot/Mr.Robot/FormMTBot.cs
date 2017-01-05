using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mr.Robot.MacroSwitchAnalyser;

namespace Mr.Robot
{
	public partial class FormMTBot : Form
	{
		public FormMTBot()
		{
			InitializeComponent();
		}

		private void btnOpenRoot_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				tbxRootPath.Text = dlg.SelectedPath;
				tbxSourcePath.Text = dlg.SelectedPath;
			}
		}

		private void btnOpenSource_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				tbxSourcePath.Text = dlg.SelectedPath;
			}
		}

		private void btnStart_Click(object sender, EventArgs e)
		{

		}
	}
}
