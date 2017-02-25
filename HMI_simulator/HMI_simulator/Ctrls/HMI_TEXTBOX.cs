using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HMI_simulator.Ctrls
{
	public class HMI_TEXTBOX
	{
		public string FixedText = "部品上固定表示文字";
		public string Text = string.Empty;
		public Font TextFont = new Font("SimSun", 10);
		public Brush TextBrush = new SolidBrush(Color.BlueViolet);

		public const int Width = 240;
		public const int Height = 30;

		public int Pos_X = 10;
		public int Pos_Y = 10;

		public HMI_TEXTBOX(string text, int x, int y)
		{
			this.Text = text;
			this.Pos_X = x;
			this.Pos_Y = y;
		}

		public void DrawTextBox(Graphics g)
		{
			g.FillRectangle(new SolidBrush(Color.LightGray), this.Pos_X, this.Pos_Y, HMI_TEXTBOX.Width, HMI_TEXTBOX.Height);
			g.DrawRectangle(new Pen(Color.Black, 1), this.Pos_X, this.Pos_Y, HMI_TEXTBOX.Width, HMI_TEXTBOX.Height);

			g.DrawString(this.FixedText + " : " + this.Text, this.TextFont, this.TextBrush, this.Pos_X + 3, this.Pos_Y + 8);
		}
	}
}
