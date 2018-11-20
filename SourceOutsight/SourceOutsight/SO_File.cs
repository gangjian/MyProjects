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
		List<string> CodeList = new List<string>();
		List<CodeElement> ElementList = new List<CodeElement>();
		public SO_File(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && File.Exists(path));
			this.FullName = path;
			this.CodeList = File.ReadAllLines(path).ToList();
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
				//if (this.FullName.EndsWith("\\gbdlist.h")
				//	&& element.Row >= 0)
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
				if (!string.IsNullOrEmpty(this.CodeList[i]))
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
			string line_str = this.CodeList[cur_pos.Row].Substring(cur_pos.Col);
			if (line_str.StartsWith("//"))
			{
				// 行注释
				int row = cur_pos.Row;
				int end_col = this.CodeList[row].Length - 1;
				int len = end_col - cur_pos.Col + 1;
				CodeElement comment_element = new CodeElement(ElementType.Comments, cur_pos, len);
				this.ElementList.Add(comment_element);
				cur_pos = cur_pos.GetNextPosN(this.CodeList, len - 1);
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
						end_col = this.CodeList[row].Length - 1;
					}
					CodePosition s_pos = new CodePosition(row, start_col);
					int len = end_col - start_col + 1;
					CodeElement comment_element = new CodeElement(ElementType.Comments, new CodePosition(row, start_col), len);
					this.ElementList.Add(comment_element);
				}
				cur_pos = end_pos.GetNextPosN(this.CodeList, 1);
			}
			return cur_pos;
		}

		void DefineProc(CodeElement def_element)
		{
			List<CodeElement> element_list = GetLineElementList(def_element.GetStartPosition());
			TagTreeNode macro_node = MacroProc.MakeDefTagTreeNode(element_list, this.CodeList);
			this.TagTable.Add(macro_node);
		}
		void UndefProc(CodeElement undef_element)
		{
			List<CodeElement> element_list = GetLineElementList(undef_element.GetStartPosition());
			TagTreeNode macro_node = MacroProc.MakeUndefTagTreeNode(element_list, this.CodeList);
			this.TagTable.Add(macro_node);
		}

		void IncludeProc(CodeElement include_element)
		{
			List<CodeElement> element_list = GetLineElementList(include_element.GetStartPosition());
			TagTreeNode include_node = IncProc.MakeIncludeTagTreeNode(element_list, this.CodeList);
			this.TagTable.Add(include_node);
		}

		void PrecompileSwitchProc(CodeElement precompileswitch_element)
		{
			List<CodeElement> element_list = GetLineElementList(precompileswitch_element.GetStartPosition());
			TagTreeNode precompileswitch_node = PrecompileProc.MakePrecompileSwitchTagTreeNode(element_list, this.CodeList);
			this.TagTable.Add(precompileswitch_node);
		}

		void PrecompileCommandProc(CodeElement precompilecommand_element)
		{
			List<CodeElement> element_list = GetLineElementList(precompilecommand_element.GetStartPosition());
			TagTreeNode precompilecommand_node = PrecompileProc.MakePrecompileCommandTagTreeNode(element_list, this.CodeList);
			this.TagTable.Add(precompilecommand_node);
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
				char ch = this.CodeList[cur_pos.Row][cur_pos.Col];
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
					string line_str = this.CodeList[cur_pos.Row].Substring(cur_pos.Col);
					int len = Common.GetIdentifierStringLength(line_str, 0);
					CodeElement ret_element = new CodeElement(ElementType.Identifier, cur_pos, len);
					this.CurrentPosition = cur_pos.GetNextPosN(this.CodeList, len);
					return ret_element;
				}
				else if (ch.Equals('"'))
				{
					// 字符串
					CodePosition right_quote_pos = Common.GetNextQuotePosition(cur_pos, this.CodeList);
					Trace.Assert(null != right_quote_pos);
					CodeElement ret_element = new CodeElement(ElementType.String, cur_pos, right_quote_pos);
					this.CurrentPosition = right_quote_pos.GetNextPos(this.CodeList);
					return ret_element;
				}
				else if (ch.Equals('\''))
				{
					// 字符
					CodePosition right_quote_pos = Common.GetNextQuotePosition(cur_pos, this.CodeList);
					Trace.Assert(null != right_quote_pos);
					CodeElement ret_element = new CodeElement(ElementType.Char, cur_pos, right_quote_pos);
					this.CurrentPosition = right_quote_pos.GetNextPos(this.CodeList);
					return ret_element;
				}
				else if (Char.IsDigit(ch))
				{
					// 数字
					string line_str = this.CodeList[cur_pos.Row].Substring(cur_pos.Col);
					int len = Common.GetNumberStrLength(line_str, 0);
					CodeElement ret_element = new CodeElement(ElementType.Number, cur_pos, len);
					this.CurrentPosition = cur_pos.GetNextPosN(this.CodeList, len);
					return ret_element;
				}
				else if (Char.IsSymbol(ch))
				{
					// 运算符
					CodeElement ret_element = new CodeElement(ElementType.Symbol, cur_pos, 1);
					this.CurrentPosition = cur_pos.GetNextPos(this.CodeList);
					return ret_element;
				}
				else if (Char.IsPunctuation(ch))
				{
					CodeElement ret_element = new CodeElement(ElementType.Punctuation, cur_pos, 1);
					this.CurrentPosition = cur_pos.GetNextPos(this.CodeList);
					return ret_element;
				}
				cur_pos = cur_pos.GetNextPos(this.CodeList);
				if (null == cur_pos)
				{
					break;
				}
			}
			return null;
		}
		CodeElement GetPrecompileElement(CodePosition cur_pos)
		{
			string line_str = this.CodeList[cur_pos.Row].Substring(cur_pos.Col + 1).Trim();
			int keyword_len = Common.GetIdentifierStringLength(line_str, 0);
			string keyword = line_str.Substring(0, keyword_len);
			string element_str = "#" + keyword;
			int keyword_idx = this.CodeList[cur_pos.Row].Substring(cur_pos.Col + 1).IndexOf(keyword);
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
			else if (Common.IsConditionalComilationStart(element_str))
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
			this.CurrentPosition = cur_pos.GetNextPosN(this.CodeList, len);
			return ret_element;
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
				if (this.CodeList[cur_pos.Row].Substring(cur_pos.Col).StartsWith(find_str))
				{
					return cur_pos;
				}
				cur_pos = cur_pos.GetNextPos(this.CodeList);
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
					CodeElement last_element = ret_list.Last();
					if (last_element.ToString(this.CodeList).Equals("\\"))
					{
						// 续行符
						ret_list.Remove(last_element);									// 去掉续行符
						if (element.Row == last_element.Row + 1)
						{
							row = element.Row;
							ret_list.Add(element);
						}
						else
						{
							// 超过一行,说明隔了空行
							this.CurrentPosition = element.GetStartPosition();
							break;
						}
					}
					else
					{
						this.CurrentPosition = element.GetStartPosition();
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
	}
}
