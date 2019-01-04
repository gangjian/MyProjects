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

		DefParas Paras = null;

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
							CodePosition pos, CodeScope scope, string path, DefParas paras)
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
			this.Paras = paras;
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
			DefParas paras = GetParas(ref element_list, file_info.CodeList);
			string val_str = CommProc.ElementListStrCat(element_list, file_info.CodeList);
			DefineInfo ret_info = new DefineInfo(	def_element, def_name, val_str,
													macro_element.GetStartPosition(),
													scope, file_info.FullName, paras);
			return ret_info;
		}
		static DefParas GetParas(ref List<CodeElement> element_list, List<string> code_list)
		{
			if (element_list.Count > 3
				&& element_list[2].ToString(code_list).Equals("(")
				&& element_list[2].CloseTo(element_list[1], code_list))
			{
				CodePosition left_pos = new CodePosition(element_list[2].GetStartPosition());
				List<CodeElement> para_list = new List<CodeElement>();
				for (int i = 3; i < element_list.Count; i++)
				{
					if (element_list[i].ToString(code_list).Equals(")"))
					{
						CodePosition right_pos = new CodePosition(element_list[i].GetStartPosition());
						List<DefPara> paras = new List<DefPara>();
						for (int j = 0; j < para_list.Count; j++)
						{
							CodeElement item = para_list[j];
							if (item.Type == ElementType.Identifier)
							{
								if (j < para_list.Count - 1)
								{
									Trace.Assert(para_list[j + 1].ToString(code_list).Equals(","));
								}
								DefPara para = new DefPara(item.GetStartPosition(), item.GetLen());
								paras.Add(para);
							}
							else
							{
								Trace.Assert(item.ToString(code_list).Equals(","));
							}
						}
						element_list.RemoveRange(0, i + 1);
						return new DefParas(left_pos, right_pos, paras);
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
		public DefParas(CodePosition left_brace, CodePosition right_brace, List<DefPara> para_list)
		{
			Trace.Assert(null != left_brace);
			Trace.Assert(null != right_brace);
			Trace.Assert(null != para_list);
			this.LeftBrace = left_brace;
			this.RightBrace = right_brace;
			this.Paras = para_list;
		}
	}

	class DefPara
	{
		CodePosition Pos = null;
		int Len = 0;
		public DefPara(CodePosition pos, int len)
		{
			Trace.Assert(null != pos);
			Trace.Assert(len > 0);
			this.Pos = pos;
			this.Len = len;
		}
	}

	class DefValue
	{
		CodeScope Scope = null;
	}
}
