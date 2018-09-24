using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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
				string line_str = in_list[cur_pos.RowNum].Substring(cur_pos.ColNum);
				if (char.IsWhiteSpace(ch))
				{
					// 白空格
				}
				else if (line_str.StartsWith("//")
						 || line_str.StartsWith("/*"))
				{
					// 注释
					start_pos = GetNextPos(in_list, cur_pos, 2);
					return new CodeSymbol(line_str.Substring(0, 2), cur_pos);
				}
				else if (ch.Equals('#'))
				{
					// 预编译命令
					if (line_str.StartsWith("#include"))
					{
						// 头文件包含
						int len = "#include".Length;
						start_pos = GetNextPos(in_list, cur_pos, len);
						return new CodeSymbol(line_str.Substring(0, len), cur_pos);
					}
					else if (line_str.StartsWith("#define"))
					{
						// 宏定义
						int len = "#define".Length;
						start_pos = GetNextPos(in_list, cur_pos, len);
						return new CodeSymbol(line_str.Substring(0, len), cur_pos);
					}
					else if (line_str.StartsWith("#if"))
					{
						// 条件编译开关
					}
				}
				// 标识符
				// 字符串,字符
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

		static CodePosition GetNextPos(List<string> in_list, CodePosition in_pos)
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

		static CodePosition GetNextPos(List<string> in_list, CodePosition in_pos, int count)
		{
			for (int i = 0; i < count; i++)
			{
				in_pos = GetNextPos(in_list, in_pos);
			}
			return in_pos;
		}
	}

	public class CodeSymbol
	{
		public string SymbolStr = null;
		public CodePosition StartPosition = null;

		public CodeSymbol(string symbol_str, CodePosition start_pos)
		{
			this.SymbolStr = symbol_str;
			this.StartPosition = start_pos;
		}
	}
}
