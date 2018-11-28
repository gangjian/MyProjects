using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper
{
	class CodePosition
	{
		public int Row = -1;
		public int Col = -1;
		public CodePosition(int row, int col)
		{
			this.Row = row;
			this.Col = col;
		}
		/// <summary>
		/// 比较两个位置先后
		/// </summary>
		public int CompareTo(CodePosition another)
		{
			if (this.Row < another.Row)
			{
				return -1;
			}
			else if (this.Row > another.Row)
			{
				return 1;
			}
			else
			{
				if (this.Col < another.Col)
				{
					return -1;
				}
				else if (this.Col > another.Col)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}
		/// <summary>
		/// 判断两个位置是否紧邻
		/// </summary>
		public bool IsCloseTo(CodePosition another, List<string> code_list)
		{
			Trace.Assert(null != code_list);
			Trace.Assert(this.IsValid(code_list));
			Trace.Assert(another.IsValid(code_list));
			if (this.Row == another.Row)
			{
				if (this.Col == another.Col + 1
					|| this.Col == another.Col - 1)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				CodePosition pos_1 = this, pos_2 = another;
				if (this.Row > another.Row)
				{
					pos_1 = another;
					pos_2 = this;
				}
				if (pos_1.Col == code_list[pos_1.Row].Length - 1
					&& 0 == pos_2.Col)
				{
					// 如果不是紧邻的行,中间的全都是空行
					for (int i = pos_1.Row + 1; i < pos_2.Row; i++)
					{
						if (string.Empty != code_list[i])
						{
							return false;
						}
					}
					return true;
				}
				else
				{
					return false;
				}

			}
		}
		public bool IsValid(List<string> code_list)
		{
			Trace.Assert(null != code_list);
			if (this.Row < code_list.Count
				&& this.Col < code_list[this.Row].Length)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public CodePosition GetNextPosition(List<string> code_list)
		{
			Trace.Assert(null != code_list);
			Trace.Assert(this.IsValid(code_list));
			if (this.Col < code_list[this.Row].Length - 1)
			{
				// 非最后一列, 返回同行下一列的位置
				return new CodePosition(this.Row, this.Col + 1);
			}
			else
			{
				// 最后一列, 返回下一非空行的开头
				int row = this.Row + 1;
				while (row < code_list.Count)
				{
					if (0 != code_list[row].Length)
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
		public CodePosition GetNextPositionN(List<string> code_list, int count)
		{
			CodePosition ret_pos = this;
			for (int i = 0; i < count; i++)
			{
				ret_pos = ret_pos.GetNextPosition(code_list);
			}
			return ret_pos;
		}
		public static CodePosition GetFileStartPosition(List<string> code_list)
		{
			for (int i = 0; i < code_list.Count; i++)
			{
				if (!string.IsNullOrEmpty(code_list[i]))
				{
					return new CodePosition(i, 0);
				}
			}
			return null;
		}
	}

	public class CodeScope
	{
		public CodePosition Start = null;
		public CodePosition End = null;
		public CodeScope(CodePosition start, CodePosition end)
		{
			this.Start = start;
			this.End = end;
		}
	}
}
