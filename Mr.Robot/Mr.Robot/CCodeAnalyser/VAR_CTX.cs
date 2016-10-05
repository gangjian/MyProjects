﻿using System;
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
		private string _name = string.Empty;											// 变量名
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private string _type = string.Empty;											// 类型名
		public string Type
		{
			get { return _type; }
			set { _type = value; }
		}

		private string _real_type = string.Empty;										// 如果类型名是"typedef"定义的别名的话,原类型名
		public string RealType
		{
			get { return _real_type; }
			set { _real_type = value; }
		}

		private MeaningGroup _meanning_group = null;									// 构成该变量的成分组合
		public MeaningGroup MeanningGroup
		{
			get { return _meanning_group; }
			set { _meanning_group = value; }
		}

		private string _called_function_readout = string.Empty;							// 可能被函数调用的读出值赋值(函数名)
		public string CalledFunctionReadOut
		{
			get { return _called_function_readout; }
			set { _called_function_readout = value; }
		}

		private object _cur_val = new object();											// 当前值
		public object CurVal
		{
			get { return _cur_val; }
			set { _cur_val = value; }
		}

		private List<VAR_CTX> _memberList = new List<VAR_CTX>();						// 成员列表(下一层级)
		public List<VAR_CTX> MemberList
		{
			get { return _memberList; }
			set { _memberList = value; }
		}

		VAR_CTX _parent = null;															// 上一层级

		public VAR_CTX Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		public VAR_CTX(string type_name, string var_name)								// 构造方法
		{
			Trace.Assert(!string.IsNullOrEmpty(type_name));
			Trace.Assert(!string.IsNullOrEmpty(var_name));
			this.Type = type_name;
			this.Name = var_name;
		}
	}

	public partial class InOutAnalysis
	{
		/// <summary>
		/// 根据变量名,从当前解析上下文中找出变量的上下文
		/// </summary>
		/// <param name="var_name"></param>
		/// <param name="ctx"></param>
		/// <returns></returns>
		static public VAR_CTX GetVarCtx(string var_name, AnalysisContext ctx, string type_name = null)
		{
			VAR_CTX var_ctx = null;
			if (null != (var_ctx = SearchVarCtxList(var_name, ctx)))
			{
				// 如果在当前的各个变量上下文列表中找到, 说明变量上下文已经创建, 直接返回
				return var_ctx;
			}
			else
			{
				VariableInfo vi = ctx.parseResult.FindGlobalVarInfoByName(var_name);
				if (null != vi)
				{
					var_ctx = new VAR_CTX(vi.typeName, vi.varName);
					var_ctx.RealType = vi.realTypeName;
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
			if ((null != (var_ctx = FindVarInVarCtxList(var_name, ctx.parameter_list)))		// (1)参数?
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, ctx.local_list)))		// (2)临时变量?
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, ctx.inputGlobalList)))	// (3)全局变量?
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, ctx.outputGlobalList)))
				|| (null != (var_ctx = FindVarInVarCtxList(var_name, ctx.otherGlobalList)))
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

		public static VAR_CTX CreateNewVarCtx(string type_name, string var_name)
		{
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(type_name) && !string.IsNullOrEmpty(var_name));

			// 提取类型名(分离前缀,后缀)
			List<string> prefixList, suffixList;
			string typeName = CommonProcess.ExtractCoreTypeName(type_name, out prefixList, out suffixList);
			// 根据类型名检索类型定义
			if (CommonProcess.IsBasicVarType(typeName))
			{
				
			}
			else
			{

			}

			VAR_CTX var_ctx = new VAR_CTX(typeName, var_name);
			return var_ctx;
		}

	}
}
