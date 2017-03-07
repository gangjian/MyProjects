using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HMI_simulator.Ctrls
{
	public class HMI_PROGRESSBAR
	{
		public int Id = -1;
		public int Width = 300;
		public int Height = 20;

		public int Pos_X = 10;
		public int Pos_Y = 10;

		public Brush ProgressBrush = new SolidBrush(Color.DeepSkyBlue);
		public Font TextFont = new Font("SimSun", 10);
		public Brush TextBrush = new SolidBrush(Color.Black);

		public int Value = 0;
		public int Total = 100;

		public HMI_PROGRESSBAR(int id)
		{
			this.Id = id;
		}

		public void DrawProgressBar(Graphics g)
		{
			System.Diagnostics.Trace.Assert(this.Value >= 0 && this.Total >= this.Value);
			int progressLen = (int)(((decimal)this.Value / this.Total) * this.Width);
			g.FillRectangle(this.ProgressBrush, this.Pos_X, this.Pos_Y, progressLen, this.Height);
			g.DrawRectangle(new Pen(Color.Black, 1), this.Pos_X, this.Pos_Y, this.Width, this.Height);
			g.DrawString(this.Value.ToString() + "/" + this.Total.ToString(), this.TextFont, this.TextBrush, this.Pos_X + 3, this.Pos_Y + 3);
		}

		public static HMI_PROGRESSBAR GetProgressBarCtrlInfo(string str)
		{
			List<HMI_CTRL_PROPERTY> propertyList = ComProc.GetPropertyList(str);
			int? id = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "ID"));
			int? posX = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "POS_X"));
			int? posY = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "POS_Y"));
			int? width = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "WIDTH"));
			int? height = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "HEIGHT"));
			int? val = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "VALUE"));
			int? total = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "TOTAL"));
			if (null == id)
			{
				return null;
			}
			HMI_PROGRESSBAR retPbar = new HMI_PROGRESSBAR((int)id);
			ComProc.SetOptionalIntPropertyVal(ref retPbar.Pos_X, posX);
			ComProc.SetOptionalIntPropertyVal(ref retPbar.Pos_Y, posY);
			ComProc.SetOptionalIntPropertyVal(ref retPbar.Width, width);
			ComProc.SetOptionalIntPropertyVal(ref retPbar.Height, height);
			ComProc.SetOptionalIntPropertyVal(ref retPbar.Value, val);
			ComProc.SetOptionalIntPropertyVal(ref retPbar.Total, total);
			return retPbar;
		}
	}
}
