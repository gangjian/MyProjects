using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SourceOutsight
{
	class MacroProc
	{
		public static TagTreeNode MakeDefTagTreeNode(List<CodeElement> element_list, List<string> code_list)
		{
			Trace.Assert(element_list.Count >= 2);
			CodeElement def_element = element_list.First();
			CodeElement macro_element = element_list[1];
			Trace.Assert(macro_element.Type == ElementType.Identifier);
			string macro_name = macro_element.ToString(code_list);
			CodeScope scope = new CodeScope(def_element.GetStartPosition(), element_list.Last().EndPos);
			TagNodeType type = TagNodeType.MacroDef;
			// 判断是否为宏函数
			List<string> paras = GetParaList(ref element_list, code_list);
			if (null != paras)
			{
				type = TagNodeType.MacroFunc;
			}
			string val_str = Common.ElementListStrCat(element_list, code_list);
			DefInfo def_info = new DefInfo(macro_name, val_str);
			def_info.Paras = paras;
			TagTreeNode ret_node = new TagTreeNode(macro_name, null, macro_element.GetStartPosition(), scope, type);
			ret_node.InfoRef = def_info;
			return ret_node;
		}
		static List<string> GetParaList(ref List<CodeElement> element_list, List<string> code_list)
		{
			if (element_list.Count > 3
				&& element_list[2].ToString(code_list).Equals("(")
				&& element_list[2].CloseTo(element_list[1], code_list))
			{
				List<CodeElement> para_list = new List<CodeElement>();
				for (int i = 3; i < element_list.Count; i++)
				{
					if (element_list[i].ToString(code_list).Equals(")"))
					{
						string para_str = Common.ElementListStrCat(para_list, code_list);
						string[] arr = para_str.Split(',');
						List<string> paras = new List<string>();
						foreach (var item in arr)
						{
							paras.Add(item.Trim());
						}
						element_list.RemoveRange(0, i + 1);
						return paras;
					}
					else
					{
						para_list.Add(element_list[i]);
					}
				}
				Trace.Assert(false);
				return null;
			}
			else
			{
				element_list.RemoveRange(0, 2);
				return null;
			}
		}
		public static TagTreeNode MakeUndefTagTreeNode(List<CodeElement> element_list, List<string> code_list)
		{
			Trace.Assert(element_list.Count == 2);
			CodeElement undef_element = element_list.First();
			CodeElement macro_element = element_list[1];
			Trace.Assert(macro_element.Type == ElementType.Identifier);
			string macro_name = macro_element.ToString(code_list);
			CodeScope scope = new CodeScope(undef_element.GetStartPosition(), element_list.Last().EndPos);
			TagNodeType type = TagNodeType.Undef;
			TagTreeNode ret_node = new TagTreeNode(undef_element.ToString(code_list), macro_name, macro_element.GetStartPosition(), scope, type);
			return ret_node;
		}
	}

	class DefInfo
	{
		public string Name = null;
		public List<string> Paras = new List<string>();
		public string ValueStr = null;
		public DefInfo(string name, string val_str)
		{
			this.Name = name;
			this.ValueStr = val_str;
		}
	}
}
