using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace CodeCreeper
{
	class CodeFileInfo
	{
		string fullName = null;
		public string FullName
		{
			get { return fullName; }
		}
		List<string> codeList = null;
		public List<string> CodeList
		{
			get { return codeList; }
		}
		CodePosition currentPosition = null;
		public CodeFileInfo(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && File.Exists(path));
			this.fullName = path;
			this.codeList = File.ReadAllLines(path).ToList();
			this.currentPosition = GetFileStartPosition();
		}
		CodePosition GetFileStartPosition()
		{
			if (null == this.CodeList)
			{
				return null;
			}
			for (int i = 0; i < this.CodeList.Count; i++)
			{
				if (!string.IsNullOrEmpty(this.CodeList[i]))
				{
					return new CodePosition(i, 0);
				}
			}
			return null;
		}

		public CodeElement GetNextElement()
		{
			while (true)
			{
				if (null == this.currentPosition)
				{
					break;
				}
				char ch = GetCurrentChar();
				if (char.IsWhiteSpace(ch))
				{
					// 白空格
				}
				else if (ch.Equals('/')
						 && CommProc.IsCommentStart(GetCurrentLineString()))
				{
					// 注释
					return CommentProc();
				}
				else if (ch.Equals('#'))
				{
					CodeElement ret_element = GetPrecompileElement();
					if (null != ret_element)
					{
						return ret_element;
					}
				}
				this.currentPosition = this.currentPosition.GetNextPosition(this.CodeList);
			}
			return null;
		}
		char GetCurrentChar()
		{
			return this.CodeList[this.currentPosition.Row][this.currentPosition.Col];
		}
		string GetCurrentLineString()
		{
			return this.CodeList[this.currentPosition.Row].Substring(this.currentPosition.Col);
		}
		CodePosition FindStringPosition(CodePosition start_pos, string find_str)
		{
			Trace.Assert(null != start_pos);
			Trace.Assert(start_pos.IsValid(this.CodeList));
			Trace.Assert(!string.IsNullOrEmpty(find_str));
			CodePosition cur_pos = new CodePosition(start_pos);
			while (true)
			{
				if (null == cur_pos)
				{
					break;
				}
				if (this.CodeList[cur_pos.Row].Substring(cur_pos.Col).StartsWith(find_str))
				{
					return cur_pos;
				}
				cur_pos = cur_pos.GetNextPosition(this.CodeList);
			}
			return null;
		}

		CodeElement CommentProc()
		{
			string line_str = GetCurrentLineString();
			if (line_str.StartsWith("//"))
			{
				// 行注释
				int row = this.currentPosition.Row;
				int end_col = this.CodeList[row].Length - 1;
				int len = end_col - this.currentPosition.Col + 1;
				CodeElement comment_element = new CodeElement(ElementType.Comments, this.currentPosition, len);
				this.currentPosition = this.currentPosition.GetNextPositionN(this.CodeList, len - 1);
				return comment_element;
			}
			else if (line_str.StartsWith("/*"))
			{
				// 块注释
				CodePosition end_pos = FindStringPosition(this.currentPosition, "*/");
				Trace.Assert(null != end_pos);
				CodeElement comment_element = new CodeElement(ElementType.Comments, this.currentPosition, end_pos);
				this.currentPosition = end_pos.GetNextPosition(this.CodeList);
				return comment_element;
			}
			else
			{
				return null;
			}
		}

		CodeElement GetPrecompileElement()
		{
			string line_str = this.CodeList[this.currentPosition.Row].Substring(this.currentPosition.Col + 1).Trim();
			int keyword_len = CommProc.GetIdentifierStringLength(line_str, 0);
			string keyword = line_str.Substring(0, keyword_len);
			string element_str = "#" + keyword;
			int keyword_idx = this.CodeList[this.currentPosition.Row].Substring(this.currentPosition.Col + 1).IndexOf(keyword);
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
			else if (CommProc.IsPrecompileStart(element_str))
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
			CodeElement ret_element = new CodeElement(element_type, this.currentPosition, len);
			this.currentPosition = this.currentPosition.GetNextPositionN(this.CodeList, len);
			return ret_element;
		}
	}
}
