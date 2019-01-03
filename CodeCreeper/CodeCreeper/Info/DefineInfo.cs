using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper
{
	class DefineInfo
	{
		string name = null;
		public string Name
		{
			get { return name; }
		}

		List<string> paras = new List<string>();
		public List<string> Paras
		{
			get { return paras; }
		}

		string valueStr = null;
		public string ValueStr
		{
			get { return valueStr; }
		}

		CodePosition namePosition = null;
		public CodePosition NamePosition
		{
			get { return namePosition; }
		}
		CodeScope scope = null;
		public CodeScope Scope
		{
			get { return scope; }
		}
		string filePath = null;
		public string FilePath
		{
			get { return filePath; }
		}

		public DefineInfo(	string name, string val_str,
							CodePosition pos, CodeScope scope, string path,
							List<string> paras = null)
		{
			Trace.Assert(!string.IsNullOrEmpty(name));
			Trace.Assert(null != val_str);
			Trace.Assert(null != pos);
			Trace.Assert(null != scope);
			Trace.Assert(!string.IsNullOrEmpty(path));
			this.name = name;
			this.valueStr = val_str;
			this.namePosition = pos;
			this.scope = scope;
			this.filePath = path;
			if (null != paras)
			{
				this.paras = paras;
			}
		}

		public static DefineInfo TryParse(CodeElement def_element, CodeFileInfo file_info)
		{
			Trace.Assert(null != def_element);
			Trace.Assert(null != file_info);
			List<CodeElement> element_list
						= file_info.GetLineElementList(def_element.GetStartPosition());
			element_list = RemoveComments(element_list);
			Trace.Assert(element_list.Count >= 2);
			CodeElement macro_element = element_list[1];
			Trace.Assert(macro_element.Type == ElementType.Identifier);
			string macro_name = macro_element.ToString(file_info.CodeList);
			CodeScope scope = new CodeScope(def_element.GetStartPosition(), element_list.Last().EndPos);
			List<string> paras = GetParaList(ref element_list, file_info.CodeList);
			string val_str = CommProc.ElementListStrCat(element_list, file_info.CodeList);
			DefineInfo ret_info = new DefineInfo(	macro_name, val_str,
													macro_element.GetStartPosition(),
													scope, file_info.FullName, paras);
			return ret_info;
		}
		static List<CodeElement> RemoveComments(List<CodeElement> element_list)
		{
			List<CodeElement> ret_list = new List<CodeElement>();
			foreach (var item in element_list)
			{
				if (item.Type != ElementType.Comments)
				{
					ret_list.Add(item);
				}
			}
			return ret_list;
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
						string para_str = CommProc.ElementListStrCat(para_list, code_list);
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
	}
}
