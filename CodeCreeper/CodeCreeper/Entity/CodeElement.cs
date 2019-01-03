using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper
{
	public class CodeElement
	{
		ElementType type = ElementType.Unknown;
		public ElementType Type
		{
			get { return type; }
		}
		int row = -1;
		public int Row
		{
			get { return row; }
		}
		int offset = -1;
		public int Offset
		{
			get { return offset; }
		}
		CodePosition endPos = null;
		internal CodePosition EndPos
		{
			get { return endPos; }
		}
		public CodeElement(ElementType type, CodePosition start_pos, int len)
		{
			this.type = type;
			this.row = start_pos.Row;
			this.offset = start_pos.Col;
			this.endPos = new CodePosition(this.row, this.offset + len - 1);
		}
		public CodeElement(ElementType type, CodePosition start_pos, CodePosition end_pos)
		{
			this.type = type;
			this.row = start_pos.Row;
			this.offset = start_pos.Col;
			this.endPos = end_pos;
		}

		public CodePosition GetStartPosition()
		{
			return new CodePosition(this.row, this.offset);
		}
		public string ToString(List<string> code_line_list)
		{
			Trace.Assert(null != code_line_list);
			Trace.Assert(this.row < code_line_list.Count);
			StringBuilder sb = new StringBuilder();
			for (int i = this.row; i <= this.endPos.Row; i++)
			{
				int start_idx = 0;
				if (this.row == i)
				{
					start_idx = this.offset;
				}
				int end_idx = code_line_list[i].Length - 1;
				if (this.endPos.Row == i)
				{
					end_idx = this.endPos.Col;
				}
				sb.Append(code_line_list[i].Substring(start_idx, end_idx - start_idx + 1));
			}
			return sb.ToString();
		}
		/// <summary>
		/// 判断两个element的位置是否紧邻
		/// </summary>
		public bool CloseTo(CodeElement another_element, List<string> code_list)
		{
			if (this.endPos.CompareTo(another_element.GetStartPosition()) < 0
				&& this.endPos.IsCloseTo(another_element.GetStartPosition(), code_list))
			{
				return true;
			}
			else if (this.GetStartPosition().CompareTo(another_element.endPos) > 0
					 && this.GetStartPosition().IsCloseTo(another_element.endPos, code_list))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
	public enum ElementType
	{
		Unknown,
		Identifier,
		Comments,
		Define,
		Undefine,
		Include,
		PrecompileSwitch,
		PrecompileCommand,
		ReservedWord,
		HeaderName,
		Macro,
		Function,
		Global,
		Local,
		Member,
		Symbol,
		Punctuation,
		Number,
		String,
		Char,
	}
}
