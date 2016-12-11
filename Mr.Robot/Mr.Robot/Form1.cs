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

namespace Mr.Robot
{
	public partial class Form1 : Form
	{
	#region 全局字段
		List<string> _CSourceFilesList = new List<string>();
		List<string> _CHeaderFilesList = new List<string>();
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
			if (string.Empty != root_path)
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

			_CSourceFilesList = new List<string>();
			_CHeaderFilesList = new List<string>();
            // 遍历文件夹, 取得所有.c源文件和.h头文件
            IOProcess.GetAllCCodeFiles(path_name, ref _CSourceFilesList, ref _CHeaderFilesList);

            UpdateFileListViewCtrl(_CSourceFilesList);
		}

        void UpdateFileListViewCtrl(List<string> CSourceFileList)
        {
            lvFileList.Items.Clear();
            // 将得到的.c源文件加入UI文件列表
            foreach (string cfile in CSourceFileList)
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

		List<CodeParseInfo> _ccodeParseResultList = new List<CodeParseInfo>();

		public List<CodeParseInfo> CCodeParseResultList
		{
			get { return _ccodeParseResultList; }
			set { _ccodeParseResultList = value; }
		}

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
			foreach (CodeParseInfo srcResult in CCodeParseResultList)
			{
				string path;
				if (sourceFileName == IOProcess.GetFileName(srcResult.SourceParseInfo.FullName, out path))
				{
					foreach (FuncParseInfo functionInfo in srcResult.SourceParseInfo.FunDefineList)
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
        /// <param varName="sender"></param>
        /// <param varName="e"></param>
        private void cbxFile_CheckedChanged(object sender, EventArgs e)
        {
            CheckAllListViewItems(lvFileList, cbxFile.Checked);
        }

        /// <summary>
        /// 函数列表check all
        /// </summary>
        /// <param varName="sender"></param>
        /// <param varName="e"></param>
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
        /// <param varName="sender"></param>
        /// <param varName="e"></param>
        private void btnStartFile_Click(object sender, EventArgs e)
        {
            CCodeParseResultList.Clear();
            List<string> csfList = new List<string>();
            foreach (ListViewItem item in lvFileList.Items)
            {
                if (item.Checked)
                {
                    if (item.SubItems.Count >= 2)
                    {
                        string fpath = item.SubItems[1].Text;
                        csfList.Add(fpath);
                    }
                }
            }
            CCodeParseResultList
                = CCodeAnalyser.CFileListProcess(csfList, _CHeaderFilesList);

            lvFunctionList.Enabled = true;
            lvVariableList.Enabled = true;
            lvBranchList.Enabled = true;
            if (0 != lvFileList.SelectedItems.Count
                && lvFileList.SelectedItems[0].Checked)
            {
                labelStatus.Text = lvFileList.SelectedItems[0].Text;
                UpdateFunctionListViewCtrl(lvFileList.SelectedItems[0].Text);
            }
        }

        /// <summary>
        /// 函数解析开始按钮Click
        /// </summary>
        /// <param varName="sender"></param>
        /// <param varName="e"></param>
        private void btnStartFunction_Click(object sender, EventArgs e)
        {
            if (   0 == CCodeParseResultList.Count
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
                    CCodeAnalyser.FunctionAnalysis(fullName, functionName, CCodeParseResultList);
                }
            }
        }
	}
}
