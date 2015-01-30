﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GDIPlusTest.ImageTools;

namespace GDIPlusTest.GameRobots.Robot2
{
    class ImageIdentify
    {
        static BitmapPixelColorData _imgData = null;
        public ImageIdentify(Bitmap img)
        {
            System.Diagnostics.Trace.Assert(null != img);
            _imgData = new BitmapPixelColorData(img);
        }

        /// <summary>
        /// 找出游戏画面上石块的区域
        /// </summary>
        /// <returns></returns>
        static public List<Rectangle> FindRocks()
        {
            System.Diagnostics.Trace.Assert(null != _imgData);
            List<Rectangle> rocksList = new List<Rectangle>();
            List<Rectangle> whiteSquareList = getWhiteSquares(_imgData);

            Rectangle rockRect = Rectangle.Empty;
            while (Rectangle.Empty != (rockRect = findSingleRock(ref whiteSquareList, _imgData)))
            {
                rocksList.Add(rockRect);
            }

            return rocksList;
        }

        /// <summary>
        /// 找出游戏画面上树丛的区域
        /// </summary>
        /// <returns></returns>
        static public List<Rectangle> FindTrees()
        {
            System.Diagnostics.Trace.Assert(null != _imgData);
            List<Rectangle> treesList = new List<Rectangle>();

            Point startPoint = new Point(0, 0);
            Rectangle treeRect = Rectangle.Empty;
            while (Rectangle.Empty != (treeRect = findSingleTree(startPoint, _imgData, treesList)))
            {
                treesList.Add(treeRect);
                startPoint = new Point(treeRect.Right, treeRect.Top);
            }

            return treesList;
        }

        /// <summary>
        /// 找出游戏画面上砖墙的区域
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        static public List<Rectangle> FindBricks(Bitmap img)
        {
            List<Rectangle> bricksList = new List<Rectangle>();

            return bricksList;
        }

/////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 找出一片独立的树丛区域
        /// </summary>
        /// <param name="imgData"></param>
        /// <returns></returns>
        static Rectangle findSingleTree(Point pt, BitmapPixelColorData imgData, List<Rectangle> foundList)
        {
            int imgHeight = imgData.m_pixelColorMatrix.GetLength(0);
            int imgWidth = imgData.m_pixelColorMatrix.GetLength(1);

            // 找到第一个绿色的点
            for (int i = pt.Y; i < imgHeight; i++)
            {
                for (int j = pt.X; j < imgWidth; j++)
                {
                    if (inConfirmedArea(j, i, foundList))
                    {
                        continue;
                    }
                    Color pixel = imgData.m_pixelColorMatrix[i, j];
                    if (isColorGreen(pixel))
                    {
                        // 找到第一个绿色点附近所有绿色的点
                        Rectangle rect = findGreenGroup(new Point(j, i), imgData);
                        if (Rectangle.Empty != rect)
                        {
                            return rect;
                        }
                    }
                }
            }

            return Rectangle.Empty;
        }

