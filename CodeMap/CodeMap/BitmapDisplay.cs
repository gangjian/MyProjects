using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace CodeMap
{
    class BitmapDisplay
    {
        const int MIN_DISPLAY_FONT_SIZE = 1;                // 显示文字的最小字号
        const int MAX_DISPLAY_FONT_SIZE = 15;               // 显示文字的最大字号
        const string DISPLAY_FONT_NAME = "Verdana";         // 字体名称
        const string SPACE_STRING = "XX";                   // 显示区块间的空位(间距)为2个字符宽度

        SolidBrush textBrush = new SolidBrush(Color.Red);
        SolidBrush titleBrush = new SolidBrush(Color.LightBlue);
        Pen fileFramePen = new Pen(Color.DarkCyan, 2);
        Pen folderFramePen = new Pen(Color.DarkGreen, 2);

        public BitmapDisplay()
        {
        }

        /// <summary>
        /// 根据源文件的解析结果, 描画代码图
        /// </summary>
        /// <param name="fileParseInfoList"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Bitmap DrawMap(string root, List<CFileParseInfo> fileParseInfoList, int width, int height, int scale)
        {
            Bitmap bitMap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitMap);
            Point startPoint = new Point(3, 3);

            // 先计算每个源文件各个字体大小对应的SizeF
            List<DisplayScaleInfo> fileDisplayScaleList = MeasureFileDisplayScale(fileParseInfoList, g, scale);
            // 再计算各个文件夹各个字体大小对应的SizeF
            List<DisplayScaleInfo> folderDisplayScaleList = new List<DisplayScaleInfo>();
            SizeF rootSize = MeasureFolderDisplayScale(root, fileDisplayScaleList, ref folderDisplayScaleList, g, scale);
            bitMap = new Bitmap((int)rootSize.Width, (int)rootSize.Height);
            g = Graphics.FromImage(bitMap);
            g.Clear(Color.Black);

            // 递归描画各个文件夹
            DrawFolder(startPoint, root, fileDisplayScaleList, folderDisplayScaleList, fileParseInfoList, g, scale);

            return bitMap;
        }

        /// <summary>
        /// 计算所有源文件的各个字体大小对应的图形化显示尺寸
        /// </summary>
        List<DisplayScaleInfo> MeasureFileDisplayScale(List<CFileParseInfo> fileParseInfoList, Graphics g, int scale)
        {
            List<DisplayScaleInfo> fileDisplayScaleList = new List<DisplayScaleInfo>();

            for (int i = 0; i < fileParseInfoList.Count; i++)
            {
                CFileParseInfo curFileInfo = fileParseInfoList[i];

                // 分别测试每种字体大小对应的显示尺寸
                DisplayScale fds = new DisplayScale();
                int fontSize = scale + 1;
                Font textFont = new Font(DISPLAY_FONT_NAME, fontSize);
                float maxWidth = 0;
                float totalHeight = 0;
                // 文件名显示size
                string fileName = GetFileName(curFileInfo.full_name);
                SizeF nameSize = g.MeasureString(fileName, textFont);
                maxWidth = nameSize.Width;
                totalHeight += (float)(nameSize.Height * 1.5);

                // 暂时只考虑在表示文件的矩形框里显示文件内有定义的函数名的情况
                foreach (CFunctionInfo curFunc in curFileInfo.fun_define_list)
                {
                    nameSize = g.MeasureString(curFunc.name, textFont);
                    if (nameSize.Width > maxWidth)
                    {
                        maxWidth = nameSize.Width;
                    }
                    totalHeight += nameSize.Height;
                }
                SizeF fileDisplaySize = new SizeF((float)(maxWidth * 1.1), totalHeight);
                fds.displaySize = fileDisplaySize;

                DisplayScaleInfo fdi = new DisplayScaleInfo(curFileInfo.full_name, fds);
                fileDisplayScaleList.Add(fdi);
            }

            return fileDisplayScaleList;
        }

        /// <summary>
        /// 计算文件夹的显示尺寸
        /// </summary>
        SizeF MeasureFolderDisplayScale(string path, List<DisplayScaleInfo> fileDisplayScaleList,
                                       ref List<DisplayScaleInfo> folderDisplayScaleList, Graphics g, int scale)
        {
            Font textFont = new Font(DISPLAY_FONT_NAME, scale + 1);
            // 计算间距宽度
            float spaceDistance = g.MeasureString(SPACE_STRING, textFont).Width;

            // 分别取得其下所有文件和子文件夹的显示尺寸
            List<string> subDirsList = GetSubDirectoriesFromDsList(path, fileDisplayScaleList);
            List<string> subFilesList = GetFilesFromDsList(path, fileDisplayScaleList);
            // 各个矩形块的SizeF列表, 用以进行布局排列
            List<SizeF> rectSizeList = new List<SizeF>();
            foreach (string subDir in subDirsList)
            {
                MeasureFolderDisplayScale(subDir, fileDisplayScaleList, ref folderDisplayScaleList, g, scale);
                SizeF subFolderSize = GetFolderDisplaySizeF(subDir, folderDisplayScaleList);
                rectSizeList.Add(subFolderSize);
            }
            foreach (string fname in subFilesList)
            {
                SizeF srcFileSize = GetFileDisplaySizeF(fname, fileDisplayScaleList);
                rectSizeList.Add(srcFileSize);
            }

            List<RectLayout> layoutList = null;
            SizeF retSizeF = SizeF.Empty;
            // 找到最高的块
            // 对全部矩形块按高度降序排序
            rectSizeList.Sort(RectSizeCompareByHeight);
            // 以最高的块高度为基准, 对其它低于这个高度的文件夹进行重新排列
            float maxHeight = rectSizeList[0].Height;
            foreach (string subDir in subDirsList)
            {
                SizeF subFolderSize = GetFolderDisplaySizeF(subDir, folderDisplayScaleList);
                if (subFolderSize.Height < maxHeight)
                {
                    layoutList = GetFolderRectLayoutList(subDir, folderDisplayScaleList);
                    List<SizeF> tmpSizeList = new List<SizeF>();
                    foreach (RectLayout rl in layoutList)
                    {
                        tmpSizeList.Add(rl.size);
                    }
                    List<RectLayout> newLayoutList = MakeRectsLayout(tmpSizeList, spaceDistance, maxHeight);
                    SetFolderRectLayoutList(subDir, folderDisplayScaleList, newLayoutList);
                    SizeF newSize = GetLayoutFrameSize(newLayoutList, spaceDistance);
                    SetFolderDisplaySize(subDir, folderDisplayScaleList, newSize);
                }
            }
            rectSizeList = new List<SizeF>();
            foreach (string subDir in subDirsList)
            {
                SizeF subFolderSize = GetFolderDisplaySizeF(subDir, folderDisplayScaleList);
                rectSizeList.Add(subFolderSize);
            }
            foreach (string fname in subFilesList)
            {
                SizeF srcFileSize = GetFileDisplaySizeF(fname, fileDisplayScaleList);
                rectSizeList.Add(srcFileSize);
            }

            // 先不考虑纵向分层, 暂时先一字排开
            // 计算当前文件夹的显示SizeF

            // 考虑怎样进行合理显示布局
            layoutList = MakeRectsLayout(rectSizeList, spaceDistance);
            retSizeF = GetLayoutFrameSize(layoutList, spaceDistance);

            DisplayScale ds = new DisplayScale();
            ds.displaySize = retSizeF;
            ds.layoutScaleList = layoutList;

            DisplayScaleInfo curDsi = new DisplayScaleInfo(path, ds);
            // 将当前文件夹的显示SizeF加入到文件夹SizeF列表中
            folderDisplayScaleList.Add(curDsi);

            return retSizeF;
        }

        SizeF GetFileDisplaySizeF(string fname, List<DisplayScaleInfo> fileDisplayScaleList)
        {
            foreach (DisplayScaleInfo fdi in fileDisplayScaleList)
            {
                if (fdi.fullName.Equals(fname))
                {
                    return fdi.displayScale.displaySize;
                }
            }
            return SizeF.Empty;
        }

        SizeF GetFolderDisplaySizeF(string fname, List<DisplayScaleInfo> folderDisplayScaleList)
        {
            foreach (DisplayScaleInfo fds in folderDisplayScaleList)
            {
                if (fds.fullName.Equals(fname))
                {
                    return fds.displayScale.displaySize;
                }
            }
            return SizeF.Empty;
        }

        List<RectLayout> GetFolderRectLayoutList(string fname, List<DisplayScaleInfo> folderDisplayScaleList)
        {
            foreach (DisplayScaleInfo fds in folderDisplayScaleList)
            {
                if (fds.fullName.Equals(fname))
                {
                    return fds.displayScale.layoutScaleList;
                }
            }
            return null;
        }

        void SetFolderRectLayoutList(string fname, List<DisplayScaleInfo> folderDisplayScaleList, List<RectLayout> newLayoutList)
        {
            foreach (DisplayScaleInfo fds in folderDisplayScaleList)
            {
                if (fds.fullName.Equals(fname))
                {
                    fds.displayScale.layoutScaleList = newLayoutList;
                }
            }
        }

        void SetFolderDisplaySize(string fname, List<DisplayScaleInfo> folderDisplayScaleList, SizeF newDisplaySize)
        {
            foreach (DisplayScaleInfo fds in folderDisplayScaleList)
            {
                if (fds.fullName.Equals(fname))
                {
                    fds.displayScale.displaySize = newDisplaySize;
                }
            }
        }

        List<string> GetSubDirectoriesFromDsList(string root, List<DisplayScaleInfo> fileDisplayScaleList)
        {
            if (!root.EndsWith("\\"))
            {
                root += "\\";
            }
            List<string> subDirList = new List<string>();
            foreach (DisplayScaleInfo dsi in fileDisplayScaleList)
            {
                string fullName = dsi.fullName;
                if (fullName.StartsWith(root))
                {
                    string leftStr = fullName.Substring(root.Length);
                    int idx = leftStr.IndexOf("\\");
                    if (-1 != idx)
                    {
                        leftStr = leftStr.Remove(idx);
                        string subDir = root + leftStr;
                        if (!subDirList.Contains(subDir))
                        {
                            subDirList.Add(subDir);
                        }
                    }
                }
            }
            return subDirList;
        }

        List<string> GetFilesFromDsList(string root, List<DisplayScaleInfo> fileDisplayScaleList)
        {
            if (!root.EndsWith("\\"))
            {
                root += "\\";
            }
            List<string> filesList = new List<string>();
            foreach (DisplayScaleInfo dsi in fileDisplayScaleList)
            {
                string fullName = dsi.fullName;
                if (fullName.StartsWith(root))
                {
                    string leftStr = fullName.Substring(root.Length);
                    int idx = leftStr.IndexOf("\\");
                    if (-1 == idx)
                    {
                        string fileName = root + leftStr;
                        if (!filesList.Contains(fileName))
                        {
                            filesList.Add(fileName);
                        }
                    }
                }
            }
            return filesList;
        }

        /// <summary>
        /// 文件夹描画
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="path"></param>
        /// <param name="fileDisplayScaleList"></param>
        /// <param name="folderDisplayScaleList"></param>
        /// <param name="fileParseInfoList"></param>
        /// <param name="g"></param>
        /// <param name="scale"></param>
        void DrawFolder(PointF startPoint, string path, List<DisplayScaleInfo> fileDisplayScaleList,
                        List<DisplayScaleInfo> folderDisplayScaleList, List<CFileParseInfo> fileParseInfoList,
                        Graphics g, int scale = 0)
        {
            // 取得当前路径下所有子文件夹和文件的列表
            List<string> subDirsList = GetSubDirectoriesFromDsList(path, fileDisplayScaleList);
            List<string> subFilesList = GetFilesFromDsList(path, fileDisplayScaleList);

            List<RectLayout> rectLayoutList = GetFolderRectLayoutList(path, folderDisplayScaleList);

            Font textFont = new Font(DISPLAY_FONT_NAME, scale + 1);
            // 计算间距宽度
            float spaceDistance = g.MeasureString(SPACE_STRING, textFont).Width;
//          startPoint.X += (int)(spaceDistance / 2);

            // 从左至右先描画各个文件
            foreach (string sf in subFilesList)
            {
                SizeF fileSizeF = GetFileDisplaySizeF(sf, fileDisplayScaleList);
                PointF offsetPoint = GetRectDrawOffset(rectLayoutList, fileSizeF, spaceDistance);
                PointF drawPoint = new PointF(startPoint.X + offsetPoint.X, startPoint.Y + offsetPoint.Y);
                // 画文件边框
                Rectangle frameRect = new Rectangle((int)drawPoint.X, (int)drawPoint.Y, (int)fileSizeF.Width, (int)fileSizeF.Height);
                g.DrawRectangle(fileFramePen, frameRect);
                // 画文件内部
                CFileParseInfo curFileParseInfo = GetFileParsedInfoByName(sf, fileParseInfoList);
                if (null != curFileParseInfo)
                {
                    float totalHeight = 0;
                    // 画文件名
                    string fname = GetFileName(sf);
                    SizeF nameSize = g.MeasureString(fname, textFont);
                    PointF textPoint = new PointF(drawPoint.X, drawPoint.Y + totalHeight);
                    g.DrawString(fname, textFont, titleBrush, textPoint);
                    totalHeight += (float)(nameSize.Height * 1.1);
                    // 画内部函数定义名
                    foreach (CFunctionInfo fi in curFileParseInfo.fun_define_list)
                    {
                        nameSize = g.MeasureString(fi.name, textFont);
                        textPoint = new PointF(drawPoint.X, drawPoint.Y + totalHeight);
                        g.DrawString(fi.name, textFont, textBrush, textPoint);
                        totalHeight += nameSize.Height;
                    }
                }
            }
            // 接着画各个子文件夹
            foreach (string sd in subDirsList)
            {
                SizeF folderSizeF = GetFolderDisplaySizeF(sd, folderDisplayScaleList);
                PointF offsetPoint = GetRectDrawOffset(rectLayoutList, folderSizeF, spaceDistance);
                PointF drawPoint = new PointF(startPoint.X + offsetPoint.X, startPoint.Y + offsetPoint.Y);
                // 画文件夹边框
                Rectangle frameRect = new Rectangle((int)drawPoint.X, (int)drawPoint.Y, (int)folderSizeF.Width, (int)folderSizeF.Height);
                g.DrawRectangle(folderFramePen, frameRect);
                // 递归画子文件夹内部
                DrawFolder(drawPoint, sd, fileDisplayScaleList, folderDisplayScaleList, fileParseInfoList, g, scale);
            }
        }

        PointF GetRectDrawOffset(List<RectLayout> layoutList, SizeF rectSize, float spaceDistance)
        {
            PointF retOffset = new PointF(spaceDistance / 2, spaceDistance / 2);

            // 统计所有的行列号
            List<int> rowList = new List<int>();
            List<int> colList = new List<int>();
            foreach (RectLayout rl in layoutList)
            {
                if (!rowList.Contains(rl.row))
                {
                    rowList.Add(rl.row);
                }
                if (!colList.Contains(rl.col))
                {
                    colList.Add(rl.col);
                }
            }

            // 找到指定size的矩形块
            int row = -1;
            int col = -1;
            int idx = 0;
            foreach (RectLayout rl in layoutList)
            {
                if (   rl.size.Equals(rectSize)
                    && (false == rl.isDrawn)    )
                {
                    row = rl.row;
                    col = rl.col;
                    rl.isDrawn = true;
                    break;
                }
                idx++;
            }
            if (    (-1 == row)
                ||  (-1 == col))
            {
                // 没找到
                return PointF.Empty;
            }

            float totalWidth = 0;
            foreach (int c in colList)
            {
                // 指定矩形块左侧的列
                if (c < col)
                {
                    float maxWidth = 0;
                    foreach (RectLayout rl in layoutList)
                    {
                        if (c == rl.col)
                        {
                            if (rl.size.Width > maxWidth)
                            {
                                maxWidth = rl.size.Width;
                            }
                        }
                    }
                    totalWidth += (maxWidth + spaceDistance);
                }
            }
            float totalHeight = 0;
            foreach (int r in rowList)
            {
                if (r < row)
                {
                    float maxHeight = 0;
                    foreach (RectLayout rl in layoutList)
                    {
                        if (r == rl.row)
                        {
                            if (rl.size.Height > maxHeight)
                            {
                                maxHeight = rl.size.Height;
                            }
                        }
                    }
                    totalHeight += (maxHeight + spaceDistance);
                }
            }

            RectLayout layout = layoutList[idx];
            if (0 != layout.order)
            {
                // 序号不为0, 说明这一位置(行, 列)是由多个较小的矩形块摞在一起组成的
                // 所以要找到所有排在指定矩形块上面的其它块, 进以确定其offset
                foreach (RectLayout rl in layoutList)
                {
                    if (   (rl.col == layout.col)
                        && (rl.row == layout.row)
                        && (rl.order < layout.order))
                    {
                        // 行列号一样, 但是序号更靠前, 这正是摞在指定矩形块上面那些矩形块
                        totalHeight += (rl.size.Height + spaceDistance);
                    }
                }
            }
            retOffset.X += totalWidth;
            retOffset.Y += totalHeight;

            return retOffset;
        }

        CFileParseInfo GetFileParsedInfoByName(string fullName, List<CFileParseInfo> fileParseInfoList)
        {
            foreach (CFileParseInfo fpi in fileParseInfoList)
            {
                if (fpi.full_name.Equals(fullName))
                {
                    return fpi;
                }
            }
            return null;
        }

        string GetFileName(string fullName)
        {
            string retName = fullName;
            int idx = fullName.LastIndexOf('\\');
            if (-1 != idx)
            {
                retName = fullName.Substring(idx + 1).Trim();
            }

            return retName;
        }

        /// <summary>
        /// 根据一组矩形的size决定这些矩形如何排列布局
        /// </summary>
        /// <param name="sizeList">入力: 矩形的size列表</param>
        /// <returns>出力: 按先后顺序各矩形的行列号</returns>
        List<RectLayout> MakeRectsLayout(List<SizeF> sizeList, float spaceDistance, float targetHeight = 0)
        {
            List<RectLayout> retLayoutList = new List<RectLayout>();

            int col = 0;                                                        // 列号
            RectLayout rl = null;
            // 取得最高块的高度
            sizeList.Sort(RectSizeCompareByHeight);
            float maxHeight = sizeList[0].Height;
            if (0 != targetHeight
                && targetHeight > maxHeight)
            {
                maxHeight = targetHeight;
            }
            else
            {
                // 首先只排成一行
                rl = new RectLayout(sizeList[0], 0, col);
                retLayoutList.Add(rl);
                sizeList.RemoveAt(0);
                col += 1;
            }
            // 以这个高度为基准,看看能否把矮的块摞在一起以接近这个高度
            while (0 != sizeList.Count)
            {
                List<SizeF> rectStack = GetRectStackByHeight(ref sizeList, maxHeight, spaceDistance);
                if (0 != rectStack.Count)
                {
                    if (0 != retLayoutList.Count)
                    {
                        int orderNum = 0;       // 序号, 多个小块摞到一起时, 行列号是一样的, 用序号表示其上下次序关系
                        foreach (SizeF s in rectStack)
                        {
                            rl = new RectLayout(s, 0, col, orderNum);
                            retLayoutList.Add(rl);
                            orderNum += 1;
                        }
                    }
                    else
                    {
                        int row = 0;
                        foreach (SizeF s in rectStack)
                        {
                            rl = new RectLayout(s, row, col, 0);
                            retLayoutList.Add(rl);
                            row += 1;
                        }
                    }
                    col += 1;
                }
            }


            // 判断这一行是否水平方向上过于细长, 如果是, 尝试换成多行的布局

            return retLayoutList;
        }

        /// <summary>
        /// 矩形块排序方法
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        int RectSizeCompareByHeight(SizeF s1, SizeF s2)
        {
            if (s1.Height > s2.Height)
            {
                return -1;
            }
            else if (s1.Height < s2.Height)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        List<SizeF> GetRectStackByHeight(ref List<SizeF> inputList, float targetHeight, float spaceDistance)
        {
            List<SizeF> retList = new List<SizeF>();
            float fstHeight = inputList[0].Height;
            retList.Add(inputList[0]);
            inputList.RemoveAt(0);
            // 然后尝试还能不能再放下别的块
            float curHeight = fstHeight;

            List<int> removeIdxList = new List<int>();
            for (int idx = 0; idx < inputList.Count; idx++)
            {
                SizeF s = inputList[idx];
                if (s.Height + spaceDistance + curHeight < targetHeight)
                {
                    removeIdxList.Add(idx);
                    curHeight += (s.Height + spaceDistance);
                }
            }
            for (int i = removeIdxList.Count - 1; i >= 0; i--)
            {
                int idx = removeIdxList[i];
                retList.Add(inputList[idx]);
                inputList.RemoveAt(idx);
            }

            return retList;
        }

        SizeF GetLayoutFrameSize(List<RectLayout> layoutList, float spaceDistance)
        {
            List<int> rowList = new List<int>();
            List<int> colList = new List<int>();
            foreach (RectLayout rl in layoutList)
            {
                if (!rowList.Contains(rl.row))
                {
                    rowList.Add(rl.row);
                }
                if (!colList.Contains(rl.col))
                {
                    colList.Add(rl.col);
                }
            }
            float totalHeight = 0;
            float totalWidth = 0;
            for (int i = 0; i < rowList.Count; i++)
            {
                int rowNum = rowList[i];
                float maxHeight = 0;
                foreach (RectLayout rl in layoutList)
                {
                    if (rowNum == rl.row)
                    {
                        if (rl.size.Height > maxHeight)
                        {
                            maxHeight = rl.size.Height;
                        }
                    }
                }
                totalHeight += (maxHeight + spaceDistance);
            }
            for (int i = 0; i < colList.Count; i++)
            {
                int colNum = colList[i];
                float maxWidth = 0;
                foreach (RectLayout rl in layoutList)
                {
                    if (colNum == rl.col)
                    {
                        if (rl.size.Width > maxWidth)
                        {
                            maxWidth = rl.size.Width;
                        }
                    }
                }
                totalWidth += (maxWidth + spaceDistance);
            }

            return new SizeF(totalWidth, totalHeight);
        }
    }

    // 文件的图形表示尺寸
    public class DisplayScale
    {
        // 当前scale对应字体的显示尺寸
        public SizeF displaySize = new SizeF();
        public List<RectLayout> layoutScaleList = new List<RectLayout>();
    }

    public class DisplayScaleInfo
    {
        public string fullName;
        public DisplayScale displayScale;

        public DisplayScaleInfo(string fn, DisplayScale ds)
        {
            fullName = fn;
            displayScale = ds;
        }
    }

    // 表示矩形区块的布局信息(行列号)
    public class RectLayout
    {
        public SizeF size = new SizeF();
        public int row = -1;
        public int col = -1;
        public int order = 0;          // 序号: 当多个小块被摞到一起时, 它们的行列号是一样的, 这时用序号来表示其上下次序关系;
        public bool isDrawn = false;   // 用于描画时标示是否画过

        public RectLayout(SizeF s, int r, int c, int o = -1)
        {
            size = s;
            row = r;
            col = c;
            order = o;
        }
    }
}
