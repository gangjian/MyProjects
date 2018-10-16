using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Mr.Robot.Creeper
{
	public class CCodeProbe
	{
		// 源文件列表(入力)
		List<string> HeaderList = new List<string>();
		List<string> SourceList = new List<string>();

		//public EventHandler UpdateProgressHandler = null;

		// 源文件解析结果(处理)
		List<CodeFileProbeInfo> HeaderProbeResultList = new List<CodeFileProbeInfo>();
		List<CodeFileProbeInfo> SourceProbeResultList = new List<CodeFileProbeInfo>();

		public CCodeProbe(List<string> source_list, List<string> header_list)
		{
			Trace.Assert(null != source_list && null != header_list);
			this.SourceList = source_list;
			this.HeaderList = header_list;
		}

		public CCodeProbe(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && Directory.Exists(path));
			Common.GetCodeFileList(path, this.SourceList, this.HeaderList);
		}

		public void ProbeStart()
		{
			ClearResults();
			foreach (string src_name in this.SourceList)
			{
				CodeFileProbeInfo probe_info = new CodeFileProbeInfo(src_name);
				probe_info.GoProbe();
			}
		}

		void ClearResults()
		{
			this.HeaderProbeResultList.Clear();
			this.SourceProbeResultList.Clear();
		}
	}

	class CodeFileProbeInfo
	{
		string FileName = null;
		List<string> CodeLineList = null;
		List<CodeLineProbeInfo> LineProbeInfoList = null;
		public CodeFileProbeInfo(string file_name)
		{
			this.FileName = file_name;
			this.CodeLineList = File.ReadAllLines(file_name).ToList();
			this.LineProbeInfoList = new List<CodeLineProbeInfo>();
			for (int i = 0; i < this.CodeLineList.Count; i++)
			{
				this.LineProbeInfoList.Add(new CodeLineProbeInfo());
			}
		}

		public void GoProbe()
		{
			CodePosition probe_pos = new CodePosition(0, 0);
			while (true)
			{
				if (null == probe_pos)
				{
					break;
				}
				CodeSymbol symbol = Common.GetNextSymbol(this.CodeLineList, ref probe_pos);
				if (null == symbol)
				{
					break;
				}
				if (Common.IsCommentStart(symbol.TextStr))
				{
					probe_pos = CommentProc(symbol);									// 注释
				}
				else if (Common.IsDefineStart(symbol.TextStr))
				{
					probe_pos = DefineProc(symbol);										// 宏定义
				}
			}
		}

		CodeElement GetNextElement(ref CodePosition start_pos)
		{
			CodePosition cur_pos = new CodePosition(start_pos);
			while (true)
			{
				char ch = this.CodeLineList[cur_pos.RowNum][cur_pos.ColNum];
				if (char.IsWhiteSpace(ch))
				{
					// 白空格
				}
				else if (ch.Equals('/'))
				{
					string line_str = this.CodeLineList[cur_pos.RowNum].Substring(cur_pos.ColNum);
					if (line_str.StartsWith("//"))
					{
						// 行注释
					}
					else if (line_str.StartsWith("/*"))
					{
						// 块注释
					}
				}
				else if (ch.Equals('#'))
				{
					string line_str = this.CodeLineList[cur_pos.RowNum].Substring(cur_pos.ColNum);
					// 预编译命令
					if (line_str.StartsWith("#include"))
					{
						// 头文件包含
					}
					else if (line_str.StartsWith("#define"))
					{
						// 宏定义
					}
					else if (Common.IsConditionalComilationStart(line_str))
					{
						// 条件编译
					}
				}
				// 标识符
				else if (Char.IsLetter(ch) || ch.Equals('_'))
				{
					int len = Common.GetIdentifierLength(this.CodeLineList[cur_pos.RowNum], cur_pos.ColNum);
					start_pos = Common.GetNextPosN(this.CodeLineList, cur_pos, len);
				}
				// 字符串,字符
				else if (ch.Equals('"')
					|| ch.Equals('\''))
				{

				}
				// 数字
				// 运算符

				cur_pos = Common.GetNextPos(this.CodeLineList, cur_pos);
				if (null == cur_pos)
				{
					break;
				}
			}
			return null;
		}

		CodePosition CommentProc(CodeSymbol symbol)
		{
			Trace.Assert(Common.IsCommentStart(symbol.TextStr));
			CodePosition search_pos = Common.GetNextPosN(this.CodeLineList, symbol.StartPosition, 2);
			if (symbol.TextStr.Equals("/*"))
			{
				CodePosition end_pos = Common.FindStrPosition(this.CodeLineList, search_pos, "*/");
				Trace.Assert(null != end_pos);
				for (int row = symbol.StartPosition.RowNum; row <= end_pos.RowNum; row++)
				{
					int start_col, end_col;
					if (row == symbol.StartPosition.RowNum)
					{
						start_col = symbol.StartPosition.ColNum;
					}
					else
					{
						start_col = 0;
					}
					if (row == end_pos.RowNum)
					{
						end_col = end_pos.ColNum;
					}
					else
					{
						end_col = this.CodeLineList[row].Length - 1;
					}
					CodeElement comment_element	= new CodeElement(
														CodeElementType.BlockComments,
														new CodePosition(row, start_col),
														new CodePosition(row, end_col));
					this.LineProbeInfoList[row].AddElement(comment_element);
				}
				return Common.GetNextPosN(this.CodeLineList, end_pos, 2);
			}
			else if (symbol.TextStr.Equals("//"))
			{
				int row = symbol.StartPosition.RowNum;
				int col = this.CodeLineList[row].Length - 1;
				CodePosition end_pos = new CodePosition(row, col);
				CodeElement comment_element = new CodeElement(CodeElementType.LineComments,
																search_pos, end_pos);
				this.LineProbeInfoList[row].AddElement(comment_element);
				return Common.GetNextPosN(this.CodeLineList, end_pos, 1);
			}
			else
			{
				return null;
			}
		}

		CodePosition DefineProc(CodeSymbol def_symbol)
		{
			Trace.Assert(Common.IsDefineStart(def_symbol.TextStr));
			// 定位到"#define"后
			CodePosition search_pos = Common.GetNextPosN(this.CodeLineList,
											def_symbol.StartPosition, "#define".Length);
			int row = def_symbol.StartPosition.RowNum;
			List<CodeSymbol> symbol_list = GetDefineSymbolList(ref search_pos);
			return search_pos;
		}

		List<CodeSymbol> GetDefineSymbolList(ref CodePosition start_position)
		{
			List<CodeSymbol> ret_list = new List<CodeSymbol>();
			int row = start_position.RowNum;
			CodePosition comment_pos = null;
			while (true)
			{
				CodeSymbol symbol = Common.GetNextSymbol(this.CodeLineList, ref start_position);
				if (null == symbol)
				{
					break;
				}
				else if (symbol.StartPosition.RowNum != row)
				{
					// 注意有续行符的情况
					break;
				}
				else if (Common.IsCommentStart(symbol.TextStr))
				{
					comment_pos = CommentProc(symbol);									// 注释
					start_position = new CodePosition(comment_pos);
				}
				else
				{
					ret_list.Add(symbol);
				}
			}
			CodeSymbol last_symbol = ret_list.Last();
			// 位置定位到末尾
			start_position = Common.GetNextPosN(this.CodeLineList,
												last_symbol.StartPosition,
												last_symbol.TextStr.Length);
			if (null != comment_pos
				&& CodePosition.Compare(comment_pos, start_position) > 0)
			{
				// 如果以注释结尾,要定位到注释结束
				start_position = new CodePosition(comment_pos);
			}
			return ret_list;
		}
	}

	class CodeLineProbeInfo
	{
		public List<CodeElement> ElementsList = new List<CodeElement>();				// 元素列表
		public void AddElement(CodeElement element)
		{
			this.ElementsList.Add(element);
		}
	}
}
