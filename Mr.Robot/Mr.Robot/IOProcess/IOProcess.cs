using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Mr.Robot
{
	public static class IOProcess
	{
		/// <summary>
		/// 遍历文件夹
		/// </summary>
		public static void GetAllCCodeFiles(string rootPath, ref List<string> cSourceFilesList, ref List<string> cHeaderFilesList)
		{
			DirectoryInfo di = new DirectoryInfo(rootPath);
			try
			{
				foreach (DirectoryInfo subDir in di.GetDirectories())
				{
					GetAllCCodeFiles(subDir.FullName, ref cSourceFilesList, ref cHeaderFilesList);
				}
				foreach (FileInfo fi in di.GetFiles())
				{
					if (".c" == fi.Extension.ToLower())
					{
						cSourceFilesList.Add(fi.FullName);
					}
					else if (".h" == fi.Extension.ToLower())
					{
						cHeaderFilesList.Add(fi.FullName);
					}
					else
					{
					}
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.ToString());
			}
		}

		/// <summary>
		/// 从完整路径中取得文件名
		/// </summary>
		/// <param varName="fullName"></param>
		/// <returns></returns>
		public static string GetFileName(string fullName, out string path)
		{
			string retName = fullName;
			path = string.Empty;
			int idx = fullName.LastIndexOf('\\');
			if (-1 != idx)
			{
				retName = fullName.Substring(idx + 1).Trim();
				path = fullName.Substring(0, idx);
			}

			return retName;
		}

	}
}
