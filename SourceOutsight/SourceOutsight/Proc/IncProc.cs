using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SourceOutsight
{
	class IncProc
	{
		public static TagTreeNode MakeIncludeTagTreeNode(List<CodeElement> element_list,
														 List<string> code_list,
														 SO_Project prj_ref)
		{
			CodeElement include_element = element_list.First();
			element_list.RemoveAt(0);
			string header_name_str = Common.ElementListStrCat(element_list, code_list);
			SO_File header_info = ParseHeader(header_name_str, prj_ref);
			CodeScope scope = new CodeScope(include_element.GetStartPosition(), element_list.Last().EndPos);
			TagNodeType type = TagNodeType.IncludeHeader;
			TagTreeNode ret_node = new TagTreeNode(	include_element.ToString(code_list),
													header_name_str,
													include_element.GetStartPosition(),
													scope, type);
			ret_node.Info.DataRef = header_info;
			return ret_node;
		}

		static SO_File ParseHeader(string header_name, SO_Project prj_ref)
		{
			Trace.Assert(!string.IsNullOrEmpty(header_name) && header_name.Length > 3);
			if ((header_name.StartsWith("\"") && header_name.EndsWith("\""))
				|| (header_name.StartsWith("<") && header_name.EndsWith(">")))
			{
				header_name = header_name.Substring(1, header_name.Length - 2).Trim();
			}
			string full_name = prj_ref.GetFileFullPath(header_name);
			if (string.IsNullOrEmpty(full_name))
			{
				// 尖括号括起的系统头文件会返回null
				return null;
			}
			else if (prj_ref.ParseFileStack.Contains(full_name))
			{
				// 用堆栈防止互相包含
				return null;
			}
			SO_File header_info = prj_ref.GetFileInfo(full_name);
			if (null == header_info)
			{
				header_info = new SO_File(full_name, prj_ref);
				prj_ref.AddHeaderInfo(header_info);
			}
			return header_info;
		}
	}
}
