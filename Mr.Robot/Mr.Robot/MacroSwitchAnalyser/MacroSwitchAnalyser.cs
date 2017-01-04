using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mr.Robot;

namespace Mr.Robot.MacroSwitchAnalyser
{
	public class MacroSwitchAnalyser
	{
		FileParseInfo SourceParseInfo = null;
		public MacroSwitchAnalyser(FileParseInfo source_parse_info)
		{
			System.Diagnostics.Trace.Assert(null != source_parse_info);
			this.SourceParseInfo = source_parse_info;

			// 只删注释不做预编译处理(保留预编译宏开关)
			this.SourceParseInfo.CodeList.Clear();
			this.SourceParseInfo.CodeList = CCodeAnalyser.RemoveComments(this.SourceParseInfo.FullName);
		}

		void ProcessStart()
		{
			if (null == this.SourceParseInfo)
			{
				return;
			}
			for (int i = 0; i < this.SourceParseInfo.CodeList.Count; i++)
			{
				ProcessCodeLine(this.SourceParseInfo.CodeList[i]);
			}
		}

		void ProcessCodeLine(string code_line)
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
	}
}
