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

namespace GDIPlusTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Bitmap m_srcBitmap = null;
        private Bitmap m_subBitmap = null;
        private List<Bitmap> m_subImgList;

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            m_srcBitmap = openBitmapFile();
            if (null != m_srcBitmap)
            {
                this.pictureBox1.Width = m_srcBitmap.Width;
                this.pictureBox1.Height = m_srcBitmap.Height;
                this.pictureBox1.Image = m_srcBitmap;

                // m_srcBmPixColData = new BitmapPixelColorData(m_srcBitmap);
                // dumpBitmap2File(m_srcBitmap, 0, 0, m_srcBitmap.Width, m_srcBitmap.Height, "srcimgdump.txt");
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
            m_subBitmap = openBitmapFile();
            if (null != m_subBitmap)
            {
                this.pictureBox2.Width = m_subBitmap.Width;
                this.pictureBox2.Height = m_subBitmap.Height;
                this.pictureBox2.Image = m_subBitmap;

                // m_subBmPixColData = new BitmapPixelColorData(m_subBitmap);
                // dumpBitmap2File(m_subBitmap, 0, 0, m_subBitmap.Width, m_subBitmap.Height, "subimgdump.txt");
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            string exePath = Application.ExecutablePath.Remove(Application.ExecutablePath.LastIndexOf("\\"));
            if (!loadSubImgList(exePath + "\\pics\\subbitmaps\\"))
            {
                // load子图失败
                return;
            }

            Thread worker = new Thread(new ThreadStart(workerStart));
            worker.Start();
        }

        private delegate void workerFinishDelegate();
        private List<FoundPosition> m_totalResultList;

        private enum E_STATUS
        {
            E_STATUS_IDLE = 0,
            E_STATUS_FIND_START_POS,
            E_STATUS_READY
        };

        private E_STATUS m_status = E_STATUS.E_STATUS_IDLE;

        /// <summary>
        /// 工作线程开始函数
        /// </summary>
        private void workerStart()
        {
            m_totalResultList = new List<FoundPosition>();
            // 工作完成后的通知代理
            workerFinishDelegate workerfinishDelegate = workerFinish;

            DateTime startTime = DateTime.Now;
            BitmapProcess bp = new BitmapProcess(m_srcBitmap, m_subImgList);
            for (int i = 0; i < m_subImgList.Count; i++)
            {
                List<FoundPosition> foundPosList = bp.searchSubBitmap(i);
                m_totalResultList.AddRange(foundPosList);

//                if (E_STATUS.E_STATUS_READY == m_status)
                {
                    foreach (FoundPosition fPos in foundPosList)
                    {
                        // 在原图上画红框
                        Graphics g = this.pictureBox1.CreateGraphics();
                        g.DrawRectangle(new Pen(Color.Red, 2), fPos.X, fPos.Y, m_subImgList[i].Width, m_subImgList[i].Height);
                        //g.DrawLine(new Pen(Color.Red, 2), fPos.X, fPos.Y, fPos.X + m_subImgList[i].Width, fPos.Y + m_subImgList[i].Height);
                        //g.DrawLine(new Pen(Color.Red, 2), fPos.X + m_subImgList[i].Width, fPos.Y, fPos.X, fPos.Y + m_subImgList[i].Height);
                        // 画区块的编号
                        g.DrawString(fPos.subImgInfo.subIdx.ToString(), new Font("Verdana", 11), new SolidBrush(Color.Black), fPos.X, fPos.Y);
                    }
                }
            }

            // 计算工作用时
            DateTime finishTime = DateTime.Now;
            TimeSpan span = (finishTime - startTime);

            // 在这里对找到的区块数据(起始点, size)进行矩阵化(统一转换成矩阵的数学模型: 区块行列编号)
            double avWidth = LianLianKanLogic.Matrixing(ref m_totalResultList);
            PrintMatrix();

            workerfinishDelegate();
            //MessageBox.Show("Found: " + foundPosList.Count.ToString() + " in " + span.TotalSeconds.ToString() + " seconds!");
            //if (0 != foundPosList.Count)
            //{
            //    FoundPosition fp = foundPosList[0];
            //    Win32Api.mouseClick(fp.X + (m_subBitmap.Width / 2), fp.Y + (m_subBitmap.Height / 2));
            //}
        }

        private Point m_gameImgStartPos = new Point(0, 0);

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

        private void workerFinish()
        {
            if (E_STATUS.E_STATUS_FIND_START_POS == m_status)
            {
                if (1 == m_totalResultList.Count)
                {
                    m_gameImgStartPos.X = m_totalResultList[0].X;
                    m_gameImgStartPos.Y = m_totalResultList[0].Y;
                    m_status = E_STATUS.E_STATUS_READY;
                    m_srcBitmap.Dispose();
                    m_srcBitmap = null;
                    m_subImgList.Clear();

//                  buttonCapture_Click(null, null);
                    System.Diagnostics.Trace.WriteLine("find level.png successfully!");
                }
            }
            else if (E_STATUS.E_STATUS_READY == m_status)
            {
                if (0 != m_totalResultList.Count)
                {
                    MessageBox.Show("workerFinish! totalResultList.Count = " + m_totalResultList.Count.ToString());
                }
                m_status = E_STATUS.E_STATUS_IDLE;
            }
            else
            {
                m_status = E_STATUS.E_STATUS_IDLE;
            }
            setBtnEnable(buttonCapture, true);
        }

        private void buttonCapture_Click(object sender, EventArgs e)
        {
            if (E_STATUS.E_STATUS_IDLE == m_status)
            {
                pictureBox1.Image = null;
                findLevelPicPos();
            }
            else if (E_STATUS.E_STATUS_READY == m_status)
            {
                Bitmap scrCap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                Graphics g = Graphics.FromImage(scrCap);
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

                g.CopyFromScreen(m_gameImgStartPos.X, m_gameImgStartPos.Y, 0, 0, scrCap.Size);
                this.pictureBox1.Image = scrCap;
                g.Dispose();

                m_srcBitmap = new Bitmap(scrCap);
                string exePath = Application.ExecutablePath.Remove(Application.ExecutablePath.LastIndexOf("\\"));
                if (!loadSubImgList(exePath + "\\pics\\subbitmaps\\"))
                {
                    // load子图失败
                    return;
                }
                setBtnEnable(buttonCapture, false);

                Thread worker = new Thread(new ThreadStart(workerStart));
                worker.Start();
            }
        }

        private bool loadSubImgList(string fPath)
        {
            m_subImgList = new List<Bitmap>();
            DirectoryInfo di = new DirectoryInfo(fPath);
            if (!di.Exists)
            {
                MessageBox.Show(fPath + " is not exists!");
                return false;
            }
            foreach (FileInfo fi in di.GetFiles())
            {
                if ((".png" == fi.Extension.ToLower())
                    || (".bmp" == fi.Extension.ToLower()))
                {
                    Bitmap subImg = new Bitmap(fi.FullName);
                    m_subImgList.Add(subImg);
                }
            }
            return true;
        }

        // 找"level"图片在屏幕上出现的位置, 用以确定连连看游戏画面的位置
        private bool findLevelPicPos()
        {
            System.Diagnostics.Trace.WriteLine("enter findLevelPicPos()");

            // 全屏截图
            Bitmap scrCap = new Bitmap(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(scrCap);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;

            g.CopyFromScreen(0, 0, 0, 0, scrCap.Size);
            g.Dispose();

            m_srcBitmap = new Bitmap(scrCap);
            FileInfo fi = new FileInfo(".\\pics\\level.PNG");
            if (fi.Exists)
            {
                m_subImgList = new List<Bitmap>();
                Bitmap subImgLevel = new Bitmap(".\\pics\\level.PNG");
                m_subImgList.Add(subImgLevel);

                m_status = E_STATUS.E_STATUS_FIND_START_POS;
                Thread worker = new Thread(new ThreadStart(workerStart));
                worker.Start();
                System.Diagnostics.Trace.WriteLine("start worker thread to find level.png");
            }

            return false;
        }

        /// <summary>
        /// 打印矩阵化后的区块位置分布(测试用函数)
        /// </summary>
        void PrintMatrix()
        {
            int[,] arr = new int[12, 15];
            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    arr[i, j] = -1;
                }
            }

            foreach (FoundPosition fp in m_totalResultList)
            {
                if ((-1 != fp.Row) && (-1 != fp.Col))
                {
                    arr[fp.Row, fp.Col] = fp.subImgInfo.subIdx;
                }
            }

            StreamWriter sw = new StreamWriter("d:\\LianLianKanMatrix.txt");
            for (int i = 0; i < 12; i++)
            {
                string wtLine = "";
                for (int j = 0; j < 15; j++)
                {
                    wtLine += " " + string.Format("{0}", arr[i,j]).PadLeft(2, '0');
                }
                sw.WriteLine(wtLine);
            }
            sw.Close();
        }
    }
}
