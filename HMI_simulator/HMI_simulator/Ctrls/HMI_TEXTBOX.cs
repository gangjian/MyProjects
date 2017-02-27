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
		public int Id = -1;
		public string FixedText = "部品上固定表示文字";
		public string Text = string.Empty;
		public Font TextFont = new Font("SimSun", 12);
		public Brush TextBrush = new SolidBrush(Color.BlueViolet);

		public int Width = 240;
		public int Height = 30;

		public int Pos_X = 10;
		public int Pos_Y = 10;

		public HMI_TEXTBOX(int id, string text, int x, int y, string fixed_text = "部品上固定表示文字", int width = 240, int height = 30)
		{
			this.Id = id;
			this.Text = text;
			this.Pos_X = x;
			this.Pos_Y = y;
			this.FixedText = fixed_text;
			this.Width = width;
			this.Height = height;
		}

		public void DrawTextBox(Graphics g)
		{
			g.FillRectangle(new SolidBrush(Color.LightGray), this.Pos_X, this.Pos_Y, this.Width, this.Height);
			g.DrawRectangle(new Pen(Color.Black, 1), this.Pos_X, this.Pos_Y, this.Width, this.Height);

			g.DrawString(this.FixedText + " : " + this.Text, this.TextFont, this.TextBrush, this.Pos_X + 3, this.Pos_Y + 8);
		}
	}
}
