using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CodeCreeper;

namespace CreeperGUI
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			this.tbxPrjPath.Text = "C:\\Users\\GangJian\\03_work\\github\\MyProjects\\Mr.Robot\\TestSrc\\swc_in_oilp";
			this.tbxFileName.Text = "Compiler_Cfg.h";
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			string prj_dir = this.tbxPrjPath.Text;
			string file_name = this.tbxFileName.Text;
			CodeProjectInfo prj_info = new CodeProjectInfo(prj_dir);
			Creeper code_creeper = new Creeper(prj_info);
			code_creeper.CreepFile(prj_dir + "\\" + file_name);
			var print_list = code_creeper.GetSyntaxTreePrintList();
			foreach (var item in print_list)
			{
				this.tbxLog.AppendText(item + System.Environment.NewLine);
			}
		}

		private void btnOpenPrjPath_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				this.tbxPrjPath.Text = fbd.SelectedPath;
			}
		}

		private void btnOpenFileName_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.RestoreDirectory = true;
			ofd.Multiselect = false;
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				FileInfo fi = new FileInfo(ofd.FileName);
				this.tbxFileName.Text = fi.Name;
			}
		}
	}
}
