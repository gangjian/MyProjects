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
		public const int Width = 300;
		public const int Height = 20;

		public readonly int Pos_X = 10;
		public readonly int Pos_Y = 10;

		public Brush ProgressBrush = new SolidBrush(Color.DeepSkyBlue);

		public int Value = 0;
		public int Total = 100;

		public HMI_PROGRESSBAR(int value, int total, int x, int y)
		{
			this.Value = value;
			this.Total = total;
			this.Pos_X = x;
			this.Pos_Y = y;
		}

		public void DrawProgressBar(Graphics g)
		{
			System.Diagnostics.Trace.Assert(this.Value >= 0 && this.Total >= this.Value);
			int progressLen = (int)(((decimal)this.Value / this.Total) * HMI_PROGRESSBAR.Width);
			g.FillRectangle(this.ProgressBrush, this.Pos_X, this.Pos_Y, progressLen, HMI_PROGRESSBAR.Height);
			g.DrawRectangle(new Pen(Color.Black, 1), this.Pos_X, this.Pos_Y, HMI_PROGRESSBAR.Width, HMI_PROGRESSBAR.Height);
		}
	}
}
