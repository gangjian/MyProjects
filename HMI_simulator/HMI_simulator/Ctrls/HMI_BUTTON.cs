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
		public bool Displayed = true;
		public bool Selected = false;
		public bool Enabled = true;

		public string FixedText = string.Empty;
		public Font TextFont = new Font("SimSun", 14);
		public Brush TextBrush = new SolidBrush(Color.DarkBlue);
		public Brush SelectedBrush = new SolidBrush(Color.Violet);
		public Brush UnselectedBrush_Enable = new SolidBrush(Color.DeepSkyBlue);
		public Brush UnselectedBrush_Disable = new SolidBrush(Color.Gray);

		public int Width = 120;
		public int Height = 25;

		public int Pos_X = 10;
		public int Pos_Y = 10;

		public HMI_BUTTON(int id)
		{
			this.Id = id;
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

		public static HMI_BUTTON GetButtonCtrlInfo(string str)
		{
			List<HMI_CTRL_PROPERTY> propertyList = ComProc.GetPropertyList(str);
			int? id = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "ID"));
			bool? displayed = ComProc.GetBoolPropertyVal(ComProc.GetPropertyValueStr(propertyList, "DISPLAYED"));
			bool? selected = ComProc.GetBoolPropertyVal(ComProc.GetPropertyValueStr(propertyList, "SELECTED"));
			bool? enabled = ComProc.GetBoolPropertyVal(ComProc.GetPropertyValueStr(propertyList, "ENABLED"));
			int? posX = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "POS_X"));
			int? posY = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "POS_Y"));
			int? width = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "WIDTH"));
			int? height = ComProc.GetIntPropertyVal(ComProc.GetPropertyValueStr(propertyList, "HEIGHT"));
			string fixedTextStr = ComProc.GetPropertyValueStr(propertyList, "FIXED_TEXT");

			if (null == id)
			{
				return null;
			}
			HMI_BUTTON retBtn = new HMI_BUTTON((int)id);
			ComProc.SetOptionalBoolPropertyVal(ref retBtn.Displayed, displayed);
			ComProc.SetOptionalBoolPropertyVal(ref retBtn.Selected, selected);
			ComProc.SetOptionalBoolPropertyVal(ref retBtn.Enabled, enabled);
			ComProc.SetOptionalIntPropertyVal(ref retBtn.Pos_X, posX);
			ComProc.SetOptionalIntPropertyVal(ref retBtn.Pos_Y, posY);
			ComProc.SetOptionalIntPropertyVal(ref retBtn.Width, width);
			ComProc.SetOptionalIntPropertyVal(ref retBtn.Height, height);
			ComProc.SetOptionalStringPropertyVal(ref retBtn.FixedText, fixedTextStr);
			return retBtn;
		}

	}
}
