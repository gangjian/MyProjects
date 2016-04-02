using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.IO;

namespace Mr.Robot
{
	public static class IniFileProcess
	{
		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

		private static string _dirName = System.Windows.Forms.Application.StartupPath + "\\data\\";
		private static string _cfgName = "config.ini";

		private static string _fullName = _dirName + _cfgName;

		public static void IniWriteValue(string Section, string Key, string Value)
		{
			if (!Directory.Exists(_dirName))
			{
				Directory.CreateDirectory(_dirName);
			}
			WritePrivateProfileString(Section, Key, Value, _fullName);
		}

		public static string IniReadValue(string Section, string Key)
		{
			StringBuilder temp = new StringBuilder(500);
			int i = GetPrivateProfileString(Section, Key, "", temp, 500, _fullName);
			return temp.ToString();
		}
	}
}
