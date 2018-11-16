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

		public TagTreeTable TagTable = new TagTreeTable();
		CodePosition CurrentPosition = new CodePosition(0, 0);
		void DoParse()
		{
			this.CurrentPosition = GetFileStartPosition();
			while (true)
			{
				CodeElement element = GetNextElement();
				if (null == element)
				{
					break;
				}
				//if (this.FullName.EndsWith("AmigoSscCd_ConvEvent.c")
				//	&& element.Row > 224)
				//{
				//	Trace.WriteLine("WTF");
				//}
				CodeElementProc(element);
			}
		}
		CodePosition GetFileStartPosition()
		{
			for (int i = 0; i < this.CodeList.Count; i++)
			{
				if (!string.IsNullOrEmpty(this.CodeList[i].TextStr))
				{
					return new CodePosition(i, 0);
				} 
			}
			return null;
		}
		void CodeElementProc(CodeElement element)
		{
			if (element.Type.Equals(ElementType.Define))
			{
				DefineProc(element);
			}
			else if (element.Type.Equals(ElementType.Undefine))
			{
				UndefProc(element);
			}
			else if (element.Type.Equals(ElementType.PrecompileCommand))
			{
				PrecompileCommandProc(element);
			}
			else if (element.Type.Equals(ElementType.Include))
			{
				IncludeProc(element);
			}
			else if (element.Type.Equals(ElementType.PrecompileSwitch))
			{
				PrecompileSwitchProc(element);
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
				CodeElement comment_element = new CodeElement(ElementType.Comments, cur_pos, len);
				this.CodeList[row].AddElement(comment_element);
				cur_pos = GetNextPosN(cur_pos, len - 1);
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
					CodeElement comment_element = new CodeElement(ElementType.Comments, new CodePosition(row, start_col), len);
					this.CodeList[row].AddElement(comment_element);
				}
				cur_pos = GetNextPosN(end_pos, 1);
			}
			return cur_pos;
		}

		void DefineProc(CodeElement def_element)
		{
			List<CodeElement> element_list = GetLineElementList(def_element.GetStartPosition());
			TagTreeNode macro_node = MakeMacroTagTreeNode(element_list);
			this.TagTable.Add(macro_node);
		}
		TagTreeNode MakeMacroTagTreeNode(List<CodeElement> element_list)
		{
			Trace.Assert(element_list.Count >= 2);
			CodeElement def_element = element_list.First();
			CodeElement macro_element = element_list[1];
			Trace.Assert(macro_element.Type == ElementType.Identifier);
			string macro_name = macro_element.ToString(this.CodeList);
			CodeScope scope = new CodeScope(def_element.GetStartPosition(), element_list.Last().GetEndPosition());
			TagNodeType type = TagNodeType.MacroDef;
			// 判断是否为宏函数
			if (IsMacroFunction(element_list))
			{
				type = TagNodeType.MacroFunc;
			}
			TagTreeNode ret_node = new TagTreeNode(macro_name, null, macro_element.GetStartPosition(), scope, type);
			return ret_node;
		}
		bool IsMacroFunction(List<CodeElement> element_list)
		{
			if (element_list.Count > 3
				&& element_list[2].ToString(this.CodeList).Equals("(")
				&& element_list[2].CloseTo(element_list[1]))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		void UndefProc(CodeElement undef_element)
		{
			List<CodeElement> element_list = GetLineElementList(undef_element.GetStartPosition());
			TagTreeNode macro_node = MakeUndefTagTreeNode(element_list);
			this.TagTable.Add(macro_node);
		}
		TagTreeNode MakeUndefTagTreeNode(List<CodeElement> element_list)
		{
			Trace.Assert(element_list.Count == 2);
			CodeElement undef_element = element_list.First();
			CodeElement macro_element = element_list[1];
			Trace.Assert(macro_element.Type == ElementType.Identifier);
			string macro_name = macro_element.ToString(this.CodeList);
			CodeScope scope = new CodeScope(undef_element.GetStartPosition(), element_list.Last().GetEndPosition());
			TagNodeType type = TagNodeType.Undef;
			TagTreeNode ret_node = new TagTreeNode(undef_element.ToString(this.CodeList), macro_name, macro_element.GetStartPosition(), scope, type);
			return ret_node;
		}

		void IncludeProc(CodeElement include_element)
		{
			List<CodeElement> element_list = GetLineElementList(include_element.GetStartPosition());
			TagTreeNode include_node = MakeIncludeTagTreeNode(element_list);
			this.TagTable.Add(include_node);
		}
		TagTreeNode MakeIncludeTagTreeNode(List<CodeElement> element_list)
		{
			CodeElement include_element = element_list.First();
			element_list.RemoveAt(0);
			string header_name_str = ElementListStrCat(element_list);
			CodeScope scope = new CodeScope(include_element.GetStartPosition(), element_list.Last().GetEndPosition());
			TagNodeType type = TagNodeType.IncludeHeader;
			TagTreeNode ret_node = new TagTreeNode(include_element.ToString(this.CodeList), header_name_str, include_element.GetStartPosition(), scope, type);
			return ret_node;
		}

		void PrecompileSwitchProc(CodeElement precompileswitch_element)
		{
			List<CodeElement> element_list = GetLineElementList(precompileswitch_element.GetStartPosition());
			TagTreeNode precompileswitch_node = MakePrecompileSwitchTagTreeNode(element_list);
			this.TagTable.Add(precompileswitch_node);
		}
		TagTreeNode MakePrecompileSwitchTagTreeNode(List<CodeElement> element_list)
		{
			Trace.Assert(element_list.Count > 0);
			CodeElement switch_element = element_list.First();
			string expression_str = null;
			if (element_list.Count > 1)
			{
				element_list.RemoveAt(0);
				expression_str = ElementListStrCat(element_list);
			}
			CodeScope scope = new CodeScope(switch_element.GetStartPosition(), element_list.Last().GetEndPosition());
			TagNodeType type = TagNodeType.PrecompileSwitch;
			string tag_str = switch_element.ToString(this.CodeList);
			Trace.Assert(tag_str.StartsWith("#"));
			tag_str = "#" + tag_str.Substring(1).Trim();	// 这样做是为了防止'#'后面有空格,比如"# if"
			TagTreeNode ret_node = new TagTreeNode(tag_str, expression_str, switch_element.GetStartPosition(), scope, type);
			return ret_node;
		}

		void PrecompileCommandProc(CodeElement precompilecommand_element)
		{
			List<CodeElement> element_list = GetLineElementList(precompilecommand_element.GetStartPosition());
			TagTreeNode precompilecommand_node = MakePrecompileCommandTagTreeNode(element_list);
			this.TagTable.Add(precompilecommand_node);
		}
		TagTreeNode MakePrecompileCommandTagTreeNode(List<CodeElement> element_list)
		{
			Trace.Assert(element_list.Count > 0);
			CodeElement command_element = element_list.First();
			string expression_str = null;
			if (element_list.Count > 1)
			{
				element_list.RemoveAt(0);
				expression_str = ElementListStrCat(element_list);
			}
			CodeScope scope = new CodeScope(command_element.GetStartPosition(), element_list.Last().GetEndPosition());
			TagNodeType type = TagNodeType.PrecompileCommand;
			TagTreeNode ret_node = new TagTreeNode(command_element.ToString(this.CodeList), expression_str, command_element.GetStartPosition(), scope, type);
			return ret_node;
		}

		CodeElement GetNextElement()
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
					CodeElement ret_element = GetPrecompileElement(cur_pos);
					if (null != ret_element)
					{
						return ret_element;
					}
				}
				else if (Char.IsLetter(ch) || ch.Equals('_'))
				{
					// 标识符
					string line_str = this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col);
					int len = SO_Common.GetIdentifierStringLength(line_str, 0);
					CodeElement ret_element = new CodeElement(ElementType.Identifier, cur_pos, len);
					this.CurrentPosition = GetNextPosN(cur_pos, len);
					return ret_element;
				}
				else if (ch.Equals('"'))
				{
					// 字符串
					string line_str = this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col + 1);
					int idx = GetNextQuoteIndex('"', line_str);
					Trace.Assert(-1 != idx);
					int len = idx + 2;
					CodeElement ret_element = new CodeElement(ElementType.String, cur_pos, len);
					this.CurrentPosition = GetNextPosN(cur_pos, len);
					return ret_element;
				}
				else if (ch.Equals('\''))
				{
					// 字符
					string line_str = this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col + 1);
					int idx = GetNextQuoteIndex('\'', line_str);
					Trace.Assert(-1 != idx);
					int len = idx + 2;
					//Trace.Assert(3 == len);	// 可能会有像'\n'这样转义字符的情况
					CodeElement ret_element = new CodeElement(ElementType.Char, cur_pos, len);
					this.CurrentPosition = GetNextPosN(cur_pos, len);
					return ret_element;
				}
				else if (Char.IsDigit(ch))
				{
					// 数字
					string line_str = this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col);
					int len = SO_Common.GetNumberStrLength(line_str, 0);
					CodeElement ret_element = new CodeElement(ElementType.Number, cur_pos, len);
					this.CurrentPosition = GetNextPosN(cur_pos, len);
					return ret_element;
				}
				else if (Char.IsSymbol(ch))
				{
					// 运算符
					CodeElement ret_element = new CodeElement(ElementType.Symbol, cur_pos, 1);
					this.CurrentPosition = GetNextPos(cur_pos);
					return ret_element;
				}
				else if (Char.IsPunctuation(ch))
				{
					CodeElement ret_element = new CodeElement(ElementType.Punctuation, cur_pos, 1);
					this.CurrentPosition = GetNextPos(cur_pos);
					return ret_element;
				}
				cur_pos = GetNextPos(cur_pos);
				if (null == cur_pos)
				{
					break;
				}
			}
			return null;
		}
		int GetNextQuoteIndex(char left_quote, string line_str)
		{
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
			for (int i = 0; i < line_str.Length; i++)
			{
				if (line_str[i].Equals(right_quote))
				{
					if (0 != i && line_str[i - 1].Equals('\\'))
					{
						// 例如'\'', "\"": 如果前面一个字符是反斜线时表示是转义字符,不是右引号
						continue;
					}
					else
					{
						return i;
					}
				}
			}
			return -1;
		}
		CodeElement GetPrecompileElement(CodePosition cur_pos)
		{
			string line_str = this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col + 1).Trim();
			int keyword_len = SO_Common.GetIdentifierStringLength(line_str, 0);
			string keyword = line_str.Substring(0, keyword_len);
			string element_str = "#" + keyword;
			int keyword_idx = this.CodeList[cur_pos.Row].TextStr.Substring(cur_pos.Col + 1).IndexOf(keyword);
			ElementType element_type = ElementType.Unknown;
			if (element_str.Equals("#include"))
			{
				// 头文件包含
				element_type = ElementType.Include;
			}
			else if (element_str.Equals("#define"))
			{
				// 宏定义
				element_type = ElementType.Define;
			}
			else if (element_str.Equals("#undef"))
			{
				element_type = ElementType.Undefine;
			}
			else if (SO_Common.IsConditionalComilationStart(element_str))
			{
				// 条件编译
				element_type = ElementType.PrecompileSwitch;
			}
			else if (element_str.Equals("#error")
					 || element_str.Equals("#warning")
					 || element_str.Equals("#line"))
			{
				element_type = ElementType.PrecompileCommand;
			}
			else
			{
				//Trace.Assert(false);
				// 函数体内嵌汇编代码中可能有'#',因为暂时还没对应函数体处理,所以暂时忽略
				return null;
			}
			int len = keyword_idx + keyword_len + 1;
			CodeElement ret_element = new CodeElement(element_type, cur_pos, len);
			this.CurrentPosition = GetNextPosN(cur_pos, len);
			return ret_element;
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
		/// 取得一行内的element_list
		/// </summary>
		List<CodeElement> GetLineElementList(CodePosition start_position)
		{
			List<CodeElement> ret_list = new List<CodeElement>();
			int row = start_position.Row;
			this.CurrentPosition = start_position;
			while (true)
			{
				CodeElement element = GetNextElement();
				if (null == element)
				{
					break;
				}
				else if (element.Row != row)
				{
					if (ret_list.Last().ToString(this.CodeList).Equals("\\"))
					{
						// 续行符
						row = element.Row;
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
					ret_list.Add(element);
				}
			}
			return ret_list;
		}
		/// <summary>
		/// 把一个element_list拼接成一个字符串
		/// </summary>
		string ElementListStrCat(List<CodeElement> element_list)
		{
			string ret_str = string.Empty;
			for (int i = 0; i < element_list.Count; i++)
			{
				ret_str += element_list[i].ToString(this.CodeList);
				if (i != element_list.Count - 1)
				{
					if (element_list[i].CloseTo(element_list[i + 1]))
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
		List<CodeElement> ElementList = new List<CodeElement>();

		public SO_CodeLineInfo(string code_line_str)
		{
			this.TextStr = code_line_str;
		}

		public void AddElement(CodeElement mark)
		{
			this.ElementList.Add(mark);
		}
	}

	class CodeElement
	{
		public ElementType Type = ElementType.Unknown;
		public int Row = -1;
		public int Offset = -1;
		public int Len = 0;
		public CodeElement(ElementType type, CodePosition start_pos, int len)
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
		/// 判断两个element的位置是否紧邻
		/// </summary>
		public bool CloseTo(CodeElement another_element)
		{
			if (this.Row == another_element.Row)
			{
				if (this.Offset < another_element.Offset
					&& this.Offset + this.Len == another_element.Offset)
				{
					return true;
				}
				else if (this.Offset > another_element.Offset
					&& another_element.Offset + another_element.Len == this.Offset)
				{
					return true;
				}
			}
			return false;
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
