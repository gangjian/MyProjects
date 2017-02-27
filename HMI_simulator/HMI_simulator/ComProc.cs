﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMI_simulator.Ctrls;

namespace HMI_simulator
{
	public class ComProc
	{
		public static HMI_CTRL_TYPE GetCtrlType(string str)
		{
			string[] arr = str.Split(';');
			foreach (string item in arr)
			{
				int idx = item.IndexOf('=');
				if (item.StartsWith("ITEM_TYPE")
					&& -1 != idx)
				{
					string typeStr = item.Substring(idx + 1).Trim();
					switch (typeStr)
					{
						case "BUTTON":
							return HMI_CTRL_TYPE.BUTTON;
						case "TEXT_BOX":
							return HMI_CTRL_TYPE.TEXTBOX;
						case "TEXT_PROGRESSBAR":
							return HMI_CTRL_TYPE.PROGRESSBAR;
						default:
							return HMI_CTRL_TYPE.NULL;
					}
				}
			}
			return HMI_CTRL_TYPE.NULL;
		}

		public static HMI_BUTTON GetButtonCtrlInfo(string str)
		{
			string[] arr = str.Split(';');
			int? id = null;
			bool? displayed = null;
			bool? selected = null;
			bool? enabled = null;
			int? posX = null;
			int? posY = null;
			int? width = null;
			int? height = null;
			string fixedTextStr = null;
			int iVal;
			bool bVal;
			foreach (string item in arr)
			{
				int idx = item.IndexOf('=');
				if (-1 == idx)
				{
					continue;
				}
				string keyStr = item.Substring(0, idx).Trim();
				string valStr = item.Substring(idx + 1).Trim();
				switch (keyStr)
				{
					case "ID":
						if (int.TryParse(valStr, out iVal))
						{
							id = iVal;
						}
						break;
					case "DISPLAYED":
						if (GetBoolVal(valStr, out bVal))
						{
							displayed = bVal;
						}
						break;
					case "SELECTED":
						if (GetBoolVal(valStr, out bVal))
						{
							selected = bVal;
						}
						break;
					case "ENABLED":
						if (GetBoolVal(valStr, out bVal))
						{
							enabled = bVal;
						}
						break;
					case "FIXED_TEXT":
						if (!string.IsNullOrEmpty(valStr))
						{
							fixedTextStr = valStr;
						}
						break;
					case "POS_X":
						if (int.TryParse(valStr, out iVal))
						{
							posX = iVal;
						}
						break;
					case "POS_Y":
						if (int.TryParse(valStr, out iVal))
						{
							posY = iVal;
						}
						break;
					case "WIDTH":
						if (int.TryParse(valStr, out iVal))
						{
							width = iVal;
						}
						break;
					case "HEIGHT":
						if (int.TryParse(valStr, out iVal))
						{
							height = iVal;
						}
						break;
					default:
						break;
				}
			}
			if (null != id
				&& null != displayed
				&& null != selected
				&& null != enabled
				&& null != posX
				&& null != posY
				&& null != fixedTextStr)
			{
				if (null != width && null != height)
				{
					return new HMI_BUTTON((bool)displayed, (bool)selected, (bool)enabled, (int)posX, (int)posY, fixedTextStr, (int)width, (int)height);
				}
				else
				{
					return new HMI_BUTTON((bool)displayed, (bool)selected, (bool)enabled, (int)posX, (int)posY, fixedTextStr);
				}
			}

			return null;
		}

		public static HMI_TEXTBOX GetTextBoxCtrlInfo(string str)
		{
			string[] arr = str.Split(';');
			int? id = null;
			int? posX = null;
			int? posY = null;
			int? width = null;
			int? height = null;
			string fixedTextStr = null;
			string textStr = null;
			int iVal;
			foreach (string item in arr)
			{
				int idx = item.IndexOf('=');
				if (-1 == idx)
				{
					continue;
				}
				string keyStr = item.Substring(0, idx).Trim();
				string valStr = item.Substring(idx + 1).Trim();
				switch (keyStr)
				{
					case "ID":
						if (int.TryParse(valStr, out iVal))
						{
							id = iVal;
						}
						break;
					case "FIXED_TEXT":
						if (!string.IsNullOrEmpty(valStr))
						{
							fixedTextStr = valStr;
						}
						break;
					case "TEXT":
						if (!string.IsNullOrEmpty(valStr))
						{
							textStr = valStr;
						}
						break;
					case "POS_X":
						if (int.TryParse(valStr, out iVal))
						{
							posX = iVal;
						}
						break;
					case "POS_Y":
						if (int.TryParse(valStr, out iVal))
						{
							posY = iVal;
						}
						break;
					case "WIDTH":
						if (int.TryParse(valStr, out iVal))
						{
							width = iVal;
						}
						break;
					case "HEIGHT":
						if (int.TryParse(valStr, out iVal))
						{
							height = iVal;
						}
						break;
					default:
						break;
				}
			}
			if (null != id
				&& null != fixedTextStr
				&& null != textStr
				&& null != posX
				&& null != posY)
			{
				if (null != width && null != height)
				{
					return new HMI_TEXTBOX(textStr, (int)posX, (int)posY, fixedTextStr, (int)width, (int)height);
				}
				else
				{
					return new HMI_TEXTBOX(textStr, (int)posX, (int)posY, fixedTextStr);
				}
			}
			return null;
		}

		public static HMI_PROGRESSBAR GetProgressBarCtrlInfo(string str)
		{
			string[] arr = str.Split(';');
			int? id = null;
			int? posX = null;
			int? posY = null;
			int? width = null;
			int? height = null;
			int? val = null;
			int? total = null;
			int iVal;
			foreach (string item in arr)
			{
				int idx = item.IndexOf('=');
				if (-1 == idx)
				{
					continue;
				}
				string keyStr = item.Substring(0, idx).Trim();
				string valStr = item.Substring(idx + 1).Trim();
				switch (keyStr)
				{
					case "ID":
						if (int.TryParse(valStr, out iVal))
						{
							id = iVal;
						}
						break;
					case "VALUE":
						if (int.TryParse(valStr, out iVal))
						{
							val = iVal;
						}
						break;
					case "TOTAL":
						if (int.TryParse(valStr, out iVal))
						{
							total = iVal;
						}
						break;
					case "POS_X":
						if (int.TryParse(valStr, out iVal))
						{
							posX = iVal;
						}
						break;
					case "POS_Y":
						if (int.TryParse(valStr, out iVal))
						{
							posY = iVal;
						}
						break;
					case "WIDTH":
						if (int.TryParse(valStr, out iVal))
						{
							width = iVal;
						}
						break;
					case "HEIGHT":
						if (int.TryParse(valStr, out iVal))
						{
							height = iVal;
						}
						break;
					default:
						break;
				}
			}
			if (null != id
				&& null != val
				&& null != total
				&& null != posX
				&& null != posY)
			{
				if (null != width && null != height)
				{
					return new HMI_PROGRESSBAR((int)val, (int)total, (int)posX, (int)posY, (int)width, (int)height);
				}
				else
				{
					return new HMI_PROGRESSBAR((int)val, (int)total, (int)posX, (int)posY);
				}
			}
			return null;
		}

		static bool GetBoolVal(string str, out bool val)
		{
			val = false;
			if (!string.IsNullOrEmpty(str))
			{
				if ("true" == str.ToLower().Trim())
				{
					val = true;
					return true;
				}
				else if ("false" == str.ToLower().Trim())
				{
					val = false;
					return true;
				}
			}
			return false;
		}

	}
}