using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mr.Robot.CDeducer;

namespace Mr.Robot
{
	/// <summary>
	/// 入出力分析
	/// </summary>
	public partial class InOutAnalysis
	{
		public static bool LeftRightValueAnalysis(	List<MEANING_GROUP> mgList,
													FILE_PARSE_INFO parse_info,
													FUNC_CONTEXT func_ctx)
		{
			List<MEANING_GROUP> rightValue = new List<MEANING_GROUP>();
			int eqIdx = -1;
			if (-1 != (eqIdx = FindEqualMarkIndex(mgList)))
			{
				// 如果有赋值符号"="
				List<MEANING_GROUP> leftGroupList = new List<MEANING_GROUP>();
				for (int i = 0; i < eqIdx; i++)
				{
					leftGroupList.Add(mgList[i]);
				}
				if (1 == leftGroupList.Count
					&& MeaningGroupType.GlobalVariable == leftGroupList[0].Type)
				{
					VAR_DESCRIPTION varDescrp = GetVarDescriptionFromExpression(leftGroupList[0]);
					if (null != varDescrp)
					{
						func_ctx.OutputGlobalList.Add(varDescrp);
					}
				}

				List<MEANING_GROUP> rightGroupList = new List<MEANING_GROUP>();
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

		static int FindEqualMarkIndex(List<MEANING_GROUP> mgList)
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

		static void RightValProcess(List<MEANING_GROUP> rightList, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
		{
			foreach (MEANING_GROUP rightVal in rightList)
			{
				if (MeaningGroupType.GlobalVariable == rightVal.Type					// 全局变量
					&& null != func_ctx)
				{
					VAR_DESCRIPTION varDescrp = GetVarDescriptionFromExpression(rightVal);
					if (null != varDescrp)
					{
						func_ctx.InputGlobalList.Add(varDescrp);
					}
				}
				else if (MeaningGroupType.FunctionCalling == rightVal.Type)				// 函数调用
				{
					CalledFunctionProcess(rightVal, parse_info, func_ctx);
				}
				else if (MeaningGroupType.Expression == rightVal.Type)					// 表达式
				{
					C_DEDUCER.ExpressionProc(rightVal.ComponentList, parse_info, func_ctx, null, null);
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

		static void CalledFunctionProcess(MEANING_GROUP mg, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
		{
			CALLED_FUNCTION cf = new CALLED_FUNCTION();
			cf.MeaningGroup = mg;
			if (COMN_PROC.IsStandardIdentifier(mg.ComponentList[0].Text))
			{
				cf.FunctionName = mg.ComponentList[0].Text;
			}
			// 找出实参列表, 根据实参的类型和前缀确定是传值,还是传引用
			List<MEANING_GROUP> act_para_list = GetActualParameterList(mg, func_ctx);
			foreach (MEANING_GROUP ap in act_para_list)
			{
				// 分别判断各实参是传值还是传引用
				cf.ActualParaInfoList.Add(GetActualParaInfo(ap, parse_info, func_ctx));
			}
			foreach (ACTUAL_PARA_INFO api in cf.ActualParaInfoList)
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
		static void RegisterVarPossibleReadOut(ACTUAL_PARA_INFO api,
												CALLED_FUNCTION cf,
												FILE_PARSE_INFO parse_info,
												FUNC_CONTEXT func_ctx)
		{
			// 在上下文中找出该变量
			VAR_CTX var_ctx = GetVarCtxByName(api.varName, parse_info, func_ctx);
			if (null != var_ctx)
			{
				var_ctx.CalledFunctionReadOut = cf.FunctionName;
			}
		}

		static List<MEANING_GROUP> GetActualParameterList(MEANING_GROUP mg, FUNC_CONTEXT func_ctx)
		{
			List<MEANING_GROUP> act_para_list = new List<MEANING_GROUP>();
			if (mg.ComponentList.Count >= 3
				&& COMN_PROC.IsStandardIdentifier(mg.ComponentList.First().Text)	// 函数名
				&& "(" == mg.ComponentList[1].Text										// 函数名后跟着左括号
				&& ")" == mg.ComponentList.Last().Text)									// 最后是右括号
			{
				// 取得实参列表
				MEANING_GROUP act_para = new MEANING_GROUP();
				for (int i = 2; i < mg.ComponentList.Count - 1; i++)
				{
					if ("," == mg.ComponentList[i].Text
						&& (0 != act_para.ComponentList.Count))
					{
						act_para_list.Add(act_para);
						act_para = new MEANING_GROUP();
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
		static ACTUAL_PARA_INFO GetActualParaInfo(MEANING_GROUP act_para, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
		{
			ACTUAL_PARA_INFO actParaInfo = new ACTUAL_PARA_INFO();
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
		static ActParaPassType JudgeVarParaType(string var_name, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
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

		static VAR_TYPE GetVarType(string var_name, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
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
		static void CheckRightVarReadOut(MEANING_GROUP rightVal, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
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
					foreach (CALLED_FUNCTION cf in func_ctx.CalledFunctionList)
					{
						if (cf.FunctionName.Equals(varCtx.CalledFunctionReadOut))
						{
							foreach (ACTUAL_PARA_INFO api in cf.ActualParaInfoList)
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

		static string GetPrimaryVarName(MEANING_GROUP mg)
		{
			foreach (STATEMENT_COMPONENT sc in mg.ComponentList)
			{
				if (COMN_PROC.IsStandardIdentifier(sc.Text))
				{
					return sc.Text;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// 从变量表达式得到一个对变量结构层次的描述
		/// </summary>
		static VAR_DESCRIPTION GetVarDescriptionFromExpression(MEANING_GROUP meaning_group)
		{
			int cmpntCount = meaning_group.ComponentList.Count;
			System.Diagnostics.Trace.Assert(cmpntCount > 0);

			VAR_DESCRIPTION retDesp = new VAR_DESCRIPTION();
			if (meaning_group.ComponentList.Last().Type == StatementComponentType.Identifier)
			{
				VAR_LEVEL lastVarLever = new VAR_LEVEL(meaning_group.ComponentList.Last().Text);
				if (cmpntCount > 1)
				{
					string prevStr = meaning_group.ComponentList[cmpntCount - 2].Text;
					VAR_MEMBER_OPERATOR prevMemOpt = GetVarMemberOper(prevStr);
					if (VAR_MEMBER_OPERATOR.NONE != prevMemOpt)
					{																	// 有层次结构的变量
						MEANING_GROUP prevGroup = new MEANING_GROUP();
						for (int i = 0; i < cmpntCount - 2; i++)
						{
							prevGroup.ComponentList.Add(meaning_group.ComponentList[i]);
						}
						VAR_DESCRIPTION prevDesp = GetVarDescriptionFromExpression(prevGroup);
						if (0 != prevDesp.VarLevelList.Count)
						{
							prevDesp.VarLevelList.Last().MemberOperator = prevMemOpt;
							retDesp.VarLevelList.AddRange(prevDesp.VarLevelList);
							retDesp.VarLevelList.Add(lastVarLever);
							retDesp.SetTextStr();
							return retDesp;
						}
					}
					else if ((prevStr.Equals("&") || prevStr.Equals("*")) && 2 == cmpntCount)
					{																	// 变量前有修饰符(暂限定只有一个:取地址符&或者星号*)
						lastVarLever.PrefixList.Add(prevStr);
						retDesp.VarLevelList.Add(lastVarLever);
						retDesp.SetTextStr();
						return retDesp;
					}
					else
					{
					}
				}
				else
				{																		// 单一的变量(无层次结构)
					retDesp.VarLevelList.Add(lastVarLever);
					retDesp.SetTextStr();
					return retDesp;
				}
			}
			else if (meaning_group.ComponentList.Last().Text.Equals(")"))
			{																			// 圆括号括起的表达式
				// 向前找到对应的圆括号
				int idx = FindPreviousMatchBrace(meaning_group.ComponentList, cmpntCount - 1);
				if (-1 != idx)
				{
					MEANING_GROUP innerGroup = new MEANING_GROUP();
					for (int i = idx + 1; i < cmpntCount - 1; i++)
					{
						innerGroup.ComponentList.Add(meaning_group.ComponentList[i]);
					}
					// 递归
					retDesp = GetVarDescriptionFromExpression(innerGroup);
					if (idx > 0)
					{
						for (int i = 0; i < idx; i++)
						{
							retDesp.VarLevelList.Last().PrefixList.Add(meaning_group.ComponentList[i].Text);
						}
					}
					retDesp.SetTextStr();
					return retDesp;
				}
			}
			else
			{
			}
			throw new MyException("GetVarDescriptionFromExpression(..) : 内部逻辑错误");
		}

		static int FindPreviousMatchBrace(List<STATEMENT_COMPONENT> cmpnt_list, int idx)
		{
			System.Diagnostics.Trace.Assert(cmpnt_list.Count > 0 && idx >= 0 && idx < cmpnt_list.Count);
			System.Diagnostics.Trace.Assert(cmpnt_list[idx].Text.Equals(")"));
			int counter = 1;
			for (int i = idx - 1; i >= 0; i--)
			{
				if (cmpnt_list[i].Text.Equals(")"))
				{
					counter += 1;
				}
				else if (cmpnt_list[i].Text.Equals("("))
				{
					counter -= 1;
					if (0 == counter)
					{
						return i;
					}
				}
			}
			return -1;
		}

		static VAR_MEMBER_OPERATOR GetVarMemberOper(string text_str)
		{
			if (text_str.Equals("."))
			{
				return VAR_MEMBER_OPERATOR.DOT;
			}
			else if (text_str.Equals("->"))
			{
				return VAR_MEMBER_OPERATOR.ARROW;
			}
			else
			{
				return VAR_MEMBER_OPERATOR.NONE;
			}
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

	public class ACTUAL_PARA_INFO
	{
		public string varName = string.Empty;
		public VAR_TYPE typeName = null;
		public List<string> prefixList = new List<string>();
		public ActParaPassType passType = ActParaPassType.Value;
		public bool readOut = false;													// 是否是读出值
	}

	public class VAR_DESCRIPTION
	{
		public List<VAR_LEVEL> VarLevelList = new List<VAR_LEVEL>();
		public string Text = string.Empty;

		public void SetTextStr()
		{
			this.Text = string.Empty;
			foreach (var vl in this.VarLevelList)
			{
				vl.SetTextStr();
				this.Text += vl.Text;
			}
		}
	}

	public class VAR_LEVEL
	{
		public List<string> PrefixList = new List<string>();							// 前缀列表(比如取地址符&,或者*)
		public string Name = string.Empty;
		public VAR_MEMBER_OPERATOR MemberOperator = VAR_MEMBER_OPERATOR.NONE;
		public string Text = string.Empty;

		public VAR_LEVEL(string name)
		{
			this.Name = name;
		}

		public void SetTextStr()
		{
			this.Text = string.Empty;
			foreach (var pf in this.PrefixList)
			{
				this.Text += pf;
			}
			this.Text += this.Name;
			if (this.MemberOperator == VAR_MEMBER_OPERATOR.ARROW)
			{
				this.Text += "->";
			}
			else if (this.MemberOperator == VAR_MEMBER_OPERATOR.DOT)
			{
				this.Text += ".";
			}
		}
	}

	public enum VAR_MEMBER_OPERATOR
	{
		NONE,
		DOT,				// .
		ARROW,				// ->
	}
}
