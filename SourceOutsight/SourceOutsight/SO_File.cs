using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SourceOutsight
{
	class SO_File
	{
		public string FullName = null;
		List<SO_CodeLineInfo> m_LineInfoList = new List<SO_CodeLineInfo>();
		public SO_File(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && File.Exists(path));
			this.FullName = path;
			List<string> code_list = File.ReadAllLines(path).ToList();
			foreach (var item in code_list)
			{
				this.m_LineInfoList.Add(new SO_CodeLineInfo(item));
			}
			DoParse();
		}

		IDTreeTable IDTable = new IDTreeTable();
		CodePosition m_CurrentPosition = new CodePosition(0, 0);
		void DoParse()
		{
			this.m_CurrentPosition = new CodePosition(0, 0);
			while (true)
			{
				if (null == this.m_CurrentPosition)
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

		CodeTag GetNextTag()
		{
			CodePosition cur_pos = new CodePosition(this.m_CurrentPosition);
			while (true)
			{
				char ch = this.m_LineInfoList[cur_pos.Row].TextStr[cur_pos.Col];
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
					string line_str = this.m_LineInfoList[cur_pos.Row].TextStr.Substring(cur_pos.Col);
					int len;
					if (line_str.StartsWith("#include"))
					{
						// 头文件包含
						CodeTag inc_tag = new CodeTag(TagType.Include, cur_pos, "#include".Length);
						this.m_CurrentPosition = GetNextPosN(cur_pos, "#include".Length);
						return inc_tag;
					}
					else if (line_str.StartsWith("#define"))
					{
						// 宏定义
						CodeTag def_tag = new CodeTag(TagType.Define, cur_pos, "#define".Length);
						this.m_CurrentPosition = GetNextPosN(cur_pos, "#define".Length);
						return def_tag;
					}
					else if (SO_Common.IsConditionalComilationStart(line_str, out len))
					{
						// 条件编译
						CodeTag ret_tag = new CodeTag(TagType.PrecompileSwitch, cur_pos, len);
						this.m_CurrentPosition = GetNextPosN(cur_pos, len);
						return ret_tag;
					}
				}
				else if (Char.IsLetter(ch) || ch.Equals('_'))
				{
					// 标识符
					string line_str = this.m_LineInfoList[cur_pos.Row].TextStr.Substring(cur_pos.Col);
					int len = SO_Common.GetIdentifierStringLength(line_str, 0);
					CodeTag ret_tag = new CodeTag(TagType.Identifier, cur_pos, len);
					this.m_CurrentPosition = GetNextPosN(cur_pos, len);
					return ret_tag;
				}
				else if (ch.Equals('"') || ch.Equals('\''))
				{
					// 字符串,字符
				}
				else if (Char.IsDigit(ch))
				{
					// 数字
					string line_str = this.m_LineInfoList[cur_pos.Row].TextStr.Substring(cur_pos.Col);
					int len = SO_Common.GetNumberStrLength(line_str, 0);
					CodeTag ret_tag = new CodeTag(TagType.Number, cur_pos, len);
					this.m_CurrentPosition = GetNextPosN(cur_pos, len);
					return ret_tag;
				}
				else if (Char.IsSymbol(ch))
				{
					// 运算符
					CodeTag ret_tag = new CodeTag(TagType.Symbol, cur_pos, 1);
					this.m_CurrentPosition = GetNextPos(cur_pos);
					return ret_tag;
				}
				else if (Char.IsPunctuation(ch))
				{
					CodeTag ret_tag = new CodeTag(TagType.Punctuation, cur_pos, 1);
					this.m_CurrentPosition = GetNextPos(cur_pos);
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

		CodePosition CommentProc(CodePosition cur_pos)
		{
			string line_str = this.m_LineInfoList[cur_pos.Row].TextStr.Substring(cur_pos.Col);
			if (line_str.StartsWith("//"))
			{
				// 行注释
				int row = cur_pos.Row;
				int end_col = this.m_LineInfoList[row].TextStr.Length - 1;
				int len = end_col - cur_pos.Col + 1;
				CodeTag comment_tag = new CodeTag(TagType.Comments, cur_pos, len);
				this.m_LineInfoList[row].AddTag(comment_tag);
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
						end_col = this.m_LineInfoList[row].TextStr.Length - 1;
					}
					CodePosition s_pos = new CodePosition(row, start_col);
					int len = end_col - start_col + 1;
					CodeTag comment_tag = new CodeTag(TagType.Comments, new CodePosition(row, start_col), len);
					this.m_LineInfoList[row].AddTag(comment_tag);
				}
				cur_pos = GetNextPosN(end_pos, 1);
			}
			return cur_pos;
		}

		void DefineProc(CodeTag def_tag)
		{
			string line_str = this.m_LineInfoList[def_tag.Row].TextStr.Substring(def_tag.Offset);
			List<CodeTag> tag_list = GetDefTagList(def_tag);
			IDTreeNode macro_node = CreateMacroIDTreeNode(def_tag, tag_list);
			// 添加宏名到标识符树形列表
			this.IDTable.Add(macro_node);
		}
		List<CodeTag> GetDefTagList(CodeTag def_tag)
		{
			List<CodeTag> ret_list = new List<CodeTag>();
			int row = def_tag.Row;
			while (true)
			{
				CodeTag tag = GetNextTag();
				if (tag.Row != row)
				{
					if (GetTagTextStr(ret_list.Last()).Equals("\\"))
					{
						// 续行符
						row = tag.Row;
					}
					else
					{
						// 回到#define行末尾
						int last_row = ret_list.Last().Row;
						int last_col = ret_list.Last().Offset + ret_list.Last().Len - 1;
						CodePosition last_pos = new CodePosition(last_row, last_col);
						this.m_CurrentPosition = GetNextPos(last_pos);
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
		IDTreeNode CreateMacroIDTreeNode(CodeTag def_tag, List<CodeTag> tag_list)
		{
			Trace.Assert(0 != tag_list.Count && tag_list.First().Type == TagType.Identifier);
			CodeTag macro_tag = tag_list.First();
			string macro_name = GetTagTextStr(macro_tag);
			CodeScope scope = new CodeScope(def_tag.GetStartPosition(), tag_list.Last().GetEndPosition());
			IDNodeType type = IDNodeType.MacroDef;
			// 判断是否为宏函数
			if (IsMacroFunction(tag_list))
			{
				type = IDNodeType.MacroFunc;
			}
			IDTreeNode ret_node = new IDTreeNode(macro_name, null, new CodePosition(macro_tag.Row, macro_tag.Offset), scope, type);
			return ret_node;
		}
		bool IsMacroFunction(List<CodeTag> tag_list)
		{
			if (tag_list.Count > 2
				&& GetTagTextStr(tag_list[1]).Equals("(")
				&& tag_list[1].Offset == tag_list[0].GetEndPosition().Col + 1)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		void IncludeProc(CodeTag include_tag)
		{

		}

		void PrecompileSwitchProc(CodeTag precompileswitch_tag)
		{

		}

		CodePosition GetNextPos(CodePosition cur_pos)
		{
			if (null == cur_pos
				|| cur_pos.Row >= this.m_LineInfoList.Count
				|| cur_pos.Col >= this.m_LineInfoList[cur_pos.Row].TextStr.Length)
			{
				return null;
			}
			if (cur_pos.Col < this.m_LineInfoList[cur_pos.Row].TextStr.Length - 1)
			{
				// 非最后一列, 返回同行下一列的位置
				return new CodePosition(cur_pos.Row, cur_pos.Col + 1);
			}
			else
			{
				// 最后一列, 返回下一非空行的开头
				int row = cur_pos.Row + 1;
				while (row < this.m_LineInfoList.Count)
				{
					if (0 != this.m_LineInfoList[row].TextStr.Length)
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
				if (this.m_LineInfoList[cur_pos.Row].TextStr.Substring(cur_pos.Col).StartsWith(find_str))
				{
					return cur_pos;
				}
				cur_pos = GetNextPos(cur_pos);
			}
			return null;
		}
		string GetTagTextStr(CodeTag tag)
		{
			if (tag.Row < this.m_LineInfoList.Count)
			{
				string line_str = this.m_LineInfoList[tag.Row].TextStr;
				if (tag.Offset + tag.Len <= line_str.Length)
				{
					return line_str.Substring(tag.Offset, tag.Len);
				}
			}
			return null;
		}
	}

	class CodePosition
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

	class CodeScope
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
	}

	enum TagType
	{
		Unknown,
		Identifier,
		Comments,
		Define,
		Include,
		PrecompileSwitch,
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
