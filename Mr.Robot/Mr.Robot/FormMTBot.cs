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
		List<string> SourceList = new List<string>();									// .c源文件
		List<string> HeaderList = new List<string>();									// .h头文件
		List<string> MtpjFileList = new List<string>();									// .mtpj文件
		List<string> MkFileList = new List<string>();									// .mk文件

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
				if (Directory.Exists(root_path))
				{
					this.tbxRootPath.Text = root_path;
					this.tbxSourcePath.Text = root_path;

					this.SourceList.Clear();
					this.HeaderList.Clear();
					this.MtpjFileList.Clear();
					this.MkFileList.Clear();
					// 取得所有源文件和头文件列表
					IOProcess.GetAllCCodeFiles(root_path, ref this.SourceList, ref this.HeaderList, ref this.MtpjFileList, ref this.MkFileList);
					UpdateSourceTreeView(root_path);
				}
			}
		}

		void UpdateSourcePath(string source_path)
		{
			if (!string.IsNullOrEmpty(source_path))
			{
				if (Directory.Exists(source_path))
				{
					tbxSourcePath.Text = source_path;

					List<string> tmpHdList = new List<string>();
					List<string> tmpMtptList = new List<string>();
					List<string> tmpMkList = new List<string>();
					// 取得所有源文件
					this.SourceList.Clear();
					IOProcess.GetAllCCodeFiles(source_path, ref this.SourceList, ref tmpHdList, ref tmpMtptList, ref tmpMkList);
					UpdateSourceTreeView(source_path);
				}
			}
		}

		void UpdateSourceTreeView(string root_path)
		{
			if (string.IsNullOrEmpty(root_path)
				|| !Directory.Exists(root_path))
			{
				return;
			}
			this.treeViewSrcFile.Nodes.Clear();
			DirectoryInfo di = new DirectoryInfo(root_path);
			TreeNode rootNode = new TreeNode(di.Name);
			rootNode.Tag = root_path;
			rootNode.ImageIndex = 2;
			rootNode.SelectedImageIndex = 2;
			if (AddDirsAndFiles2TreeView(ref rootNode, root_path, root_path))
			{
				this.treeViewSrcFile.Nodes.Add(rootNode);
				rootNode.Checked = true;
				rootNode.ExpandAll();
			}
		}

		bool AddDirsAndFiles2TreeView(ref TreeNode parent_node, string current_path, string root_path)
		{
			if (string.IsNullOrEmpty(current_path)
				|| !Directory.Exists(current_path))
			{
				return false;
			}
			bool ret = false;
			string[] dirs = Directory.GetDirectories(current_path);
			string[] files = Directory.GetFiles(current_path);
			foreach (var dir in dirs)
			{
				DirectoryInfo di = new DirectoryInfo(dir);
				TreeNode node = new TreeNode(di.Name);
				node.Tag = dir;
				node.ImageIndex = 2;
				node.SelectedImageIndex = 2;
				if (AddDirsAndFiles2TreeView(ref node, dir, root_path))
				{
					parent_node.Nodes.Add(node);
					ret = true;
				}
			}
			foreach (var fname in files)
			{
				FileInfo fi = new FileInfo(fname);
				if (fi.Extension.ToLower().Equals(".c"))
				{
					string src_name = fname;
					if (VerifySourceFile(ref src_name, root_path))
					{
						TreeNode node = parent_node.Nodes.Add(fi.Name);
						node.Tag = src_name;
						node.ImageIndex = 0;
						node.SelectedImageIndex = 0;
						// 只要有一个以上验证正确的就返回true
						ret = true;
					}
					else
					{
						string errMsg = "*****Source File Verify Error!***** : " + fname;
						this.tbxOutputLog.AppendText(errMsg + Environment.NewLine);
						System.Diagnostics.Trace.WriteLine(errMsg);
					}
				}
			}
			return ret;
		}

		bool VerifySourceFile(ref string full_name, string root_path)
		{
			// 要验证这个文件:	1.(Target路径不是root路径下子路径的场合)在root路径下有对应一致的文件(1.相对路径一致; 2.内容一致);
			//					2.或者(Target路径是root路径下的子路径的场合)本身就是root路径下的文件;
			if (root_path.StartsWith(this.tbxRootPath.Text))
			{
				return true;
			}
			if (full_name.StartsWith(root_path))
			{
				string relativePath = full_name.Remove(0, root_path.Length);
				int matchCount = 0;
				string actualPath = string.Empty;										// root路径下对应的.c文件
				foreach (var src in this.SourceList)
				{
					if (src.EndsWith(relativePath)
						&& CommonProc.CompareTextFileContentsSame(src, full_name))
					{
						matchCount += 1;
						actualPath = src;
					}
				}
				if (1 == matchCount)
				{
					full_name = actualPath;
					return true;
				}
			}
			return false;
		}

		List<string> GetSourceListFromTreeView()
		{
			List<string> retList = new List<string>();
			foreach (TreeNode node in this.treeViewSrcFile.Nodes)
			{
				retList.AddRange(GetSourceListFromTreeNode(node));
			}
			return retList;
		}

		List<string> GetSourceListFromTreeNode(TreeNode root_node)
		{
			List<string> retList = new List<string>();
			foreach (TreeNode node in root_node.Nodes)
			{
				string pathStr = node.Tag.ToString();
				if (pathStr.ToLower().EndsWith(".c")
					&& node.Checked
					&& File.Exists(pathStr))
				{
					retList.Add(pathStr);
				}
				else if (Directory.Exists(pathStr))
				{
					retList.AddRange(GetSourceListFromTreeNode(node));
				}
			}
			return retList;
		}

		MACRO_SWITCH_ANALYSER MacroSwitchAnalyzer = null;
		System.Diagnostics.Stopwatch StopWatch = new System.Diagnostics.Stopwatch();

		private void btnStart_Click(object sender, EventArgs e)
		{
			this.SourceList = GetSourceListFromTreeView();
			if (0 == this.SourceList.Count)
			{
				return;
			}
			ClearAllLastResults();														// 清除所有上一次的解析结果
			SetUICtrlEnabled(false);													// 设置GUI控件无效化
			StartMacroSwitchAnalyzer();													// 启动MacroSwitchAnalyzer开始解析
		}

		void ClearAllLastResults()
		{
			this.lvDetailList.Items.Clear();
			this.lvSummaryList.Items.Clear();
			this.tbxLog.Clear();
			this.tbxOutputLog.Clear();
			this.progressBar1.Value = 0;
			//this.DetailResultList.Clear();
			this.SummaryResultList.Clear();
			this.ProcessedSoureList.Clear();
		}

		void StartMacroSwitchAnalyzer()
		{
			this.MacroSwitchAnalyzer = new MACRO_SWITCH_ANALYSER(new MSA_INPUT_PARA(this.SourceList, this.HeaderList, this.MtpjFileList, this.MkFileList));
			this.MacroSwitchAnalyzer.ReportProgressHandler += new EventHandler(UpdateProgressHandler);
			this.MacroSwitchAnalyzer.ProcStart();
			this.StopWatch.Restart();
		}

		Queue<string> MacroSwitchResultQueue = new Queue<string>();						// 用来接收MacroSwitchAnalyzer解析出的宏开关结果的队列

		public class SUMMARY_INFO
		{
			public string MacroName = string.Empty;										// 宏定义名
			public string Conclusion = string.Empty;									// 解析结论(是否定义, 定义值)
			public string FileName = string.Empty;										// (宏定义)所在文件名)

			public SUMMARY_INFO(string macro_name, string conclusion, string file_name)
			{
				this.MacroName = macro_name;
				this.Conclusion = conclusion;
				this.FileName = file_name;
			}
		}

		//List<string> DetailResultList = new List<string>();							// 详细结果
		List<SUMMARY_INFO> SummaryResultList = new List<SUMMARY_INFO>();				// 汇总结果

		List<string> ProcessedSoureList = new List<string>();							// 已经处理完的源文件列表

		void UpdateProgressHandler(object o, EventArgs e)
		{
			foreach (var rsltInfo in this.MacroSwitchAnalyzer.OutputResult.SourceResultList)
			{
				if (!this.ProcessedSoureList.Contains(rsltInfo.SourceFileName))
				{
					this.ProcessedSoureList.Add(rsltInfo.SourceFileName);
					if (null != rsltInfo.MacroSwitchResultList)
					{
						foreach (var rsltStr in rsltInfo.MacroSwitchResultList)
						{
							lock (this.MacroSwitchResultQueue)
							{
								this.MacroSwitchResultQueue.Enqueue(rsltStr);
							}
						}
						UpdateProgress(this.MacroSwitchAnalyzer.OutputResult.Progress);
					}
					else
					{
						UpdateProgress(this.MacroSwitchAnalyzer.OutputResult.Progress);
					}
				}
			}
		}

		delegate void UpdateProgressDel(MSA_PROGRESS msa_progress);

		void UpdateProgress(MSA_PROGRESS msa_progress)
		{
			if (this.InvokeRequired)
			{
				UpdateProgressDel del = new UpdateProgressDel(UpdateProgress);
				this.BeginInvoke(del, new object[] { msa_progress });
			}
			else
			{
				UpdateProgressCtrlView(msa_progress);
			}
		}

		void UpdateProgressCtrlView(MSA_PROGRESS msa_progress)
		{
			UpdateProgressBarView(msa_progress);

			List<string> tmpList = new List<string>();
			lock (this.MacroSwitchResultQueue)
			{
				while (0 != this.MacroSwitchResultQueue.Count)
				{
					tmpList.Add(this.MacroSwitchResultQueue.Dequeue());
				}
			}
			// 详细结果列表
			//this.DetailResultList.AddRange(tmpList);
			foreach (string rslt in tmpList)
			{
				string[] arr = rslt.Split(',');
				if (arr.Length >= 6)
				{
					string macro_name = arr[arr.Length - 3];
					string analysis_conclusion = arr[arr.Length - 2];
					string macro_file_name = arr[arr.Length - 1];
					UpdateSummaryList(macro_name, analysis_conclusion, macro_file_name);
				}
			}

			UpdateDetailListView(tmpList);
			UpdateSummaryListView();
		}

		void SetUICtrlEnabled(bool enabled)
		{
			this.btnOpenRoot.Enabled = enabled;
			this.btnOpenSource.Enabled = enabled;
			this.btnStart.Enabled = enabled;
			this.btnSaveDetail2CSV.Enabled = enabled;
			this.btnSaveSummary2CSV.Enabled = enabled;
			this.treeViewSrcFile.Enabled = enabled;
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
				StreamWriter sw = new StreamWriter(dlg.FileName, false, Encoding.UTF8);
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
			if (null != this.MacroSwitchAnalyzer)
			{
				this.MacroSwitchAnalyzer.ProcAbort();
				this.MacroSwitchAnalyzer = null;
			}
		}

		void UpdateSummaryList(string macro_name, string conclusion_str, string macro_file_name)
		{
			for (int i = 0; i < this.SummaryResultList.Count; i++)
			{
				if (this.SummaryResultList[i].MacroName == macro_name)
				{
					if (@"▼" != this.SummaryResultList[i].Conclusion
						&& conclusion_str != this.SummaryResultList[i].Conclusion)
					{
						this.SummaryResultList[i].Conclusion = @"▼";
					}
					else
					{
					}
					return;
				}
			}
			SUMMARY_INFO sm_info = new SUMMARY_INFO(macro_name, conclusion_str, macro_file_name);
			this.SummaryResultList.Add(sm_info);
		}

		private void btnSaveSummary2CSV_Click(object sender, EventArgs e)
		{
			SaveListView2CsvFile(this.lvSummaryList, "Summary");
		}

		void UpdateSummaryListView()
		{
			lock (this.SummaryResultList)
			{
				for (int i = 0; i < this.lvSummaryList.Items.Count; i++)
				{
					ListViewItem lvItem = this.lvSummaryList.Items[i];
					if (lvItem.SubItems[1].Text != this.SummaryResultList[i].MacroName)
					{
						lvItem.SubItems[1].Text = this.SummaryResultList[i].MacroName;
					}
					if (lvItem.SubItems[2].Text != this.SummaryResultList[i].Conclusion)
					{
						lvItem.SubItems[2].Text = this.SummaryResultList[i].Conclusion;
					}
					if (lvItem.SubItems[3].Text != this.SummaryResultList[i].FileName)
					{
						lvItem.SubItems[3].Text = this.SummaryResultList[i].FileName;
					}
				}
				if (this.SummaryResultList.Count > this.lvSummaryList.Items.Count)
				{
					int addCount = this.SummaryResultList.Count - this.lvSummaryList.Items.Count;
					for (int i = 0; i < addCount; i++)
					{
						SUMMARY_INFO sm_info = this.SummaryResultList[this.SummaryResultList.Count - addCount + i];
						int num = this.lvSummaryList.Items.Count + 1;
						ListViewItem lvItem = new ListViewItem(num.ToString());
						lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, sm_info.MacroName));
						lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, sm_info.Conclusion));
						lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, sm_info.FileName));
						this.lvSummaryList.Items.Add(lvItem);
					}
				}
			}
		}

		void UpdateDetailListView(List<string> add_list)
		{
			ListViewItem lvItem = null;
			foreach (var item in add_list)
			{
				string[] arr = item.Split(',');
				int num = this.lvDetailList.Items.Count + 1;
				lvItem = new ListViewItem(num.ToString());
				for (int i = 0; i < this.lvDetailList.Columns.Count - 1; i++)
				{
					lvItem.SubItems.Add(new ListViewItem.ListViewSubItem(lvItem, arr[i].Trim()));
				}
				this.lvDetailList.Items.Add(lvItem);
			}
			if (null != lvItem)
			{
				this.lvDetailList.TopItem = lvItem;
			}
		}

		void UpdateProgressBarView(MSA_PROGRESS msa_progress)
		{
			string progressStr = msa_progress.CurrentSourceName + " ==> "
						+ msa_progress.ProcResult.ToString() + " : "
						+ msa_progress.CurrentCount.ToString() + "/" + msa_progress.TotalCount.ToString();
			this.tbxLog.AppendText(progressStr + " : " + this.StopWatch.Elapsed.ToString() + System.Environment.NewLine);
			if (msa_progress.TotalCount != this.progressBar1.Maximum)
			{
				this.progressBar1.Maximum = msa_progress.TotalCount;
			}
			if (msa_progress.CurrentCount != this.progressBar1.Value)
			{
				this.progressBar1.Value = msa_progress.CurrentCount;
			}
			if (msa_progress.CurrentCount == msa_progress.TotalCount)
			{
				this.StopWatch.Stop();
				SetUICtrlEnabled(true);
				MessageBox.Show("Complete!");
			}
		}

		private void treeViewSrcFile_AfterCheck(object sender, TreeViewEventArgs e)
		{
			TreeNode node = e.Node;
			for (int i = 0; i < node.Nodes.Count; i++)
			{
				node.Nodes[i].Checked = node.Checked;
			}
		}
	}
}
