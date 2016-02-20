using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace Mr.Robot
{
	public static class IniFileProcess
	{
		[DllImport("kernel32")]
		private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
		[DllImport("kernel32")]
		private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

		private static string _fullname = System.Windows.Forms.Application.StartupPath + "\\data\\config.ini";

		public static string Fullname
		{
			get { return IniFileProcess._fullname; }
			set { IniFileProcess._fullname = value; }
		}

		public static void IniWriteValue(string Section, string Key, string Value)
		{
			WritePrivateProfileString(Section, Key, Value, Fullname);
		}

		public static string IniReadValue(string Section, string Key)
		{
			StringBuilder temp = new StringBuilder(500);
			int i = GetPrivateProfileString(Section, Key, "", temp, 500, Fullname);
			return temp.ToString();
		}
	}
}
