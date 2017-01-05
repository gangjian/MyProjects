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
		List<string> SourceList = new List<string>();
		List<string> HeaderList = new List<string>();

		public FormMTBot()
		{
			InitializeComponent();
		}

		private void btnOpenRoot_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				this.tbxRootPath.Text = dlg.SelectedPath;
				this.tbxSourcePath.Text = dlg.SelectedPath;

				this.SourceList.Clear();
				this.HeaderList.Clear();
				// 取得所有源文件和头文件列表
				IOProcess.GetAllCCodeFiles(dlg.SelectedPath, ref this.SourceList, ref this.HeaderList);

				UpdateSourceListView();
			}
		}

		private void btnOpenSource_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				tbxSourcePath.Text = dlg.SelectedPath;

				List<string> tmpHdList = new List<string>();
				// 取得所有源文件
				this.SourceList.Clear();
				IOProcess.GetAllCCodeFiles(dlg.SelectedPath, ref this.SourceList, ref tmpHdList);

				UpdateSourceListView();
			}
		}

		void UpdateSourceListView()
		{
			this.lvSourceList.Items.Clear();
			for (int i = 0; i < this.SourceList.Count; i++)
			{
				ListViewItem item = new ListViewItem((i + 1).ToString());
				string file_name = this.SourceList[i];
				if (file_name.StartsWith(this.tbxRootPath.Text))
				{
					file_name = "." + file_name.Remove(0, this.tbxRootPath.Text.Length).Trim();
				}
				item.SubItems.Add(new ListViewItem.ListViewSubItem(item, file_name));
				this.lvSourceList.Items.Add(item);
			}
		}

		CCodeAnalyser CodeAnalyser = null;

		private void btnStart_Click(object sender, EventArgs e)
		{
			this.CodeAnalyser = new CCodeAnalyser();
			this.CodeAnalyser.UpdateProgress += new EventHandler(UpdateProgressHandler);
			this.CodeAnalyser.ProcessStart(this.SourceList, this.HeaderList);
		}

		void UpdateProgressHandler(object sender, EventArgs args)
		{
			UpdateProgress(sender as string);
		}

		delegate void UpdateProgressDel(string text);

		void UpdateProgress(string progress_str)
		{
			if (this.InvokeRequired)
			{
				UpdateProgressDel del = new UpdateProgressDel(UpdateProgress);
				this.BeginInvoke(del, new object[] { progress_str });
			}
			else
			{
				this.tbxLog.AppendText(progress_str);
			}
		}

	}
}
