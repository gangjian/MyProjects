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
            List<string> cSourceFilesList = new List<string>();
            List<string> cHeaderFilesList = new List<string>();
            GetFiles(tbxRootFolder.Text, ref cSourceFilesList, ref cHeaderFilesList);
            List<CFileInfo> fileInfoList = CSourceProcess.CFileListProcess(cSourceFilesList, cHeaderFilesList);

            BitmapDisplay bd = new BitmapDisplay();
            Bitmap codeMap = bd.DrawMap(fileInfoList, pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = codeMap;

            lbStatus.Text = "Finish";
        }

        /// <summary>
        /// 遍历文件夹
        /// </summary>
        void GetFiles(string rootPath, ref List<string> cSourceFilesList, ref List<string> cHeaderFilesList)
        {
            DirectoryInfo di = new DirectoryInfo(rootPath);
            //try
            {
                foreach (DirectoryInfo subDir in di.GetDirectories())
                {
                    GetFiles(subDir.FullName, ref cSourceFilesList, ref cHeaderFilesList);
                }
                foreach (FileInfo fi in di.GetFiles())
                {
                    if (".c" == fi.Extension.ToLower())
                    {
                        cSourceFilesList.Add(fi.FullName);
                    }
                    else if (".h" == fi.Extension.ToLower())
                    {
                        cHeaderFilesList.Add(fi.FullName);
                    }
                    else
                    {
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("pictureBox1_Click");
        }

    }
}
