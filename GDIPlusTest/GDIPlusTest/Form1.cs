using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.IO;

using SuperKeys;

using GDIPlusTest.ImageTools;
using GDIPlusTest.GameRobots.Robot1;
using GDIPlusTest.GameRobots.Robot2;
using GDIPlusTest.Tools.AutoKeys;

namespace GDIPlusTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpenSrc_Click(object sender, EventArgs e)
        {
            Bitmap subBitmap = openBitmapFile();
            if (null != subBitmap)
            {
                this.pbxSrcImg.Width = subBitmap.Width;
                this.pbxSrcImg.Height = subBitmap.Height;
                this.pbxSrcImg.Image = subBitmap;
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

        private void buttonOpenSub_Click(object sender, EventArgs e)
        {
            Bitmap subBitmap = openBitmapFile();
            if (null != subBitmap)
            {
                this.pbxSubImg.Width = subBitmap.Width;
                this.pbxSubImg.Height = subBitmap.Height;
                this.pbxSubImg.Image = subBitmap;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (    null == this.pbxSrcImg.Image
                ||  null == this.pbxSubImg.Image)
            {
                return;
            }
            List<Bitmap> subImgList = new List<Bitmap>();
            Bitmap subImg = (Bitmap)this.pbxSubImg.Image.Clone();
            subImgList.Add(subImg);
            Bitmap srcImg = (Bitmap)this.pbxSrcImg.Image.Clone();
            BitmapProcess bp = new BitmapProcess(srcImg, subImgList);
            List<FoundPosition> foundPosList = bp.searchSubBitmap(0);
            if (0 != foundPosList.Count)
            {
                Graphics g = Graphics.FromImage(srcImg);
                foreach (FoundPosition fp in foundPosList)
                {
                    g.DrawRectangle(new Pen(Color.Red, 2), fp.X, fp.Y, subImg.Width, subImg.Height);
                }
                this.pbxSrcImg.Image = srcImg;
                g.Dispose();
            }
        }

        private delegate void workerThreadBtnDelegate(Button btn, bool enable);

        private void setBtnEnable(Button btn, bool enable = true)
        {
            System.Diagnostics.Trace.Assert(btn != null);

            if (btn.InvokeRequired)
            {
                workerThreadBtnDelegate btnDlg = new workerThreadBtnDelegate(setBtnEnable);
                this.BeginInvoke(btnDlg, new object[] { btn, enable});
            }
            else
            {
                btn.Enabled = enable;
            }
        }

        private void buttonCapture_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 这个按钮用于试验网页相关的动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWebTest_Click(object sender, EventArgs e)
        {
            DateTime time_s = DateTime.Now;
            List<Bitmap> subImgList = new List<Bitmap>();
            string subImgPath = ".\\pics\\DengLuBtn.PNG";
            FileInfo fi = new FileInfo(subImgPath);
            if (!fi.Exists)
            {
                return;
            }
            Bitmap subImgDengLuBtn = new Bitmap(subImgPath);
            subImgList.Add(subImgDengLuBtn);

            Bitmap scrCap = new Bitmap(pbxSrcImg.Width, pbxSrcImg.Height);
            Graphics g = Graphics.FromImage(scrCap);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

//          for (int i = 0; i < 10; i++)
//          {
//          Point startPos = new Point(870, 290);
            Point startPos = new Point(600, 100);
            Point dstPos = new Point(0, 0);

            g.CopyFromScreen(startPos, dstPos, scrCap.Size);
            pbxSrcImg.Image = scrCap;
            BitmapProcess bp = new BitmapProcess(scrCap, subImgList);
            List<FoundPosition> foundPosList = bp.searchSubBitmap(0);
//          }
            g.Dispose();
            DateTime time_e = DateTime.Now;
            TimeSpan ts = time_e - time_s;
            if (0 != foundPosList.Count)
            {
                FoundPosition fp = foundPosList[0];
                Win32Api.MouseClick(startPos.X + fp.X + (subImgDengLuBtn.Width / 2), startPos.Y + fp.Y + (subImgDengLuBtn.Height / 2));
                MessageBox.Show("DengLuBtn is found!(*^__^*)! " + ts.TotalMilliseconds.ToString());
            }
        }

        private void btnRobot1_Click(object sender, EventArgs e)
        {
            FormRobot1 fgr1 = new FormRobot1();
            fgr1.Show();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (null == pbxSrcImg.Image)
            {
                return;
            }
            Bitmap dstBitmap = (Bitmap)pbxSrcImg.Image;
            Point mouse_pt = MousePosition;
            Point picbx_pt = pbxSrcImg.PointToScreen(new Point(pbxSrcImg.Left - pbxSrcImg.Bounds.Left,
                                                    pbxSrcImg.Top - pbxSrcImg.Bounds.Top));
            int pos_x = mouse_pt.X - picbx_pt.X;
            int pos_y = mouse_pt.Y - picbx_pt.Y;
            if (    (pos_x >= dstBitmap.Width)
                ||  (pos_y >= dstBitmap.Height))
            {
                return;
            }
            Color color = dstBitmap.GetPixel(pos_x, pos_y);
            label1.Text = " R:" + color.R.ToString().PadLeft(3, '0') + " G:" + color.G.ToString().PadLeft(3, '0') + " B:" + color.B.ToString().PadLeft(3, '0');
        }

        private void btnRobot2_Click(object sender, EventArgs e)
        {
            FormRobot2 fgr2 = new FormRobot2();
            fgr2.Show();
        }

        private void btnZoom_Click(object sender, EventArgs e)
        {
            Bitmap srcBitmap = openBitmapFile();
            if (null != srcBitmap)
            {
                if ((srcBitmap.Width > pbxSrcImg.Width)
                    || (srcBitmap.Height > pbxSrcImg.Height))
                {
                    MessageBox.Show("Opened image's size is bigger than the picturebox!");
                    srcBitmap.Dispose();
                    return;
                }
                int mag = 0;
                if (srcBitmap.Width > srcBitmap.Height)
                {
                    mag = pbxSrcImg.Width / srcBitmap.Width;
                }
                else
                {
                    mag = pbxSrcImg.Height / srcBitmap.Height;
                }
                Bitmap dstBitmap = new Bitmap(mag * srcBitmap.Width, mag * srcBitmap.Height);
                Graphics g = Graphics.FromImage(dstBitmap);
                int last_x = 0, last_y = 0;
                for (int i = 0; i < dstBitmap.Height; i++)
                {
                    for (int j = 0; j < dstBitmap.Width; j++)
                    {
                        int src_x = j / mag;
                        int src_y = i / mag;
                        Color color = srcBitmap.GetPixel(src_x, src_y);
                        dstBitmap.SetPixel(j, i, color);
                        // 以下画原图像素的网格线, 太密了的话看不清楚, 就干脆不画了
                        if (mag > 10)
                        {
                            Pen p = new Pen(new SolidBrush(Color.Gray));
                            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                            if (src_x != last_x)
                            {
                                last_x = src_x;
                                p.Width = 1;
                                g.DrawLine(p, j, 0, j, dstBitmap.Height);
                            }
                            if (src_y != last_y)
                            {
                                last_y = src_y;
                                p.Width = 2;
                                g.DrawLine(p, 0, i, dstBitmap.Width, i);
                            }
                        }
                    }
                }
                pbxSrcImg.Image = dstBitmap;
                srcBitmap.Dispose();
            }
        }

        private void btnDumpSubImg_Click(object sender, EventArgs e)
        {
            if (null == pbxSubImg.Image)
            {
                MessageBox.Show("pbxSubImg.Image is null!");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "txt files(*.txt)|*.txt";
            sfd.FilterIndex = 0;
            sfd.RestoreDirectory = true;
            if (DialogResult.OK == sfd.ShowDialog())
            {
                Bitmap subImg = (Bitmap)pbxSubImg.Image.Clone();
                BitmapProcess.dumpBitmap2File(subImg, 0, 0, subImg.Width, subImg.Height, sfd.FileName);
            }
        }

        private void btnAutoKeys_Click(object sender, EventArgs e)
        {
            FormAutoKeys akf = new FormAutoKeys();
            akf.Show();
        }
    }
}
