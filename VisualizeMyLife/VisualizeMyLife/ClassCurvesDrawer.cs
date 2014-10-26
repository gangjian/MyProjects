using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace VisualizeMyLife
{
    class ClassCurvesDrawer
    {
        private Bitmap _bitMap;

        // 以下四个量确定了绘图区域的大小暨边框的位置
        private double _XStartPos = -1;     // X轴开始位置
        private double _XEndPos = -1;       // X轴结束位置
        private double _YStartPos = -1;     // Y轴开始位置
        private double _YEndPos = -1;       // Y轴结束为止

        public DrawerProperties m_properties;      // 属性信息
        public List<LinesChartData> m_dataList;

        public ClassCurvesDrawer(Bitmap bitMap)
        {
            System.Diagnostics.Trace.Assert(null != bitMap);
            _bitMap = bitMap;

            _XStartPos = _bitMap.Width * 0.03;
            _XEndPos = _bitMap.Width * (1 - 0.03);
            _YStartPos = _bitMap.Height * 0.05;
            _YEndPos = _bitMap.Height * (1 - 0.05);

            Graphics g = Graphics.FromImage(_bitMap);
            g.Clear(Color.Black);
        }

        public void DrawBorderFrame()
        {
            Pen pen = new Pen(Color.Yellow, (float)1.5);
            Graphics g = Graphics.FromImage(_bitMap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            g.DrawRectangle(pen, (float)_XStartPos, (float)_YStartPos, (float)(_XEndPos - _XStartPos), (float)(_YEndPos - _YStartPos));
        }

        public void DrawLinesChart()
        {
            // 换算数据列表里每个点的横纵坐标值
            double xMinVal = m_dataList[0].xVal;
            double xMaxVal = m_dataList[0].xVal;
            double yMinVal = m_dataList[0].yVal;
            double yMaxVal = m_dataList[0].yVal;

            foreach (LinesChartData data in m_dataList)
            {
                if (data.xVal < xMinVal)
                {
                    xMinVal = data.xVal;
                }
                else if (data.xVal > xMaxVal)
                {
                    xMaxVal = data.xVal;
                }
                if (data.yVal < yMinVal)
                {
                    yMinVal = data.yVal;
                }
                else if (data.yVal > yMaxVal)
                {
                    yMaxVal = data.yVal;
                }
            }
            double xScale = (xMaxVal - xMinVal) / (_XEndPos - _XStartPos);
            double yScale = (yMaxVal - yMinVal) / ((_YEndPos - _YStartPos) * 0.6);

            Pen pen = new Pen(Color.Yellow, (float)1.5);
            Graphics g = Graphics.FromImage(_bitMap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Point[] pointArr = new Point[m_dataList.Count];
            int idx = 0;
            foreach (LinesChartData data in m_dataList)
            {
                Point point = new Point();
                point.X = (int)((data.xVal - xMinVal) * xScale + _XStartPos);
                point.Y = (int)((data.yVal - yMinVal) * yScale + _YStartPos);
                pointArr[idx] = point;
                idx++;
            }
            g.DrawLines(pen, pointArr);
            g.FillRectangle(new SolidBrush(Color.Red), 100, 100, 200, 300);
        }
    }

    public class LinesChartData
    {
        public string xTxt = "";
        public double yVal = 0;
        public double xVal = 0;
    }

    public class DrawerProperties
    {
    }

}
