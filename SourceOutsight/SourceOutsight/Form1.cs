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

namespace SourceOutsight
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			this.tbxPath.Text = "C:\\Users\\GangJian\\03_work\\github\\MyProjects\\Mr.Robot\\TestSrc\\swc_in_oilp";
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			string prj_dir = this.tbxPath.Text;
			SO_Project so_prj = new SO_Project(prj_dir);
			string target_file = "default_MemMap.h";
			SO_File file_info = so_prj.GetFileInfo(prj_dir + "\\" + target_file);
			Trace.Assert(null != file_info);
			this.tbxLog.Clear();
			List<string> log_list = file_info.IDTable.ToStringList();
			foreach (var item in log_list)
			{
				this.tbxLog.AppendText(System.Environment.NewLine + item);
			}
		}
	}
}
