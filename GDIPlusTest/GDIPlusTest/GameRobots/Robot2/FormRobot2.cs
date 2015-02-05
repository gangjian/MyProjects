using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

using SuperKeys;

namespace GDIPlusTest.GameRobots.Robot2
{
    public partial class FormRobot2 : Form
    {
        public FormRobot2()
        {
            InitializeComponent();
        }

        WinIOApi _winio_api;

        private void FormRobot2_Load(object sender, EventArgs e)
        {
            _winio_api = new WinIOApi();
        }

        private void FormRobot2_FormClosing(object sender, FormClosingEventArgs e)
        {
            _winio_api.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread.Sleep(3000);
            for (int i = 0; i < 100; i++)
            {
                this._winio_api.KeyPress(WinIoSys.Key.VK_CONTROL, 100);
                this._winio_api.KeyPress(WinIoSys.Key.VK_SHIFT, 100);
            }
        }

        private void btnLoadPic_Click(object sender, EventArgs e)
        {
            Bitmap subBitmap = openBitmapFile();
            if (null != subBitmap)
            {
                this.pictureBox1.Width = subBitmap.Width;
                this.pictureBox1.Height = subBitmap.Height;
                this.pictureBox1.Image = subBitmap;
            }
        }

        private Bitmap openBitmapFile()
        {
            Bitmap bitMap = null;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图像文件(JPeg, Gif, Bmp, etc.)|*.jpg;*.jpeg;*.gif;*.bmp;*.tif; *.tiff; *.png| JPeg 图像文件(*.jpg;*.jpeg)|*.jpg;*.jpeg |GIF 图像文件(*.gif)|*.gif |BMP图像文件(*.bmp)|*.bmp|Tiff图像文件(*.tif;*.tiff)|*.tif;*.tiff|Png图像文件(*.png)| *.png |所有文件(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                bitMap = new Bitmap(openFileDialog.FileName);
            }

            return bitMap;
        }

        private void btnFindRocks_Click(object sender, EventArgs e)
        {
            if (null == pictureBox1.Image)
            {
                return;
            }
            Bitmap bitMap = (Bitmap)pictureBox1.Image.Clone();
            ImageIdentify imgID = new ImageIdentify(bitMap);
            DateTime startTime = DateTime.Now;
            List<Rectangle> foundList = ImageIdentify.FindRocks();
            DateTime endTime = DateTime.Now;
            TimeSpan duringTime = endTime - startTime;
            Graphics g = Graphics.FromImage(bitMap);
            foreach(Rectangle rect in foundList)
            {
                g.DrawRectangle(new Pen(new SolidBrush(Color.Red), 2), rect);
            }
            pictureBox1.Image = bitMap;
            MessageBox.Show(duringTime.TotalMilliseconds.ToString());
        }

        private void btnFindTrees_Click(object sender, EventArgs e)
        {
            if (null == pictureBox1.Image)
            {
                return;
            }
            Bitmap bitMap = (Bitmap)pictureBox1.Image.Clone();
            ImageIdentify imgID = new ImageIdentify(bitMap);
            DateTime startTime = DateTime.Now;
            List<Rectangle> foundList = ImageIdentify.FindTrees();
            DateTime endTime = DateTime.Now;
            TimeSpan duringTime = endTime - startTime;
            Graphics g = Graphics.FromImage(bitMap);
            foreach (Rectangle rect in foundList)
            {
                g.DrawRectangle(new Pen(new SolidBrush(Color.Red), 2), rect);
            }
            pictureBox1.Image = bitMap;
            MessageBox.Show(duringTime.TotalMilliseconds.ToString());
        }

        private void btnFindBricks_Click(object sender, EventArgs e)
        {
            if (null == pictureBox1.Image)
            {
                return;
            }
            Bitmap bitMap = (Bitmap)pictureBox1.Image.Clone();
            ImageIdentify imgID = new ImageIdentify(bitMap);
            DateTime startTime = DateTime.Now;
            List<Rectangle> foundList = ImageIdentify.FindBricks();
            DateTime endTime = DateTime.Now;
            TimeSpan duringTime = endTime - startTime;
            Graphics g = Graphics.FromImage(bitMap);
            foreach (Rectangle rect in foundList)
            {
                g.DrawRectangle(new Pen(new SolidBrush(Color.Red), 2), rect);
            }
            pictureBox1.Image = bitMap;
            MessageBox.Show(duringTime.TotalMilliseconds.ToString());
        }
    }
}
