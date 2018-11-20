using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SourceOutsight
{
	class CodeElement
	{
		public ElementType Type = ElementType.Unknown;
		public int Row = -1;
		public int Offset = -1;
		public CodePosition EndPos = null;
		public CodeElement(ElementType type, CodePosition start_pos, int len)
		{
			this.Type = type;
			this.Row = start_pos.Row;
			this.Offset = start_pos.Col;
			this.EndPos = new CodePosition(this.Row, this.Offset + len - 1);
		}
		public CodeElement(ElementType type, CodePosition start_pos, CodePosition end_pos)
		{
			this.Type = type;
			this.Row = start_pos.Row;
			this.Offset = start_pos.Col;
			this.EndPos = end_pos;
		}

		public CodePosition GetStartPosition()
		{
			return new CodePosition(this.Row, this.Offset);
		}
		public string ToString(List<string> code_line_list)
		{
			Trace.Assert(null != code_line_list);
			Trace.Assert(this.Row < code_line_list.Count);
			string ret_str = string.Empty;
			for (int i = this.Row; i <= this.EndPos.Row; i++)
			{
				int start_idx = 0;
				if (this.Row == i)
				{
					start_idx = this.Offset;
				}
				int end_idx = code_line_list[i].Length - 1;
				if (this.EndPos.Row == i)
				{
					end_idx = this.EndPos.Col;
				}
				ret_str += code_line_list[i].Substring(start_idx, end_idx - start_idx + 1);
			}
			return ret_str;
		}
		/// <summary>
		/// 判断两个element的位置是否紧邻
		/// </summary>
		public bool CloseTo(CodeElement another_element, List<string> code_list)
		{
			if (this.EndPos.CompareTo(another_element.GetStartPosition()) < 0
				&& this.EndPos.CloseTo(another_element.GetStartPosition(), code_list))
			{
				return true;
			}
			else if (this.GetStartPosition().CompareTo(another_element.EndPos) > 0
					 && this.GetStartPosition().CloseTo(another_element.EndPos, code_list))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
	enum ElementType
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
