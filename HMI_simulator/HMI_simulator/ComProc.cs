using System;
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
			List<HMI_CTRL_PROPERTY> propertyList = GetPropertyList(str);
			string typeStr = GetPropertyValueStr(propertyList, "ITEM_TYPE");
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

		public static int? GetIntPropertyVal(string str)
		{
			int val;
			if (!string.IsNullOrEmpty(str) && int.TryParse(str, out val))
			{
				return val;
			}
			else
			{
				return null;
			}
		}

		public static bool? GetBoolPropertyVal(string str)
		{
			bool val;
			if (!string.IsNullOrEmpty(str) && bool.TryParse(str, out val))
			{
				return val;
			}
			else
			{
				return null;
			}
		}

		public static void SetOptionalIntPropertyVal(ref int set_val, int? read_val)
		{
			if (null != read_val)
			{
				set_val = (int)read_val;
			}
		}

		public static void SetOptionalBoolPropertyVal(ref bool set_val, bool? read_val)
		{
			if (null != read_val)
			{
				set_val = (bool)read_val;
			}
		}

		public static void SetOptionalStringPropertyVal(ref string set_val, string read_val)
		{
			if (null != read_val)
			{
				set_val = read_val;
			}
		}

		public static List<HMI_CTRL_PROPERTY> GetPropertyList(string property_str)
		{
			System.Diagnostics.Trace.Assert(null != property_str);
			List<HMI_CTRL_PROPERTY> retList = new List<HMI_CTRL_PROPERTY>();
			string[] arr = property_str.Split(';');
			foreach (string item in arr)
			{
				int idx = item.IndexOf('=');
				if (-1 == idx)
				{
					continue;
				}
				string keyStr = item.Substring(0, idx).Trim();
				string valStr = item.Substring(idx + 1).Trim();
				if (!string.IsNullOrEmpty(keyStr)
					&& !string.IsNullOrEmpty(valStr))
				{
					retList.Add(new HMI_CTRL_PROPERTY(keyStr, valStr));
				}
			}
			return retList;
		}

		public static string GetPropertyValueStr(List<HMI_CTRL_PROPERTY> property_list, string key_str)
		{
			foreach (var item in property_list)
			{
				if (item.Key.Equals(key_str))
				{
					return item.Value;
				}
			}
			return null;
		}
	}

	public class HMI_CTRL_PROPERTY
	{
		public string Key = string.Empty;
		public string Value = string.Empty;
		public HMI_CTRL_PROPERTY(string key, string val)
		{
			this.Key = key;
			this.Value = val;
		}
	}
}
