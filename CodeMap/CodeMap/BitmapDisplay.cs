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
        Bitmap _bitMap = null;

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
        public Bitmap DrawMap(string root, List<CFileParseInfo> fileParseInfoList, int width, int height)
        {
            _bitMap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(_bitMap);
            g.Clear(Color.Black);
            Point startPoint = new Point(3, 3);

            // 先计算每个源文件各个字体大小对应的SizeF
            List<DisplayScaleInfo> fileDisplayScaleList = MeasureFileDisplayScale(fileParseInfoList, g);
            // 再计算各个文件夹各个字体大小对应的SizeF
            List<DisplayScaleInfo> folderDisplayScaleList = new List<DisplayScaleInfo>();
            MeasureFolderDisplayScale(root, fileDisplayScaleList, ref folderDisplayScaleList, g);

            // 递归描画各个文件夹
            DrawFolder(startPoint, root, fileDisplayScaleList, folderDisplayScaleList, fileParseInfoList, g, 7);

            return _bitMap;
        }

        /// <summary>
        /// 计算所有源文件的各个字体大小对应的图形化显示尺寸
        /// </summary>
        List<DisplayScaleInfo> MeasureFileDisplayScale(List<CFileParseInfo> fileParseInfoList, Graphics g)
        {
            List<DisplayScaleInfo> fileDisplayScaleList = new List<DisplayScaleInfo>();

            for (int i = 0; i < fileParseInfoList.Count; i++)
            {
                CFileParseInfo curFileInfo = fileParseInfoList[i];

                // 分别测试每种字体大小对应的显示尺寸
                DisplayScale fds = new DisplayScale();
                for (int f = MIN_DISPLAY_FONT_SIZE; f <= MAX_DISPLAY_FONT_SIZE; f++)
                {
                    Font textFont = new Font(DISPLAY_FONT_NAME, f);
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
                    fds.displaySizeList.Add(fileDisplaySize);
                }

                DisplayScaleInfo fdi = new DisplayScaleInfo(curFileInfo.full_name, fds);
                fileDisplayScaleList.Add(fdi);
            }

            return fileDisplayScaleList;
        }

        /// <summary>
        /// 计算文件夹的显示尺寸
        /// </summary>
        void MeasureFolderDisplayScale(string path, List<DisplayScaleInfo> fileDisplayScaleList,
                                       ref List<DisplayScaleInfo> folderDisplayScaleList, Graphics g)
        {
            float[] hSizeArray = new float[MAX_DISPLAY_FONT_SIZE] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            float[] vSizeArray = new float[MAX_DISPLAY_FONT_SIZE] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            // 分别取得其下所有文件和子文件夹的显示尺寸
            List<string> subDirsList = GetSubDirectoriesFromDsList(path, fileDisplayScaleList);
            List<string> subFilesList = GetFilesFromDsList(path, fileDisplayScaleList);
            // 各个矩形块的SizeF列表, 用以进行布局排列
            List<SizeF> rectSizeList = new List<SizeF>();

            foreach (string subDir in subDirsList)
            {
                MeasureFolderDisplayScale(subDir, fileDisplayScaleList, ref folderDisplayScaleList, g);
                SizeF subFolderSize = new SizeF();
                for (int scale = MIN_DISPLAY_FONT_SIZE - 1; scale < MAX_DISPLAY_FONT_SIZE; scale++)
                {
                    subFolderSize = GetFolderDisplaySizeF(subDir, folderDisplayScaleList, scale);
                    hSizeArray[scale] += subFolderSize.Width;
                    if (subFolderSize.Height > vSizeArray[scale])
                    {
                        vSizeArray[scale] = subFolderSize.Height;
                    }
                }
                rectSizeList.Add(subFolderSize);
            }
            foreach (string fname in subFilesList)
            {
                SizeF srcFileSize = new SizeF();
                for (int scale = MIN_DISPLAY_FONT_SIZE - 1; scale < MAX_DISPLAY_FONT_SIZE; scale++)
                {
                    srcFileSize = GetFileDisplaySizeF(fname, fileDisplayScaleList, scale);
                    hSizeArray[scale] += srcFileSize.Width;
                    if (srcFileSize.Height > vSizeArray[scale])
                    {
                        vSizeArray[scale] = srcFileSize.Height;
                    }
                }
                rectSizeList.Add(srcFileSize);
            }

            // 考虑怎样进行合理显示布局
            List<RectLayout> layoutList = GetRectsLayout(rectSizeList);
            // 先不考虑纵向分层, 暂时先一字排开
            // 计算当前文件夹的显示SizeF
            DisplayScale ds = new DisplayScale();
            for (int scale = MIN_DISPLAY_FONT_SIZE - 1; scale < MAX_DISPLAY_FONT_SIZE; scale++)
            {
                float hSize = hSizeArray[scale];
                float vSize = vSizeArray[scale];

                Font textFont = new Font(DISPLAY_FONT_NAME, scale + 1);
                // 计算间距宽度
                float spaceDistance = g.MeasureString(SPACE_STRING, textFont).Width;

                SizeF retSizeF = new SizeF(hSize + spaceDistance * (subDirsList.Count + subFilesList.Count), vSize + spaceDistance);
                ds.displaySizeList.Add(retSizeF);
            }
            DisplayScaleInfo curDsi = new DisplayScaleInfo(path, ds);
            // 将当前文件夹的显示SizeF加入到文件夹SizeF列表中
            folderDisplayScaleList.Add(curDsi);
        }

        SizeF GetFileDisplaySizeF(string fname, List<DisplayScaleInfo> fileDisplayScaleList, int scale = 0)
        {
            foreach (DisplayScaleInfo fdi in fileDisplayScaleList)
            {
                if (fdi.fullName.Equals(fname))
                {
                    if (scale < fdi.displayScale.displaySizeList.Count)
                    {
                        return fdi.displayScale.displaySizeList[scale];
                    }
                }
            }
            return new SizeF();
        }

        SizeF GetFolderDisplaySizeF(string fname, List<DisplayScaleInfo> folderDisplayScaleList, int scale = 0)
        {
            foreach (DisplayScaleInfo fdi in folderDisplayScaleList)
            {
                if (fdi.fullName.Equals(fname))
                {
                    if (scale < fdi.displayScale.displaySizeList.Count)
                    {
                        return fdi.displayScale.displaySizeList[scale];
                    }
                }
            }
            return new SizeF();
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

        void DrawFolder(Point startPoint, string path, List<DisplayScaleInfo> fileDisplayScaleList,
                        List<DisplayScaleInfo> folderDisplayScaleList, List<CFileParseInfo> fileParseInfoList,
                        Graphics g, int scale = 0)
        {
            // 取得当前路径下所有子文件夹和文件的列表
            List<string> subDirsList = GetSubDirectoriesFromDsList(path, fileDisplayScaleList);
            List<string> subFilesList = GetFilesFromDsList(path, fileDisplayScaleList);

            Font textFont = new Font(DISPLAY_FONT_NAME, scale + 1);
            // 计算间距宽度
            float spaceDistance = g.MeasureString(SPACE_STRING, textFont).Width;
            startPoint.X += (int)(spaceDistance / 2);

            // 从左至右先描画各个文件
            foreach (string sf in subFilesList)
            {
                SizeF fileSizeF = GetFileDisplaySizeF(sf, fileDisplayScaleList, scale);
                // 画文件边框
                Rectangle frameRect = new Rectangle((int)startPoint.X, (int)startPoint.Y, (int)fileSizeF.Width, (int)fileSizeF.Height);
                g.DrawRectangle(fileFramePen, frameRect);
                // 画文件内部
                CFileParseInfo curFileParseInfo = GetFileParsedInfoByName(sf, fileParseInfoList);
                if (null != curFileParseInfo)
                {
                    float totalHeight = 0;
                    // 画文件名
                    string fname = GetFileName(sf);
                    SizeF nameSize = g.MeasureString(fname, textFont);
                    PointF textPoint = new PointF(startPoint.X, startPoint.Y + totalHeight);
                    g.DrawString(fname, textFont, titleBrush, textPoint);
                    totalHeight += (float)(nameSize.Height * 1.1);
                    // 画内部函数定义名
                    foreach (CFunctionInfo fi in curFileParseInfo.fun_define_list)
                    {
                        nameSize = g.MeasureString(fi.name, textFont);
                        textPoint = new PointF(startPoint.X, startPoint.Y + totalHeight);
                        g.DrawString(fi.name, textFont, textBrush, textPoint);
                        totalHeight += nameSize.Height;
                    }
                }
                startPoint.X += (int)(fileSizeF.Width + spaceDistance);
            }
            // 接着画各个子文件夹
            foreach (string sd in subDirsList)
            {
                SizeF folderSizeF = GetFolderDisplaySizeF(sd, folderDisplayScaleList, scale);
                // 画文件夹边框
                Rectangle frameRect = new Rectangle((int)startPoint.X, (int)startPoint.Y, (int)folderSizeF.Width, (int)folderSizeF.Height);
                g.DrawRectangle(folderFramePen, frameRect);
                // 递归画子文件夹内部
                DrawFolder(startPoint, sd, fileDisplayScaleList, folderDisplayScaleList, fileParseInfoList, g, scale);
                startPoint.X += (int)(folderSizeF.Width + spaceDistance);
            }
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
        List<RectLayout> GetRectsLayout(List<SizeF> sizeList)
        {
            List<RectLayout> retLayoutList = new List<RectLayout>();
            // 对全部矩形块按高度降序排序
            sizeList.Sort(RectSizeCompareByHeight);

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
    }

    // 文件的图形表示尺寸
    public class DisplayScale
    {
        // 各种大小字体对应的显示尺寸
        public List<SizeF> displaySizeList = new List<SizeF>();
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
        public int row = 0;
        public int col = 0;
    }
}
