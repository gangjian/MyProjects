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
			code_line = RemoveStringSegment(code_line);
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
			MacroPrintInfo printInfo = new MacroPrintInfo(this.SourceName, line_num.ToString(), code_line);
			MacroSwitchExpressionAnalysis(expStr, printInfo, ref result_list);
		}

		string RemoveStringSegment(string code_line)
		{
			while (true)
			{
				int idx_s = code_line.IndexOf('\"');
				if (   (-1 != idx_s)
					&& (code_line.Length - 1 != idx_s) )
				{
					int idx_e = code_line.IndexOf('\"', idx_s + 1);
					if (-1 != idx_e)
					{
						code_line = code_line.Remove(idx_s, idx_e - idx_s + 1);
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}
			}
			return code_line;
		}

		string RemoveExpressionBrackets(string exp_str)
		{
			exp_str = exp_str.Trim();
			if (exp_str.StartsWith("(")
				&& exp_str.EndsWith(")"))
			{
				exp_str = exp_str.Remove(exp_str.Length - 1, 1);
				exp_str = exp_str.Remove(0, 1).Trim();
			}
			return exp_str;
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

		void MacroSwitchExpressionAnalysis(string macro_exp, MacroPrintInfo print_info, ref List<string> result_list)
		{
			if (string.IsNullOrEmpty(macro_exp))
			{
				return;
			}
			List<StatementComponent> cpntList = StatementAnalysis.GetComponents(macro_exp, null);
			foreach (StatementComponent cpnt in cpntList)
			{
				if (cpnt.Type == StatementComponentType.Identifier && "defined" != cpnt.Text)
				{
					MacroDefineInfo mdi = this.SourceParseInfo.FindMacroDefInfo(cpnt.Text);
					if (null != mdi)
					{
						string valStr = mdi.Value;
						// 宏值分析
						MacroValueType mType = MacroValueStrAnalysis(ref valStr);
						// 立即数
						if (mType == MacroValueType.ConstNumber)
						{
							string resultStr = MakeResultStr(mdi.Name, valStr, print_info);
							result_list.Add(resultStr);
						}
						// 表达式
						else if (mType == MacroValueType.Expression)
						{
							MacroSwitchExpressionAnalysis(valStr, print_info, ref result_list);
						}
						// 空
						else if (mType == MacroValueType.Empty)
						{
							string resultStr = MakeResultStr(mdi.Name, string.Empty, print_info);
							result_list.Add(resultStr);
						}
					}
					else
					{
						// 未定义
						string resultStr = MakeResultStr(cpnt.Text, @"未定义", print_info);
						result_list.Add(resultStr);
					}
				}
			}
		}

		MacroValueType MacroValueStrAnalysis(ref string macro_value_str)
		{
			if (string.IsNullOrEmpty(macro_value_str.Trim()))
			{
				return MacroValueType.Empty;
			}
			macro_value_str = RemoveExpressionBrackets(macro_value_str);
			List<StatementComponent> cpntList = StatementAnalysis.GetComponents(macro_value_str, null);
			int numberCount = 0;
			int identifierCount = 0;
			foreach (StatementComponent cpnt in cpntList)
			{
				if (cpnt.Type == StatementComponentType.ConstantNumber)
				{
					numberCount += 1;
				}
				else if (cpnt.Type == StatementComponentType.Identifier && cpnt.Text != "defined")
				{
					identifierCount += 1;
				}
			}
			if (1 == numberCount && 0 == identifierCount)
			{
				return MacroValueType.ConstNumber;
			}
			else
			{
				return MacroValueType.Expression;
			}
		}

		string MakeResultStr(string macro_name, string value_str, MacroPrintInfo print_info)
		{
			string resultStr = print_info.SourceName + "," + print_info.LineNumStr
							+ "," + print_info.CodeText + "," + macro_name + "," + value_str;
			return resultStr;
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

		enum MacroValueType
		{
			ConstNumber,
			Expression,
			Empty,
		}
	}
}
