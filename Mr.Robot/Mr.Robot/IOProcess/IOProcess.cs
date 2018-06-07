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
		public static void GetAllCCodeFiles(string root_path,
											List<string> source_file_list,
											List<string> header_file_list,
											List<string> mtpj_file_list,
											List<string> mk_file_list)
		{
			DirectoryInfo di = new DirectoryInfo(root_path);
			try
			{
				foreach (DirectoryInfo subDir in di.GetDirectories())
				{
					GetAllCCodeFiles(subDir.FullName, source_file_list, header_file_list, mtpj_file_list, mk_file_list);
				}
				foreach (FileInfo fi in di.GetFiles())
				{
					if (".c" == fi.Extension.ToLower())
					{
						source_file_list.Add(fi.FullName);
					}
					else if (".h" == fi.Extension.ToLower())
					{
						header_file_list.Add(fi.FullName);
					}
					else if (".mtpj" == fi.Extension.ToLower())
					{
						mtpj_file_list.Add(fi.FullName);
					}
					else if (".mk" == fi.Extension.ToLower())
					{
						mk_file_list.Add(fi.FullName);
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
