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
using Mr.Robot.CDeducer;

namespace Mr.Robot
{
	public partial class Form1 : Form
	{
	#region 全局字段
		List<string> m_CSourceFileList = new List<string>();
		List<string> m_CHeaderFileList = new List<string>();
		List<string> m_MtpjFileList = new List<string>();
		List<string> m_MkFileList = new List<string>();

		C_PROSPECTOR m_CProspector = null;
	#endregion

		public Form1()
		{
			InitializeComponent();
			UI_Init();
		}

		private void UI_Init()
		{
			// 设定listView列标题和列宽
			lvFileList.Columns.Add("", 326);
			lvFunctionList.Columns.Add("", 326);

			lvVariableList.Columns.Add("Name", 220);
			lvVariableList.Columns.Add("Type", 53);
			lvVariableList.Columns.Add("I/O", 53);

			lvBranchList.Columns.Add("", 326);

			// load根目录路径
			string root_path = IniFileProcess.IniReadValue("PATH_INFO", "root_path");
			if (!string.IsNullOrEmpty(root_path))
			{
				DirectoryInfo di = new DirectoryInfo(root_path);
				if (di.Exists)
				{
					tbxRootPath.Text = root_path;
                    LoadCSourceFiles(root_path);
				}
			}
		}

		private void btnSetFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = "Select the root folder";
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				tbxRootPath.Text = dialog.SelectedPath;
				DirectoryInfo di = new DirectoryInfo(tbxRootPath.Text);
				if (di.Exists)
				{
                    LoadCSourceFiles(tbxRootPath.Text);
					IniFileProcess.IniWriteValue("PATH_INFO", "root_path", tbxRootPath.Text);
				}
			}
		}

		private void LoadCSourceFiles(string path_name)
		{
			// 检查文件夹路径的合法性
			if (string.IsNullOrEmpty(path_name)
                || !Directory.Exists(path_name))
			{
				return;
			}

			this.m_CSourceFileList = new List<string>();
			this.m_CHeaderFileList = new List<string>();
            // 遍历文件夹, 取得所有.c源文件和.h头文件
			IOProcess.GetAllCCodeFiles(path_name, ref this.m_CSourceFileList, ref m_CHeaderFileList, ref this.m_MtpjFileList, ref this.m_MkFileList);

            UpdateFileListViewCtrl(this.m_CSourceFileList);
		}

        void UpdateFileListViewCtrl(List<string> src_list)
        {
            lvFileList.Items.Clear();
            // 将得到的.c源文件加入UI文件列表
            foreach (string cfile in src_list)
            {
                // 分别取得文件名和路径名
                string path;
                string fname = IOProcess.GetFileName(cfile, out path);
                // 暂时只在文件列表里显示不包含完整路径的文件名
                ListViewItem new_item = new ListViewItem(fname);
                // 完整路径放在SubItem里
                new_item.SubItems.Add(cfile);
                lvFileList.Items.Add(new_item);
            }
            lvFunctionList.Items.Clear();
            lvFunctionList.Enabled = false;
            lvVariableList.Items.Clear();
            lvVariableList.Enabled = false;
            lvBranchList.Items.Clear();
            lvBranchList.Enabled = false;
        }

		public List<FILE_PARSE_INFO> CSourceParseInfoList = new List<FILE_PARSE_INFO>();

		/// <summary>
		/// 源文件列表控件选中状态改变
		/// </summary>
		/// <param varName="sender"></param>
		/// <param varName="e"></param>
		private void lvFileList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (0 == lvFileList.SelectedItems.Count)
			{
				return;
			}
			if (labelStatus.Text == lvFileList.SelectedItems[0].Text)
			{
				return;
			}
			labelStatus.Text = lvFileList.SelectedItems[0].Text;
			UpdateFunctionListViewCtrl(lvFileList.SelectedItems[0].Text);
		}

		/// <summary>
		/// 函数列表控件选中状态改变
		/// </summary>
		/// <param varName="sender"></param>
		/// <param varName="e"></param>
		private void lvFunctionList_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// 更新函数列表控件
		/// </summary>
		/// <param varName="sourceFileName"></param>
		void UpdateFunctionListViewCtrl(string sourceFileName)
		{
			lvFunctionList.Items.Clear();
			foreach (FILE_PARSE_INFO srcInfo in this.CSourceParseInfoList)
			{
				string path;
				if (sourceFileName == IOProcess.GetFileName(srcInfo.SourceName, out path))
				{
					foreach (FUNCTION_PARSE_INFO functionInfo in srcInfo.FunDefineList)
					{
						ListViewItem item = new ListViewItem(functionInfo.Name
                            //+ ": "
                            //+ functionInfo.body_start_pos.row_num.ToString() + ", "
                            //+ functionInfo.body_start_pos.col_num.ToString() + "; "
                            //+ functionInfo.body_end_pos.row_num.ToString() + ", "
                            //+ functionInfo.body_end_pos.col_num.ToString()
                            );
						lvFunctionList.Items.Add(item);
					}
					break;
				}
			}
		}

        /// <summary>
        /// 文件列表check all
        /// </summary>
        private void cbxFile_CheckedChanged(object sender, EventArgs e)
        {
            CheckAllListViewItems(lvFileList, cbxFile.Checked);
        }

        /// <summary>
        /// 函数列表check all
        /// </summary>
        private void cbxFunction_CheckedChanged(object sender, EventArgs e)
        {
            CheckAllListViewItems(lvFunctionList, cbxFunction.Checked);
        }

        void CheckAllListViewItems(ListView ctrl, bool bChecked)
        {
            foreach (ListViewItem item in ctrl.Items)
            {
                item.Checked = bChecked;
            }
        }

        /// <summary>
        /// 文件列表解析开始按钮Click
        /// </summary>
        private void btnStartFile_Click(object sender, EventArgs e)
        {
            this.CSourceParseInfoList.Clear();
            List<string> srcList = new List<string>();
            foreach (ListViewItem item in lvFileList.Items)
            {
                if (item.Checked)
                {
                    if (item.SubItems.Count >= 2)
                    {
                        string fpath = item.SubItems[1].Text;
                        srcList.Add(fpath);
                    }
                }
            }
			this.m_CProspector = new C_PROSPECTOR(srcList, this.m_CHeaderFileList);
			this.m_CProspector.CProspectorUpdateProgressHandler += new EventHandler(UpdateAnalyserProgress);
			this.m_CProspector.AsyncStart();
        }

		void UpdateAnalyserProgress(object sender, EventArgs args)
		{
			if (null == sender)
			{
				return;
			}
			string progressStr = sender as string;
			UpdateProgress(progressStr);
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
				this.labelStatus.Text = progress_str;
			}
		}

        /// <summary>
        /// 函数解析开始按钮Click
        /// </summary>
        private void btnStartFunction_Click(object sender, EventArgs e)
        {
            if (   0 == CSourceParseInfoList.Count
                || 0 == lvFileList.SelectedItems.Count
                || 0 == lvFileList.SelectedItems[0].SubItems.Count)
            {
                return;
            }
            // 取得要解析的函数所在的文件名
            string fullName = lvFileList.SelectedItems[0].SubItems[1].Text;
            // 取得要解析的函数名
            string functionName = string.Empty;
            foreach (ListViewItem item in lvFunctionList.Items)
            {
                if (item.Checked)
                {
                    functionName = item.Text;
					FILE_PARSE_INFO srcParseInfo;
					STATEMENT_NODE rootNode = C_FUNC_LOCATOR.FuncLocatorStart(fullName, functionName, CSourceParseInfoList, out srcParseInfo);
					// 函数语句分析: 分析入出力
					C_DEDUCER.DeducerStart(rootNode, srcParseInfo);

                }
            }
        }
	}
}
