using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceOutsight
{
	class IncProc
	{
		public static TagTreeNode MakeIncludeTagTreeNode(List<CodeElement> element_list, List<string> code_list)
		{
			CodeElement include_element = element_list.First();
			element_list.RemoveAt(0);
			string header_name_str = Common.ElementListStrCat(element_list, code_list);
			CodeScope scope = new CodeScope(include_element.GetStartPosition(), element_list.Last().EndPos);
			TagNodeType type = TagNodeType.IncludeHeader;
			TagTreeNode ret_node = new TagTreeNode(include_element.ToString(code_list), header_name_str, include_element.GetStartPosition(), scope, type);
			return ret_node;
		}
	}
}
