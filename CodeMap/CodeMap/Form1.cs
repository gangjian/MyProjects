using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CodeMap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设定工程根目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrjFdr_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "Select the root folder";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                tbxRootFolder.Text = dlg.SelectedPath;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (string.Empty == tbxRootFolder.Text)
            {
                return;
            }
            DirectoryInfo di = new DirectoryInfo(tbxRootFolder.Text);
            if (!di.Exists)
            {
                return;
            }
            FindFiles(tbxRootFolder.Text);
            lbStatus.Text = "Finish";
        }

        /// <summary>
        /// 遍历文件夹
        /// </summary>
        void FindFiles(string rootPath)
        {
            List<CFileInfo> CSourceInfoList = new List<CFileInfo>();
            DirectoryInfo di = new DirectoryInfo(rootPath);
            try
            {
                foreach (DirectoryInfo subDir in di.GetDirectories())
                {
                    FindFiles(subDir.FullName);
                }
                foreach (FileInfo fi in di.GetFiles())
                {
                    if (".c" == fi.Extension.ToLower())
                    {
                        CFileInfo cfi = CSourceProcess.CFileProcess(fi.FullName);
                        CSourceInfoList.Add(cfi);
                        lbStatus.Text = fi.FullName;
                    }
                    else if (".h" == fi.Extension.ToLower())
                    {
                        CSourceProcess.HeaderFileProcess(fi.FullName);
                        lbStatus.Text = fi.FullName;
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
