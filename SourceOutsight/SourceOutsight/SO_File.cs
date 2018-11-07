using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SourceOutsight
{
	public class SO_File
	{
		public string FullName = null;
		List<SO_CodeLineInfo> CodeList = new List<SO_CodeLineInfo>();
		public SO_File(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && File.Exists(path));
			this.FullName = path;
			List<string> code_list = File.ReadAllLines(path).ToList();
			foreach (var item in code_list)
			{
				this.CodeList.Add(new SO_CodeLineInfo(item));
			}
			DoParse();
		}

		public IDTreeTable IDTable = new IDTreeTable();
		CodePosition CurrentPosition = new CodePosition(0, 0);
		void DoParse()
		{
			this.CurrentPosition = new CodePosition(0, 0);
			while (true)
			{
				if (null == this.CurrentPosition)
				{
					break;
				}
				CodeTag tag = GetNextTag();
				if (null == tag)
				{
					break;
				}
				else if (tag.Type.Equals(TagType.Define))
				{
					DefineProc(tag);
				}
				else if (tag.Type.Equals(TagType.Undefine))
				{
					UndefProc(tag);
				}
				else if (tag.Type.Equals(TagType.PrecompileCommand))
				{
					PrecompileCommandProc(tag);
				}
				else if (tag.Type.Equals(TagType.Include))
				{
					IncludeProc(tag);
				}
				else if (tag.Type.Equals(TagType.PrecompileSwitch))
				{
					PrecompileSwitchProc(tag);
				}
			}
		}

		CodePosition CommentProc(CodePosition cur_pos)
		{
			string line_str = this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col);
			if (line_str.StartsWith("//"))
			{
				// 行注释
				int row = cur_pos.Row;
				int end_col = this.CodeList[row].TextStr.Length - 1;
				int len = end_col - cur_pos.Col + 1;
				CodeTag comment_tag = new CodeTag(TagType.Comments, cur_pos, len);
				this.CodeList[row].AddTag(comment_tag);
				cur_pos = GetNextPosN(cur_pos, len);
			}
			else if (line_str.StartsWith("/*"))
			{
				// 块注释
				CodePosition end_pos = FindStrPosition(cur_pos, "*/");
				Trace.Assert(null != end_pos);
				for (int row = cur_pos.Row; row <= end_pos.Row; row++)
				{
					int start_col, end_col;
					if (row == cur_pos.Row)
					{
						start_col = cur_pos.Col;
					}
					else
					{
						start_col = 0;
					}
					if (row == end_pos.Row)
					{
						end_col = end_pos.Col;
					}
					else
					{
						end_col = this.CodeList[row].TextStr.Length - 1;
					}
					CodePosition s_pos = new CodePosition(row, start_col);
					int len = end_col - start_col + 1;
					CodeTag comment_tag = new CodeTag(TagType.Comments, new CodePosition(row, start_col), len);
					this.CodeList[row].AddTag(comment_tag);
				}
				cur_pos = GetNextPosN(end_pos, 1);
			}
			return cur_pos;
		}

		void DefineProc(CodeTag def_tag)
		{
			List<CodeTag> tag_list = GetLineTagList(def_tag.GetStartPosition());
			IDTreeNode macro_node = MakeMacroIDTreeNode(tag_list);
			this.IDTable.Add(macro_node);
		}
		IDTreeNode MakeMacroIDTreeNode(List<CodeTag> tag_list)
		{
			Trace.Assert(tag_list.Count >= 2);
			CodeTag def_tag = tag_list.First();
			CodeTag macro_tag = tag_list[1];
			Trace.Assert(macro_tag.Type == TagType.Identifier);
			string macro_name = macro_tag.ToString(this.CodeList);
			CodeScope scope = new CodeScope(def_tag.GetStartPosition(), tag_list.Last().GetEndPosition());
			IDNodeType type = IDNodeType.MacroDef;
			// 判断是否为宏函数
			if (IsMacroFunction(tag_list))
			{
				type = IDNodeType.MacroFunc;
			}
			IDTreeNode ret_node = new IDTreeNode(macro_name, null, macro_tag.GetStartPosition(), scope, type);
			return ret_node;
		}
		bool IsMacroFunction(List<CodeTag> tag_list)
		{
			if (tag_list.Count > 3
				&& tag_list[2].ToString(this.CodeList).Equals("(")
				&& tag_list[2].CloseTo(tag_list[1]))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		void UndefProc(CodeTag undef_tag)
		{
			List<CodeTag> tag_list = GetLineTagList(undef_tag.GetStartPosition());
			IDTreeNode macro_node = MakeUndefIDTreeNode(tag_list);
			this.IDTable.Add(macro_node);
		}
		IDTreeNode MakeUndefIDTreeNode(List<CodeTag> tag_list)
		{
			Trace.Assert(tag_list.Count == 2);
			CodeTag undef_tag = tag_list.First();
			CodeTag macro_tag = tag_list[1];
			Trace.Assert(macro_tag.Type == TagType.Identifier);
			string macro_name = macro_tag.ToString(this.CodeList);
			CodeScope scope = new CodeScope(undef_tag.GetStartPosition(), tag_list.Last().GetEndPosition());
			IDNodeType type = IDNodeType.Undef;
			IDTreeNode ret_node = new IDTreeNode(undef_tag.ToString(this.CodeList), macro_name, macro_tag.GetStartPosition(), scope, type);
			return ret_node;
		}

		void IncludeProc(CodeTag include_tag)
		{
			List<CodeTag> tag_list = GetLineTagList(include_tag.GetStartPosition());
			IDTreeNode include_node = MakeIncludeIDTreeNode(tag_list);
			this.IDTable.Add(include_node);
		}
		IDTreeNode MakeIncludeIDTreeNode(List<CodeTag> tag_list)
		{
			Trace.Assert(tag_list.Count > 3);
			CodeTag include_tag = tag_list.First();
			string left_quote = tag_list[1].ToString(this.CodeList);
			string right_quote = tag_list.Last().ToString(this.CodeList);
			tag_list.RemoveAt(0);
			string header_name_str = TagListStrCat(tag_list);
			CodeScope scope = new CodeScope(include_tag.GetStartPosition(), tag_list.Last().GetEndPosition());
			IDNodeType type = IDNodeType.IncludeHeader;
			IDTreeNode ret_node = new IDTreeNode(include_tag.ToString(this.CodeList), header_name_str, include_tag.GetStartPosition(), scope, type);
			return ret_node;
		}

		void PrecompileSwitchProc(CodeTag precompileswitch_tag)
		{
			List<CodeTag> tag_list = GetLineTagList(precompileswitch_tag.GetStartPosition());
			IDTreeNode precompileswitch_node = MakePrecompileSwitchIDTreeNode(tag_list);
			this.IDTable.Add(precompileswitch_node);
		}
		IDTreeNode MakePrecompileSwitchIDTreeNode(List<CodeTag> tag_list)
		{
			Trace.Assert(tag_list.Count > 0);
			CodeTag switch_tag = tag_list.First();
			string expression_str = null;
			if (tag_list.Count > 1)
			{
				tag_list.RemoveAt(0);
				expression_str = TagListStrCat(tag_list);
			}
			CodeScope scope = new CodeScope(switch_tag.GetStartPosition(), tag_list.Last().GetEndPosition());
			IDNodeType type = IDNodeType.PrecompileSwitch;
			IDTreeNode ret_node = new IDTreeNode(switch_tag.ToString(this.CodeList), expression_str, switch_tag.GetStartPosition(), scope, type);
			return ret_node;
		}

		void PrecompileCommandProc(CodeTag precompilecommand_tag)
		{
			List<CodeTag> tag_list = GetLineTagList(precompilecommand_tag.GetStartPosition());
			IDTreeNode precompilecommand_node = MakePrecompileCommandIDTreeNode(tag_list);
			this.IDTable.Add(precompilecommand_node);
		}
		IDTreeNode MakePrecompileCommandIDTreeNode(List<CodeTag> tag_list)
		{
			Trace.Assert(tag_list.Count > 0);
			CodeTag command_tag = tag_list.First();
			string expression_str = null;
			if (tag_list.Count > 1)
			{
				tag_list.RemoveAt(0);
				expression_str = TagListStrCat(tag_list);
			}
			CodeScope scope = new CodeScope(command_tag.GetStartPosition(), tag_list.Last().GetEndPosition());
			IDNodeType type = IDNodeType.PrecompileCommand;
			IDTreeNode ret_node = new IDTreeNode(command_tag.ToString(this.CodeList), expression_str, command_tag.GetStartPosition(), scope, type);
			return ret_node;
		}

		CodeTag GetNextTag()
		{
			if (null == this.CurrentPosition)
			{
				return null;
			}
			CodePosition cur_pos = new CodePosition(this.CurrentPosition);
			while (true)
			{
				char ch = this.CodeList[cur_pos.Row].TextStr[cur_pos.Col];
				if (char.IsWhiteSpace(ch))
				{
					// 白空格
				}
				else if (ch.Equals('/'))
				{
					// 注释
					cur_pos = CommentProc(cur_pos);
				}
				else if (ch.Equals('#'))
				{
					return GetPrecompileTag(cur_pos);
				}
				else if (Char.IsLetter(ch) || ch.Equals('_'))
				{
					// 标识符
					string line_str = this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col);
					int len = SO_Common.GetIdentifierStringLength(line_str, 0);
					CodeTag ret_tag = new CodeTag(TagType.Identifier, cur_pos, len);
					this.CurrentPosition = GetNextPosN(cur_pos, len);
					return ret_tag;
				}
				else if (ch.Equals('"'))
				{
					// 字符串
					string line_str = this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col);
					int idx = line_str.IndexOf('"');
					Trace.Assert(-1 != idx);
					int len = idx + 1;
					CodeTag ret_tag = new CodeTag(TagType.String, cur_pos, len);
					this.CurrentPosition = GetNextPosN(cur_pos, len);
					return ret_tag;
				}
				else if (ch.Equals('\''))
				{
					// 字符
					string line_str = this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col);
					int idx = line_str.IndexOf('\'');
					Trace.Assert(-1 != idx);
					int len = idx + 1;
					Trace.Assert(3 == len);
					CodeTag ret_tag = new CodeTag(TagType.Char, cur_pos, len);
					this.CurrentPosition = GetNextPosN(cur_pos, len);
					return ret_tag;
				}
				else if (Char.IsDigit(ch))
				{
					// 数字
					string line_str = this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col);
					int len = SO_Common.GetNumberStrLength(line_str, 0);
					CodeTag ret_tag = new CodeTag(TagType.Number, cur_pos, len);
					this.CurrentPosition = GetNextPosN(cur_pos, len);
					return ret_tag;
				}
				else if (Char.IsSymbol(ch))
				{
					// 运算符
					CodeTag ret_tag = new CodeTag(TagType.Symbol, cur_pos, 1);
					this.CurrentPosition = GetNextPos(cur_pos);
					return ret_tag;
				}
				else if (Char.IsPunctuation(ch))
				{
					CodeTag ret_tag = new CodeTag(TagType.Punctuation, cur_pos, 1);
					this.CurrentPosition = GetNextPos(cur_pos);
					return ret_tag;
				}
				cur_pos = GetNextPos(cur_pos);
				if (null == cur_pos)
				{
					break;
				}
			}
			return null;
		}
		CodeTag GetPrecompileTag(CodePosition cur_pos)
		{
			string line_str = this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col);
			int len = SO_Common.GetIdentifierStringLength(line_str, 1);
			string tag_str = line_str.Substring(0, len + 1);
			TagType tag_type = TagType.Unknown;
			if (tag_str.Equals("#include"))
			{
				// 头文件包含
				tag_type = TagType.Include;
			}
			else if (tag_str.Equals("#define"))
			{
				// 宏定义
				tag_type = TagType.Define;
			}
			else if (tag_str.Equals("#undef"))
			{
				tag_type = TagType.Undefine;
			}
			else if (SO_Common.IsConditionalComilationStart(tag_str))
			{
				// 条件编译
				tag_type = TagType.PrecompileSwitch;
			}
			else if (tag_str.Equals("#error")
					 || tag_str.Equals("#warning")
					 || tag_str.Equals("#line"))
			{
				tag_type = TagType.PrecompileCommand;
			}
			else
			{
				Trace.Assert(false);
			}
			CodeTag ret_tag = new CodeTag(tag_type, cur_pos, tag_str.Length);
			this.CurrentPosition = GetNextPosN(cur_pos, tag_str.Length);
			return ret_tag;
		}
		CodePosition GetNextPos(CodePosition cur_pos)
		{
			if (null == cur_pos
				|| cur_pos.Row >= this.CodeList.Count
				|| cur_pos.Col >= this.CodeList[cur_pos.Row].TextStr.Length)
			{
				return null;
			}
			if (cur_pos.Col < this.CodeList[cur_pos.Row].TextStr.Length - 1)
			{
				// 非最后一列, 返回同行下一列的位置
				return new CodePosition(cur_pos.Row, cur_pos.Col + 1);
			}
			else
			{
				// 最后一列, 返回下一非空行的开头
				int row = cur_pos.Row + 1;
				while (row < this.CodeList.Count)
				{
					if (0 != this.CodeList[row].TextStr.Length)
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
		CodePosition GetNextPosN(CodePosition cur_pos, int count)
		{
			for (int i = 0; i < count; i++)
			{
				cur_pos = GetNextPos(cur_pos);
			}
			return cur_pos;
		}
		CodePosition FindStrPosition(CodePosition start_pos, string find_str)
		{
			Trace.Assert(null != start_pos && !string.IsNullOrEmpty(find_str));
			CodePosition cur_pos = start_pos;
			while (true)
			{
				if (null == cur_pos)
				{
					break;
				}
				if (this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col).StartsWith(find_str))
				{
					return cur_pos;
				}
				cur_pos = GetNextPos(cur_pos);
			}
			return null;
		}
		/// <summary>
		/// 取得一行内的tag_list
		/// </summary>
		List<CodeTag> GetLineTagList(CodePosition start_position)
		{
			List<CodeTag> ret_list = new List<CodeTag>();
			int row = start_position.Row;
			this.CurrentPosition = start_position;
			while (true)
			{
				CodeTag tag = GetNextTag();
				if (null == tag)
				{
					break;
				}
				else if (tag.Row != row)
				{
					if (ret_list.Last().ToString(this.CodeList).Equals("\\"))
					{
						// 续行符
						row = tag.Row;
					}
					else
					{
						// 回到行末尾
						int last_row = ret_list.Last().Row;
						int last_col = ret_list.Last().Offset + ret_list.Last().Len - 1;
						CodePosition last_pos = new CodePosition(last_row, last_col);
						this.CurrentPosition = GetNextPos(last_pos);
						break;
					}
				}
				else
				{
					ret_list.Add(tag);
				}
			}
			return ret_list;
		}
		/// <summary>
		/// 把一个tag_list拼接成一个字符串
		/// </summary>
		string TagListStrCat(List<CodeTag> tag_list)
		{
			string ret_str = string.Empty;
			for (int i = 0; i < tag_list.Count; i++)
			{
				ret_str += tag_list[i].ToString(this.CodeList);
				if (i != tag_list.Count - 1)
				{
					if (tag_list[i].CloseTo(tag_list[i + 1]))
					{
						// 如果跟下一个tag紧邻,就直接连接
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
	}

	public class CodePosition
	{
		public int Row = -1;
		public int Col = -1;
		public CodePosition(int row, int col)
		{
			this.Row = row;
			this.Col = col;
		}
		public CodePosition(CodePosition pos)
		{
			this.Row = pos.Row;
			this.Col = pos.Col;
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

	class SO_CodeLineInfo
	{
		public string TextStr = null;
		List<CodeTag> TagList = new List<CodeTag>();

		public SO_CodeLineInfo(string code_line_str)
		{
			this.TextStr = code_line_str;
		}

		public void AddTag(CodeTag mark)
		{
			this.TagList.Add(mark);
		}
	}

	class CodeTag
	{
		public TagType Type = TagType.Unknown;
		public int Row = -1;
		public int Offset = -1;
		public int Len = 0;
		public CodeTag(TagType type, CodePosition start_pos, int len)
		{
			this.Type = type;
			this.Row = start_pos.Row;
			this.Offset = start_pos.Col;
			this.Len = len;
		}

		public CodePosition GetStartPosition()
		{
			return new CodePosition(this.Row, this.Offset);
		}
		public CodePosition GetEndPosition()
		{
			return new CodePosition(this.Row, this.Offset + this.Len - 1);
		}
		public string ToString(List<SO_CodeLineInfo> code_line_list)
		{
			Trace.Assert(null != code_line_list);
			if (this.Row < code_line_list.Count)
			{
				string line_str = code_line_list[this.Row].TextStr;
				if (this.Offset + this.Len <= line_str.Length)
				{
					return line_str.Substring(this.Offset, this.Len);
				}
			}
			return null;
		}
		/// <summary>
		/// 判断两个tag是否紧邻
		/// </summary>
		public bool CloseTo(CodeTag another_tag)
		{
			if (this.Row == another_tag.Row)
			{
				if (this.Offset < another_tag.Offset
					&& this.Offset + this.Len == another_tag.Offset)
				{
					return true;
				}
				else if (this.Offset > another_tag.Offset
					&& another_tag.Offset + another_tag.Len == this.Offset)
				{
					return true;
				}
			}
			return false;
		}
	}

	enum TagType
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
