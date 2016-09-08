﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot
{
	/// <summary>
	/// 入出力分析
	/// </summary>
	public class InOutAnalysis
	{
		public static bool LeftRightValueAnalysis(List<MeaningGroup> mgList,
												  AnalysisContext analysisContext)
		{
			List<MeaningGroup> rightValue = new List<MeaningGroup>();
			int eqIdx = -1;
			if (-1 != (eqIdx = FindEqualMarkIndex(mgList)))
			{
				// 如果有赋值符号"="
				List<MeaningGroup> leftGroupList = new List<MeaningGroup>();
				for (int i = 0; i < eqIdx; i++)
				{
					leftGroupList.Add(mgList[i]);
				}
				MeaningGroup leftVal = LeftValProcess(leftGroupList);
				if (null != leftVal)
				{
					VAR_CTX varContext = new VAR_CTX();
					varContext.meanning_group = leftVal;
					varContext.name = leftVal.Text;
					analysisContext.outputGlobalList.Add(varContext);
				}

				List<MeaningGroup> rightGroupList = new List<MeaningGroup>();
				for (int i = eqIdx + 1; i < mgList.Count; i++)
				{
					rightGroupList.Add(mgList[i]);
				}
				RightValProcess(rightGroupList, analysisContext);
			}
			else
			{
				// 如果没有赋值符号
				// 可能是没有初始化赋值的临时变量定义
				// 可能是函数调用
				// 可能是自增,自减等一元运算符
				RightValProcess(mgList, analysisContext);
			}
			return false;
		}

		static int FindEqualMarkIndex(List<MeaningGroup> mgList)
		{
			for (int i = 0; i < mgList.Count; i++)
			{
				if (mgList[i].Type == MeaningGroupType.EqualMark)						// "=", 等号赋值符
				{
					return i;
				}
			}
			return -1;
		}

		static MeaningGroup LeftValProcess(List<MeaningGroup> leftList)
		{
			if (leftList.Count == 1
				&& leftList[0].Type == MeaningGroupType.GlobalVariable)
			{
				// 局部或者全局变量做左值
				return leftList[0];
			}
			else
			{
				return null;
			}
		}

		static void RightValProcess(List<MeaningGroup> rightList, AnalysisContext ctx)
		{
			foreach (MeaningGroup rightVal in rightList)
			{
				if (rightVal.Type == MeaningGroupType.GlobalVariable)					// 全局变量
				{
					VAR_CTX varContext = new VAR_CTX();
					varContext.meanning_group = rightVal;
					varContext.name = rightVal.Text;
					ctx.inputGlobalList.Add(varContext);
				}
				else if (rightVal.Type == MeaningGroupType.FunctionCalling)				// 函数调用
				{
					CalledFunctionProcess(rightVal, ctx);
				}
				else if (rightVal.Type == MeaningGroupType.Expression)					// 表达式
				{
					CCodeAnalyser.ExpressionAnalysis(rightVal.ComponentList, ctx);
				}

				if (   MeaningGroupType.GlobalVariable == rightVal.Type
					|| MeaningGroupType.LocalVariable == rightVal.Type)
				{
					// 如果是变量, 确认其是否被标记为曾做过函数调用的实参并传引用,
					// 如果是, 找到该函数调用并标记该调用对应实参位置是读出值
					CheckRightVarReadOut(rightVal, ctx);
				}
			}
		}

		static void CalledFunctionProcess(MeaningGroup mg, AnalysisContext ctx)
		{
			CalledFunction cf = new CalledFunction();
			cf.meaningGroup = mg;
			if (CommonProcess.IsStandardIdentifier(mg.ComponentList[0].Text))
			{
				cf.functionName = mg.ComponentList[0].Text;
			}
			// 找出实参列表, 根据实参的类型和前缀确定是传值,还是传引用
			List<MeaningGroup> act_para_list = GetActualParameterList(mg, ctx);
			foreach (MeaningGroup ap in act_para_list)
			{
				// 分别判断各实参是传值还是传引用
				cf.actParaInfoList.Add(GetActualParaInfo(ap, ctx));
			}
			foreach (ActualParaInfo api in cf.actParaInfoList)
			{
				// TODO: 如果实参是传引用, 那可能是函数调用读出值
				if (api.passType == ActParaPassType.Reference)
				{
					// 在上下文中标记该变量曾作为函数实参传引用(可能是读出值)
					// 在上下文中登录该函数调用可能是读出值
					// 以后若该标记的变量作为右值, 那就证实该函数调用的实参是读出值;
					RegisterVarPossibleReadOut(api, cf, ctx);
				}
			}

			ctx.calledFunctionList.Add(cf);
		}

		/// <summary>
		/// 在上下文中登录该变量可能被函数调用读出值赋值
		/// </summary>
		static void RegisterVarPossibleReadOut(ActualParaInfo api,
											   CalledFunction cf,
											   AnalysisContext ctx)
		{
			// 在上下文中找出该变量
			VAR_CTX var_ctx = SearchVarInContext(api.varName, ctx);
			if (null != var_ctx)
			{
				var_ctx.called_function_readout = cf.functionName;
			}
			// 如果在上下文中没找到, 看是否是在其它地方定义的全局变量
			else
			{
				VariableInfo vi = ctx.parseResult.FindGlobalVarInfoByName(api.varName);
				if (null != vi)
				{
					var_ctx = new VAR_CTX();
					var_ctx.name = vi.varName;
					var_ctx.type = vi.typeName;
					var_ctx.real_type = vi.realTypeName;
					var_ctx.called_function_readout = cf.functionName;
				}
			}
		}

		static VAR_CTX SearchVarInContext(string var_name, AnalysisContext ctx)
		{
			VAR_CTX var_ctx = null;
			if (   (null != (var_ctx = SearchVarInContextList(var_name, ctx.parameter_list)))	// (1)参数?
				|| (null != (var_ctx = SearchVarInContextList(var_name, ctx.local_list)))		// (2)临时变量?
				|| (null != (var_ctx = SearchVarInContextList(var_name, ctx.inputGlobalList)))	// (3)全局变量?
				|| (null != (var_ctx = SearchVarInContextList(var_name, ctx.outputGlobalList)))
				|| (null != (var_ctx = SearchVarInContextList(var_name, ctx.otherGlobalList)))
				)
			{
				return var_ctx;
			}
			else
			{
				return null;
			}
		}

		static VAR_CTX SearchVarInContextList(string var_name, List<VAR_CTX> ctx_list)
		{
			foreach (VAR_CTX ctx in ctx_list)
			{
				if (ctx.name.Equals(var_name))
				{
					return ctx;
				}
			}
			return null;
		}

		static List<MeaningGroup> GetActualParameterList(MeaningGroup mg, AnalysisContext ctx)
		{
			List<MeaningGroup> act_para_list = new List<MeaningGroup>();
			if (mg.ComponentList.Count >= 3
				&& CommonProcess.IsStandardIdentifier(mg.ComponentList.First().Text)	// 函数名
				&& "(" == mg.ComponentList[1].Text										// 函数名后跟着左括号
				&& ")" == mg.ComponentList.Last().Text)									// 最后是右括号
			{
				// 取得实参列表
				MeaningGroup act_para = new MeaningGroup();
				for (int i = 2; i < mg.ComponentList.Count - 1; i++)
				{
					if ("," == mg.ComponentList[i].Text
						&& (0 != act_para.ComponentList.Count))
					{
						act_para_list.Add(act_para);
						act_para = new MeaningGroup();
					}
					else
					{
						act_para.ComponentList.Add(mg.ComponentList[i]);
						act_para.Text += mg.ComponentList[i].Text;
					}
				}
				if (0 != act_para.ComponentList.Count)
				{
					act_para_list.Add(act_para);
				}
			}
			return act_para_list;
		}

		// 判断实参的种别(传值或者传引用)
		static ActualParaInfo GetActualParaInfo(MeaningGroup act_para, AnalysisContext ctx)
		{
			ActualParaInfo actParaInfo = new ActualParaInfo();
			// 前缀 + 变量名
			actParaInfo.varName = act_para.ComponentList.Last().Text;
			actParaInfo.typeName = GetVarTypeName(actParaInfo.varName, ctx);
			for (int i = 0; i < act_para.ComponentList.Count - 1; i++)
			{
				actParaInfo.prefixList.Add(act_para.ComponentList[i].Text);
			}
			// 根据变量类型和前缀,判定实参的传递方式(传值或者传引用)

			// 确定变量的类型, 是值类型还是引用类型(指针类型)
			ActParaPassType var_type = JudgeVarParaType(actParaInfo.varName, ctx);
			// 在确定前缀(取地址&, 或者取指针指向的变量*)
			if (2 == act_para.ComponentList.Count)
			{
				if ("&" == act_para.ComponentList.First().Text
					&& var_type == ActParaPassType.Value)
				{
					actParaInfo.passType = ActParaPassType.Reference;
				}
				else if ("*" == act_para.ComponentList.First().Text
						 && var_type == ActParaPassType.Reference)
				{
					actParaInfo.passType = ActParaPassType.Value;
				}
				else
				{
				}
			}
			else
			{
			}
			return actParaInfo;
		}

		/// <summary>
		/// 判断变量的类型是值类型亦或是指针类型
		/// </summary>
		static ActParaPassType JudgeVarParaType(string var_name, AnalysisContext ctx)
		{
			// 在当前的上下文中查找该名称的变量, 取得其类型名
			string var_type = GetVarTypeName(var_name, ctx);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(var_type));
			if (var_type.Trim().EndsWith("*"))
			{
				return ActParaPassType.Reference;
			}
			else
			{
				return ActParaPassType.Value;
			}
		}

		static string GetVarTypeName(string var_name, AnalysisContext ctx)
		{
			// TODO: 引数?

			// 临时变量?
			foreach (VAR_CTX local_var in ctx.local_list)
			{
				if (local_var.name.Equals(var_name))
				{
					if (string.Empty != local_var.real_type)
					{
						return local_var.real_type;
					}
					else
					{
						return local_var.type;
					}
				}
			}
			// 全局变量?
			VariableInfo vi = ctx.parseResult.FindGlobalVarInfoByName(var_name);
			if (null != vi)
			{
				if (string.Empty != vi.realTypeName)
				{
					return vi.realTypeName;
				}
				else
				{
					return vi.typeName;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// 确定一个作右值的变量是否被标记过做过函数调用的实参并传引用
		/// </summary>
		static void CheckRightVarReadOut(MeaningGroup rightVal, AnalysisContext ctx)
		{
			string rVarName = GetPrimaryVarName(rightVal);
			if (string.Empty == rVarName)
			{
				return;
			}
			VAR_CTX varCtx = SearchVarInContext(rVarName, ctx);
			if (null != varCtx)
			{
				// 如果该变量曾被标记过作为函数调用的实参并传引用
				if (string.Empty != varCtx.called_function_readout)
				{
					// 找到该函数调用并且标记其对应的实参位置为函数调用读出值
					foreach (CalledFunction cf in ctx.calledFunctionList)
					{
						if (cf.functionName.Equals(varCtx.called_function_readout))
						{
							foreach (ActualParaInfo api in cf.actParaInfoList)
							{
								if (api.varName.Equals(varCtx.name))
								{
									api.readOut = true;
								}
							}
						}
					}
				}
			}
		}

		static string GetPrimaryVarName(MeaningGroup mg)
		{
			foreach (StatementComponent sc in mg.ComponentList)
			{
				if (CommonProcess.IsStandardIdentifier(sc.Text))
				{
					return sc.Text;
				}
			}
			return string.Empty;
		}
	}

	/// <summary>
	/// 函数调用实参的传递方式(传值或者传引用)
	/// </summary>
	public enum ActParaPassType
	{
		Reference,
		Value,
	}

	public class ActualParaInfo
	{
		public string varName = string.Empty;
		public string typeName = string.Empty;
		public List<string> prefixList = new List<string>();
		public ActParaPassType passType = ActParaPassType.Value;
		public bool readOut = false;													// 是否是读出值
	}
}
