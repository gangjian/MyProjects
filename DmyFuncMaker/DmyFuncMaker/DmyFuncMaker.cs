using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DmyFuncMaker
{
	class DmyFuncMaker
	{
		public static List<string> DmyFuncPrototypeProc(string statement_str)
		{
			statement_str = statement_str.Trim();
			if (string.IsNullOrEmpty(statement_str))
			{
				return null;
			}
			int offset = 0;
			List<CodeIdentifier> idtfList = new List<CodeIdentifier>();
			while (true)
			{
				CodeIdentifier idtf = CommProc.GetNextIdentifier(statement_str, ref offset);
				if (null == idtf)
				{
					break;
				}

				if ("(" == idtf.Text)
				{
					if (1 == idtfList.Count && "FUNC" == idtfList[0].Text)
					{
					}
					else
					{
						CodeIdentifier rightBracket = GetMatchRightBracket(statement_str, ref offset);
						if (null == rightBracket)
						{
							break;
						}
						string funcDefTypeStr = string.Empty;
						string funcName = GetFuncNameStr(idtfList);
						string retTypeStr = GetReturnTypeStr(idtfList, statement_str, out funcDefTypeStr);
						string paraStr = GetParaStr(statement_str, idtf, rightBracket);
						if (null != funcName && null != retTypeStr && null != paraStr)
						{
							return MakeDmyFuncList(funcName, retTypeStr, paraStr, funcDefTypeStr);
						}
						else
						{
							return null;
						}
					}
				}
				idtfList.Add(idtf);
			}
			return null;
		}

		static CodeIdentifier GetMatchRightBracket(string statement_str, ref int offset)
		{
			int counter = 1;
			while (true)
			{
				CodeIdentifier idtf = CommProc.GetNextIdentifier(statement_str, ref offset);
				if (null == idtf)
				{
					break;
				}
				if ("(" == idtf.Text)
				{
					counter += 1;
				}
				else if (")" == idtf.Text)
				{
					counter -= 1;
					if (0 == counter)
					{
						return idtf;
					}
				}
			}
			return null;
		}

		static string GetFuncNameStr(List<CodeIdentifier> idtf_list)
		{
			if (idtf_list.Count > 1 && CommProc.IsStandardIdentifier(idtf_list.Last().Text))
			{
				return idtf_list.Last().Text;
			}
			return null;
		}

		static string GetReturnTypeStr(List<CodeIdentifier> idtf_list, string statement_str, out string func_def_type_str)
		{
			func_def_type_str = string.Empty;
			if (idtf_list.Count > 1)
			{
				idtf_list.RemoveAt(idtf_list.Count - 1);									// 去掉函数名
				if ("FUNC" == idtf_list.First().Text)
				{
					int startOffset = idtf_list.First().Offset;
					int endOffset = idtf_list.Last().Offset;
					int len = endOffset - startOffset + 1;
					func_def_type_str = statement_str.Substring(startOffset, len);
					return GetFuncDefRetTypeStr(idtf_list, statement_str);
				}
				else
				{
					string retType = string.Empty;
					foreach (CodeIdentifier idtf in idtf_list)
					{
						retType += idtf.Text;
					}
					return retType.Trim();
				}
			}
			return null;
		}

		static string GetFuncDefRetTypeStr(List<CodeIdentifier> idtf_list, string statement_str)
		{
			if (idtf_list.Count >= 6
				&& "(" == idtf_list[1].Text
				&& ")" == idtf_list.Last().Text)
			{
				int startOffset = idtf_list[1].Offset + 1;
				int endOffset = idtf_list.Last().Offset - 1;
				int len = endOffset - startOffset + 1;
				string paraStr = statement_str.Substring(startOffset, len).Trim();
				string[] arr = paraStr.Split(',');
				if (arr.Length == 2)
				{
					return arr[0].Trim();
				}
			}
			return null;
		}

		static string GetParaStr(string statement_str, CodeIdentifier left_bracket, CodeIdentifier right_bracket)
		{
			int start_offset = left_bracket.Offset + 1;
			int end_offset = right_bracket.Offset - 1;
			int len = end_offset - start_offset + 1;
			return statement_str.Substring(start_offset, len);
		}

		static List<string> MakeDmyFuncList(string func_name, string ret_type, string para_str, string func_def_type_str)
		{
			List<string> dmyFuncList = new List<string>();
			dmyFuncList.AddRange(MakeDmyFuncComments());
			dmyFuncList.AddRange(MakeDmyFuncDeclaration(func_name, ret_type, para_str));
			dmyFuncList.AddRange(MakeDmyFuncHeader(func_name, ret_type, para_str, func_def_type_str));
			dmyFuncList.AddRange(MakeDmyFuncBody(func_name, ret_type, para_str));
			return dmyFuncList;
		}

		static List<string> MakeDmyFuncComments()
		{
			List<string> retList = new List<string>();
			retList.Add("/*------------------------------------------------------------------------------*/");
			retList.Add("/*            ダミー関数                                                        */");
			retList.Add("/*------------------------------------------------------------------------------*/");
			return retList;
		}

		static List<string> MakeDmyFuncDeclaration(string func_name, string ret_type, string para_str)
		{
			List<string> retList = new List<string>();
			retList.Add("static uint8 dmy_" + func_name + "_Cnt;");						// dummy函数计数变量
			List<VarInfo> varList = GetDmyFuncParaList(para_str);
			foreach (var vi in varList)
			{																			// 各参数
				string typeStr = vi.TypeStr;
				if (typeStr.EndsWith("*"))
				{
					typeStr = typeStr.Remove(typeStr.Length - 1).Trim();
				}
				retList.Add("static " + typeStr + " dmy_" + func_name + "_" + vi.Name + ";");
			}
			if ("void" != ret_type && !string.IsNullOrEmpty(ret_type))
			{																			// 返回值
				retList.Add("static " + ret_type + " dmy_" + func_name + "_Ret;");
			}
			return retList;
		}

		const string P2VAR_MARK = "DmyFuncMaker_MacroTypeMark";

		static List<VarInfo> GetDmyFuncParaList(string para_str)
		{
			List<VarInfo> retList = new List<VarInfo>();
			List<string> macroList = new List<string>();
			int P2VAR_idx = -1;
			while (-1 != (P2VAR_idx = para_str.IndexOf("P2VAR")))
			{
				int offset = P2VAR_idx + "P2VAR".Length;
				CodeIdentifier leftBracket = CommProc.GetNextIdentifier(para_str, ref offset);
				CodeIdentifier rightBracket = null;
				if (null != leftBracket
					&& "(" == leftBracket.Text
					&& null != (rightBracket = GetMatchRightBracket(para_str, ref offset))
					)
				{
					int startIdx = P2VAR_idx;
					int endIdx = rightBracket.Offset;
					int len = endIdx - startIdx + 1;
					macroList.Add(para_str.Substring(startIdx, len));
					para_str = para_str.Remove(startIdx, len);
					para_str = para_str.Insert(startIdx, P2VAR_MARK);
				}
				else
				{
					return retList;
				}
			}
			string[] arr = para_str.Split(',');
			for (int i = 0; i < arr.Length; i++)
			{
				string para = arr[i].Trim();
				int idx = para.IndexOf(P2VAR_MARK);
				if (-1 != idx)
				{
					para = para.Replace(P2VAR_MARK, macroList[0]);
					macroList.RemoveAt(0);
				}
				VarInfo vi = GetParaInfo(para);
				if (null != vi)
				{
					retList.Add(vi);
				}
			}
			return retList;
		}

		static VarInfo GetParaInfo(string para_str)
		{
			List<CodeIdentifier> idtfList = new List<CodeIdentifier>();
			int offset = 0;
			while (true)
			{
				CodeIdentifier idtf = CommProc.GetNextIdentifier(para_str, ref offset);
				if (null == idtf)
				{
					break;
				}
				idtfList.Add(idtf);
			}
			if (idtfList.Count >= 2 && CommProc.IsStandardIdentifier(idtfList.Last().Text))
			{
				string varName = idtfList.Last().Text;
				string typeName = string.Empty;
				idtfList.Remove(idtfList.Last());										// 去掉变量名
				if ("P2VAR" == idtfList.First().Text)
				{
					typeName = GetP2VARTypeStr(idtfList, para_str);
				}
				else
				{
					int startOffset = idtfList.First().Offset;
					int endOffset = idtfList.Last().Offset + idtfList.Last().Text.Length - 1;
					int len = endOffset - startOffset + 1;
					typeName = para_str.Substring(startOffset, len).Trim();
				}
				if (null != typeName)
				{
					VarInfo varInfo = new VarInfo(typeName, varName);
					return varInfo;
				}
			}
			return null;
		}

		static string GetP2VARTypeStr(List<CodeIdentifier> idtf_list, string statement_str)
		{
			if (idtf_list.Count >= 8
				&& "(" == idtf_list[1].Text
				&& ")" == idtf_list.Last().Text)
			{
				int startOffset = idtf_list[1].Offset + 1;
				int endOffset = idtf_list.Last().Offset - 1;
				int len = endOffset - startOffset + 1;
				string paraStr = statement_str.Substring(startOffset, len).Trim();
				string[] arr = paraStr.Split(',');
				if (arr.Length == 3)
				{
					return arr[0].Trim() + " *";
				}
			}
			return null;
		}

		static List<string> MakeDmyFuncHeader(string func_name, string ret_type, string para_str, string func_def_type_str)
		{
			List<string> retList = new List<string>();
			if (!string.IsNullOrEmpty(func_def_type_str))
			{
				ret_type = func_def_type_str;
			}
			retList.Add(ret_type + " " + func_name + "(" + para_str + ") {");
			return retList;
		}

		static List<string> MakeDmyFuncBody(string func_name, string ret_type, string para_str)
		{
			List<string> retList = new List<string>();
			retList.Add("\tdmy_" + func_name + "_Cnt++;");								// dummy函数计数变量
			List<VarInfo> varList = GetDmyFuncParaList(para_str);
			foreach (var vi in varList)
			{																			// 各参数
				string varName = vi.Name;
				if (vi.TypeStr.EndsWith("*"))
				{
					varName = "*" + varName;
				}
				retList.Add("\tdmy_" + func_name + "_" + vi.Name + " = " + varName + ";");
			}

			retList.Add(string.Empty);

			if ("void" != ret_type && !string.IsNullOrEmpty(ret_type))
			{																			// 返回值
				retList.Add("\treturn " + " dmy_" + func_name + "_Ret;");
			}
			else
			{
				retList.Add("\treturn;");
			}
			retList.Add("}");
			return retList;
		}
	}
}
