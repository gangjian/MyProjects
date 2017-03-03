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
using System.IO;

using HMI_simulator.Ctrls;

namespace HMI_simulator
{
	public partial class FormMain : Form
	{
		Bitmap _Canvas = null;
		COMMUNICATOR _Communicator = null;
		DataContainer _DataContainer = new DataContainer();

		public FormMain()
		{
			InitializeComponent();
			this._DataContainer.LoadConfigSettings();
			UpdateView();
		}

		void UpdateView()
		{
			HMI_PAGE pageInfo = this._DataContainer.GetCurrentPageInfo();
			if (null == pageInfo)
			{
				return;
			}
			Graphics g = GetPictureboxGraphics(ref this._Canvas);
			g.Clear(Color.DarkGray);
			pageInfo.DrawPage(g);
			this.pictureBox1.Image = this._Canvas;
		}

		Graphics GetPictureboxGraphics(ref Bitmap image)
		{
			image = new System.Drawing.Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
			Graphics g = Graphics.FromImage(image);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			return g;
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			this._Communicator.Stop();
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			this._Communicator = new COMMUNICATOR(this._DataContainer.GetEncodingStr());
			this._Communicator.RecvMsgDel = new RecvSocketMsgDelegate(this.RecvSocketMsg);
			this._Communicator.Start();
		}

		void RecvSocketMsg(string cmd_str)
		{
			if (CommandParse.DoParse(cmd_str, ref this._DataContainer))
			{
				UpdateView();
			}
			//MessageBox.Show(cmd_str.ToString());
		}

		Point MouseDownPoint = new Point(-1, -1);
		Point MouseUpPoint = new Point(-1, -1);

		private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			this.MouseDownPoint.X = e.X;
			this.MouseDownPoint.Y = e.Y;
		}

		private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			this.MouseUpPoint.X = e.X;
			this.MouseUpPoint.Y = e.Y;

			HMI_PAGE pageInfo = this._DataContainer.GetCurrentPageInfo();
			if (null == pageInfo)
			{
				return;
			}
			HMI_BUTTON btn = pageInfo.GetClickButtonObject(this.MouseDownPoint, this.MouseUpPoint);
			if (null == btn)
			{
				return;
			}
			System.Diagnostics.Trace.WriteLine("Button id = " + btn.Id.ToString() + " Clicked!");
		}
	}

	public enum HMI_CTRL_TYPE
	{
		NULL,
		BUTTON,
		TEXTBOX,
		PROGRESSBAR,
	}
}
