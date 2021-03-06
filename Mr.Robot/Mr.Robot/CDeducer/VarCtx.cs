﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Mr.Robot.CDeducer;

namespace Mr.Robot
{
	/// <summary>
	/// 变量上下文
	/// </summary>
	public class VAR_CTX
	{
		public string Name = string.Empty;												// 变量名

		public VAR_TYPE Type = new VAR_TYPE();											// 类型名

		public VAR_TYPE_CATEGORY VarTypeCategory = VAR_TYPE_CATEGORY.BASIC;

		public object Value = null;

		public string CalledFunctionReadOut = string.Empty;								// 可能被函数调用的读出值赋值(函数名)

		public List<VAR_CTX> MemberList = new List<VAR_CTX>();							// 成员列表(下一层级)

		public VAR_CTX(string type_name, string var_name)								// 构造方法
		{
			Trace.Assert(!string.IsNullOrEmpty(type_name));
			Trace.Assert(!string.IsNullOrEmpty(var_name));
			this.Type.Name = type_name;
			this.Name = var_name;
		}

		public void InitValue(string init_str, FILE_PARSE_INFO parse_info)
		{
			this.Value = ExpCalc.GetConstExpressionValue(init_str, parse_info);
		}
	}

	public class VAR_TYPE
	{
		public string Name = string.Empty;
		public List<string> PrefixList = new List<string>();
		public List<string> SuffixList = new List<string>();

		public string GetFullName()
		{
			List<string> tmpList = new List<string>();
			tmpList.AddRange(this.PrefixList);
			tmpList.Add(this.Name);
			tmpList.AddRange(this.SuffixList);
			string retStr = string.Empty;
			for (int i = 0; i < tmpList.Count; i++)
			{
				if (i > 0)
				{
					retStr += " ";
				}
				retStr += tmpList[i];
			}
			return retStr;
		}
	}

	public enum VAR_TYPE_CATEGORY
	{
		BASIC,				// 基本类型
		USR_DEF_TYPE,		// 用户定义类型
		POINTER,			// 指针类型
		ARRAY,				// 数组类型
	}

