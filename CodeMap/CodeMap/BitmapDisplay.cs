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
        const float DISPLAY_SPACE_RATE_H = 0.2F;            // 水平方向显示区块空位(间距)所占总长度的百分比(比率)
        const float DISPLAY_SPACE_RATE_V = 0.1F;            // 垂直方向显示区块空位(间距)所占总长度的百分比(比率)

        SolidBrush textBrush = new SolidBrush(Color.Red);
        Pen framePen = new Pen(Color.DarkCyan, 2);

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
            SizeF rootSizeF = MeasureFolderDisplayScale(root, fileDisplayScaleList, ref folderDisplayScaleList);

            // 递归描画各个文件夹
            DrawFolder(startPoint, root, fileDisplayScaleList, folderDisplayScaleList, fileParseInfoList, g);

            //for (int i = 0; i < fileParseInfoList.Count; i++)
            //{
            //    CFileParseInfo curFileInfo = fileParseInfoList[i];
            //    float maxWidth = 0;
            //    float totalHeight = 0;
            //    foreach (CFunctionInfo curFunc in curFileInfo.fun_define_list)
            //    {
            //        SizeF nameSize = g.MeasureString(curFunc.name, textFont);
            //        if (nameSize.Width > maxWidth)
            //        {
            //            maxWidth = nameSize.Width;
            //        }
            //        PointF textPoint = new PointF(startPoint.X, startPoint.Y + totalHeight);
            //        g.DrawString(curFunc.name, textFont, textBrush, textPoint);
            //        totalHeight += nameSize.Height;
            //    }
            //    Rectangle frameRect = new Rectangle((int)startPoint.X, (int)startPoint.Y, (int)maxWidth, (int)totalHeight);
            //    g.DrawRectangle(framePen, frameRect);
            //}

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
                for (int f = MIN_DISPLAY_FONT_SIZE; f < MAX_DISPLAY_FONT_SIZE; f++)
                {
                    Font textFont = new Font(DISPLAY_FONT_NAME, f);
                    float maxWidth = 0;
                    float totalHeight = 0;
                    // 暂时只考虑在表示文件的矩形框里显示文件内有定义的函数名的情况
                    foreach (CFunctionInfo curFunc in curFileInfo.fun_define_list)
                    {
                        SizeF nameSize = g.MeasureString(curFunc.name, textFont);
                        if (nameSize.Width > maxWidth)
                        {
                            maxWidth = nameSize.Width;
                        }
                        totalHeight += nameSize.Height;
                    }
                    SizeF fileDisplaySize = new SizeF(maxWidth, totalHeight);
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
        SizeF MeasureFolderDisplayScale(string root, List<DisplayScaleInfo> fileDisplayScaleList, ref List<DisplayScaleInfo> folderDisplayScaleList)
        {
            List<SizeF> displaySizeList = new List<SizeF>();   // 所有将要显示的矩形SizeF集合

            // 分别取得其下所有文件和子文件夹的显示尺寸
            List<string> subDirsList = GetSubDirectoriesFromDsList(root, fileDisplayScaleList);
            List<string> subFilesList = GetFilesFromDsList(root, fileDisplayScaleList);

            foreach (string subDir in subDirsList)
            {
                SizeF subFolderSize = MeasureFolderDisplayScale(subDir, fileDisplayScaleList, ref folderDisplayScaleList);
                displaySizeList.Add(subFolderSize);
            }
            foreach (string fname in subFilesList)
            {
                SizeF srcFileSize = GetFileDisplaySizeF(fname, fileDisplayScaleList);
                displaySizeList.Add(srcFileSize);
            }

            // 考虑怎样进行合理显示布局
            // 先不考虑纵向分层, 暂时先一字排开
            // 横向总长, 间距占总长百分比, 等间距
            // 纵向为所有子块中纵向长度最长的块的纵向长度
            float hSize = 0;
            float vSize = 0;
            foreach (SizeF blockSize in displaySizeList)
            {
                hSize += blockSize.Width;
                if (blockSize.Height > vSize)
                {
                    vSize = blockSize.Height;
                }
            }
            // 计算当前文件夹的显示SizeF
            SizeF retSizeF = new SizeF(hSize * (1 + DISPLAY_SPACE_RATE_H), vSize * (1 + DISPLAY_SPACE_RATE_V));
            DisplayScale ds = new DisplayScale();
            ds.displaySizeList.Add(retSizeF);
            DisplayScaleInfo curDsi = new DisplayScaleInfo(root, ds);
            // 将当前文件夹的显示SizeF加入到文件夹SizeF列表中
            folderDisplayScaleList.Add(curDsi);

            return retSizeF;
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

        void DrawFolder(Point startPoint, string path, List<DisplayScaleInfo> fileDisplayScaleList, List<DisplayScaleInfo> folderDisplayScaleList, List<CFileParseInfo> fileParseInfoList, Graphics g)
        {
            List<string> subDirsList = GetSubDirectoriesFromDsList(path, fileDisplayScaleList);
            List<string> subFilesList = GetFilesFromDsList(path, fileDisplayScaleList);
            // 计算间距大小
            float totalWidth = 0;
            int totalCount = 0;
            foreach (string sf in subFilesList)
            {
                SizeF fileSizeF = GetFileDisplaySizeF(sf, fileDisplayScaleList);
                totalWidth += fileSizeF.Width;
                totalCount++;
            }
            foreach (string sd in subDirsList)
            {
                SizeF folderSizeF = GetFolderDisplaySizeF(sd, folderDisplayScaleList);
                totalWidth += folderSizeF.Width;
                totalCount++;
            }
            float spaceDistance = (float)((0.2 * totalWidth) / totalCount);

            startPoint.X += (int)(0.5 * spaceDistance);
            Font textFont = new Font(DISPLAY_FONT_NAME, 1);

            // 从左至右先描画各个文件
            foreach (string sf in subFilesList)
            {
                SizeF fileSizeF = GetFileDisplaySizeF(sf, fileDisplayScaleList);
                // 画文件边框
                Rectangle frameRect = new Rectangle((int)startPoint.X, (int)startPoint.Y, (int)fileSizeF.Width, (int)fileSizeF.Height);
                g.DrawRectangle(framePen, frameRect);
                // 画文件内部
                CFileParseInfo curFileParseInfo = GetFileParsedInfoByName(sf, fileParseInfoList);
                if (null != curFileParseInfo)
                {
                    float totalHeight = 0;
                    foreach (CFunctionInfo fi in curFileParseInfo.fun_define_list)
                    {
                        SizeF nameSize = g.MeasureString(fi.name, textFont);
                        PointF textPoint = new PointF(startPoint.X, startPoint.Y + totalHeight);
                        g.DrawString(fi.name, textFont, textBrush, textPoint);
                        totalHeight += nameSize.Height;
                    }
                }
                startPoint.X += (int)(fileSizeF.Width + spaceDistance);
            }
            // 接着画各个子文件夹
            foreach (string sd in subDirsList)
            {
                SizeF folderSizeF = GetFolderDisplaySizeF(sd, folderDisplayScaleList);
                // 画文件夹边框
                Rectangle frameRect = new Rectangle((int)startPoint.X, (int)startPoint.Y, (int)folderSizeF.Width, (int)folderSizeF.Height);
                g.DrawRectangle(framePen, frameRect);
                // 递归画子文件夹内部
                DrawFolder(startPoint, sd, fileDisplayScaleList, folderDisplayScaleList, fileParseInfoList, g);
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
}
