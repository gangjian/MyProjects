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
		public string FixedText = string.Empty;
		public string Text = string.Empty;
		public Font TextFont = new Font("SimSun", 12);
		public Brush TextBrush = new SolidBrush(Color.BlueViolet);

		public int Width = 240;
		public int Height = 30;

		public int Pos_X = 10;
		public int Pos_Y = 10;

		public HMI_TEXTBOX(int id)
		{
			this.Id = id;
		}

		public void DrawTextBox(Graphics g)
		{
			g.FillRectangle(new SolidBrush(Color.LightGray), this.Pos_X, this.Pos_Y, this.Width, this.Height);
			g.DrawRectangle(new Pen(Color.Black, 1), this.Pos_X, this.Pos_Y, this.Width, this.Height);

			g.DrawString(this.FixedText + " : " + this.Text, this.TextFont, this.TextBrush, this.Pos_X + 3, this.Pos_Y + 8);
		}

		public static HMI_TEXTBOX GetTextBoxCtrlInfo(string str)
		{
			List<HMI_CTRL_PROPERTY> propertyList = ComProc.GetPropertyList(str);
			int? id = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "ID"));
			int? posX = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "POS_X"));
			int? posY = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "POS_Y"));
			int? width = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "WIDTH"));
			int? height = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "HEIGHT"));
			string fixedTextStr = ComProc.GetPropertyValueStr(propertyList, "FIXED_TEXT");
			string textStr = ComProc.GetPropertyValueStr(propertyList, "TEXT");
			if (null == id)
			{
				return null;
			}
			HMI_TEXTBOX retTbx = new HMI_TEXTBOX((int)id);
			ComProc.SetOptionalIntPropertyVal(ref retTbx.Pos_X, posX);
			ComProc.SetOptionalIntPropertyVal(ref retTbx.Pos_Y, posY);
			ComProc.SetOptionalIntPropertyVal(ref retTbx.Width, width);
			ComProc.SetOptionalIntPropertyVal(ref retTbx.Height, height);
			ComProc.SetOptionalStringPropertyVal(ref retTbx.Text, textStr);
			ComProc.SetOptionalStringPropertyVal(ref retTbx.FixedText, fixedTextStr);
			return retTbx;
		}
	}
}
