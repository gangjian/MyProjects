using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Mr.Robot
{
	/// <summary>
	/// 变量上下文
	/// </summary>
	public class VAR_CTX
	{
		public string Name = string.Empty;												// 变量名

		public VAR_TYPE Type = new VAR_TYPE();											// 类型名

		public VAR_TYPE_CATEGORY Category = VAR_TYPE_CATEGORY.BASIC;

		public MeaningGroup MeanningGroup = null;										// 构成该变量的成分组合
																						// TODO: 这个成员在VAR_CTX类改造完成后要删掉!
																						// 即函数出入力列表与变量上下文分离, 20161206

		public string CalledFunctionReadOut = string.Empty;								// 可能被函数调用的读出值赋值(函数名)

		public List<VAR_CTX> MemberList = new List<VAR_CTX>();							// 成员列表(下一层级)

		public VAR_CTX(string type_name, string var_name)								// 构造方法
		{
			Trace.Assert(!string.IsNullOrEmpty(type_name));
			Trace.Assert(!string.IsNullOrEmpty(var_name));
			this.Type.Name = type_name;
			this.Name = var_name;
		}
	}

	public class VAR_TYPE
	{
		public string Name = string.Empty;
		public List<string> PrefixList = new List<string>();
		public List<string> SuffixList = new List<string>();
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
		static public VAR_CTX GetVarCtxByName(string var_name, FileParseInfo parse_info, FuncAnalysisContext func_ctx)
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

		static VAR_CTX SearchVarCtxList(string var_name, FuncAnalysisContext func_ctx)
		{
			if (null == func_ctx)
			{
				return null;
			}
			VAR_CTX var_ctx = null;
			if ((null != (var_ctx = FindVarInVarCtxList(var_name, func_ctx.ParameterList)))			// (1)参数?
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, func_ctx.LocalVarList)))		// (2)临时变量?
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, func_ctx.InputGlobalList)))	// (3)全局变量?
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, func_ctx.OutputGlobalList)))
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

		public static VAR_CTX CreateVarCtx(	MeaningGroup type_group,
											MeaningGroup var_group,
											MeaningGroup init_group,
											FileParseInfo parse_info)
		{
			string var_name = var_group.Text;
			List<string> prefixList = new List<string>();
			List<string> suffixList = new List<string>();
			string typeName = string.Empty;
			for (int i = 0; i < type_group.ComponentList.Count; i++)
			{
				string str = type_group.ComponentList[i].Text;
				if (i < type_group.PrefixCount)
				{
					prefixList.Add(str);
				}
				else if (i < type_group.ComponentList.Count - type_group.SuffixCount)
				{
					typeName += " " + str;
					typeName = typeName.Trim();
				}
				else
				{
					suffixList.Add(str);
				}
			}
			TypeDefineInfo tdi = null;
			while (null != (tdi = parse_info.FindTypeDefInfo(typeName)))
			{
				typeName = tdi.OldName;
			}
			int arrSize = 0;
			if (var_name.EndsWith("]"))													// 包含下标,说明是数组
			{
				arrSize = GetArraySizeFromVarName(ref var_name, parse_info);
			}
			VAR_CTX retVarCtx = new VAR_CTX(typeName, var_name);
			retVarCtx.Type.PrefixList = prefixList;
			retVarCtx.Type.SuffixList = suffixList;
			if (suffixList.Contains("*"))
			{
				retVarCtx.Category = VAR_TYPE_CATEGORY.POINTER;
			}
			else
			{
				if (CommonProcess.IsBasicTypeName(typeName))
				{
					retVarCtx.Category = VAR_TYPE_CATEGORY.BASIC;
				}
				else
				{
					UsrDefTypeInfo udti = null;
					if (CommonProcess.IsUsrDefTypeName(typeName, parse_info, out udti))
					{
						retVarCtx.Category = VAR_TYPE_CATEGORY.USR_DEF_TYPE;
						retVarCtx.MemberList = GetUsrDefTypeVarCtxMemberList(udti, parse_info);
					}
					else
					{
						return null;
					}
				}
			}
			return retVarCtx;
		}

		static List<VAR_CTX> GetUsrDefTypeVarCtxMemberList(	UsrDefTypeInfo usr_def_type_var,
															FileParseInfo parse_info)
		{
			List<VAR_CTX> retCtxList = new List<VAR_CTX>();
			for (int i = 0; i < usr_def_type_var.MemberList.Count; i++)
			{
				string memberStr = usr_def_type_var.MemberList[i];
				List<StatementComponent> componentList = StatementAnalysis.GetComponents(memberStr, parse_info);
				List<MeaningGroup> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, parse_info, null);
				VAR_CTX varCtx = null;
				if (null != (varCtx = StatementAnalysis.IsNewDefineVarible(meaningGroupList, parse_info, null)))
				{
					retCtxList.Add(varCtx);
				}
			}
			return retCtxList;
		}

		static int GetArraySizeFromVarName(ref string var_name, FileParseInfo parse_info)
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
				sizeVal = ExpCalc.GetLogicalExpressionValue(sizeStr, parse_info);
			}
			return 0;
		}
	}
}
