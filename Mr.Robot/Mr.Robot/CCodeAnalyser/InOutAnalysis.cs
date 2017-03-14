using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot
{
	/// <summary>
	/// 入出力分析
	/// </summary>
	public partial class InOutAnalysis
	{
		public static bool LeftRightValueAnalysis(	List<MeaningGroup> mgList,
													FileParseInfo parse_info,
													FuncAnalysisContext func_ctx)
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
				if (1 == leftGroupList.Count
					&& MeaningGroupType.GlobalVariable == leftGroupList[0].Type)
				{
					VAR_CTX varCtx = GetVarCtxByName(leftGroupList[0].Text, parse_info, func_ctx);
					if (null != varCtx)
					{
						//if (null == varCtx.MeanningGroup)
						//{
						//	varCtx.MeanningGroup = leftGroupList[0];
						//}
						func_ctx.OutputGlobalList.Add(varCtx);
					}
				}

				List<MeaningGroup> rightGroupList = new List<MeaningGroup>();
				for (int i = eqIdx + 1; i < mgList.Count; i++)
				{
					rightGroupList.Add(mgList[i]);
				}
				RightValProcess(rightGroupList, parse_info, func_ctx);
			}
			else
			{
				// 如果没有赋值符号
				// 可能是没有初始化赋值的临时变量定义
				// 可能是函数调用
				// 可能是自增,自减等一元运算符
				RightValProcess(mgList, parse_info, func_ctx);
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

		static void RightValProcess(List<MeaningGroup> rightList, FileParseInfo parse_info, FuncAnalysisContext func_ctx)
		{
			foreach (MeaningGroup rightVal in rightList)
			{
				if (MeaningGroupType.GlobalVariable == rightVal.Type)					// 全局变量
				{
					VAR_CTX varCtx = GetVarCtxByName(rightVal.Text, parse_info, func_ctx);
					if (null != varCtx)
					{
						//varCtx.MeanningGroup = rightVal;
						if (null != func_ctx)
						{
							func_ctx.InputGlobalList.Add(varCtx);
						}
					}
				}
				else if (MeaningGroupType.FunctionCalling == rightVal.Type)				// 函数调用
				{
					CalledFunctionProcess(rightVal, parse_info, func_ctx);
				}
				else if (MeaningGroupType.Expression == rightVal.Type)					// 表达式
				{
					StatementAnalysis.ExpressionAnalysis(rightVal.ComponentList, parse_info, func_ctx);
				}

				if (   MeaningGroupType.GlobalVariable == rightVal.Type
					|| MeaningGroupType.LocalVariable == rightVal.Type)
				{
					// 如果是变量, 确认其是否被标记为曾做过函数调用的实参并传引用,
					// 如果是, 找到该函数调用并标记该调用对应实参位置是读出值
					CheckRightVarReadOut(rightVal, parse_info, func_ctx);
				}
			}
		}

		static void CalledFunctionProcess(MeaningGroup mg, FileParseInfo parse_info, FuncAnalysisContext func_ctx)
		{
			CalledFunction cf = new CalledFunction();
			cf.MeaningGroup = mg;
			if (CommonProcess.IsStandardIdentifier(mg.ComponentList[0].Text))
			{
				cf.FunctionName = mg.ComponentList[0].Text;
			}
			// 找出实参列表, 根据实参的类型和前缀确定是传值,还是传引用
			List<MeaningGroup> act_para_list = GetActualParameterList(mg, func_ctx);
			foreach (MeaningGroup ap in act_para_list)
			{
				// 分别判断各实参是传值还是传引用
				cf.ActualParaInfoList.Add(GetActualParaInfo(ap, parse_info, func_ctx));
			}
			foreach (ActualParaInfo api in cf.ActualParaInfoList)
			{
				// 如果实参是传引用, 那可能是函数调用读出值
				if (api.passType == ActParaPassType.Reference)
				{
					// 在上下文中标记该变量曾作为函数实参传引用(可能是读出值)
					// 在上下文中登录该函数调用可能是读出值
					// 以后若该标记的变量作为右值, 那就证实该函数调用的实参是读出值;
					RegisterVarPossibleReadOut(api, cf, parse_info, func_ctx);
				}
			}

			func_ctx.CalledFunctionList.Add(cf);
		}

		/// <summary>
		/// 在上下文中登录该变量可能被函数调用读出值赋值
		/// </summary>
		static void RegisterVarPossibleReadOut(ActualParaInfo api,
												CalledFunction cf,
												FileParseInfo parse_info,
												FuncAnalysisContext func_ctx)
		{
			// 在上下文中找出该变量
			VAR_CTX var_ctx = GetVarCtxByName(api.varName, parse_info, func_ctx);
			if (null != var_ctx)
			{
				var_ctx.CalledFunctionReadOut = cf.FunctionName;
			}
		}

		static List<MeaningGroup> GetActualParameterList(MeaningGroup mg, FuncAnalysisContext func_ctx)
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
		static ActualParaInfo GetActualParaInfo(MeaningGroup act_para, FileParseInfo parse_info, FuncAnalysisContext func_ctx)
		{
			ActualParaInfo actParaInfo = new ActualParaInfo();
			// 前缀 + 变量名
			actParaInfo.varName = act_para.ComponentList.Last().Text;
			actParaInfo.typeName = GetVarType(actParaInfo.varName, parse_info, func_ctx);
			for (int i = 0; i < act_para.ComponentList.Count - 1; i++)
			{
				actParaInfo.prefixList.Add(act_para.ComponentList[i].Text);
			}
			// 根据变量类型和前缀,判定实参的传递方式(传值或者传引用)

			// 确定变量的类型, 是值类型还是引用类型(指针类型)
			ActParaPassType var_type = JudgeVarParaType(actParaInfo.varName, parse_info, func_ctx);
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
		static ActParaPassType JudgeVarParaType(string var_name, FileParseInfo parse_info, FuncAnalysisContext func_ctx)
		{
			// 在当前的上下文中查找该名称的变量, 取得其类型名
			VAR_TYPE var_type = GetVarType(var_name, parse_info, func_ctx);
			if (null == var_type)
			{
				return ActParaPassType.Unknown;
			}
			if (var_type.SuffixList.Contains("*"))
			{
				return ActParaPassType.Reference;
			}
			else
			{
				return ActParaPassType.Value;
			}
		}

		static VAR_TYPE GetVarType(string var_name, FileParseInfo parse_info, FuncAnalysisContext func_ctx)
		{
			// TODO: 引数?

			// 临时变量?
			foreach (VAR_CTX local_var in func_ctx.LocalVarList)
			{
				if (local_var.Name.Equals(var_name))
				{
					return local_var.Type;
				}
			}
			// 全局变量?
			VAR_CTX varCtx = parse_info.FindGlobalVarInfoByName(var_name);
			if (null != varCtx)
			{
				return varCtx.Type;
			}
			return null;
		}

		/// <summary>
		/// 确定一个作右值的变量是否被标记过做过函数调用的实参并传引用
		/// </summary>
		static void CheckRightVarReadOut(MeaningGroup rightVal, FileParseInfo parse_info, FuncAnalysisContext func_ctx)
		{
			string rVarName = GetPrimaryVarName(rightVal);
			if (string.Empty == rVarName)
			{
				return;
			}
			VAR_CTX varCtx = GetVarCtxByName(rVarName, parse_info, func_ctx);
			if (null != varCtx)
			{
				// 如果该变量曾被标记过作为函数调用的实参并传引用
				if (string.Empty != varCtx.CalledFunctionReadOut)
				{
					// 找到该函数调用并且标记其对应的实参位置为函数调用读出值
					foreach (CalledFunction cf in func_ctx.CalledFunctionList)
					{
						if (cf.FunctionName.Equals(varCtx.CalledFunctionReadOut))
						{
							foreach (ActualParaInfo api in cf.ActualParaInfoList)
							{
								if (api.varName.Equals(varCtx.Name))
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
		Unknown,
	}

	public class ActualParaInfo
	{
		public string varName = string.Empty;
		public VAR_TYPE typeName = null;
		public List<string> prefixList = new List<string>();
		public ActParaPassType passType = ActParaPassType.Value;
		public bool readOut = false;													// 是否是读出值
	}
}
