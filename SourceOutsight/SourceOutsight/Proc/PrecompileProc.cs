using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SourceOutsight
{
	class PrecompileProc
	{
		public static TagTreeNode MakePrecompileSwitchTagTreeNode(List<CodeElement> element_list, List<string> code_list)
		{
			Trace.Assert(element_list.Count > 0);
			CodeElement switch_element = element_list.First();
			string expression_str = null;
			if (element_list.Count > 1)
			{
				element_list.RemoveAt(0);
				expression_str = Common.ElementListStrCat(element_list, code_list);
			}
			CodeScope scope = new CodeScope(switch_element.GetStartPosition(), element_list.Last().EndPos);
			TagNodeType type = TagNodeType.PrecompileSwitch;
			string tag_str = switch_element.ToString(code_list);
			Trace.Assert(tag_str.StartsWith("#"));
			tag_str = "#" + tag_str.Substring(1).Trim();	// 这样做是为了防止'#'后面有空格,比如"# if"
			TagTreeNode ret_node = new TagTreeNode(tag_str, expression_str, switch_element.GetStartPosition(), scope, type);
			return ret_node;
		}

		public static TagTreeNode MakePrecompileCommandTagTreeNode(List<CodeElement> element_list, List<string> code_list)
		{
			Trace.Assert(element_list.Count > 0);
			CodeElement command_element = element_list.First();
			string expression_str = null;
			if (element_list.Count > 1)
			{
				element_list.RemoveAt(0);
				expression_str = Common.ElementListStrCat(element_list, code_list);
			}
			CodeScope scope = new CodeScope(command_element.GetStartPosition(), element_list.Last().EndPos);
			TagNodeType type = TagNodeType.PrecompileCommand;
			TagTreeNode ret_node = new TagTreeNode(command_element.ToString(code_list), expression_str, command_element.GetStartPosition(), scope, type);
			return ret_node;
		}
	}
}
