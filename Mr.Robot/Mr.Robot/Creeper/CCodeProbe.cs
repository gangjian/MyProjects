using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Mr.Robot.Creeper
{
	class CCodeProbe
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
			this.LineProbeInfoList = new List<CodeLineProbeInfo>(this.CodeLineList.Count);
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
			}
		}
	}

	class CodeLineProbeInfo
	{
		List<CodeElement> ElementsList = new List<CodeElement>();						// 元素列表
	}

	class CodeElement
	{
		CodeScope Scope = null;
		CodeElementType Type = CodeElementType.None;
	}

	enum CodeElementType
	{
		None,
		Comments,				// 注释
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