        /// <summary>
        /// 找出从某一点开始所有相邻的绿色点的集合
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="imgData"></param>
        static Rectangle findGreenGroup(Point pt, BitmapPixelColorData imgData)
        {
            Rectangle rect = Rectangle.Empty;
            int imgHeight = imgData.m_pixelColorMatrix.GetLength(0);
            int imgWidth = imgData.m_pixelColorMatrix.GetLength(1);
            int right = pt.X;
            int left = pt.X;
            int top = pt.Y;
            int bottom = pt.Y;
            int i, j;

            for (i = pt.Y; i < imgHeight; i++)
            {
                int blackCount = 0;
                bool greenFlg = false;
                // 向右侧扫描所有绿色点
                for (j = pt.X; j < imgWidth; j++)
                {
                    Color pixel = imgData.m_pixelColorMatrix[i, j];
                    if (isColorGreen(pixel))
                    {
                        blackCount = 0;
                        greenFlg = true;
                    }
                    else
                    {
                        // 除了绿点, 只允许在中间出现连续的两个黑点
                        if (    (isColorBlack(pixel) || isColorGray(pixel))
                            &&  (j > pt.X)  )
                        {
                            blackCount += 1;
                            if (blackCount > 2)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                // 行末确定右侧边界
                if (greenFlg)
                {
                    if (j - blackCount > right)
                    {
                        if ((pt.X == right)
                            || (Math.Abs(j - blackCount - right) <= 2))
                        {
                            right = j - blackCount;
                        }
                        else
                        {
                            // 差距过大的话, 表明是另一个矩形区域
                            greenFlg = false;
                            break;
                        }
                    }
                    bottom = i;
                }
                else
                {
                    break;
                }
            }

            if (    (right > pt.X)
                &&  (bottom > pt.Y) )
            {
                rect = new Rectangle(pt.X, pt.Y, right - pt.X, bottom - pt.Y);
            }

            return rect;
        }

        /// <summary>
        /// 石块上白色小方块间的最大距离
        /// </summary>
        private const int D_MAX_WHITE_SQUARE_DISTANCE = (10 + 2);

        /// <summary>
        /// 找出一片独立的石块区域
        /// </summary>
        /// <param name="whiteSquareList"></param>
        /// <param name="imgData"></param>
        /// <returns></returns>
        static Rectangle findSingleRock(ref List<Rectangle> whiteSquareList, BitmapPixelColorData imgData)
        {
            int i, j;
            Rectangle rect = Rectangle.Empty;
            if (0 == whiteSquareList.Count)
            {
                return rect;
            }
            // 找到左上方第一个白色小方块
            int firstIdx = 0;
            Rectangle firstRect = whiteSquareList[firstIdx];
            Rectangle thisRect = whiteSquareList[firstIdx];
            for (i = 1; i < whiteSquareList.Count; i++)
            {
                firstRect = whiteSquareList[firstIdx];
                thisRect = whiteSquareList[i];

                bool swapFlag = false;
                // 如果在同一行, 那么更靠左优先
                if (Math.Abs(thisRect.Y - firstRect.Y) < (firstRect.Height / 2))
                {
                    if (thisRect.X < firstRect.X)
                    {
                        swapFlag = true;
                    }
                }
                // 如果在不同行, 那么更靠上优先
                else
                {
                    if (thisRect.Y < firstRect.Y)
                    {
                        swapFlag = true;
                    }
                }
                if (swapFlag)
                {
                    firstIdx = i;
                }
            }
            // 找出跟它距离接近的其它小方块
            List<int> groupList = new List<int>();
            groupList.Add(firstIdx);
            firstRect = whiteSquareList[firstIdx];
            int firstPosX = firstRect.X + (firstRect.Width / 2);
            int firstPosY = firstRect.Y + (firstRect.Height / 2);
            for (i = 0; i < whiteSquareList.Count; i++)
            {
                // 除了左上角的小方块
                if (i == firstIdx)
                {
                    continue;
                }
                thisRect = whiteSquareList[i];
                int thisPosX = thisRect.X + (thisRect.Width / 2);
                int thisPosY = thisRect.Y + (thisRect.Height / 2);
                // 判断与左上角小方块之间的距离
                if (
                    (Math.Abs(thisPosX - firstPosX) <= (D_MAX_WHITE_SQUARE_DISTANCE + (firstRect.Width + thisRect.Width) / 2))
                    &&
                    (Math.Abs(thisPosY - firstPosY) <= (D_MAX_WHITE_SQUARE_DISTANCE + (firstRect.Height + thisRect.Height) / 2))
                    )
                {
                    groupList.Add(i);
                }
            }
            // 确定相邻白色小方块组成的group的区域范围
            int top = firstRect.Y;
            int left = firstRect.X;
            int right = firstRect.X + firstRect.Width;
            int bottom = firstRect.Y + firstRect.Height;
            foreach (int idx in groupList)
            {
                thisRect = whiteSquareList[idx];
                int thisTop =       thisRect.Y;
                int thisLeft =      thisRect.X;
                int thisRight =     thisRect.X + thisRect.Width;
                int thisBottom =    thisRect.Y + thisRect.Height;
                if (thisTop < top)
                {
                    top = thisTop;
                }
                if (thisLeft < left)
                {
                    left = thisLeft;
                }
                if (thisRight > right)
                {
                    right = thisRight;
                }
                if (thisBottom > bottom)
                {
                    bottom = thisBottom;
                }
            }

            // 从白色小方块列表中去掉这些已经确认的小方块
            List<Rectangle> newWhiteSquareList = new List<Rectangle>();
            for (i = 0; i < whiteSquareList.Count; i++)
            {
                if (groupList.Contains(i))
                {
                    continue;
                }
                newWhiteSquareList.Add(whiteSquareList[i]);
            }
            whiteSquareList = newWhiteSquareList;

            // 扩展周围灰色区域的范围
            rect = new Rectangle(left, top, right - left, bottom - top);
            rect = grayExpand(rect, imgData);

            return rect;
        }

        /// <summary>
        /// 灰色扩展
        /// 用以扩展石块边缘的灰色区域范围
        /// </summary>
        /// <param name="rect">扩张前的区域</param>
        /// <returns>扩张后的区域</returns>
        static Rectangle grayExpand(Rectangle rect, BitmapPixelColorData imgData)
        {
            Rectangle expandRect = rect;
            int imgHeight = imgData.m_pixelColorMatrix.GetLength(0);
            int imgWidth = imgData.m_pixelColorMatrix.GetLength(1);
            while (true)
            {
                // 将矩形区域扩大一圈
                rect.X -= 1;
                rect.Y -= 1;
                rect.Width += 2;
                rect.Height += 2;
                // 如果扩大后已超出整个图片的范围, 退出
                if (
                       (rect.X < 0)
                    || (rect.Y < 0)
                    || (rect.X + rect.Width > imgWidth)
                    || (rect.Y + rect.Height > imgHeight)
                    )
                {
                    break;
                }
                // 遍历扩大后的四边是否都是灰色
                bool grayFlag = true;
                for (int j = rect.X; j <= rect.X + rect.Width; j++)
                {
                    Color pixel_1 = imgData.m_pixelColorMatrix[rect.Y, j];
                    Color pixel_2 = imgData.m_pixelColorMatrix[rect.Y + rect.Height, j];
                    if (    !isColorGray(pixel_1)
                        ||  !isColorGray(pixel_2))
                    {
                        grayFlag = false;
                        break;
                    }
                }
                if (grayFlag)
                {
                    for (int i = rect.Y; i <= rect.Y + rect.Height; i++)
                    {
                        Color pixel_1 = imgData.m_pixelColorMatrix[i, rect.X];
                        Color pixel_2 = imgData.m_pixelColorMatrix[i, rect.X + rect.Width];
                        if (    !isColorGray(pixel_1)
                            ||  !isColorGray(pixel_2))
                        {
                            grayFlag = false;
                            break;
                        }
                    }
                    if (grayFlag)
                    {
                        expandRect = rect;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return expandRect;
        }

        /// <summary>
        /// 判断像素点是否是灰色
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        static bool isColorGray(Color pixel)
        {
            if (
                (0 != pixel.R)
                &&
                (pixel.R == pixel.G)
                &&
                (pixel.G == pixel.B)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断像素点是否是绿色
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        static bool isColorGreen(Color pixel)
        {
            if (    (pixel.G > pixel.R)
                &&  (pixel.G > pixel.B))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断像素点是否是黑色
        /// </summary>
        /// <param name="pixel"></param>
        /// <returns></returns>
        static bool isColorBlack(Color pixel)
        {
            if (    (0 == pixel.R)
                &&  (0 == pixel.G)
                &&  (0 == pixel.B))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static List<Rectangle> getWhiteSquares(BitmapPixelColorData img_data)
        {
            List<Rectangle> whiteSquareList = new List<Rectangle>();
            int imgHeight = img_data.m_pixelColorMatrix.GetLength(0);
            int imgWidth = img_data.m_pixelColorMatrix.GetLength(1);

            // 逐行列扫描, 找白色的小方块
            for (int y = 0; y < imgHeight; y++)
            {
                for (int x = 0; x < imgWidth; x++)
                {
                    // 如果该点已在确认区域的范围内, continue.
                    if (inConfirmedArea(x, y, whiteSquareList))
                    {
                        continue;
                    }
                    // 如果发现白色的方块
                    Size foundSquareSize;
                    if (findWhiteSquare(x, y, img_data.m_pixelColorMatrix, out foundSquareSize))
                    {
                        whiteSquareList.Add(new Rectangle(x, y, foundSquareSize.Width, foundSquareSize.Height));
                    }
                }
            }
            return whiteSquareList;
        }

        static bool findWhiteSquare(int x, int y, Color[,] mat, out Size squareSize)
        {
            int imgHeight = mat.GetLength(0);
            int imgWidth = mat.GetLength(1);
            int hSize = 0;
            int vSize = 0;
            squareSize = new Size(0, 0);
            for (int i = y; i < imgHeight; i++)
            {
                for (int j = x; j < imgWidth; j++)
                {
                    if (Color.White.ToArgb() != mat[i,j].ToArgb())
                    {
                        if (    (i == y)
                            &&  (j == x))
                        {
                            return false;
                        }
                        if (j > x)
                        {   // 到达最右端
                            if (hSize == 0)
                            {
                                hSize = (j - x);
                            }
                            else
                            {
                                if (hSize != (j - x))
                                {
                                    return false;
                                }
                            }
                            vSize += 1;
                            break;
                        }
                        else
                        {   // 到达最底端
                            if (hSize != vSize)
                            {
                                return false;
                            }
                            else if (hSize < 5)
                            {
                                return false;
                            }
                            else
                            {
                                squareSize.Height = vSize;
                                squareSize.Width = hSize;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        static bool inConfirmedArea(int x, int y, List<Rectangle> confirmedList)
        {
            foreach (Rectangle rect in confirmedList)
            {
                if ((x >= rect.X)
                    && (x <= (rect.X + rect.Width))
                    && (y >= rect.Y)
                    && (y <= rect.Y + rect.Height))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
