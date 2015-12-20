﻿using System;
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
			lvFileList.Columns.Add("", 330);
			lvFunctionList.Columns.Add("", 330);

			lvVariableList.Columns.Add("Name", 220);
			lvVariableList.Columns.Add("Type", 55);
			lvVariableList.Columns.Add("I/O", 55);

			lvBranchList.Columns.Add("", 330);

			// load根目录路径
			string root_path = IniFileProcess.IniReadValue("PATH_INFO", "root_path");
			if (string.Empty != root_path)
			{
				DirectoryInfo di = new DirectoryInfo(root_path);
				if (di.Exists)
				{
					tbxRootPath.Text = root_path;
					LoadCSourceFiles();
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
					LoadCSourceFiles();
					IniFileProcess.IniWriteValue("PATH_INFO", "root_path", tbxRootPath.Text);
				}
			}
		}

		private void LoadCSourceFiles()
		{
			// 检查文件夹路径的合法性
			if (string.Empty == tbxRootPath.Text)
			{
				return;
			}
			DirectoryInfo di = new DirectoryInfo(tbxRootPath.Text);
			if (!di.Exists)
			{
				return;
			}

			// 遍历文件夹, 取得所有.c源文件和.h头文件
			_CSourceFilesList = new List<string>();
			_CHeaderFilesList = new List<string>();

			IOProcess.GetFiles(tbxRootPath.Text, ref _CSourceFilesList, ref _CHeaderFilesList);

			lvFileList.Items.Clear();
			// 将得到的.c源文件加入UI文件列表
			foreach (string cfile in _CSourceFilesList)
			{
				// 分别取得文件名和路径名
				string path;
				string fname = IOProcess.GetFileName(cfile, out path);
				// 暂时只在文件列表里显示不包含完整路径的文件名
				ListViewItem new_item = new ListViewItem(fname);
				lvFileList.Items.Add(new_item);
			}
			lvFunctionList.Items.Clear();
			lvFunctionList.Enabled = false;
			lvVariableList.Items.Clear();
			lvVariableList.Enabled = false;
			lvBranchList.Items.Clear();
			lvBranchList.Enabled = false;
		}

		List<CCodeParseResult> _ccodeParseResultList = new List<CCodeParseResult>();

		public List<CCodeParseResult> CCodeParseResultList
		{
			get { return _ccodeParseResultList; }
			set { _ccodeParseResultList = value; }
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			CCodeParseResultList.Clear();
			CCodeParseResultList
				= CCodeAnalyser.CFileListProcess(_CSourceFilesList, _CHeaderFilesList);

			lvFunctionList.Enabled = true;
			lvVariableList.Enabled = true;
			lvBranchList.Enabled = true;
		}

		/// <summary>
		/// 源文件列表控件选中状态改变
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
			UpdateFunctionListView(lvFileList.SelectedItems[0].Text);
		}

		/// <summary>
		/// 函数列表控件选中状态改变
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lvFunctionList_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// 更新函数列表控件
		/// </summary>
		/// <param name="sourceFileName"></param>
		void UpdateFunctionListView(string sourceFileName)
		{
			lvFunctionList.Items.Clear();
			foreach (CCodeParseResult srcResult in CCodeParseResultList)
			{
				string path;
				if (sourceFileName == IOProcess.GetFileName(srcResult.SourceParseInfo.full_name, out path))
				{
					foreach (CFunctionInfo functionInfo in srcResult.SourceParseInfo.fun_define_list)
					{
						ListViewItem item = new ListViewItem(functionInfo.name + ": "
							+ functionInfo.body_start_pos.row_num.ToString() + ", "
							+ functionInfo.body_start_pos.col_num.ToString() + "; "
							+ functionInfo.body_end_pos.row_num.ToString() + ", "
							+ functionInfo.body_end_pos.col_num.ToString());
						lvFunctionList.Items.Add(item);
					}
					break;
				}
			}
		}

	}
}
