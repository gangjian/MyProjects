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
			//this.tbxPath.Text = "C:\\Users\\GangJian\\03_work\\99_Data\\MTbot_TestData\\Honda18HMI_soft";
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			string prj_dir = this.tbxPath.Text;
			List<string> prj_id_tree_list = GetPrjectIDTreeList(prj_dir);

			this.tbxLog.Clear();
			StringBuilder sb = new StringBuilder();
			foreach (var item in prj_id_tree_list)
			{
				sb.AppendLine(item);
			}
			this.tbxLog.AppendText(sb.ToString());
		}

		List<string> GetPrjectIDTreeList(string prj_dir)
		{
			List<string> ret_list = new List<string>();
			ret_list.Add(MakeNameCommentsLineString(prj_dir));
			ret_list.Add(string.Empty);

			SO_Project so_prj = new SO_Project(prj_dir);
			List<SO_File> src_list = so_prj.GetSourceInfoList();
			foreach (var item in src_list)
			{
				ret_list.AddRange(GetFileIDTreeList(item, prj_dir));
			}
			List<SO_File> hd_list = so_prj.GetHeaderInfoList();
			foreach (var item in hd_list)
			{
				ret_list.AddRange(GetFileIDTreeList(item, prj_dir));
			}
			return ret_list;
		}
		List<string> GetFileIDTreeList(SO_File file_info, string prj_dir)
		{
			List<string> ret_list = new List<string>();
			string file_name = file_info.FullName;
			ret_list.Add(string.Empty);
			if (file_name.StartsWith(prj_dir))
			{
				file_name = file_name.Substring(prj_dir.Length);
			}
			ret_list.Add(MakeNameCommentsLineString(file_name));
			ret_list.AddRange(file_info.IDTable.ToStringList());
			return ret_list;
		}
		const int NAME_COMMENTS_LEN = 80;
		string MakeNameCommentsLineString(string name_str)
		{
			if (name_str.Length > NAME_COMMENTS_LEN)
			{
				return name_str;
			}
			int side_len = (NAME_COMMENTS_LEN - name_str.Length) / 2;
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < side_len; i++)
			{
				sb.Append('*');
			}
			string side_str = sb.ToString();
			return side_str + name_str + side_str;
		}

		private void btnOpen_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				this.tbxPath.Text = fbd.SelectedPath;
				this.tbxLog.Clear();
			}
		}
	}
}
