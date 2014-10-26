using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VisualizeMyLife
{
    public partial class DiagramForm : Form
    {
        private Bitmap _backGroundBitmap;
        private List<DataInfo> _dataList;

        public DiagramForm(List<DataInfo> dataList)
        {
            InitializeComponent();
            _dataList = dataList;
            picBoxResize();
        }

        private void DiagramForm_Resize(object sender, EventArgs e)
        {
            picBoxResize();
        }

        private void picBoxResize()
        {
            this.pictureBox1.Width = this.ClientSize.Width;
            this.pictureBox1.Height = (int)(this.ClientSize.Height * 0.9);
            this.pictureBox1.Location = new Point(0, 0);
            if (null != _backGroundBitmap)
            {
                _backGroundBitmap.Dispose();
            }
            _backGroundBitmap = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            ClassCurvesDrawer curveDrawer = new ClassCurvesDrawer(_backGroundBitmap);
            curveDrawer.DrawBorderFrame();

            if (0 == _dataList.Count)
            {
                return;
            }
            DateTime startDate = _dataList[0]._dateTime;
            List<LinesChartData> bodyWeightList = new List<LinesChartData>();
            foreach (DataInfo di in _dataList)
            {
                LinesChartData data = new LinesChartData();
                data.xTxt = di._dateTime.Year.ToString() + "/" + di._dateTime.Month.ToString() + "/" + di._dateTime.Day.ToString();
                data.yVal = di._bodyWeight;
                TimeSpan ts = (di._dateTime - startDate);
                data.xVal = ts.TotalDays;

                bodyWeightList.Add(data);
            }
            curveDrawer.m_dataList = bodyWeightList;
            curveDrawer.DrawLinesChart();
        }

        private void updateDiagramView()
        {
            pictureBox1.Image = _backGroundBitmap;
        }

        private void DiagramForm_Paint(object sender, PaintEventArgs e)
        {
            updateDiagramView();
        }
    }

}
