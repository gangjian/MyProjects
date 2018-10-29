using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SourceOutsight
{
	class SO_Common
	{
		public static void GetCodeFileList(string path, List<string> source_list, List<string> header_list)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && Directory.Exists(path));
			Trace.Assert(null != source_list && null != header_list);
			string[] files = Directory.GetFiles(path);
			foreach (var item in files)
			{
				FileInfo fi = new FileInfo(item);
				if (fi.Extension.ToLower().Equals(".c"))
				{
					source_list.Add(item);
				}
				else if (fi.Extension.ToLower().Equals(".h"))
				{
					header_list.Add(item);
				}
			}
			string[] dirs = Directory.GetDirectories(path);
			foreach (var item in dirs)
			{
				GetCodeFileList(item, source_list, header_list);
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

		public static bool IsConditionalComilationStart(string line_str, out int len)
		{
			len = 0;
			if (string.IsNullOrEmpty(line_str)
				|| !line_str.StartsWith("#"))
			{
				return false;
			}
			len = GetIdentifierStringLength(line_str, 1) + 1;
			string cc_str = line_str.Substring(0, len);
			// 注意还有"defined"
			if (cc_str.Equals("#if"))
			{
			}
			else if (cc_str.Equals("#ifdef"))
			{
			}
			else if (cc_str.Equals("#ifndef"))
			{
			}
			else if (cc_str.Equals("#elif"))
			{
			}
			else if (cc_str.Equals("#else"))
			{
			}
			else if (cc_str.Equals("#endif"))
			{
			}
			else if (cc_str.Equals("#pragma"))
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
