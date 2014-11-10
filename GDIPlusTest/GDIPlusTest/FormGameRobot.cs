using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GDIPlusTest
{
    public partial class FormGameRobot : Form
    {
        public FormGameRobot()
        {
            InitializeComponent();
            LogAppend("Hi, what's up?");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Threading.Thread thWorker = new System.Threading.Thread(WorkerStart);
            thWorker.Start();
//            thWorker.Join();
//            MessageBox.Show("worker finish!");
        }

        /// <summary>
        /// 工作子线程的入口函数
        /// </summary>
        void WorkerStart()
        {
            // 开始在屏幕上找"level"的位置
            LogAppend("开始在屏幕上找level的位置...");
            Point pt = FindLevel();
            if ((-1 != pt.X) && (-1 != pt.Y))
            {
                LogAppend("找到了!");
                LogAppend("X = " + pt.X.ToString() + "\r\nY = " + pt.Y.ToString());
            }
            else
            {
                LogAppend("啥也没找到!");
                return;
            }

            LogAppend("开始截取连连看的游戏画面");
            Bitmap game_img = CaptureGameImage(pt);
            if (null != game_img)
            {
                LogAppend("OK, 截图成功了!(*^__^*) !");
            }
            else
            {
                LogAppend("TMD, 截图失败了...(┬＿┬)");
                return;
            }

            LogAppend("开始对游戏画面进行区块识别...");
            List<FoundPosition> foundBlocksList = GameBlocksIdentify(game_img);
            if (0 != foundBlocksList.Count)
            {
                LogAppend("还行, 区块识别也OK了...");
            }
            else
            {
                LogAppend("不妙啊, 区块识别个数为0..., 什么原因?");
                return;
            }

            // 根据识别后的区块位置列表, 进行矩阵化
            LianLianKanLogic.Matrixing(ref foundBlocksList);
            LogAppend("区块矩阵化也完成了...");
            LogAppend("接着检查一下矩阵化的结果...");
            for (int i = 0; i < foundBlocksList.Count; i++)
            {
                FoundPosition fp = foundBlocksList[i];
                if ((-1 == fp.Row) || (-1 == fp.Col))
                {
                    LogAppend("区块矩阵化第: " + i.ToString() + "个结果不对, 行序号或者列序号为-1!");
                    return;
                }
            }
            LogAppend("走到这儿说明区块矩阵化的结果没问题!");
            // 根据矩阵化的结果, 开始进行区块消除动作
            LogAppend("下面该开始进行区块消除了...Let's do it!");
        }

        /// <summary>
        /// 找"level"是否出现在屏幕上
        /// </summary>
        Point FindLevel()
        {
            Point retPt = new Point(-1, -1);    // 返回找到的位置, 初始化值-1表示没找到
            List<Bitmap> subImgList = new List<Bitmap>();
            string subImgPath = ".\\pics\\level.PNG";
            FileInfo fi = new FileInfo(subImgPath);
            if (!fi.Exists)
            {
                return retPt;
            }
            Bitmap subImgLevel = new Bitmap(subImgPath);
            subImgList.Add(subImgLevel);

            // 全屏截图, 找"Level", 现在只对应"主屏幕(PrimaryScreen)", 以后要在所有屏幕(AllScreens)范围内找
            Rectangle scrRect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            Bitmap scrCapture = new Bitmap(scrRect.Width, scrRect.Height);
            Graphics g = Graphics.FromImage(scrCapture);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.CopyFromScreen(0, 0, 0, 0, scrRect.Size);
            g.Dispose();

            BitmapProcess bp = new BitmapProcess(scrCapture, subImgList);
            List<FoundPosition> foundPosList = bp.searchSubBitmap(0);
            if (0 != foundPosList.Count)
            {
                // 找到了
                retPt.X = foundPosList[0].X;
                retPt.Y = foundPosList[0].Y;
            }
            else
            {
            }
            return retPt;
        }

        delegate void LogOutDlg(string txt);

        /// <summary>
        /// Log输出
        /// </summary>
        /// <param name="logStr"></param>
        void LogAppend(string logStr)
        {
            if (tbxLogOutPut.InvokeRequired)
            {
                LogOutDlg dlg = new LogOutDlg(LogAppend);
                tbxLogOutPut.BeginInvoke(dlg, new object[] { logStr });
            }
            else
            {
                tbxLogOutPut.Text += (logStr + "\r\n");
            }
        }

        /// <summary>
        /// 截取连连看游戏画面
        /// </summary>
        Bitmap CaptureGameImage(Point startPt)
        {
            Size GameImgSize = new System.Drawing.Size(640, 520);
            Bitmap scrCapture = new Bitmap(GameImgSize.Width, GameImgSize.Height);
            Graphics g = Graphics.FromImage(scrCapture);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.CopyFromScreen(startPt.X, startPt.Y, 0, 0, GameImgSize);
            g.Dispose();

            return scrCapture;
        }

        List<FoundPosition> GameBlocksIdentify(Bitmap gamePic)
        {
            string exePath = Application.ExecutablePath.Remove(Application.ExecutablePath.LastIndexOf("\\"));
            List<Bitmap> subImgList = loadSubImgList(exePath + "\\pics\\subbitmaps\\");
            List<FoundPosition> totalFoundList = new List<FoundPosition>();
            BitmapProcess bp = new BitmapProcess(gamePic, subImgList);
            for (int i = 0; i < subImgList.Count; i++)
            {
                LogAppend("区块识别进度: " + i.ToString() + "/" + subImgList.Count.ToString());
                List<FoundPosition> foundPosList = bp.searchSubBitmap(i);
                totalFoundList.AddRange(foundPosList);
                LogAppend("识别的区块个数: " + totalFoundList.Count.ToString());
            }

            return totalFoundList;
        }

        private List<Bitmap> loadSubImgList(string fPath)
        {
            List<Bitmap> subImgList = new List<Bitmap>();
            DirectoryInfo di = new DirectoryInfo(fPath);
            if (!di.Exists)
            {
                MessageBox.Show("loadSubImgList failed!\r\n" + fPath + " is not exists!");
            }
            else
            {
                foreach (FileInfo fi in di.GetFiles())
                {
                    if ((".png" == fi.Extension.ToLower())
                        || (".bmp" == fi.Extension.ToLower()))
                    {
                        Bitmap subImg = new Bitmap(fi.FullName);
                        subImgList.Add(subImg);
                    }
                }
            }
            return subImgList;
        }

        /// <summary>
        /// 连连看区块消除处理动作(以后应该整理移到LianLianKanLogic类里去)
        /// </summary>
        void LianLianKanBlocsElimination(List<FoundPosition> foundList)
        {
        }
    }
}