	public partial class InOutAnalysis
	{
		/// <summary>
		/// 根据变量名,从当前解析上下文中找出变量的上下文
		/// </summary>
		/// <param name="var_name"></param>
		/// <param name="func_ctx"></param>
		/// <returns></returns>
		static public VAR_CTX GetVarCtxByName(string var_name, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
		{
			VAR_CTX var_ctx = null;
			if (null != (var_ctx = SearchVarCtxList(var_name, func_ctx)))
			{
				// 如果在当前的各个变量上下文列表中找到, 说明变量上下文已经创建, 直接返回
				return var_ctx;
			}
			else
			{
				var_ctx = parse_info.FindGlobalVarInfoByName(var_name);
				if (null != var_ctx)
				{
					return var_ctx;
				}
				else
				{
					return null;
				}
			}
		}

		static VAR_CTX SearchVarCtxList(string var_name, FUNC_CONTEXT func_ctx)
		{
			if (null == func_ctx)
			{
				return null;
			}
			VAR_CTX var_ctx = null;
			if ((null != (var_ctx = FindVarInVarCtxList(var_name, func_ctx.ParameterList)))			// (1)参数?
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, func_ctx.LocalVarList)))		// (2)临时变量?
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, func_ctx.OtherGlobalList)))
				)
			{
				return var_ctx;
			}
			else
			{
				return null;
			}
		}

		static VAR_CTX FindVarInVarCtxList(string var_name, List<VAR_CTX> ctx_list)
		{
			foreach (VAR_CTX ctx in ctx_list)
			{
				if (ctx.Name.Equals(var_name))
				{
					return ctx;
				}
			}
			return null;
		}

		public static VAR_CTX CreateVarCtx(	MEANING_GROUP type_group,
											string var_name,
											MEANING_GROUP init_group,
											FILE_PARSE_INFO parse_info)
		{
			List<string> prefixList = new List<string>();
			List<string> suffixList = new List<string>();
																						// 分离前后缀, 核心名
			string typeCoreName = GetTypeNameFromGroup(type_group, out prefixList, out suffixList);

			typeCoreName = parse_info.GetOriginalTypeName(typeCoreName);

			VAR_CTX retVarCtx = new VAR_CTX(typeCoreName, var_name);
			retVarCtx.Type.PrefixList = prefixList;
			retVarCtx.Type.SuffixList = suffixList;
			int arrSize = 0;
			if (var_name.EndsWith("]"))													// 包含下标,说明是数组
			{
				arrSize = GetArraySizeFromVarName(ref var_name, parse_info);
				if (arrSize <= 0)
				{
					return null;
				}
				int idx = retVarCtx.Name.IndexOf('[');
				retVarCtx.Name = retVarCtx.Name.Remove(idx);
				List<MEANING_GROUP> arrayMemberInitList = null;
				if (null != init_group)
				{
					arrayMemberInitList = GetMemberInitGroupListFromCodeBlock(init_group, parse_info);
					if (null == arrayMemberInitList
						|| arrSize != arrayMemberInitList.Count)
					{
						return null;
					}
				}
				for (int i = 0; i < arrSize; i++)
				{																		// 分别创建数组各成员
					string memberName = "arrayMember" + "_" + i.ToString();
					MEANING_GROUP memberInitGroup = null;
					if (null != arrayMemberInitList)
					{
						memberInitGroup = arrayMemberInitList[i];
					}
					VAR_CTX memberCtx = CreateVarCtx(type_group, memberName, memberInitGroup, parse_info);
					retVarCtx.MemberList.Add(memberCtx);
				}
				retVarCtx.VarTypeCategory = VAR_TYPE_CATEGORY.ARRAY;
			}
			else
			{
				if (suffixList.Contains("*"))
				{
					retVarCtx.VarTypeCategory = VAR_TYPE_CATEGORY.POINTER;
					if (null != init_group)
					{
						retVarCtx.Value = init_group.TextStr;
					}
				}
				else
				{
					if (BasicTypeProc.IsBasicTypeName(typeCoreName))
					{
						retVarCtx.VarTypeCategory = VAR_TYPE_CATEGORY.BASIC;
						if (null != init_group)
						{
							retVarCtx.InitValue(init_group.TextStr, parse_info);
						}
						else
						{
							retVarCtx.Value = 0;
						}
					}
					else
					{
						UserDefineTypeInfo udti = null;
						if (COMN_PROC.IsUsrDefTypeName(typeCoreName, parse_info, out udti))
						{
							retVarCtx.VarTypeCategory = VAR_TYPE_CATEGORY.USR_DEF_TYPE;
							retVarCtx.MemberList = GetUsrDefTypeVarCtxMemberList(udti, parse_info, init_group);
						}
						else
						{
							return null;
						}
					}
				}
			}
			return retVarCtx;
		}

		static string GetTypeNameFromGroup(MEANING_GROUP type_group, out List<string> prefix_list, out List<string> suffix_list)
		{
			prefix_list = new List<string>();
			suffix_list = new List<string>();
			string typeName = string.Empty;
			for (int i = 0; i < type_group.ComponentList.Count; i++)
			{
				string str = type_group.ComponentList[i].Text;
				if (i < type_group.PrefixCount)
				{
					prefix_list.Add(str);
				}
				else if (i < type_group.ComponentList.Count - type_group.SuffixCount)
				{
					typeName += " " + str;
					typeName = typeName.Trim();
				}
				else
				{
					suffix_list.Add(str);
				}
			}
			return typeName;
		}

		static List<VAR_CTX> GetUsrDefTypeVarCtxMemberList(	UserDefineTypeInfo usr_def_type_var,
															FILE_PARSE_INFO parse_info,
															MEANING_GROUP init_group)
		{
			List<VAR_CTX> retCtxList = new List<VAR_CTX>();
			List<MEANING_GROUP> memberInitList = null;
			if (null != init_group)
			{
				memberInitList = GetMemberInitGroupListFromCodeBlock(init_group, parse_info);
				if (null == memberInitList
					|| usr_def_type_var.MemberList.Count != memberInitList.Count)
				{
					return null;
				}
			}
			for (int i = 0; i < usr_def_type_var.MemberList.Count; i++)
			{
				string memberStr = usr_def_type_var.MemberList[i];
				List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(memberStr, parse_info);
				List<MEANING_GROUP> mgList = COMN_PROC.GetMeaningGroups(componentList, parse_info, null);
				MEANING_GROUP memberInitGroup = null;
				if (null != memberInitList)
				{
					memberInitGroup = memberInitList[i];
				}
				VAR_CTX varCtx = null;
				if (null != (varCtx = InOutAnalysis.CreateVarCtx(mgList[0], mgList[1].TextStr, memberInitGroup, parse_info)))
				{
					retCtxList.Add(varCtx);
				}
			}
			return retCtxList;
		}

		static int GetArraySizeFromVarName(ref string var_name, FILE_PARSE_INFO parse_info)
		{
			int startIdx = var_name.IndexOf('[');
			if (-1 == startIdx || !var_name.EndsWith("]"))
			{
				return 0;
			}
			int sizeVal = 0;
			string sizeStr = var_name.Substring(startIdx + 1, var_name.Length - startIdx - 2);
			if (!string.IsNullOrEmpty(sizeStr))
			{
				sizeVal = ExpCalc.GetConstExpressionValue(sizeStr, parse_info);
				var_name = var_name.Remove(startIdx).Trim();
			}
			return sizeVal;
		}

		static List<MEANING_GROUP> GetMemberInitGroupListFromCodeBlock(MEANING_GROUP block_init_group, FILE_PARSE_INFO parse_info)
		{
			if (block_init_group.Type != MeaningGroupType.CodeBlock
				|| block_init_group.ComponentList.Count <= 2
				|| "{" != block_init_group.ComponentList.First().Text
				|| "}" != block_init_group.ComponentList.Last().Text)
			{
				return null;
			}
			List<STATEMENT_COMPONENT> componentList = new List<STATEMENT_COMPONENT>();
			for (int i = 1; i < block_init_group.ComponentList.Count - 1; i++)
			{
				componentList.Add(block_init_group.ComponentList[i]);
			}
			List<MEANING_GROUP> retList = new List<MEANING_GROUP>();
			List<MEANING_GROUP> tmpList = COMN_PROC.GetMeaningGroups(componentList, parse_info, null);
			MEANING_GROUP addGroup = new MEANING_GROUP();
			addGroup.Type = MeaningGroupType.Expression;
			for (int i = 0; i < tmpList.Count; i++)
			{
				if (i == tmpList.Count - 1)
				{
					// 最后一个成员
					addGroup.ComponentList.AddRange(tmpList[i].ComponentList);
					foreach (var item in addGroup.ComponentList)
					{
						addGroup.TextStr += item.Text;
					}
					retList.Add(addGroup);
				}
				else
				{
					if (tmpList[i].TextStr.Equals(","))
					{
						foreach (var item in addGroup.ComponentList)
						{
							addGroup.TextStr += item.Text;
						}
						retList.Add(addGroup);
						addGroup = new MEANING_GROUP();
						addGroup.Type = MeaningGroupType.Expression;
					}
					else
					{
						addGroup.ComponentList.AddRange(tmpList[i].ComponentList);
					}
				}
			}
			return retList;
		}
	}
}
