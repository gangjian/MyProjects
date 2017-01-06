using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Mr.Robot;

namespace Mr.Robot.MacroSwitchAnalyser
{
	public class MacroSwitchAnalyser
	{
		public List<string> AnalyzeResultList = new List<string>();

		FileParseInfo SourceParseInfo = null;
		string SourceName = null;

		public MacroSwitchAnalyser(FileParseInfo source_parse_info)
		{
			System.Diagnostics.Trace.Assert(null != source_parse_info);
			this.SourceParseInfo = source_parse_info;
			System.Diagnostics.Trace.Assert(File.Exists(this.SourceParseInfo.FullName));
			// 只删注释不做预编译处理(保留预编译宏开关)
			this.SourceParseInfo.CodeList.Clear();
			this.SourceParseInfo.CodeList = CCodeAnalyser.RemoveComments(this.SourceParseInfo.FullName);
			FileInfo fi = new FileInfo(this.SourceParseInfo.FullName);
			this.SourceName = fi.Name;
		}

		public void ProcessStart()
		{
			if (null == this.SourceParseInfo)
			{
				return;
			}
			this.AnalyzeResultList.Clear();
			int lineNum = 1;
			foreach (string code_line in this.SourceParseInfo.CodeList)
			{
				ProcessCodeLine(lineNum++, code_line, ref this.AnalyzeResultList);
			}
		}

		void ProcessCodeLine(int line_num, string code_line, ref List<string> result_list)
		{
			int idx = code_line.IndexOf("#if");
			if (-1 == idx)
			{
				return;
			}
			string expStr = GetMacroExpression(code_line, idx);
			if (null == expStr)
			{
				return;
			}
			List<StatementComponent> cpntList = StatementAnalysis.GetComponents(expStr, null);
			foreach (StatementComponent cpnt in cpntList)
			{
				if (cpnt.Type == StatementComponentType.Identifier)
				{
					MacroPrintInfo printInfo = new MacroPrintInfo(this.SourceName, line_num.ToString(), code_line);
					MacroAnalyzeProc(cpnt.Text, printInfo, ref result_list);
				}
			}
		}

		string GetMacroExpression(string code_line, int ifIdx)
		{
			for (int i = ifIdx + 3; i < code_line.Length; i++)
			{
				if (Char.IsWhiteSpace(code_line[i]))
				{
					return code_line.Substring(i).Trim();
				}
			}
			return null;
		}

		void MacroAnalyzeProc(string macro_name, MacroPrintInfo print_info, ref List<string> result_list)
		{
			MacroDefineInfo mdi = this.SourceParseInfo.FindMacroDefInfo(macro_name);
			if (null != mdi)
			{
				string valStr = mdi.Value;
				if (CommonProcess.IsStandardIdentifier(valStr))
				{
					MacroAnalyzeProc(valStr, print_info, ref result_list);
				}
				else
				{
					string resultStr = print_info.SourceName + "," + print_info.LineNumStr
							+ "," + print_info.CodeText + "," + macro_name + "," + valStr;
					result_list.Add(resultStr);
				}
			}
			else
			{
				string resultStr = print_info.SourceName + "," + print_info.LineNumStr
							+ "," + print_info.CodeText + "," + macro_name + "," + @"未定义";
				result_list.Add(resultStr);
			}
		}

		class MacroPrintInfo
		{
			public string SourceName = string.Empty;
			public string LineNumStr = string.Empty;
			public string CodeText = string.Empty;

			public MacroPrintInfo(string src_name, string line_num, string code_text)
			{
				this.SourceName = src_name;
				this.LineNumStr = line_num;
				this.CodeText = code_text.Trim().Replace('\t', ' ');
			}
		}
	}
}
