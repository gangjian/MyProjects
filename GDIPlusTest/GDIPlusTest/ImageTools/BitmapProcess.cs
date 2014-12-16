using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

using GDIPlusTest.GameRobots.Robot1;

namespace GDIPlusTest.ImageTools
{
    class BitmapProcess
    {
        private BitmapPixelColorData m_srcBmPixColData;
        private List<BitmapPixelColorData> m_subBmPixColDataList;

        public BitmapProcess(Bitmap srcBitmap, List<Bitmap> subBitmapList)
        {
            System.Diagnostics.Trace.Assert((null != srcBitmap) && (null != subBitmapList) && (0 != subBitmapList.Count));

            m_srcBmPixColData = new BitmapPixelColorData(srcBitmap);

            m_subBmPixColDataList = new List<BitmapPixelColorData>();
            foreach (Bitmap bm in subBitmapList)
            {
                m_subBmPixColDataList.Add(new BitmapPixelColorData(bm));
            }
        }

        public List<FoundPosition> searchSubBitmap(int subIdx)
        {
            List<FoundPosition> foundPosList = new List<FoundPosition>();

            int srcHeight, srcWidth, subHeight, subWidth;
            srcHeight = m_srcBmPixColData.m_pixelColorMatrix.GetLength(0);
            srcWidth = m_srcBmPixColData.m_pixelColorMatrix.GetLength(1);
            subHeight = m_subBmPixColDataList[subIdx].m_pixelColorMatrix.GetLength(0);
            subWidth = m_subBmPixColDataList[subIdx].m_pixelColorMatrix.GetLength(1);
            // 开始对源位图进行逐像素扫描
            for (int i = 0; i < srcHeight; i++)
            {
                for (int j = 0; j < srcWidth; j++)
                {
                    // 如果该点位于已经找到的区域内, 就不用再找了
                    bool bContinueFlg = false;
                    foreach (FoundPosition fp in foundPosList)
                    {
                        if (
                            ((j > fp.X - subWidth) && (j < (fp.X + subWidth)))
                            && ((i >= fp.Y) && (i < (fp.Y + subHeight)))
                            )
                        {
                            bContinueFlg = true;
                            break;
                        }
                    }
                    if (bContinueFlg)
                    {
                        continue;
                    }

                    if (bitmapAdjacentPixelSame(j, i, subIdx))
                    {
                        FoundPosition fPos = new FoundPosition(j, i);
                        fPos.subImgInfo.subIdx = subIdx;
                        fPos.subImgInfo.subWidth = subWidth;
                        fPos.subImgInfo.subHeight = subHeight;
                        foundPosList.Add(fPos);
                        j += subWidth;
                    }
                }
            }

            return foundPosList;
        }

        // 根据临近像素的一致性判断两个位图是否一致
        // 因为屏幕截图到位图格式文件, 对应的像素点可能会有一个像素距离的偏移
        // 所以如果对应像素附近有一致的像素也可以认为位图是一致的
        private bool bitmapAdjacentPixelSame(int x, int y, int subIdx)
        {
            // 取得源位图和目标子位图的宽高
            int srcWidth = m_srcBmPixColData.m_pixelColorMatrix.GetLength(1);
            int srcHeight = m_srcBmPixColData.m_pixelColorMatrix.GetLength(0);
            int subWidth = m_subBmPixColDataList[subIdx].m_pixelColorMatrix.GetLength(1);
            int subHeight = m_subBmPixColDataList[subIdx].m_pixelColorMatrix.GetLength(0);

            // 确保源位图剩余的部分不小于子位图
            // width
            if ((srcWidth - x) < subWidth)
            {
                return false;
            }
            // height
            if ((srcHeight - y) < subHeight)
            {
                return false;
            }

            int srcRow = y;
            int srcCol = x;
            if (compareByRow(srcRow, srcCol, 0, subIdx))    // 如果出现了第一行的内容比较一致
            {
                srcRow += 1;
                // 从子位图的第二行一直到最后一行开始进行逐行比对
                for (int i = 1; i < subHeight; i++)
                {
                    if (compareByRow(srcRow, srcCol, i, subIdx))
                    {
                    }
                    else if (compareByRow(srcRow - 1, srcCol, i, subIdx))
                    {
                        srcRow -= 1;
                    }
                    else if (compareByRow(srcRow + 1, srcCol, i, subIdx))
                    {
                        srcRow += 1;
                    }
                    else
                    {
                        if (i == (subHeight - 1))
                        {
                            // 允许最后一行的内容不一致
                            continue;
                        }
                        return false;
                    }
                    srcRow += 1;
                }
                return true;
            }

            return false;
        }

        private bool compareByRow(int srcRow, int srcCol, int subRow, int subIdx)
        {
            int subWidth = m_subBmPixColDataList[subIdx].m_pixelColorMatrix.GetLength(1);
            int srcWidth = m_srcBmPixColData.m_pixelColorMatrix.GetLength(1);
            for (int i = 0; i < subWidth; i++)
            {
                int subCol = i;
                if (m_subBmPixColDataList[subIdx].m_pixelColorMatrix[subRow, subCol].Equals(m_srcBmPixColData.m_pixelColorMatrix[srcRow, srcCol]))
                {
                }
                else if (((srcCol - 1) >= 0)
                         && (m_subBmPixColDataList[subIdx].m_pixelColorMatrix[subRow, subCol].Equals(m_srcBmPixColData.m_pixelColorMatrix[srcRow, srcCol - 1])))
                {
                }
                else if (((srcCol + 1) < srcWidth)
                         && (m_subBmPixColDataList[subIdx].m_pixelColorMatrix[subRow, subCol].Equals(m_srcBmPixColData.m_pixelColorMatrix[srcRow, srcCol + 1])))
                {
                }
                else
                {
                    return false;
                }
                srcCol += 1;
            }
            return true;
        }

        public static void dumpBitmap2File(Bitmap img, int x, int y, int width, int height, string fullpath)
        {
            System.Diagnostics.Trace.Assert(((x + width) <= img.Width) && ((y + height) <= img.Height));
            StreamWriter sw = new StreamWriter(fullpath, false);
            sw.WriteLine("\r\n\r\n" + DateTime.Now.ToString() + "\r\n");
            for (int i = y; i < (y + height); i++)
            {
                for (int j = x; j < (x + width); j++)
                {
                    Color color = img.GetPixel(j, i);
                    sw.Write(color.R.ToString("X").PadLeft(2, '0') + color.G.ToString("X").PadLeft(2, '0') + color.B.ToString("X").PadLeft(2, '0'));
                    if (j < (x + width - 1))
                    {
                        sw.Write(" ");
                    }
                }
                sw.WriteLine();
            }
            sw.WriteLine("\r\n");
            sw.Close();
        }

    }
}
