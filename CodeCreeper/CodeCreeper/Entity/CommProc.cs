﻿using System;
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
		public static bool IsCommentsStart(string line_str)
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
		public static bool IsStringIdentifier(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return false;
			}
			if (CommProc.GetIdentifierStringLength(str, 0) == str.Length)
			{
				return true;
			}
			else
			{
				return false;
			}
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
		/// <summary>
		/// 把一个element_list拼接成一个字符串
		/// </summary>
		public static string ElementListStrCat(List<CodeElement> element_list, List<string> code_list)
		{
			string ret_str = string.Empty;
			for (int i = 0; i < element_list.Count; i++)
			{
				ret_str += element_list[i].ToString(code_list);
				if (i != element_list.Count - 1)
				{
					if (element_list[i].CloseTo(element_list[i + 1], code_list))
					{
						// 如果跟下一个element紧邻,就直接连接
					}
					else
					{
						// 否则就加入一个空格
						ret_str += " ";
					}
				}
			}
			return ret_str;
		}
		public static CodePosition GetNextQuotePosition(CodePosition first_quote_pos, List<string> code_list)
		{
			Trace.Assert(first_quote_pos.IsValid(code_list));
			CodePosition cur_pos = first_quote_pos;
			char left_quote = code_list[cur_pos.Row][cur_pos.Col];
			char right_quote = '\'';
			if (left_quote.Equals('\''))
			{
				right_quote = '\'';
			}
			else if (left_quote.Equals('\"'))
			{
				right_quote = '\"';
			}
			else
			{
				Trace.Assert(false);
			}
			while (true)
			{
				cur_pos = cur_pos.GetNextPosition(code_list);
				if (null == cur_pos)
				{
					break;
				}
				char ch = code_list[cur_pos.Row][cur_pos.Col];
				if (ch.Equals(right_quote))
				{
					if (0 != cur_pos.Col
						&& code_list[cur_pos.Row][cur_pos.Col - 1].Equals('\\'))
					{
						// 如果前面一个字符是反斜线时表示是转义字符,不是右引号(例如'\'', "\"")
					}
					else
					{
						return cur_pos;
					}
				}
				if (cur_pos.Col == code_list[cur_pos.Row].Length - 1)
				{
					// 到达行末
					if (ch.Equals('\\'))
					{
						// 如果是续行符,就继续
						continue;
					}
					else
					{
						break;
					}
				}
			}
			return null;
		}
		public static int GetNumberStrLength(string line_str, int start_offet)
		{
			Trace.Assert(!string.IsNullOrEmpty(line_str)
							&& start_offet >= 0
							&& start_offet < line_str.Length);
			int ret_len = 0;
			for (int i = start_offet; i < line_str.Length; i++)
			{
				char ch = line_str[i];
				if (Char.IsDigit(ch) || ch.Equals('.'))
				{
				}
				else if (Char.IsLetter(ch) && i != start_offet)
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
	}
}
