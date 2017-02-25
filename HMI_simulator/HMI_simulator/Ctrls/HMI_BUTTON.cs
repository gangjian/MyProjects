using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HMI_simulator.Ctrls
{
	public class HMI_BUTTON
	{
		public bool Displayed = false;
		public bool Selected = false;
		public bool Enabled = false;

		public string FixedText = "部品上固定表示文字";
		public Font TextFont = new Font("SimSun", 9);
		public Brush TextBrush = new SolidBrush(Color.DarkBlue);
		public Brush SelectedBrush = new SolidBrush(Color.Violet);
		public Brush UnselectedBrush = new SolidBrush(Color.DeepSkyBlue);

		public const int Width = 120;
		public const int Height = 25;

		public int Pos_X = 10;
		public int Pos_Y = 10;

		public HMI_BUTTON(bool displayed, bool selected, bool enabled, int x, int y)
		{
			this.Displayed = displayed;
			this.Selected = selected;
			this.Enabled = enabled;
			this.Pos_X = x;
			this.Pos_Y = y;
		}

		public void DrawButton(Graphics g)
		{
			if (!this.Displayed)
			{
				return;
			}
			if (this.Selected)
			{
				g.FillRectangle(this.SelectedBrush, this.Pos_X, this.Pos_Y, HMI_BUTTON.Width, HMI_BUTTON.Height);
			}
			else
			{
				g.FillRectangle(this.UnselectedBrush, this.Pos_X, this.Pos_Y, HMI_BUTTON.Width, HMI_BUTTON.Height);
			}
			g.DrawString(this.FixedText, this.TextFont, this.TextBrush, this.Pos_X + 5, this.Pos_Y + 5);
			if (!this.Enabled)
			{
				g.DrawLine(new Pen(Color.Black, 1), this.Pos_X, this.Pos_Y, this.Pos_X + HMI_BUTTON.Width, this.Pos_Y + HMI_BUTTON.Height);
				g.DrawLine(new Pen(Color.Black, 1), this.Pos_X, this.Pos_Y + HMI_BUTTON.Height, this.Pos_X + HMI_BUTTON.Width, this.Pos_Y);
			}
			g.DrawRectangle(new Pen(Color.Black, 1), this.Pos_X, this.Pos_Y, HMI_BUTTON.Width, HMI_BUTTON.Height);
		}

	}
}
