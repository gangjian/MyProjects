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
		public static void GetCodeFileList(string path, List<string> source_list, List<string> header_list)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && Directory.Exists(path));
			source_list = new List<string>();
			header_list = new List<string>();
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
		public static bool IsCommentStart(CodePosition cur_pos, List<string> code_list)
		{
			Trace.Assert(cur_pos.IsValid(code_list));
			string line_str = code_list[cur_pos.Row].Substring(cur_pos.Col);
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
		public static CodePosition FindStrPosition(CodePosition start_pos, string find_str, List<string> code_list)
		{
			Trace.Assert(null != start_pos && !string.IsNullOrEmpty(find_str));
			CodePosition cur_pos = start_pos;
			while (true)
			{
				if (null == cur_pos)
				{
					break;
				}
				if (code_list[cur_pos.Row].Substring(cur_pos.Col).StartsWith(find_str))
				{
					return cur_pos;
				}
				cur_pos = cur_pos.GetNextPosition(code_list);
			}
			return null;
		}
	}
}
