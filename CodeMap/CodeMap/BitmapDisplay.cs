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

        public BitmapDisplay()
        {
        }

        /// <summary>
        /// 根据源文件的解析结果, 描画代码图
        /// </summary>
        /// <param name="fileInfoList"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public Bitmap DrawMap(List<CFileParseInfo> fileInfoList, int width, int height)
        {
            _bitMap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(_bitMap);
            g.Clear(Color.Black);
            Font textFont = new Font(DISPLAY_FONT_NAME, 9);
            SolidBrush textBrush = new SolidBrush(Color.Red);
            Pen framePen = new Pen(Color.DarkCyan, 2);
            PointF startPoint = new PointF(3, 3);

            // 先计算每个源文件的描画尺寸
            // 再计算各个文件夹内的描画布局

            for (int i = 0; i < fileInfoList.Count; i++)
            {
                CFileParseInfo curFileInfo = fileInfoList[i];
                float maxWidth = 0;
                float totalHeight = 0;
                foreach (CFunctionInfo curFunc in curFileInfo.fun_define_list)
                {
                    SizeF nameSize = g.MeasureString(curFunc.name, textFont);
                    if (nameSize.Width > maxWidth)
                    {
                        maxWidth = nameSize.Width;
                    }
                    PointF textPoint = new PointF(startPoint.X, startPoint.Y + totalHeight);
                    g.DrawString(curFunc.name, textFont, textBrush, textPoint);
                    totalHeight += nameSize.Height;
                }
                Rectangle frameRect = new Rectangle((int)startPoint.X, (int)startPoint.Y, (int)maxWidth, (int)totalHeight);
                g.DrawRectangle(framePen, frameRect);
            }

            return _bitMap;
        }

        /// <summary>
        /// 计算文件的图形化显示尺寸
        /// </summary>
        List<FileDisplayInfo> MeasureFileDisplayScale(List<CFileParseInfo> fileInfoList, Graphics g)
        {
            List<FileDisplayInfo> fileDisplayInfoList = new List<FileDisplayInfo>();

            for (int i = 0; i < fileInfoList.Count; i++)
            {
                CFileParseInfo curFileInfo = fileInfoList[i];

                // 分别测试每种字体大小对应的显示尺寸
                FileDisplayScale fds = new FileDisplayScale();
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

                FileDisplayInfo fdi = new FileDisplayInfo(curFileInfo.full_name, fds);
                fileDisplayInfoList.Add(fdi);
            }

            return fileDisplayInfoList;
        }

        /// <summary>
        /// 计算文件夹的显示尺寸
        /// </summary>
        SizeF MeasureFolderDisplayScale(string folder, List<FileDisplayInfo> fileDisplayInfoList)
        {
            List<SizeF> displaySizeList = new List<SizeF>();   // 所有将要显示的矩形SizeF集合

            // 分别取得其下所有文件和子文件夹的显示尺寸
            DirectoryInfo di = new DirectoryInfo(folder);
            {
                foreach (DirectoryInfo subDir in di.GetDirectories())
                {
                    SizeF subFolderSize = MeasureFolderDisplayScale(subDir.FullName, fileDisplayInfoList);
                    displaySizeList.Add(subFolderSize);
                }
                foreach (FileInfo fi in di.GetFiles())
                {
                    ;
                }
            }
            // 考虑怎样进行合理显示布局

            return new SizeF();
        }

        SizeF GetFileDisplaySizeF(string fname, List<FileDisplayInfo> fileDisplayInfoList, int scale)
        {
            foreach (FileDisplayInfo fdi in fileDisplayInfoList)
            {
                if (fname == fdi.fullName)
                {
                    if (scale < fdi.fileDisplayScale.displaySizeList.Count)
                    {
                        return fdi.fileDisplayScale.displaySizeList[scale];
                    }
                }
            }

            return new SizeF();
        }
    }

    // 文件的图形表示尺寸
    public class FileDisplayScale
    {
        // 各种大小字体对应的显示尺寸
        public List<SizeF> displaySizeList = new List<SizeF>();
    }

    public class FileDisplayInfo
    {
        public string fullName;
        public FileDisplayScale fileDisplayScale;

        public FileDisplayInfo(string fname, FileDisplayScale fds)
        {
            fullName = fname;
            fileDisplayScale = fds;
        }
    }
}
