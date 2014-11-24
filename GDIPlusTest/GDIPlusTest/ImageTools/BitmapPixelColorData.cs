using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GDIPlusTest.ImageTools
{
    class BitmapPixelColorData
    {
        public Color[,] m_pixelColorMatrix;             // 原始图像的像素矩阵

        public BitmapPixelColorData(Bitmap bitmap)
        {
            m_pixelColorMatrix = new Color[bitmap.Height, bitmap.Width];

            //DateTime startTime = DateTime.Now;
            _loadPixelColorData(bitmap);
            //DateTime finishTime = DateTime.Now;
            //TimeSpan span = (finishTime - startTime);
            //MessageBox.Show("Load Bitmap to PixelColorMatrix in " + span.TotalSeconds.ToString() + " seconds!");
        }

        private void _loadPixelColorData(Bitmap bitmap)
        {
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    m_pixelColorMatrix[i, j] = bitmap.GetPixel(j, i);
                }
            }
        }

    }
}
