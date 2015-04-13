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
                if (offsetX > 0)    // 背景图相对窗口向右移
                {
                    // 保证背景图左边缘不离开PictureBox左边缘
                    if (_mouseDownTopLeft.X - offsetX < 0)
                    {
                        offsetX = _mouseDownTopLeft.X;
                    }
                }
                else                // 背景图相对窗口左移
                {
                    if (_mouseDownTopLeft.X + pictureBox1.Width < _codeMap.Width)
                    {
                        if (_mouseDownTopLeft.X + pictureBox1.Width - offsetX > _codeMap.Width)
                        {
                            offsetX = _mouseDownTopLeft.X + pictureBox1.Width - _codeMap.Width;
                        }
                    }
                    else
                    {
                        offsetX = 0;
                    }
                }
                if (offsetY > 0)
                {
                    if (_mouseDownTopLeft.Y - offsetY < 0)
                    {
                        offsetY = _mouseDownTopLeft.Y;
                    }
                }
                else
                {
                    if (_mouseDownTopLeft.Y + pictureBox1.Height < _codeMap.Height)
                    {
                        if (_mouseDownTopLeft.Y + pictureBox1.Height - offsetY > _codeMap.Height)
                        {
                            offsetY = _mouseDownTopLeft.Y + pictureBox1.Height - _codeMap.Height;
                        }
                    }
                    else
                    {
                        offsetY = 0;
                    }
                }

                Point newTopLeft = new Point(_mouseDownTopLeft.X - offsetX, _mouseDownTopLeft.Y - offsetY);
                _topLeft = newTopLeft;

                Graphics g = pictureBox1.CreateGraphics();
                g.Clear(Color.Black);
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
            float oldSize = _codeMap.Width;
            _codeMap = bd.DrawMap(tbxRootFolder.Text, _fileInfoList, pictureBox1.Width, pictureBox1.Height, cbxScale.SelectedIndex + 1);
            float newSize = _codeMap.Width;
            float zoomRate = newSize / oldSize;
            _topLeft = new Point((int)(_topLeft.X * zoomRate), (int)(_topLeft.Y * zoomRate));

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
