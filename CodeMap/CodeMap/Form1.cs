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
            for (int i = 1; i <= 15; i++)
            {
                cbxScale.Items.Add(i);
            }
            cbxScale.SelectedIndex = 4;
        }

        Bitmap _codeMap = null;

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

        Point _topLeft = new Point(0, 0);
        List<CFileParseInfo> _fileInfoList = null;

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
            _topLeft = new Point(0, 0);
            List<string> cSourceFilesList = new List<string>();
            List<string> cHeaderFilesList = new List<string>();
            GetFiles(tbxRootFolder.Text, ref cSourceFilesList, ref cHeaderFilesList);
            _fileInfoList = CSourceProcess.CFileListProcess(cSourceFilesList, cHeaderFilesList);

            BitmapDisplay bd = new BitmapDisplay();
            _codeMap = bd.DrawMap(tbxRootFolder.Text, _fileInfoList, pictureBox1.Width, pictureBox1.Height, cbxScale.SelectedIndex + 1);

            Bitmap showPic = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(showPic);
            g.DrawImage(_codeMap, _topLeft);
            pictureBox1.Image = showPic;

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
            //MessageBox.Show("pictureBox1_Click");
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            //MessageBox.Show("pictureBox1_DoubleClick");
        }

        Point _mouseDownPoint = Point.Empty;        // 鼠标按下时坐标
        Point _mouseDownTopLeft = Point.Empty;      // 保存鼠标按下时左上角的坐标

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDownPoint = new Point(e.X, e.Y);
            _mouseDownTopLeft = _topLeft;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (Point.Empty != _mouseDownPoint)
            {
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (Point.Empty != _mouseDownPoint)
            {
                int offsetX = e.X - _mouseDownPoint.X;
                int offsetY = e.Y - _mouseDownPoint.Y;
                _mouseDownPoint = Point.Empty;
                if (   Math.Abs(offsetX) < 3
                    && Math.Abs(offsetY) < 3)
                {
                    // 鼠标移动距离太短的话, 就不移动画面了, 防止点击时移动画面重画导致闪烁
                    return;
                }
                // 边界检查
                Point newTopLeft = new Point(_mouseDownTopLeft.X - offsetX, _mouseDownTopLeft.Y - offsetY);
                if (newTopLeft.X < 0)
                {
                    // 左侧边界
                    newTopLeft.X = 0;
                }
                else if (newTopLeft.X + pictureBox1.Width > _codeMap.Width)
                {
                    if (_topLeft.X + pictureBox1.Width < _codeMap.Width)
                    {
                        newTopLeft.X = _codeMap.Width - pictureBox1.Width;
                    }
                    else
                    {
                        newTopLeft.X = _topLeft.X;
                    }
                }
                if (newTopLeft.Y < 0)
                {
                    newTopLeft.Y = 0;
                }
                else if (newTopLeft.Y + pictureBox1.Height > _codeMap.Height)
                {
                    if (_topLeft.Y + pictureBox1.Height < _codeMap.Height)
                    {
                        newTopLeft.Y = _codeMap.Height - pictureBox1.Height;
                    }
                    else
                    {
                        newTopLeft.Y = _topLeft.Y;
                    }
                }
                _topLeft = newTopLeft;

                Graphics g = pictureBox1.CreateGraphics();
                g.Clear(Color.DarkBlue);
                Rectangle destRect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
                g.DrawImage(_codeMap, destRect, _topLeft.X, _topLeft.Y, pictureBox1.Width, pictureBox1.Height, GraphicsUnit.Pixel);
            }
        }

        private void cbxScale_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (null == _fileInfoList)
            {
                return;
            }
            BitmapDisplay bd = new BitmapDisplay();
            _codeMap = bd.DrawMap(tbxRootFolder.Text, _fileInfoList, pictureBox1.Width, pictureBox1.Height, cbxScale.SelectedIndex + 1);

            Bitmap showPic = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(showPic);
            Rectangle destRect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
            g.DrawImage(_codeMap, destRect, _topLeft.X, _topLeft.Y, pictureBox1.Width, pictureBox1.Height, GraphicsUnit.Pixel);
            pictureBox1.Image = showPic;

            lbStatus.Text = "Finish";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (cbxScale.SelectedIndex == cbxScale.Items.Count - 1)
                {
                    return;
                }
                cbxScale.SelectedIndex += 1;
            }
            else
            {
                if (0 == cbxScale.SelectedIndex)
                {
                    return;
                }
                cbxScale.SelectedIndex -= 1;
            }
        }

    }
}
