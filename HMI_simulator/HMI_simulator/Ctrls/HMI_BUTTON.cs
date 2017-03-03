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
		public int Id = -1;
		public bool Displayed = false;
		public bool Selected = false;
		public bool Enabled = false;

		public string FixedText = "部品上固定表示文字";
		public Font TextFont = new Font("SimSun", 14);
		public Brush TextBrush = new SolidBrush(Color.DarkBlue);
		public Brush SelectedBrush = new SolidBrush(Color.Violet);
		public Brush UnselectedBrush_Enable = new SolidBrush(Color.DeepSkyBlue);
		public Brush UnselectedBrush_Disable = new SolidBrush(Color.Gray);

		public int Width = 120;
		public int Height = 25;

		public int Pos_X = 10;
		public int Pos_Y = 10;

		public HMI_BUTTON(int id, bool displayed, bool selected, bool enabled, int x, int y, string fixed_text, int width = 120, int height = 25)
		{
			this.Id = id;
			this.Displayed = displayed;
			this.Selected = selected;
			this.Enabled = enabled;
			this.Pos_X = x;
			this.Pos_Y = y;
			this.Width = width;
			this.Height = height;
			this.FixedText = fixed_text;
		}

		public void DrawButton(Graphics g)
		{
			if (!this.Displayed)
			{
				return;
			}
			if (this.Selected)
			{
				g.FillRectangle(this.SelectedBrush, this.Pos_X, this.Pos_Y, this.Width, this.Height);
			}
			else
			{
				if (!this.Enabled)
				{
					g.FillRectangle(this.UnselectedBrush_Disable, this.Pos_X, this.Pos_Y, this.Width, this.Height);
				}
				else
				{
					g.FillRectangle(this.UnselectedBrush_Enable, this.Pos_X, this.Pos_Y, this.Width, this.Height);
				}
			}
			g.DrawString(this.FixedText, this.TextFont, this.TextBrush, this.Pos_X + 5, this.Pos_Y + 5);
			if (!this.Enabled)
			{
				// 画叉
				g.DrawLine(new Pen(Color.Black, 1), this.Pos_X, this.Pos_Y, this.Pos_X + this.Width, this.Pos_Y + this.Height);
				g.DrawLine(new Pen(Color.Black, 1), this.Pos_X, this.Pos_Y + this.Height, this.Pos_X + this.Width, this.Pos_Y);
			}
			g.DrawRectangle(new Pen(Color.Black, 1), this.Pos_X, this.Pos_Y, this.Width, this.Height);
		}

		public bool IsPointInside(Point pt)
		{
			if (   pt.X >= this.Pos_X
				&& pt.X <= this.Pos_X + this.Width - 1

				&& pt.Y >= this.Pos_Y
				&& pt.Y <= this.Pos_Y + this.Height - 1 )
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
