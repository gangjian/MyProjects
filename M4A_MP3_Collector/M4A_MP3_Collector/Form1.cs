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

namespace M4A_MP3_Collector
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				this.tbxA.Text = fbd.SelectedPath;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				this.tbxB.Text = fbd.SelectedPath;
			}
		}

		bool CheckStart()
		{
			if (!string.IsNullOrEmpty(tbxA.Text)
				&& !string.IsNullOrEmpty(tbxB.Text)
				&& Directory.Exists(tbxA.Text)
				&& Directory.Exists(tbxB.Text))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private void btnCopyM4A_Click(object sender, EventArgs e)
		{
			if (!CheckStart())
			{
				return;
			}
			string destDir = tbxB.Text + "\\";
			try
			{
				int counter = 0;
				FileInfo[] M4aFiles = GetAllFiles(tbxA.Text, "*.m4a");
				foreach (var item in M4aFiles)
				{
					File.Copy(item.FullName, destDir + item.Name, true);
					counter += 1;
				}
				MessageBox.Show(counter.ToString() + " M4A files Copied!");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void btnReplaceM4A_Click(object sender, EventArgs e)
		{
			if (!CheckStart())
			{
				return;
			}
			try
			{
				FileInfo[] M4aFiles = GetAllFiles(tbxA.Text, "*.m4a");						// 从A下取得所有M4A文件
				FileInfo[] Mp3Files = GetAllFiles(tbxB.Text, "*.mp3");						// 从B下取得所有MP3文件
				List<string> Mp3NameList = new List<string>();
				foreach (var item in Mp3Files)
				{
					string nameWithoutExt = item.Name.Remove(item.Name.Length - item.Extension.Length);
					Mp3NameList.Add(nameWithoutExt);
				}
				int counter = 0;
				foreach (var item in M4aFiles)
				{
					// 如果在B下有同名的MP3文件
					string nameWithoutExt = item.Name.Remove(item.Name.Length - item.Extension.Length);
					if (Mp3NameList.Contains(nameWithoutExt))
					{
						// 把同名的Mp3文件拷到M4a文件的路径下
						string srcFile = this.tbxB.Text + "\\" + nameWithoutExt + ".mp3";
						File.Copy(srcFile, item.DirectoryName + "\\" + nameWithoutExt + ".mp3", true);
						// 删除原来的M4a文件
						File.Delete(item.FullName);
						counter += 1;
					}
				}
				MessageBox.Show("Success to Replace " + counter.ToString() + " M4A Files!");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		FileInfo[] GetAllFiles(string path, string search_pattern)
		{
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(path)
											&& !string.IsNullOrEmpty(search_pattern)
											&& Directory.Exists(path));
			DirectoryInfo di = new DirectoryInfo(path);
			FileInfo[] fileArr = di.GetFiles(search_pattern, SearchOption.AllDirectories);
			return fileArr;
		}
	}
}
