using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeMap
{
    class BitmapDisplay
    {
        Bitmap _bitMap = null;

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
        public Bitmap DrawMap(List<CFileInfo> fileInfoList, int width, int height)
        {
            _bitMap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(_bitMap);
            g.Clear(Color.Black);
            Font textFont = new Font("Verdana", 6);
            SolidBrush textBrush = new SolidBrush(Color.Red);
            Pen framePen = new Pen(Color.DarkCyan, 2);
            PointF startPoint = new PointF(3, 3);

            // 先计算每个源文件的描画尺寸
            // 再计算各个文件夹内的描画布局

            for (int i = 0; i < fileInfoList.Count; i++)
            {
                CFileInfo curFileInfo = fileInfoList[i];
                List<string> funcNameList = new List<string>();
                float maxWidth = 0;
                float totalHeight = 0;
                foreach (CFunctionInfo curFunc in curFileInfo.fun_define_list)
                {
                    funcNameList.Add(curFunc.name);
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
    }
}
