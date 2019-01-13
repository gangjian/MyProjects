using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper
{
	class PrecompileCmdInfo
	{
		CodeElement CmdElement = null;
		public string CmdName = null;
		CodeScope ContentsScope = null;
		public string FilePath = null;

		PrecompileCmdInfo(CodeElement cmd_element, string cmd_name, CodeScope scope, string path)
		{
			this.CmdElement = cmd_element;
			this.CmdName = cmd_name;
			this.ContentsScope = scope;
			this.FilePath = path;
		}

		public static PrecompileCmdInfo Parse(CodeElement cmd_element, CodeFileInfo file_info)
		{
			Trace.Assert(null != cmd_element);
			Trace.Assert(null != file_info);
			List<CodeElement> element_list
						= file_info.GetLineElementList(cmd_element.GetStartPosition());
			Trace.Assert(element_list.Count >= 2);
			string cmd_name = cmd_element.ToString(file_info.CodeList);
			CodeScope scope = new CodeScope(element_list[1].GetStartPosition(),
											element_list.Last().EndPos);
			PrecompileCmdInfo ret_info = new PrecompileCmdInfo(cmd_element, cmd_name,
															scope, file_info.FullName);
			return ret_info;
		}
	}
}
