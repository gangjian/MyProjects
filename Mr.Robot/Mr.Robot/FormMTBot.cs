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
		System.Diagnostics.Stopwatch StopWatch = new System.Diagnostics.Stopwatch();

		private void btnStart_Click(object sender, EventArgs e)
		{
			this.lvMacroList.Items.Clear();
			this.tbxLog.Clear();
			this.progressBar1.Value = 0;

			SetUICtrlEnabled(false);
			this.CodeAnalyser = new CCodeAnalyser();
			this.CodeAnalyser.UpdateProgress += new EventHandler(UpdateProgressHandler);
			this.CodeAnalyser.ProcessStart(this.SourceList, this.HeaderList);
			this.StopWatch.Restart();
		}

		List<string> UpdateMacroSwitchList = new List<string>();

		void UpdateProgressHandler(object sender, EventArgs args)
		{
			this.UpdateMacroSwitchList = GetMacroSwitchList(this.CodeAnalyser.ParseInfoList);
			this.CodeAnalyser.ParseInfoList.Clear();
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
				UpdateProgressCtrlView(progress_str);
			}
		}

		void UpdateProgressCtrlView(string progress_str)
		{
			this.tbxLog.AppendText(progress_str + " : " + this.StopWatch.Elapsed.ToString() + System.Environment.NewLine);
			UpdateMacroListView();
			int idx = progress_str.LastIndexOf(':');
			if (-1 != idx)
			{
				string ratioStr = progress_str.Substring(idx + 1).Trim();
				string[] arr = ratioStr.Split('/');
				if (2 == arr.Length)
				{
					int count, total;
					if (int.TryParse(arr[0].Trim(), out count)
						&& int.TryParse(arr[1].Trim(), out total))
					{
						if (total != this.progressBar1.Maximum)
						{
							this.progressBar1.Maximum = total;
						}
						if (count != this.progressBar1.Value)
						{
							this.progressBar1.Value = count;
						}
						if (count == total)
						{
							this.StopWatch.Stop();
							SetUICtrlEnabled(true);
						}
					}
				}
			}
		}

		void SetUICtrlEnabled(bool enabled)
		{
			this.btnOpenRoot.Enabled = enabled;
			this.btnOpenSource.Enabled = enabled;
			this.btnStart.Enabled = enabled;
		}

		List<string> GetMacroSwitchList(List<FileParseInfo> parse_info_list)
		{
			List<string> retList = new List<string>();
			foreach (FileParseInfo fpi in parse_info_list)
			{
				retList.AddRange(fpi.MacroSwitchList);
			}
			return retList;
		}

		void UpdateMacroListView()
		{
			foreach (string msStr in this.UpdateMacroSwitchList)
			{
				string[] arr = msStr.Split(',');
				int num = this.lvMacroList.Items.Count + 1;
				ListViewItem lvItem = new ListViewItem(num.ToString());
				for (int i = 0; i < this.lvMacroList.Columns.Count - 1; i++)
				{
					lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, arr[i].Trim()));
				}
				this.lvMacroList.Items.Add(lvItem);
			}
			this.UpdateMacroSwitchList.Clear();
		}
	}
}
