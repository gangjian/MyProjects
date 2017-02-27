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
		List<HMI_PAGE> _PageInfoList = null;
		const int INVALID_PAGE_ID = -10;
		int _CurPageId = INVALID_PAGE_ID;

		public FormMain()
		{
			InitializeComponent();

			_PageInfoList = ReadHmiPageCtrlSetting("PageSetting.config");
			this._CurPageId = 1;
			UpdateView();
		}

		void DrawHmiPageNameText(Graphics g, string name_text)
		{
			g.DrawString(name_text, new Font("Consolas", 14), new SolidBrush(Color.DarkBlue), 5, 5);
		}

		void DrawHmiButtons(Graphics g, List<HMI_BUTTON> btn_list)
		{
			//List<HMI_BUTTON> btnList = GetButtonList();
			foreach (var btn in btn_list)
			{
				btn.DrawButton(g);
			}
		}

		void DrawTextBoxes(Graphics g, List<HMI_TEXTBOX> tbx_list)
		{
			//List<HMI_TEXTBOX> tbxList = GetTextBoxList();
			foreach (var tbx in tbx_list)
			{
				tbx.DrawTextBox(g);
			}
		}

		void DrawProgressBars(Graphics g, List<HMI_PROGRESSBAR> pbar_list)
		{
			//List<HMI_PROGRESSBAR> pbarList = GetProgressBarList();
			foreach (var pbar in pbar_list)
			{
				pbar.DrawProgressBar(g);
			}
		}

		void UpdateView()
		{
			if (INVALID_PAGE_ID == this._CurPageId
				|| null == this._PageInfoList
				|| 0 == this._PageInfoList.Count)
			{
				return;
			}
			HMI_PAGE page_info = null;
			foreach (var pi in this._PageInfoList)
			{
				if (this._CurPageId == pi.PageId)
				{
					page_info = pi;
					break;
				}
			}
			if (null == page_info)
			{
				return;
			}
			this._Canvas = new System.Drawing.Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
			Graphics g = Graphics.FromImage(this._Canvas);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
			g.Clear(Color.DarkGray);

			DrawHmiPageNameText(g, page_info.PageName);
			DrawHmiButtons(g, page_info.ButtonList);
			DrawTextBoxes(g, page_info.TextBoxList);
			DrawProgressBars(g, page_info.ProgressBarList);

			this.pictureBox1.Image = this._Canvas;
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			this._Communicator.Stop();
		}

		private void FormMain_Load(object sender, EventArgs e)
		{
			this._Communicator = new COMMUNICATOR(this.EncodingStr);
			this._Communicator.RecvMsgDel = new RecvSocketMsgDelegate(this.RecvSocketMsg);
			this._Communicator.Start();
		}

		void RecvSocketMsg(string cmd_str)
		{
			if (CommandParse.DoParse(cmd_str, ref _PageInfoList, ref _CurPageId))
			{
				UpdateView();
			}
			//MessageBox.Show(cmd_str.ToString());
		}

		const string PAGE_START = "[PAGE_START]";
		const string PAGE_END = "[PAGE_END]";
		const string ENCODING_INFO = "[Encoding]";

		string EncodingStr = "GB2312";

		public List<HMI_PAGE> ReadHmiPageCtrlSetting(string path)
		{
			if (!File.Exists(path))
			{
				return null;
			}
			List<HMI_PAGE> retPageList = new List<HMI_PAGE>();
			Encoding encoding = Encoding.GetEncoding("Shift_JIS");
			StreamReader sr = new StreamReader(path, encoding);
			List<string> pageCtrlList = new List<string>();
			while (true)
			{
				string rdLine = sr.ReadLine();
				if (null == rdLine)
				{
					break;
				}
				rdLine = rdLine.Trim();
				int idx = rdLine.IndexOf("//");
				if (-1 != idx)
				{
					rdLine = rdLine.Remove(idx).Trim();
				}
				if (string.Empty == rdLine)
				{
					continue;
				}
				else if (rdLine.StartsWith(ENCODING_INFO))
				{
					this.EncodingStr = rdLine.Remove(0, ENCODING_INFO.Length).Trim();
				}
				else if (rdLine.StartsWith(PAGE_START))
				{
					pageCtrlList = new List<string>();
					pageCtrlList.Add(rdLine.Remove(0, PAGE_START.Length));
				}
				else if (rdLine.StartsWith(PAGE_END))
				{
					if (0 != pageCtrlList.Count)
					{
						HMI_PAGE pageInfo = GetHmiPageInfo(pageCtrlList);
						if (null != pageInfo)
						{
							retPageList.Add(pageInfo);
						}
					}
				}
				else
				{
					pageCtrlList.Add(rdLine);
				}
			}
			sr.Close();
			return retPageList;
		}

		HMI_PAGE GetHmiPageInfo(List<string> rdSettingData)
		{
			int pageId;
			string pageName = string.Empty;
			if (0 == rdSettingData.Count
				|| -1 == (pageId = GetPageIdAndName(rdSettingData[0], out pageName)))
			{
				return null;
			}
			HMI_PAGE retPage = new HMI_PAGE(pageId, pageName);
			for (int i = 1; i < rdSettingData.Count; i++)
			{
				HMI_CTRL_TYPE ctrlType = ComProc.GetCtrlType(rdSettingData[i]);
				switch (ctrlType)
				{
					case HMI_CTRL_TYPE.NULL:
						break;
					case HMI_CTRL_TYPE.BUTTON:
						HMI_BUTTON btn = ComProc.GetButtonCtrlInfo(rdSettingData[i]);
						if (null != btn)
						{
							retPage.ButtonList.Add(btn);
						}
						break;
					case HMI_CTRL_TYPE.TEXTBOX:
						HMI_TEXTBOX tbx = ComProc.GetTextBoxCtrlInfo(rdSettingData[i]);
						if (null != tbx)
						{
							retPage.TextBoxList.Add(tbx);
						}
						break;
					case HMI_CTRL_TYPE.PROGRESSBAR:
						HMI_PROGRESSBAR pbar = ComProc.GetProgressBarCtrlInfo(rdSettingData[i]);
						if (null != pbar)
						{
							retPage.ProgressBarList.Add(pbar);
						}
						break;
					default:
						break;
				}
			}
			return retPage;
		}

		int GetPageIdAndName(string str, out string page_name)
		{
			page_name = string.Empty; 
			if (string.IsNullOrEmpty(str))
			{
				return -1;
			}
			int idx = -1;
			int retVal;
			string[] arr = str.Split(';');
			if (2 != arr.Length
				|| !arr[0].StartsWith("PAGE_ID")
				|| -1 == (idx = arr[0].IndexOf('='))
				|| !int.TryParse(arr[0].Substring(idx + 1), out retVal))
			{
				return -1;
			}
			else
			{
				string[] nameArr = arr[1].Split('=');
				if (2 == nameArr.Length && nameArr[0].Equals("NAME"))
				{
					page_name = nameArr[1].Trim();
				}
				return retVal;
			}
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
