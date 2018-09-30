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
				CodeSymbol symbol = Common.GetNextSymbol(this.CodeLineList, ref probe_pos);
				if (string.IsNullOrEmpty(symbol.SymbolStr))
				{
					break;
				}
				if (Common.IsCommentStart(symbol.SymbolStr))
				{
					probe_pos = CommentProc(symbol);									// 注释
				}
				else if (Common.IsDefineStart(symbol.SymbolStr))
				{
					
				}
			}
		}

		CodePosition CommentProc(CodeSymbol symbol)
		{
			Trace.Assert(Common.IsCommentStart(symbol.SymbolStr));
			CodePosition search_pos = Common.GetNextPosN(this.CodeLineList, symbol.StartPosition, 2);
			if (symbol.SymbolStr.Equals("/*"))
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
			else if (symbol.SymbolStr.Equals("//"))
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
			Trace.Assert(Common.IsDefineStart(def_symbol.SymbolStr));
			CodePosition search_pos = Common.GetNextPosN(this.CodeLineList,
											def_symbol.StartPosition, "#define".Length);
			int row = def_symbol.StartPosition.RowNum;
			while (true)
			{
				CodeSymbol symbol = Common.GetNextSymbol(this.CodeLineList, ref search_pos);
				if (symbol.StartPosition.RowNum != row)
				{
					break;
				}
			}
			return null;
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

	class CodeElement
	{
		CodeScope Scope = null;
		CodeElementType Type = CodeElementType.None;
		public CodeElement(CodeElementType type, CodePosition start_pos, CodePosition end_pos)
		{
			this.Type = type;
			this.Scope = new CodeScope(start_pos, end_pos);
		}
	}

	enum CodeElementType
	{
		None,
		Invalid,				// 被编译开关注掉的无效内容
		BlockComments,			// 块注释
		LineComments,			// 行注释
		ReservedWord,			// 保留字
		String,					// 字符串
		Charactor,				// 字符
		Number,					// 数字常量
		Identifier,				// 标识符
		BaseType,				// 基本类型
		UserDefType,			// 用户定义类型
		Variable,				// 变量
		FunctionName,			// 函数名
		MacroName,				// 宏名
	}
}
