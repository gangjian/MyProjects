using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GDIPlusTest.ImageTools;

namespace GDIPlusTest.GameRobots.Robot2
{
    class ImageIdentify
    {
        static public List<Rectangle> FindRocks(Bitmap img)
        {
            BitmapPixelColorData imgData = new BitmapPixelColorData(img);

            List<Rectangle> whiteSquareList = getWhiteSquares(imgData);

            while (Rectangle.Empty != findSingleRock(ref whiteSquareList))
            {
            }

            return whiteSquareList;
        }

/////////////////////////////////////////////////////////////////////////////////////
        static Rectangle findSingleRock(ref List<Rectangle> whiteSquareList)
        {
            Rectangle rect = Rectangle.Empty;
            // 找到左上方第一个白色小方块
            // 找出跟它距离接近的其它小方块
            // 扩展周围灰色的范围
            // 确定整个石块区域的范围

            return rect;
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
