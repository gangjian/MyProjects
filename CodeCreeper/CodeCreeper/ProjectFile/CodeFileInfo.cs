﻿using System;
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

		CodeElement GetNextElement()
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
						 && CommProc.IsCommentsStart(GetCurrentLineString()))
				{
					// 注释
					return CommentsProc();
				}
				else if (ch.Equals('#'))
				{
					CodeElement ret_element = GetPrecompileElement();
					if (null != ret_element)
					{
						return ret_element;
					}
				}
				else if (Char.IsLetter(ch) || ch.Equals('_'))
				{
					// 标识符
					string line_str = GetCurrentLineString();
					int len = CommProc.GetIdentifierStringLength(line_str, 0);
					CodeElement ret_element = new CodeElement(ElementType.Identifier, this.currentPosition, len);
					this.currentPosition = this.currentPosition.GetNextPositionN(this.CodeList, len);
					return ret_element;
				}
				else if (ch.Equals('"'))
				{
					// 字符串
					CodePosition right_quote_pos = CommProc.GetNextQuotePosition(this.currentPosition, this.CodeList);
					Trace.Assert(null != right_quote_pos);
					CodeElement ret_element = new CodeElement(ElementType.String, this.currentPosition, right_quote_pos);
					this.currentPosition = right_quote_pos.GetNextPosition(this.CodeList);
					return ret_element;
				}
				else if (ch.Equals('\''))
				{
					// 字符
					CodePosition right_quote_pos = CommProc.GetNextQuotePosition(this.currentPosition, this.CodeList);
					Trace.Assert(null != right_quote_pos);
					CodeElement ret_element = new CodeElement(ElementType.Char, this.currentPosition, right_quote_pos);
					this.currentPosition = right_quote_pos.GetNextPosition(this.CodeList);
					return ret_element;
				}
				else if (Char.IsDigit(ch))
				{
					// 数字
					string line_str = GetCurrentLineString();
					int len = CommProc.GetNumberStrLength(line_str, 0);
					CodeElement ret_element = new CodeElement(ElementType.Number, this.currentPosition, len);
					this.currentPosition = this.currentPosition.GetNextPositionN(this.CodeList, len);
					return ret_element;
				}
				else if (Char.IsSymbol(ch))
				{
					// 运算符
					CodeElement ret_element = new CodeElement(ElementType.Symbol, this.currentPosition, 1);
					this.currentPosition = this.currentPosition.GetNextPosition(this.CodeList);
					return ret_element;
				}
				else if (Char.IsPunctuation(ch))
				{
					CodeElement ret_element = new CodeElement(ElementType.Punctuation, this.currentPosition, 1);
					this.currentPosition = this.currentPosition.GetNextPosition(this.CodeList);
					return ret_element;
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
		CodeElement CommentsProc()
		{
			string line_str = GetCurrentLineString();
			if (line_str.StartsWith("//"))
			{
				// 行注释
				int row = this.currentPosition.Row;
				int end_col = this.CodeList[row].Length - 1;
				int len = end_col - this.currentPosition.Col + 1;
				CodeElement comment_element = new CodeElement(ElementType.Comments, this.currentPosition, len);
				this.currentPosition = this.currentPosition.GetNextPositionN(this.CodeList, len);
				return comment_element;
			}
			else if (line_str.StartsWith("/*"))
			{
				// 块注释
				CodePosition end_pos = FindStringPosition(this.currentPosition, "*/");
				Trace.Assert(null != end_pos);
				CodeElement comment_element = new CodeElement(ElementType.Comments, this.currentPosition, end_pos);
				this.currentPosition = end_pos.GetNextPositionN(this.CodeList, 2);
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

		/// <summary>
		/// 取得从指定位置开始一行内的所有element列表
		/// </summary>
		public List<CodeElement> GetLineElementList(CodePosition start_position,
													bool remove_comments = true)
		{
			List<CodeElement> ret_list = new List<CodeElement>();
			int row = start_position.Row;
			this.currentPosition = start_position;
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
							this.currentPosition = element.GetStartPosition();
							break;
						}
					}
					else
					{
						this.currentPosition = element.GetStartPosition();
						break;
					}
				}
				else
				{
					ret_list.Add(element);
				}
			}
			if (remove_comments)
			{
				List<CodeElement> tmp_list = new List<CodeElement>();
				// 过滤注释
				foreach (var item in ret_list)
				{
					if (item.Type != ElementType.Comments)
					{
						tmp_list.Add(item);
					}
				}
				return tmp_list;
			}
			else
			{
				return ret_list;
			}
		}

		public SyntaxNode GetNextSyntaxNode()
		{
			while (true)
			{
				CodeElement element = this.GetNextElement();
				if (null == element)
				{
					break;
				}
				else
				{
					SyntaxNode ret_node = this.CodeElementProc(element);
					if (null != ret_node)
					{
						return ret_node;
					}
				}
			}
			return null;
		}
		SyntaxNode CodeElementProc(CodeElement element)
		{
			SyntaxNode ret_node = null;
			if (element.Type.Equals(ElementType.Define))
			{
				ret_node = DefineProc(element);
			}
			else if (element.Type.Equals(ElementType.Undefine))
			{
				ret_node = UndefProc(element);
			}
			else if (element.Type.Equals(ElementType.PrecompileCommand))
			{
				ret_node = PrecompileCommandProc(element);
			}
			else if (element.Type.Equals(ElementType.Include))
			{
				ret_node = IncludeProc(element);
			}
			else if (element.Type.Equals(ElementType.PrecompileSwitch))
			{
				ret_node = PrecompileSwitchProc(element);
			}
			else if (element.Type.Equals(ElementType.Identifier))
			{
				//this.ProjectRef.SearchTag(element.ToString(this.CodeList));
			}
			return ret_node;
		}
		SyntaxNode DefineProc(CodeElement def_element)
		{
			DefineInfo def_info = DefineInfo.Parse(def_element, this);
			SyntaxNode def_node = new SyntaxNode("#define", def_info.DefName.GetName());
			def_node.InfoRef = def_info;
			return def_node;
		}
		SyntaxNode UndefProc(CodeElement undef_element)
		{
			UndefineInfo undef_info = UndefineInfo.Parse(undef_element, this);
			SyntaxNode undef_node = new SyntaxNode("#undef", undef_info.UndefName.GetName());
			undef_node.InfoRef = undef_info;
			return undef_node;
		}
		SyntaxNode IncludeProc(CodeElement inc_element)
		{
			IncludeInfo inc_info = IncludeInfo.Parse(inc_element, this);
			SyntaxNode inc_node = new SyntaxNode("include", inc_info.GetName());
			inc_node.InfoRef = inc_info;
			return inc_node;
		}
		SyntaxNode PrecompileSwitchProc(CodeElement element)
		{
			string tag_str = element.ToString(this.CodeList);
			// #if, #ifdef, #ifndef -> new Switch Node
			if (tag_str.Equals("#if")
				|| tag_str.Equals("#ifdef")
				|| tag_str.Equals("#ifndef"))
			{
				string expression_str = GetBranchExpressionStr(element);
				PrecompileBranchNode first_branch_node
					= new PrecompileBranchNode(tag_str, expression_str);
				PrecompileSwitchNode switch_node = new PrecompileSwitchNode(first_branch_node);
				return switch_node;
			}
			// #else, #elif			-> new Branch
			else if (tag_str.Equals("#else"))
			{
				PrecompileBranchNode branch_node = new PrecompileBranchNode(tag_str, null);
				return branch_node;
			}
			else if (tag_str.Equals("#elif"))
			{
				string expression_str = GetBranchExpressionStr(element);
				PrecompileBranchNode branch_node = new PrecompileBranchNode(tag_str, expression_str);
				return branch_node;
			}
			// #endif				-> finish Node
			else if (tag_str.Equals("#endif"))
			{
				return new PrecompileEndIfNode();
			}
			else
			{
				Trace.Assert(false);
				return null;
			}
		}
		string GetBranchExpressionStr(CodeElement element)
		{
			List<CodeElement> element_list = this.GetLineElementList(element.GetStartPosition());
			if (element_list.Count > 1)
			{
				element_list.RemoveAt(0);
				return CommProc.ElementListStrCat(element_list, this.CodeList);
			}
			else
			{
				return string.Empty;
			}
		}
		SyntaxNode PrecompileCommandProc(CodeElement element)
		{
			PrecompileCmdInfo precompile_cmd_info = PrecompileCmdInfo.Parse(element, this);
			SyntaxNode precompile_cmd_node = new SyntaxNode(precompile_cmd_info.CmdName,
								precompile_cmd_info.ContentsScope.ToString(this.codeList));
			precompile_cmd_node.InfoRef = precompile_cmd_info;
			return precompile_cmd_node;
		}
	}
}
