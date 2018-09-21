using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot.Creeper
{
	class CCodeProbe
	{
		// 源文件列表(入力)
		List<string> HeaderList = new List<string>();
		List<string> SourceList = new List<string>();

		//public EventHandler UpdateProgressHandler = null;

		// 源文件解析结果(处理)
		List<CodeProbeInfo> HeaderProbeResultList = new List<CodeProbeInfo>();
		List<CodeProbeInfo> SourceProbeResultList = new List<CodeProbeInfo>();

		public CCodeProbe(List<string> source_list, List<string> header_list)
		{
			this.SourceList = source_list;
			this.HeaderList = header_list;
		}

		public void ProbeStart()
		{
			foreach (string srcName in this.SourceList)
			{
			}
		}
	}

	class CodeProbeInfo
	{
		string FileName = null;
		List<CodeLineInfo> LineInfoList = new List<CodeLineInfo>();
		public CodeProbeInfo(string file_name)
		{
			this.FileName = file_name;
		}
	}

	class CodeLineInfo
	{
		public string LineStr = string.Empty;						// 原文
		List<CodeElement> ElementsList = new List<CodeElement>();	// 元素列表
		public CodeLineInfo(string code_line)
		{
			this.LineStr = code_line;
		}
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
