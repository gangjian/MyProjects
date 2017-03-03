using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMI_simulator.Ctrls;

namespace HMI_simulator
{
	public class CommandParse
	{
		public static bool DoParse(string cmd_str, ref DataContainer data_container)
		{
			if (string.IsNullOrEmpty(cmd_str))
			{
				return false;
			}
			bool ret = false;
			string cmd_name = GetCmdName(cmd_str);
			switch (cmd_name)
			{
				case "change_page":
					ret = CmdChangePageProc(cmd_str, ref data_container);
					break;
				case "set_ctrl_property":
					ret = CmdSetCtrlProperty(cmd_str, ref data_container);
					break;
				default:
					ret = false;
					break;
			}
			return ret;
		}

		static string GetCmdName(string cmd_str)
		{
			string[] arr = cmd_str.Split(';');
			return arr[0].ToLower().Trim();
		}

		static bool CmdChangePageProc(string cmd_str, ref DataContainer data_container)
		{
			string[] arr = cmd_str.Split(';');
			int val;
			if (arr.Length > 1
				&& int.TryParse(arr[1], out val))
			{
				foreach (var pi in data_container.PageInfoList)
				{
					if (val == pi.PageId)
					{
						data_container.CurPageIndex = val;
						return true;
					}
				}
			}
			return false;
		}

		static bool CmdSetCtrlProperty(string cmd_str, ref DataContainer data_container)
		{
			HMI_PAGE pageInfo = null;
			foreach (var pi in data_container.PageInfoList)
			{
				if (pi.PageId == data_container.CurPageIndex)
				{
					pageInfo = pi;
				}
			}
			if (null == pageInfo)
			{
				return false;
			}
			HMI_CTRL_TYPE ctrlType = ComProc.GetCtrlType(cmd_str);
			switch (ctrlType)
			{
				case HMI_CTRL_TYPE.NULL:
					break;
				case HMI_CTRL_TYPE.BUTTON:
					HMI_BUTTON btn = ComProc.GetButtonCtrlInfo(cmd_str);
					if (null != btn && -1 != btn.Id)
					{
						for (int i = 0; i < pageInfo.ButtonList.Count; i++)
						{
							if (pageInfo.ButtonList[i].Id == btn.Id)
							{
								pageInfo.ButtonList[i] = btn;
								return true;
							}
						}
					}
					break;
				case HMI_CTRL_TYPE.TEXTBOX:
					HMI_TEXTBOX tbx = ComProc.GetTextBoxCtrlInfo(cmd_str);
					if (null != tbx)
					{
						for (int i = 0; i < pageInfo.TextBoxList.Count; i++)
						{
							if (pageInfo.TextBoxList[i].Id == tbx.Id)
							{
								pageInfo.TextBoxList[i] = tbx;
								return true;
							}
						}
					}
					break;
				case HMI_CTRL_TYPE.PROGRESSBAR:
					HMI_PROGRESSBAR pbar = ComProc.GetProgressBarCtrlInfo(cmd_str);
					if (null != pbar)
					{
						for (int i = 0; i < pageInfo.ProgressBarList.Count; i++)
						{
							if (pageInfo.ProgressBarList[i].Id == pbar.Id)
							{
								pageInfo.ProgressBarList[i] = pbar;
								return true;
							}
						}
					}
					break;
				default:
					break;
			}
			return false;
		}
	}
}
