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
				ProcessCodeLine(lineNum++, code_line, this.AnalyzeResultList);
			}
		}

		void ProcessCodeLine(int line_num, string code_line, List<string> result_list)
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
					string macroSwitchStr = this.SourceName + "," + line_num.ToString() + "," + code_line.Trim() + "," + cpnt.Text + ",";
					string valStr = GetMacroValueString(cpnt.Text);
					if (null != valStr)
					{
						macroSwitchStr += valStr;
					}
					else
					{
						macroSwitchStr += "What the Hell is This?";
					}
					result_list.Add(macroSwitchStr);
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

		string GetMacroValueString(string macro_name)
		{
			MacroDefineInfo mdi = this.SourceParseInfo.FindMacroDefInfo(macro_name);
			if (null != mdi)
			{
				string valStr = mdi.Value;
				if (CommonProcess.IsStandardIdentifier(valStr))
				{
					return GetMacroValueString(valStr);
				}
				else
				{
					return valStr;
				}
			}
			else
			{
				return null;
			}
		}
	}
}
