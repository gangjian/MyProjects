using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using HMI_simulator.Ctrls;

namespace HMI_simulator
{
	public class DataContainer
	{
		List<HMI_PAGE> _PageInfoList = null;
		public List<HMI_PAGE> PageInfoList
		{
			get { return _PageInfoList; }
			set { _PageInfoList = value; }
		}

		const int INVALID_PAGE_ID = -10;

		int _CurPageIndex = INVALID_PAGE_ID;
		public int CurPageIndex
		{
			get { return _CurPageIndex; }
			set { _CurPageIndex = value; }
		}

		public void LoadConfigSettings()
		{
			this._PageInfoList = ReadHmiPageCtrlSetting("PageSetting.config");
			this._CurPageIndex = 1;
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

		public HMI_PAGE GetCurrentPageInfo()
		{
			if (INVALID_PAGE_ID == this._CurPageIndex
				|| null == this._PageInfoList
				|| 0 == this._PageInfoList.Count)
			{
				return null;
			}
			foreach (var pi in this._PageInfoList)
			{
				if (this._CurPageIndex == pi.PageId)
				{
					return pi;
				}
			}
			return null;
		}

		public string GetEncodingStr()
		{
			return this.EncodingStr;
		}
	}
}
