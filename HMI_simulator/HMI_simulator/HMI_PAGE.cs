using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMI_simulator.Ctrls;
using System.Drawing;

namespace HMI_simulator
{
	public class HMI_PAGE
	{
		public int PageId = -1;
		public string PageName = string.Empty;
		public List<HMI_BUTTON> ButtonList = new List<HMI_BUTTON>();
		public List<HMI_TEXTBOX> TextBoxList = new List<HMI_TEXTBOX>();
		public List<HMI_PROGRESSBAR> ProgressBarList = new List<HMI_PROGRESSBAR>();

		public HMI_PAGE(int id, string name)
		{
			this.PageId = id;
			this.PageName = name;
		}

		public void DrawPage(Graphics g)
		{
			DrawHmiPageNameText(g);
			DrawHmiButtons(g);
			DrawTextBoxes(g);
			DrawProgressBars(g);
		}

		void DrawHmiPageNameText(Graphics g)
		{
			g.DrawString(this.PageName, new Font("Consolas", 14), new SolidBrush(Color.DarkBlue), 5, 5);
		}

		void DrawHmiButtons(Graphics g)
		{
			//List<HMI_BUTTON> btnList = GetButtonList();
			foreach (var btn in this.ButtonList)
			{
				btn.DrawButton(g);
			}
		}

		void DrawTextBoxes(Graphics g)
		{
			foreach (var tbx in this.TextBoxList)
			{
				tbx.DrawTextBox(g);
			}
		}

		void DrawProgressBars(Graphics g)
		{
			foreach (var pbar in this.ProgressBarList)
			{
				pbar.DrawProgressBar(g);
			}
		}

		public HMI_BUTTON GetClickButtonObject(Point mouse_down, Point mouse_up)
		{
			foreach (var btn in this.ButtonList)
			{
				if (btn.IsPointInside(mouse_down)
					&& btn.IsPointInside(mouse_up))
				{
					return btn;
				}
			}
			return null;
		}
	}
}
