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

		public VAR_Type Type = new VAR_Type();											// 类型名

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

	public class VAR_Type
	{
		public string Name = string.Empty;
		public string Qualifier = string.Empty;
	}

	public partial class InOutAnalysis
	{
		/// <summary>
		/// 根据变量名,从当前解析上下文中找出变量的上下文
		/// </summary>
		/// <param name="var_name"></param>
		/// <param name="ctx"></param>
		/// <returns></returns>
		static public VAR_CTX GetVarCtxByName(string var_name, AnalysisContext ctx, string type_name = null)
		{
			VAR_CTX var_ctx = null;
			if (null != (var_ctx = SearchVarCtxList(var_name, ctx)))
			{
				// 如果在当前的各个变量上下文列表中找到, 说明变量上下文已经创建, 直接返回
				return var_ctx;
			}
			else
			{
				VariableInfo vi = ctx.ParseResult.FindGlobalVarInfoByName(var_name);
				if (null != vi)
				{
					var_ctx = new VAR_CTX(vi.typeName, vi.varName);
					var_ctx.Type.Name = vi.realTypeName;
				}
				else
				{
					// 临时变量
					if (null == type_name)
					{
						type_name = "Unknown";
					}
					var_ctx = new VAR_CTX(type_name, var_name);
				}
				// 如果是构造类型, 要遍历生成各成员上下文直至基本类型
				return var_ctx;
			}
		}

		static VAR_CTX SearchVarCtxList(string var_name, AnalysisContext ctx)
		{
			VAR_CTX var_ctx = null;
			if ((null != (var_ctx = FindVarInVarCtxList(var_name, ctx.ParameterList)))		// (1)参数?
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, ctx.LocalVarList)))		// (2)临时变量?
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, ctx.InputGlobalList)))	// (3)全局变量?
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, ctx.OutputGlobalList)))
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, ctx.OtherGlobalList)))
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

		public static VAR_CTX CreateVarCtx(string type_name, string var_name, CodeParseInfo code_info)
		{
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(type_name) && !string.IsNullOrEmpty(var_name));

			// 提取类型名(分离前缀,后缀)
			List<string> prefixList, suffixList;
			string coreTypeName = CommonProcess.ExtractCoreTypeName(type_name, out prefixList, out suffixList);
			// 根据类型名检索类型定义
			if (CommonProcess.IsBasicVarType(coreTypeName))
			{
				VAR_CTX var_ctx = new VAR_CTX(type_name, var_name);
				return var_ctx;
			}
			else
			{
				VAR_CTX var_ctx = new VAR_CTX(type_name, var_name);
				return var_ctx;
			}
		}
	}
}
