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

		public readonly int Pos_X = 10;
		public readonly int Pos_Y = 10;

		public Brush ProgressBrush = new SolidBrush(Color.DeepSkyBlue);
		public Font TextFont = new Font("SimSun", 10);
		public Brush TextBrush = new SolidBrush(Color.Black);

		public int Value = 0;
		public int Total = 100;

		public HMI_PROGRESSBAR(int value, int total, int x, int y, int width = 300, int height = 20)
		{
			this.Value = value;
			this.Total = total;
			this.Pos_X = x;
			this.Pos_Y = y;
			this.Width = width;
			this.Height = height;
		}

		public void DrawProgressBar(Graphics g)
		{
			System.Diagnostics.Trace.Assert(this.Value >= 0 && this.Total >= this.Value);
			int progressLen = (int)(((decimal)this.Value / this.Total) * this.Width);
			g.FillRectangle(this.ProgressBrush, this.Pos_X, this.Pos_Y, progressLen, this.Height);
			g.DrawRectangle(new Pen(Color.Black, 1), this.Pos_X, this.Pos_Y, this.Width, this.Height);
			g.DrawString(this.Value.ToString() + "/" + this.Total.ToString(), this.TextFont, this.TextBrush, this.Pos_X + 3, this.Pos_Y + 3);
		}
	}
}
