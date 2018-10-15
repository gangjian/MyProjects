using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Mr.Robot.Creeper
{
	public class Common
	{
		public static CodeSymbol GetNextSymbol(List<string> in_list, ref CodePosition start_pos)
		{
			CodePosition cur_pos = new CodePosition(start_pos);
			while (true)
			{
				char ch = in_list[cur_pos.RowNum][cur_pos.ColNum];
				if (char.IsWhiteSpace(ch))
				{
					// 白空格
				}
				else if (ch.Equals('/'))
				{
					string line_str = in_list[cur_pos.RowNum].Substring(cur_pos.ColNum);
					if (line_str.StartsWith("//")
						 || line_str.StartsWith("/*"))
					{
						// 注释开头
						start_pos = GetNextPosN(in_list, cur_pos, 2);
						return new CodeSymbol(line_str.Substring(0, 2), cur_pos);
					}
				}
				else if (ch.Equals('#'))
				{
					string line_str = in_list[cur_pos.RowNum].Substring(cur_pos.ColNum);
					// 预编译命令
					if (line_str.StartsWith("#include"))
					{
						// 头文件包含
						int len = "#include".Length;
						start_pos = GetNextPosN(in_list, cur_pos, len);
						return new CodeSymbol(line_str.Substring(0, len), cur_pos);
					}
					else if (line_str.StartsWith("#define"))
					{
						// 宏定义
						int len = "#define".Length;
						start_pos = GetNextPosN(in_list, cur_pos, len);
						return new CodeSymbol(line_str.Substring(0, len), cur_pos);
					}
					else if (IsConditionalComilationStart(line_str))
					{
						// 条件编译
					}
				}
				// 标识符
				else if (Char.IsLetter(ch) || ch.Equals('_'))
				{
					int len = GetIdentifierLength(in_list[cur_pos.RowNum], cur_pos.ColNum);
					start_pos = GetNextPosN(in_list, cur_pos, len);
					return new CodeSymbol(in_list[cur_pos.RowNum].Substring(cur_pos.ColNum, len), cur_pos);
				}
				// 字符串,字符
				else if (ch.Equals('"')
					|| ch.Equals('\''))
				{

				}
				// 数字
				// 运算符

				cur_pos = GetNextPos(in_list, cur_pos);
				if (null == cur_pos)
				{
					break;
				}
			}
			return null;
		}

		public static CodePosition GetNextPos(List<string> in_list, CodePosition in_pos)
		{
			if (   null == in_list
				|| null == in_pos
				|| in_pos.RowNum >= in_list.Count
				|| in_pos.ColNum >= in_list[in_pos.RowNum].Length)
			{
				return null;
			}
			if (in_pos.ColNum < in_list[in_pos.RowNum].Length - 1)
			{
				// 非最后一列, 返回同行下一列的位置
				return new CodePosition(in_pos.RowNum, in_pos.ColNum + 1);
			}
			else
			{
				// 最后一列, 返回下一非空行的开头
				int row = in_pos.RowNum + 1;
				while (row < in_list.Count)
				{
					if (0 != in_list[row].Length)
					{
						return new CodePosition(row, 0);
					}
					else
					{
						row += 1;
					}
				}
				return null;
			}
		}

		public static CodePosition GetNextPosN(List<string> in_list, CodePosition in_pos, int count)
		{
			for (int i = 0; i < count; i++)
			{
				in_pos = GetNextPos(in_list, in_pos);
			}
			return in_pos;
		}

		public static CodePosition FindStrPosition(List<string> in_list, CodePosition start_pos, string find_str)
		{
			Trace.Assert(null != in_list && null != start_pos && !string.IsNullOrEmpty(find_str));
			CodePosition cur_pos = start_pos;
			while (true)
			{
				if (null == cur_pos)
				{
					break;
				}
				if (in_list[cur_pos.RowNum].Substring(cur_pos.ColNum).StartsWith(find_str))
				{
					return cur_pos;
				}
				cur_pos = GetNextPos(in_list, cur_pos);
			}
			return null;
		}

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

		public static bool IsCommentStart(string symbol_str)
		{
			if (symbol_str.Equals("/*")
				|| symbol_str.Equals("//"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool IsDefineStart(string symbol_str)
		{
			if (symbol_str.Equals("#define"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool IsIncludeStart(string symbol_str)
		{
			if (symbol_str.Equals("#include"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		static int GetIdentifierLength(string line_str, int start_offet)
		{
			Trace.Assert(	!string.IsNullOrEmpty(line_str)
							&& start_offet >=0
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

		public static bool IsStandardIdentifier(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}
			if (0 != GetIdentifierLength(str, 0))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		static bool IsConditionalComilationStart(string line_str)
		{
			if (string.IsNullOrEmpty(line_str))
			{
				return false;
			}
			// 注意还有"defined"
			if (line_str.StartsWith("#if"))
			{
				
			}
			else if (line_str.StartsWith("#ifdef"))
			{
				
			}
			else if (line_str.StartsWith("#ifndef"))
			{

			}
			else if (line_str.StartsWith("#elif"))
			{

			}
			else if (line_str.StartsWith("#else"))
			{

			}
			else if (line_str.StartsWith("#endif"))
			{

			}
			else if (line_str.StartsWith("#pragma"))
			{
				
			}
			else
			{
				return false;
			}
			return true;
		}
	}

	public class CodeSymbol
	{
		public string TextStr = null;
		public CodePosition StartPosition = null;

		public CodeSymbol(string symbol_str, CodePosition start_pos)
		{
			this.TextStr = symbol_str;
			this.StartPosition = start_pos;
		}
	}
}
