using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mr.Robot.MacroSwitchAnalyser;
using System.IO;

namespace Mr.Robot
{
	public partial class FormMTBot : Form
	{
		List<string> SourceList = new List<string>();
		List<string> HeaderList = new List<string>();

		public FormMTBot()
		{
			InitializeComponent();
			string root_path = IniFileProcess.IniReadValue("MTbot", "root_path");
			UpdateRootPath(root_path);
			string source_path = IniFileProcess.IniReadValue("MTbot", "source_path");
			UpdateSourcePath(source_path);
		}

		private void btnOpenRoot_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (!string.IsNullOrEmpty(tbxRootPath.Text))
			{
				dlg.SelectedPath = tbxRootPath.Text;
			}
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				UpdateRootPath(dlg.SelectedPath);
				IniFileProcess.IniWriteValue("MTbot", "root_path", dlg.SelectedPath);
				IniFileProcess.IniWriteValue("MTbot", "source_path", dlg.SelectedPath);
			}
		}

		private void btnOpenSource_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (!string.IsNullOrEmpty(tbxSourcePath.Text))
			{
				dlg.SelectedPath = tbxSourcePath.Text;
			}
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				UpdateSourcePath(dlg.SelectedPath);
				IniFileProcess.IniWriteValue("MTbot", "source_path", dlg.SelectedPath);
			}
		}

		void UpdateRootPath(string root_path)
		{
			if (!string.IsNullOrEmpty(root_path))
			{
				DirectoryInfo di = new DirectoryInfo(root_path);
				if (di.Exists)
				{
					this.tbxRootPath.Text = root_path;
					this.tbxSourcePath.Text = root_path;

					this.SourceList.Clear();
					this.HeaderList.Clear();
					// 取得所有源文件和头文件列表
					IOProcess.GetAllCCodeFiles(root_path, ref this.SourceList, ref this.HeaderList);
					UpdateSourceListView();
				}
			}
		}

		void UpdateSourcePath(string source_path)
		{
			if (!string.IsNullOrEmpty(source_path))
			{
				DirectoryInfo di = new DirectoryInfo(source_path);
				if (di.Exists)
				{
					tbxSourcePath.Text = source_path;

					List<string> tmpHdList = new List<string>();
					// 取得所有源文件
					this.SourceList.Clear();
					IOProcess.GetAllCCodeFiles(source_path, ref this.SourceList, ref tmpHdList);
					UpdateSourceListView();
				}
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

		MacroSwitchAnalyzer2 MacroSwitchAnalyzer2 = null;
		System.Diagnostics.Stopwatch StopWatch = new System.Diagnostics.Stopwatch();

		private void btnStart_Click(object sender, EventArgs e)
		{
			this.lvDetailList.Items.Clear();
			this.tbxLog.Clear();
			this.progressBar1.Value = 0;
			this.DetailResultList.Clear();
			this.SummaryResultList.Clear();

			SetUICtrlEnabled(false);
			this.MacroSwitchAnalyzer2 = new MacroSwitchAnalyzer2(this.SourceList, this.HeaderList);
			this.MacroSwitchAnalyzer2.ReportProgress += UpdateProgressHandler;
			this.MacroSwitchAnalyzer2.ProcStart();
			this.StopWatch.Restart();
		}

		List<string> UpdateMacroSwitchList = new List<string>();

		List<string> DetailResultList = new List<string>();								// 详细结果
		List<string> SummaryResultList = new List<string>();							// 汇总结果

		void UpdateProgressHandler(string progress_str, List<string> result_list)
		{
			if (null != result_list)
			{
				lock (result_list)
				{
					this.UpdateMacroSwitchList.Clear();
					foreach (string resultStr in result_list)
					{
						this.UpdateMacroSwitchList.Add(resultStr);
					}
					AddDetailAndSummaryResultList();
					UpdateProgress(progress_str);
				}
			}
			else
			{
				UpdateProgress(progress_str);
			}
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
			UpdateDetailListView();
			UpdateSummaryListView();
			this.tbxLog.AppendText(progress_str + " : " + this.StopWatch.Elapsed.ToString() + System.Environment.NewLine);
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
							MessageBox.Show("Complete!");
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
			this.btnSaveDetail2CSV.Enabled = enabled;
			this.btnSaveSummary2CSV.Enabled = enabled;
		}

		void UpdateDetailListView()
		{
			if (null == this.UpdateMacroSwitchList
				|| 0 == this.UpdateMacroSwitchList.Count)
			{
				return;
			}
			ListViewItem lvItem = null;
			foreach (string msStr in this.UpdateMacroSwitchList)
			{
				string[] arr = msStr.Split(',');
				int num = this.lvDetailList.Items.Count + 1;
				lvItem = new ListViewItem(num.ToString());
				for (int i = 0; i < this.lvDetailList.Columns.Count - 1; i++)
				{
					lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, arr[i].Trim()));
				}
				this.lvDetailList.Items.Add(lvItem);
			}
			this.UpdateMacroSwitchList.Clear();
			if (null != lvItem)
			{
				this.lvDetailList.TopItem = lvItem;
			}
		}

		private void btnSaveDetail2CSV_Click(object sender, EventArgs e)
		{
			SaveListView2CsvFile(this.lvDetailList, "Detail");
		}

		void SaveListView2CsvFile(ListView list_view_ctrl, string category_str)
		{
			if (0 == list_view_ctrl.Items.Count)
			{
				return;
			}
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = DateTime.Now.ToString().Replace('/', '_').Replace(':', '_') + "_" + category_str + ".csv";
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				StreamWriter sw = new StreamWriter(dlg.FileName, false, Encoding.Default);
				try
				{
					string wtStr = string.Empty;
					foreach (ColumnHeader col in list_view_ctrl.Columns)
					{
						wtStr += col.Text + ",";
					}
					sw.WriteLine(wtStr);
					foreach (ListViewItem item in list_view_ctrl.Items)
					{
						wtStr = string.Empty;
						foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
						{
							wtStr += subItem.Text + ",";
						}
						sw.WriteLine(wtStr);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
				finally
				{
					sw.Close();
				}
			}

		}

		private void FormMTBot_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (null != this.MacroSwitchAnalyzer2)
			{
				this.MacroSwitchAnalyzer2.ProcAbort();
			}
		}

		void AddDetailAndSummaryResultList()
		{
			// 详细结果列表
			this.DetailResultList.AddRange(this.UpdateMacroSwitchList);

			// 汇总结果列表
			// 取得宏名, 解析结果
			foreach (string rslt in this.UpdateMacroSwitchList)
			{
				string[] arr = rslt.Split(',');
				if (arr.Length >= 5)
				{
					string macro_name = arr[arr.Length - 2].Trim();
					string analysis_conclusion = arr[arr.Length - 1].Trim();
					UpdateSummaryList(macro_name, analysis_conclusion);
				}
			}
		}

		void UpdateSummaryList(string macro_name, string conclusion_str)
		{
			for (int i = 0; i < this.SummaryResultList.Count; i++)
			{
				string[] arr = this.SummaryResultList[i].Split(',');
				if (2 != arr.Length)
				{
					 continue;
				}
				string curMacroName = arr[0].Trim();
				string curConclusionStr = arr[1].Trim();
				if (curMacroName == macro_name)
				{
					if ("Discordant" != curConclusionStr
						&& conclusion_str != curConclusionStr)
					{
						curConclusionStr = "Discordant";
						this.SummaryResultList[i] = curMacroName + "," + curConclusionStr;
					}
					else
					{
					}
					return;
				}
			}
			this.SummaryResultList.Add(macro_name + "," + conclusion_str);
		}

		private void btnSaveSummary2CSV_Click(object sender, EventArgs e)
		{
			SaveListView2CsvFile(this.lvSummaryList, "Summary");
		}

		void UpdateSummaryListView()
		{
			if (this.SummaryResultList.Count > this.lvSummaryList.Items.Count)
			{
				int addCount = this.SummaryResultList.Count - this.lvSummaryList.Items.Count;
				for (int i = 0; i < addCount; i++)
				{
					string curStr = this.SummaryResultList[this.SummaryResultList.Count - addCount + i];
					string[] arr = curStr.Split(',');
					if (2 != arr.Length)
					{
						continue;
					}
					int num = this.lvSummaryList.Items.Count + 1;
					ListViewItem lvItem = new ListViewItem(num.ToString());
					lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, arr[0]));
					lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, arr[1]));
					this.lvSummaryList.Items.Add(lvItem);
				}
			}
			for (int i = 0; i < this.lvSummaryList.Items.Count; i++)
			{
				string curStr = this.SummaryResultList[i];
				string[] arr = curStr.Split(',');
				if (2 != arr.Length)
				{
					continue;
				}
				ListViewItem lvItem = this.lvSummaryList.Items[i];
				if (lvItem.SubItems[1].Text != arr[0])
				{
					lvItem.SubItems[1].Text = arr[0];
				}
				if (lvItem.SubItems[2].Text != arr[1])
				{
					lvItem.SubItems[2].Text = arr[1];
				}
			}
		}
	}
}
