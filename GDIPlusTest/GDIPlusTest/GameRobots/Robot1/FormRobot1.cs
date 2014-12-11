using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using GDIPlusTest.ImageTools;

namespace GDIPlusTest.GameRobots.Robot1
{
    public partial class FormRobot1 : Form
    {
        public FormRobot1()
        {
            InitializeComponent();
            LogAppend("Hi, what's up?");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tbxLogOutPut.Clear();
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
            int leftBlocsNum = 0;
            int level = 0;
            do
            {
                // 开始在屏幕上找"level"的位置
                LogAppend("开始在屏幕上找level的位置...");
                Point pt = FindLevelPosition(out level);
                if ((-1 != pt.X) && (-1 != pt.Y))
                {
                    LogAppend("找到了!" + " 现在是level " + level.ToString());
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

                leftBlocsNum = LianLianKanBlocksElimination(pt, foundBlocksList);
                if (0 == leftBlocsNum)
                {
                    LogAppend("level " + level.ToString() + " 都消完了...");

                    if ((level >= 1) && (level <= 3))
                    {
                        LogAppend("接下来等一会儿进入level " + (level + 1).ToString());
                        System.Threading.Thread.Sleep(4500);
                    }
                }
                else
                {
                    LogAppend("还剩" + leftBlocsNum.ToString() + "个没消完, 等待一会儿后重新扫描!");
                    System.Threading.Thread.Sleep(1000);
                }
            } while (0 != leftBlocsNum || ((level >= 1) && (level <= 3)));
        }

        /// <summary>
        /// 找"level"是否出现在屏幕上
        /// </summary>
        Point FindLevelPosition(out int level)
        {
            level = 0;
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
            foreach (Screen scr in System.Windows.Forms.Screen.AllScreens)
            {
                Rectangle scrRect = scr.Bounds;
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

                    // 接下来确定level的数值
                    subImgList = new List<Bitmap>();
                    for (int i = 1; i <= 4; i++)
                    {
                        subImgPath = ".\\pics\\level" + i.ToString() + ".PNG";
                        fi = new FileInfo(subImgPath);
                        if (!fi.Exists)
                        {
                            break;
                        }
                        subImgLevel = new Bitmap(subImgPath);
                        subImgList.Add(subImgLevel);
                    }
                    bp = new BitmapProcess(scrCapture, subImgList);
                    for (int i = 1; i <= 4; i++)
                    {
                        foundPosList = bp.searchSubBitmap(i - 1);
                        if (0 != foundPosList.Count)
                        {
                            // 确定是level几
                            level = i;
                            break;
                        }
                    }

                    break;
                }
                else
                {
                }
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
                tbxLogOutPut.AppendText(logStr + "\r\n");
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
        int LianLianKanBlocksElimination(Point startPt, List<FoundPosition> foundList)
        {
            // 首先要将区块列表里的行列序号整理成二维数组(第三维多出的位置用来保存对应foundList的索引)
            int[,,] arr = new int[LianLianKanLogic.ROW_NUM, LianLianKanLogic.COL_NUM, 2];
            // 现将数组全部初始化成-1
            for (int i = 0; i < LianLianKanLogic.ROW_NUM; i++)
            {
                for (int j = 0; j < LianLianKanLogic.COL_NUM; j++)
                {
                    arr[i, j, 0] = -1;
                    arr[i, j, 1] = -1;
                }
            }
            // 将区块列表里对应数组行列序号的区块序号填到数组对应位置里去
            int idx = 0;
            foreach (FoundPosition fp in foundList)
            {
                System.Diagnostics.Trace.Assert((-1 != fp.Row) && (-1 != fp.Col));
                arr[fp.Row, fp.Col, 0] = fp.subImgInfo.subIdx;
                arr[fp.Row, fp.Col, 1] = idx;
                idx++;
            }

            // 将整理好的数组内容打印到log里
            LogAppend("打印矩阵化后的数组内容:");
            LogOutMatrix(arr);

            // 根据整理好的数组计算得出消除区块的顺序列表
            // 深度优先遍历算法
//          List<int[]> elmSeq = GetEliminateSeqDeep(arr);
            // 探索算法
            int leftBlocsNum = 0;
            List<int[]> elmSeq = GetEliminateSeqExplore(arr, out leftBlocsNum);
            if (null == elmSeq)
            {
                return 0;
            }

            // 根据消去序列表,按顺序消除各个区块
            for (int i = 0; i < elmSeq.Count; i++)
            {
                // 分别取得将要消去的两个区块的索引值
                int[] idx_arr = elmSeq[i];
                System.Diagnostics.Trace.Assert(4 == idx_arr.Length);
                int r1, r2, c1, c2;
                r1 = idx_arr[0];
                c1 = idx_arr[1];
                r2 = idx_arr[2];
                c2 = idx_arr[3];
                int idx1 = arr[r1, c1, 1];
                int idx2 = arr[r2, c2, 1];
                System.Diagnostics.Trace.Assert((idx1 >= 0 && idx1 < foundList.Count) && (idx2 >= 0 && idx2 < foundList.Count));
                // 根据索引值取得两个区块的屏幕位置
                FoundPosition fp1 = foundList[idx1];
                FoundPosition fp2 = foundList[idx2];

                // 为防止点不上, 保险起见, 用双击点选区块
                string logStr = "{" + r1.ToString().PadLeft(2, '0') + ", " + c1.ToString().PadLeft(2, '0') + "<---->"
                                    + r2.ToString().PadLeft(2, '0') + ", " + c2.ToString().PadLeft(2, '0') + "}";
                LogAppend(logStr);
                Win32Api.MouseDoubleClick(startPt.X + fp1.X + (fp1.subImgInfo.subWidth / 2), startPt.Y + fp1.Y + (fp1.subImgInfo.subHeight / 2));
                // 这里应该起个timer停几秒, 暂时姑且用Sleep代替一下看能不能凑合着用
                if (i != elmSeq.Count - 1)
                {
                    System.Threading.Thread.Sleep(500);
                    Win32Api.MouseDoubleClick(startPt.X + fp2.X + (fp2.subImgInfo.subWidth / 2), startPt.Y + fp2.Y + (fp2.subImgInfo.subHeight / 2));
                }
                else
                {
                    System.Threading.Thread.Sleep(800);
                    Win32Api.MouseClick(startPt.X + fp2.X + (fp2.subImgInfo.subWidth / 2), startPt.Y + fp2.Y + (fp2.subImgInfo.subHeight / 2));
                }

                // 每次消去后, 这里还应该起个timer等动画消失才能进行下一次消去动作
                System.Threading.Thread.Sleep(1150);
            }
            // 将鼠标移开, 防止未全部消去时, 重新布局后, 鼠标悬停于某一区块上, 导致该区块颜色发生变化, 影响下一轮识别
            Win32Api.MouseMove(0, 0);

            return leftBlocsNum;
        }

        /// <summary>
        /// 取得消去序列(深度优先遍历算法)
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        List<int[]> GetEliminateSeqDeep(int[,,] arr)
        {
            List<int[]> retSeq = null;
            int[,] arr2 = null;
            int routeCnt = 0;
            int leftBlocksCnt = 0;
            // 先根据传入三维数组的前两维做成一个只包含区块序号的二维数组
            arr2 = new int[LianLianKanLogic.ROW_NUM, LianLianKanLogic.COL_NUM];
            for (int i = 0; i < LianLianKanLogic.ROW_NUM; i++)
            {
                for (int j = 0; j < LianLianKanLogic.COL_NUM; j++)
                {
                    arr2[i, j] = arr[i, j, 0];
                }
            }
            BlocksLayoutSet[] blsArray = LianLianKanLogic.MatrixArrangement(arr2);
            do
            {
                if (null == retSeq)
                {
                    // 取得第一条消去路径
                    retSeq = LianLianKanLogic.GetFirstEliminateSeq(ref arr2, ref blsArray);
                }
                else
                {
                    retSeq = LianLianKanLogic.GetNextEliminateSeq(ref arr2, ref blsArray, retSeq);
                    if (null == retSeq)
                    {
                        MessageBox.Show("所有可能的路径都遍历完了!");
                        break;
                    }
                }
                leftBlocksCnt = LianLianKanLogic.GetLeftBlocksNum(arr2);
                routeCnt += 1;
                LogAppend("取得第 " + routeCnt.ToString() + " 条消去路线, 剩下 " + leftBlocksCnt.ToString() + " 个区块未消去!");
                // 如果没能消完所有的区块
                if (routeCnt < 30)
                {
                    PrintEliminatePath(retSeq);
                }
                else
                {
                    break;
                }
            } while (0 != leftBlocksCnt);

            return retSeq;
        }

        /// <summary>
        /// 在log里打印矩阵化后的数组内容
        /// </summary>
        void LogOutMatrix(int[, ,] arr)
        {
            for (int i = 0; i < LianLianKanLogic.ROW_NUM; i++)
            {
                string str = "";
                for (int j = 0; j < LianLianKanLogic.COL_NUM; j++)
                {
                    if ("" != str)
                    {
                        str += ",";
                    }
                    if (-1 == arr[i, j, 0])
                    {
                        str += " X";
                    }
                    else
                    {
                        str += arr[i, j, 0].ToString().PadLeft(2, ' ');
                    }
                }
                LogAppend(str);
            }
        }

        void PrintEliminatePath(List<int[]> path)
        {
            string str = "\r\n";
            foreach (int[] dp in path)
            {
                str += ("{"     + dp[0].ToString().PadLeft(2, '0') + ","
                                + dp[1].ToString().PadLeft(2, '0') + ","
                                + dp[2].ToString().PadLeft(2, '0') + ","
                                + dp[3].ToString().PadLeft(2, '0') + "}");
            }
            LogAppend(str + "\r\n");
        }

        /// <summary>
        /// 取得消去路径(探索算法)
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        List<int[]> GetEliminateSeqExplore(int[, ,] arr, out int leftBlocsNum)
        {
            List<int[]> retSeq = new List<int[]>();
            int[,] arr2 = null;
            // 先根据传入三维数组的前两维做成一个只包含区块序号的二维数组
            arr2 = new int[LianLianKanLogic.ROW_NUM, LianLianKanLogic.COL_NUM];
            for (int i = 0; i < LianLianKanLogic.ROW_NUM; i++)
            {
                for (int j = 0; j < LianLianKanLogic.COL_NUM; j++)
                {
                    arr2[i, j] = arr[i, j, 0];
                }
            }
            // 取得区块管理表
            BlocksLayoutSet[] blsArray = LianLianKanLogic.MatrixArrangement(arr2);

            do
            {
                int[] nextPairs = LianLianKanLogic.FindOptimalEliminatePairs(arr2, blsArray);
                if (null == nextPairs)
                {
                    break;
                }
                else
                {
                    LianLianKanLogic.DeleteEliminatableBlocks(nextPairs[0], nextPairs[1], nextPairs[2], nextPairs[3],
                                                                ref arr2, ref blsArray);
                    retSeq.Add(nextPairs);
                }
            } while (true);
            leftBlocsNum = LianLianKanLogic.GetLeftBlocksNum(arr2);

            return retSeq;
        }
    }
}
