using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace CodeCreeper
{
	class CommProc
	{
		public static void GetCodeFileList(string dir, List<string> src_list, List<string> hd_list)
		{
			Trace.Assert(!string.IsNullOrEmpty(dir) && Directory.Exists(dir));
			src_list = new List<string>();
			hd_list = new List<string>();
			string[] files = Directory.GetFiles(dir);
			foreach (var item in files)
			{
				FileInfo fi = new FileInfo(item);
				if (fi.Extension.ToLower().Equals(".c"))
				{
					src_list.Add(item);
				}
				else if (fi.Extension.ToLower().Equals(".h"))
				{
					hd_list.Add(item);
				}
			}
			string[] dirs = Directory.GetDirectories(dir);
			foreach (var item in dirs)
			{
				GetCodeFileList(item, src_list, hd_list);
			}
		}
		public static bool IsCommentStart(string line_str)
		{
			Trace.Assert(!string.IsNullOrEmpty(line_str));
			if (line_str.StartsWith("//")
				|| line_str.StartsWith("/*"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public static int GetIdentifierStringLength(string line_str, int start_offet)
		{
			Trace.Assert(!string.IsNullOrEmpty(line_str)
							&& start_offet >= 0
							&& start_offet < line_str.Length);
			int ret_len = 0;
			for (int i = start_offet; i < line_str.Length; i++)
			{
				char ch = line_str[i];
				if (Char.IsLetter(ch) || ch.Equals('_'))
				{
				}
				else if (Char.IsDigit(ch) && i != start_offet)
				{
				}
				else
				{
					break;
				}
				ret_len += 1;
			}
			return ret_len;
		}
		public static bool IsPrecompileStart(string element_str)
		{
			if (string.IsNullOrEmpty(element_str)
				|| !element_str.StartsWith("#"))
			{
				return false;
			}
			// 注意还有"defined"
			if (element_str.Equals("#if"))
			{
			}
			else if (element_str.Equals("#ifdef"))
			{
			}
			else if (element_str.Equals("#ifndef"))
			{
			}
			else if (element_str.Equals("#elif"))
			{
			}
			else if (element_str.Equals("#else"))
			{
			}
			else if (element_str.Equals("#endif"))
			{
			}
			else if (element_str.Equals("#pragma"))
			{
			}
			else
			{
				return false;
			}
			return true;
		}
	}
}
