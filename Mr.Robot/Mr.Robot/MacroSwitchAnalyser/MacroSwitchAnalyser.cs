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
			code_line = CommonProc.RemoveStringSegment(code_line);
			int idx = code_line.IndexOf("#if");
			if (-1 == idx)
			{
				return;
			}
            string expStr = CommonProc.GetMacroExpression(code_line, idx);
			if (null == expStr)
			{
				return;
			}
			MacroPrintInfo printInfo = new MacroPrintInfo(this.SourceName, line_num.ToString(), code_line);
            CommonProc.MacroSwitchExpressionAnalysis(expStr, printInfo, this.SourceParseInfo, ref result_list);
		}

	}
}
