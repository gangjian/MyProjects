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
		CodeElement DefElement = null;

		public DefName DefName = null;

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

		public DefineInfo(	CodeElement def_element, DefName def_name, string val_str,
							CodePosition pos, CodeScope scope, string path,
							List<string> paras = null)
		{
			Trace.Assert(null != def_element);
			Trace.Assert(null != def_name);
			Trace.Assert(null != val_str);
			Trace.Assert(null != pos);
			Trace.Assert(null != scope);
			Trace.Assert(!string.IsNullOrEmpty(path));
			this.DefElement = def_element;
			this.DefName = def_name;
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
			Trace.Assert(element_list.Count >= 2);
			CodeElement macro_element = element_list[1];
			Trace.Assert(macro_element.Type == ElementType.Identifier);
			DefName def_name = new DefName(macro_element, file_info.CodeList);
			CodeScope scope = new CodeScope(def_element.GetStartPosition(), element_list.Last().EndPos);
			List<string> paras = GetParaList(ref element_list, file_info.CodeList);
			string val_str = CommProc.ElementListStrCat(element_list, file_info.CodeList);
			DefineInfo ret_info = new DefineInfo(	def_element, def_name, val_str,
													macro_element.GetStartPosition(),
													scope, file_info.FullName, paras);
			return ret_info;
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

	class DefName
	{
		string Name = null;
		CodePosition Pos = null;
		public DefName(CodeElement name_element, List<string> code_list)
		{
			Trace.Assert(null != name_element);
			Trace.Assert(null != code_list);
			Trace.Assert(name_element.Type == ElementType.Identifier);
			this.Name = name_element.ToString(code_list);
			this.Pos = new CodePosition(name_element.Row, name_element.Offset);
		}
		public string GetName()
		{
			return Name;
		}
	}

	class DefParas
	{
		CodePosition LeftBrace = null;
		CodePosition RightBrace = null;
		List<DefPara> Paras = new List<DefPara>();
	}

	class DefPara
	{
		CodePosition Pos = null;
		int Len = 0;
	}

	class DefValue
	{
		CodeScope Scope = null;
	}
}
