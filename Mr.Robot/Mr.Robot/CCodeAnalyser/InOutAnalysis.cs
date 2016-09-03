using System;
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
					analysisContext.outputGlobalList.Add(leftVal);
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

		static void RightValProcess(List<MeaningGroup> rightList, AnalysisContext analysisContext)
		{
			foreach (MeaningGroup item in rightList)
			{
				if (item.Type == MeaningGroupType.GlobalVariable)						// 全局变量
				{
					analysisContext.inputGlobalList.Add(item);
				}
				else if (item.Type == MeaningGroupType.FunctionCalling)					// 函数调用
				{
					CalledFunctionProcess(item, analysisContext);
				}
				else if (item.Type == MeaningGroupType.Expression)						// 表达式
				{
					CCodeAnalyser.ExpressionAnalysis(item.ComponentList, analysisContext);
				}
			}
		}

		static void CalledFunctionProcess(MeaningGroup mg, AnalysisContext ctx)
		{
			CalledFunction cf = new CalledFunction();
			cf.meaningGroup = mg;
			// 找出实参列表, 根据实参的类型确定是传值,还是传引用
			if (mg.ComponentList.Count >= 3
				&& CommonProcess.IsStandardIdentifier(mg.ComponentList.First().Text)	// 函数名
				&& "(" == mg.ComponentList[1].Text										// 函数名后跟着左括号
				&& ")" == mg.ComponentList.Last().Text)									// 最后是右括号
			{
				// 取得实参列表
				MeaningGroup act_para = new MeaningGroup();
				List<MeaningGroup> act_para_list = new List<MeaningGroup>();
				for (int i = 2; i < mg.ComponentList.Count - 1; i++)
				{
					if (	"," == mg.ComponentList[i].Text
						&&	(0 != act_para.ComponentList.Count) )
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
				foreach (MeaningGroup ap in act_para_list)
				{
					// 分别判断各实参是传值还是传引用
					cf.paraTypeList.Add(JudgeActualParaType(ap, ctx));
				}
			}
			else
			{
				return;
			}

			ctx.calledFunctionList.Add(cf);
		}

		// 判断实参的种别(传值或者传引用)
		static FunParaType JudgeActualParaType(MeaningGroup act_para, AnalysisContext ctx)
		{
			// 前缀 + 变量名
			// 最后面是变量名
			string var_name = act_para.ComponentList.Last().Text;
			// 确定变量的类型, 是值类型还是引用类型(指针类型)
			FunParaType var_type = JudgeVarParaType(var_name, ctx);
			// 在确定前缀(取地址&, 或者取指针指向的变量*)
			if (2 == act_para.ComponentList.Count)
			{
				if ("&" == act_para.ComponentList.First().Text
					&& var_type == FunParaType.Value)
				{
					return FunParaType.Reference;
				}
				else if ("*" == act_para.ComponentList.First().Text
						 && var_type == FunParaType.Reference)
				{
					return FunParaType.Value;
				}
				else
				{
					return var_type;
				}
			}
			else
			{
				return var_type;
			}
		}

		/// <summary>
		/// 判断变量的类型是值类型亦或是指针类型
		/// </summary>
		static FunParaType JudgeVarParaType(string var_name, AnalysisContext ctx)
		{
			// 在当前的上下文中查找该名称的变量, 取得其类型名
			string var_type = GetVarTypeName(var_name, ctx);
			if (string.IsNullOrEmpty(var_type))
			{
				return FunParaType.Value;
			}
			return FunParaType.Value;
		}

		static string GetVarTypeName(string var_name, AnalysisContext ctx)
		{
			// 临时变量?
			foreach (VAR_CTX local_var in ctx.local_list)
			{
				if (local_var.name.Equals(var_name))
				{
					return local_var.type;
				}
			}
			// 全局变量?
			VariableInfo vi = ctx.parseResult.FindGlobalVarInfoByName(var_name);
			if (null != vi)
			{
				return vi.typeName;
			}

			return string.Empty;
		}
	}

	/// <summary>
	/// 函数引数的种别(引用类型或者值类型)
	/// </summary>
	public enum FunParaType
	{
		Reference,
		Value,
	}
}
