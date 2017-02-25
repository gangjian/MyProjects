using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Text;

using HMI_simulator.Ctrls;

namespace HMI_simulator
{
	public partial class FormMain : Form
	{
		Bitmap _Canvas = null;
		public FormMain()
		{
			InitializeComponent();
			this._Canvas = new System.Drawing.Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
			Graphics g = Graphics.FromImage(this._Canvas);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			g.Clear(Color.DarkGray);

			DrawHmiButtons(g);
			DrawTextBoxes(g);
			DrawProgressBars(g);

			this.pictureBox1.Image = this._Canvas;
		}

		void DrawHmiButtons(Graphics g)
		{
			List<HMI_BUTTON> btnList = GetButtonList();
			foreach (var btn in btnList)
			{
				btn.DrawButton(g);
			}
		}

		void DrawTextBoxes(Graphics g)
		{
			List<HMI_TEXTBOX> tbxList = GetTextBoxList();
			foreach (var tbx in tbxList)
			{
				tbx.DrawTextBox(g);
			}
		}

		void DrawProgressBars(Graphics g)
		{
			List<HMI_PROGRESSBAR> pbarList = GetProgressBarList();
			foreach (var pbar in pbarList)
			{
				pbar.DrawProgressBar(g);
			}
		}

		List<HMI_BUTTON> GetButtonList()
		{
			List<HMI_BUTTON> retBtnList = new List<HMI_BUTTON>();

			HMI_BUTTON btn = new HMI_BUTTON(true, true, true, 100, 60);
			retBtnList.Add(btn);
			btn = new HMI_BUTTON(true, true, false, 100, 90);
			retBtnList.Add(btn);
			btn = new HMI_BUTTON(true, false, true, 100, 120);
			retBtnList.Add(btn);
			btn = new HMI_BUTTON(true, false, false, 100, 150);
			retBtnList.Add(btn);
			btn = new HMI_BUTTON(false, true, true, 100, 180);
			retBtnList.Add(btn);

			return retBtnList;
		}

		List<HMI_TEXTBOX> GetTextBoxList()
		{
			List<HMI_TEXTBOX> retTbxList = new List<HMI_TEXTBOX>();
			HMI_TEXTBOX tbx = new HMI_TEXTBOX("文字列内容1", 300, 60);
			retTbxList.Add(tbx);

			tbx = new HMI_TEXTBOX("文字列内容2", 300, 100);
			retTbxList.Add(tbx);

			return retTbxList;
		}

		List<HMI_PROGRESSBAR> GetProgressBarList()
		{
			List<HMI_PROGRESSBAR> retPbarList = new List<HMI_PROGRESSBAR>();
			HMI_PROGRESSBAR pbar = new HMI_PROGRESSBAR(50, 360, 300, 180);
			retPbarList.Add(pbar);

			pbar = new HMI_PROGRESSBAR(200, 360, 300, 220);
			retPbarList.Add(pbar);

			pbar = new HMI_PROGRESSBAR(300, 360, 300, 260);
			retPbarList.Add(pbar);

			return retPbarList;
		}
	}
}
