using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot.CDeducer
{
	class LOGIC_EXPRESSION_SIMPLIFY
	{
		public static int ExpressionSpeculate(string expr_str, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			string exp_str = LogicExpressionSimplify(expr_str, parse_info, deducer_ctx);
			return 0;
		}

		static int SingleGroupExpressionSpeculate(MEANING_GROUP meaning_group, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			int retVal = 0;
			// 立即数
			if (meaning_group.Type == MeaningGroupType.Constant
				&& COMN_PROC.GetConstantNumberValue(meaning_group.Text, out retVal))
			{
				return retVal;
			}
			// 宏定义
			else if (meaning_group.Type == MeaningGroupType.Identifier)
			{
				MACRO_DEFINE_INFO mdi = parse_info.FindMacroDefInfo(meaning_group.Text);
				if (null != mdi)
				{
					return ExpressionSpeculate(mdi.ValStr, parse_info, deducer_ctx);
				}
				else
				{
					return 0;
				}
			}
			// 表达式
			else if (meaning_group.Type == MeaningGroupType.Expression
					 && meaning_group.Text.Trim().StartsWith("(")
					 && meaning_group.Text.Trim().EndsWith(")"))
			{
				string newExp = meaning_group.Text.Trim();
				newExp = newExp.Remove(newExp.Length - 1);
				newExp = newExp.Remove(0, 1);
				return ExpressionSpeculate(newExp, parse_info, deducer_ctx);
			}
			else if (meaning_group.Type == MeaningGroupType.Identifier)
			{
				
			}
			else
			{
				System.Diagnostics.Trace.Assert(false);
			}
			return 0;
		}

		static int CompoundGroupExpressionSpeculate(List<MEANING_GROUP> meaningGroupList, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			if (3 == meaningGroupList.Count
				&& meaningGroupList[1].Type == MeaningGroupType.OtherOperator)
			{
				OPERATOR opr = new OPERATOR(meaningGroupList[1].Text, 1);
				OPERAND opd_1 = new OPERAND(meaningGroupList.First().Text, 0);
				OPERAND opd_2 = new OPERAND(meaningGroupList.Last().Text, 0);
				OPERATION_GROUP opGroup = new OPERATION_GROUP(opr, opd_1, opd_2);
			}
			else
			{
				// 暂时先只考虑有一个操作符的简单情况
				System.Diagnostics.Trace.Assert(false);
			}
			return 0;
		}

		/// <summary>
		/// 不等式化简(阶段1)
		/// </summary>
		static string LogicExpressionSimplify(string exp_str, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			List<MEANING_GROUP> meaningGroups = COMN_PROC.GetMeaningGroups2(exp_str, parse_info, deducer_ctx);
			if (IsSimpleRelationExprGroups(meaningGroups))
			{
				string oprtStr = meaningGroups[1].Text;
				// 简单逻辑关系表达式(一个逻辑关系运算符, 左右两个表达式)
				List<string> leftVarList = FindVarsInGroup(meaningGroups[0], parse_info, deducer_ctx);
				List<string> rightVarList = FindVarsInGroup(meaningGroups[2], parse_info, deducer_ctx);
				string leftExpStr = string.Empty;
				string rightExpStr = string.Empty;
				if (1 == leftVarList.Count && 0 == rightVarList.Count)
				{
					// 变量在符号左边
					leftExpStr = meaningGroups[0].Text;
					rightExpStr = meaningGroups[2].Text;
				}
				else if (0 == leftVarList.Count && 1 == rightVarList.Count)
				{
					// 变量在符号右边->左右对调,符号取反
					leftExpStr = meaningGroups[2].Text;
					rightExpStr = meaningGroups[0].Text;
					oprtStr = GetReverseOperatorStr(oprtStr);
				}
				else
				{
					// 暂不考虑有多个变量, 或者一个变量多次出现(需要合并同类项)的情况
					System.Diagnostics.Trace.Assert(false);
				}
				while (ExpressionSimplify_Phase2(ref leftExpStr, ref rightExpStr, parse_info, deducer_ctx))
				{
				}
				return leftExpStr + oprtStr + rightExpStr;
			}
			else
			{
				// 非标准表达式
				System.Diagnostics.Trace.Assert(false);
				return null;
			}
		}

		static bool ExpressionSimplify_Phase2(ref string left_exp, ref string right_exp, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			// 将原来的表达式拆解为多个group, 包含变量的留在符号左边, 不包含变量的移到符号右边
			List<MEANING_GROUP> meaningGroups = COMN_PROC.GetMeaningGroups2(left_exp, parse_info, deducer_ctx);
			if (meaningGroups.Count > 1)
			{
				for (int i = 0; i < meaningGroups.Count; i++)
				{
					if (meaningGroups[i].Type == MeaningGroupType.OtherOperator)
					{
						// 运算符
					}
					else
					{
						if (0 == FindVarsInGroup(meaningGroups[i], parse_info, deducer_ctx).Count)
						{
							// 不包含变量, 要移到符号右侧
							left_exp = MoveToRightSide(meaningGroups, i, ref right_exp, parse_info);
							return true;
						}
						else
						{
							// 包含变量, 继续留在符号左侧
						}
					}
				}
				return false;
			}
			else if (1 == meaningGroups.Count)
			{
				if (meaningGroups[0].Type == MeaningGroupType.Constant)
				{
					// 常量
					return false;
				}
				else if (meaningGroups[0].Type == MeaningGroupType.FuncPara
						 || meaningGroups[0].Type == MeaningGroupType.GlobalVariable
						 || meaningGroups[0].Type == MeaningGroupType.LocalVariable)
				{
					// 变量
					return false;
				}
				else
				{
					// 不知道是啥
					System.Diagnostics.Trace.Assert(false);
					return false;
				}
			}
			else
			{
				// 什么情况?
				System.Diagnostics.Trace.Assert(false);
				return false;
			}
		}

		/// <summary>
		/// 找出一个MeaningGroup内的变量
		/// </summary>
		static List<string> FindVarsInGroup(MEANING_GROUP meaning_group, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			List<string> varList = new List<string>();
			if (meaning_group.Type == MeaningGroupType.Expression)
			{																			// 表达式
				string newExp = meaning_group.Text.Trim();
				List<MEANING_GROUP> groupList = COMN_PROC.GetMeaningGroups2(newExp, parse_info, deducer_ctx);
				foreach (var group in groupList)
				{
					List<string> tmpList = FindVarsInGroup(group, parse_info, deducer_ctx);
					foreach (var item in tmpList)
					{
						if (!varList.Contains(item))
						{
							varList.Add(item);
						}
					}
				}
			}
			else if (meaning_group.Type == MeaningGroupType.LocalVariable
					 || meaning_group.Type == MeaningGroupType.GlobalVariable)
			{
				if (!varList.Contains(meaning_group.Text))
				{
					varList.Add(meaning_group.Text);
				}
			}
			else if (meaning_group.Type == MeaningGroupType.Identifier)
			{																			// 标识符
				MACRO_DEFINE_INFO mdi = null;
				if (null != deducer_ctx.SearchByName(meaning_group.Text)
					&& !varList.Contains(meaning_group.Text))
				{																		// 在上下文中查找
					varList.Add(meaning_group.Text);
				}
				else if (null != parse_info.FindGlobalVarInfoByName(meaning_group.Text)
						 && !varList.Contains(meaning_group.Text))
				{																		// 全局量?
					varList.Add(meaning_group.Text);
				}
				else if (null != (mdi = parse_info.FindMacroDefInfo(meaning_group.Text)))
				{																		// 宏定义?
					string newExp = mdi.ValStr;
					List<MEANING_GROUP> groupList = COMN_PROC.GetMeaningGroups2(newExp, parse_info, deducer_ctx);
					foreach (var group in groupList)
					{
						List<string> tmpList = FindVarsInGroup(group, parse_info, deducer_ctx);
						foreach (var item in tmpList)
						{
							if (!varList.Contains(item))
							{
								varList.Add(item);
							}
						}
					}
				}
			}
			else if (meaning_group.Type == MeaningGroupType.Constant
					 || meaning_group.Type == MeaningGroupType.OtherOperator)
			{
			}
			else
			{
				System.Diagnostics.Trace.Assert(false);
			}
			return varList;
		}

		static bool IsSimpleRelationExprGroups(List<MEANING_GROUP> meaning_groups)
		{
			if (3 == meaning_groups.Count
				&& meaning_groups[1].Type == MeaningGroupType.OtherOperator
				&& IsRelationshipOperator(meaning_groups[1].Text))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		static bool IsRelationshipOperator(string opt)
		{
			if (opt.Equals(">")
				|| opt.Equals("<")
				|| opt.Equals(">=")
				|| opt.Equals("<=")
				|| opt.Equals("==")
				|| opt.Equals("!="))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		static bool IsCommonArithmeticOperator(string opt)
		{
			if (opt.Equals("+")
				|| opt.Equals("-")
				|| opt.Equals("*")
				|| opt.Equals("/"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		static string GetReverseOperatorStr(string oprt_str)
		{
			switch (oprt_str)
			{
				case "==":
				case "!=":
					return oprt_str;
				case ">":
					return "<";
				case ">=":
					return "<=";
				case "<":
					return ">";
				case "<=":
					return ">=";
				default:
					return null;
			}
		}

		static string GetMoveRightOperator(string oprt_str)
		{
			switch (oprt_str)
			{
				case "+":
					return "-";
				case "-":
					return "+";
				case "*":
					return "/";
				case "/":
					return "*";
				default:
					return null;
			}
		}

		static string GetOperatorInMeaningGroups(List<MEANING_GROUP> meaning_groups, int operand_idx, out int operator_idx)
		{
			System.Diagnostics.Trace.Assert(operand_idx < meaning_groups.Count);

			operator_idx = -1;
			string leftOptStr = string.Empty;
			string rightOptStr = string.Empty;
			if (0 != operand_idx)
			{
				leftOptStr = meaning_groups[operand_idx - 1].Text;
			}
			if (meaning_groups.Count - 1 != operand_idx)
			{
				rightOptStr = meaning_groups[operand_idx + 1].Text;
			}
			if (string.Empty == leftOptStr
				&& string.Empty != rightOptStr)
			{
				operator_idx = operand_idx + 1;
				return rightOptStr;
			}
			else if (string.Empty != leftOptStr
					 && string.Empty == rightOptStr)
			{
				operator_idx = operand_idx - 1;
				return leftOptStr;
			}
			else if (string.Empty != leftOptStr
					 && string.Empty != rightOptStr)
			{
				int cmpVal = OperatorCompare(leftOptStr, rightOptStr);
				if (cmpVal >= 0)
				{
					operator_idx = operand_idx - 1;
					return leftOptStr;
				}
				else
				{
					operator_idx = operand_idx + 1;
					return rightOptStr;
				}
			}
			else
			{
				System.Diagnostics.Trace.Assert(false);
				return null;
			}
		}

		static int OperatorCompare(string oprt_1, string oprt_2)
		{
			System.Diagnostics.Trace.Assert(IsCommonArithmeticOperator(oprt_1));
			System.Diagnostics.Trace.Assert(IsCommonArithmeticOperator(oprt_2));
			int op1Weight = GetArithmeticOperatorWeight(oprt_1);
			int op2Weight = GetArithmeticOperatorWeight(oprt_2);
			if (op1Weight > op2Weight)
			{
				return 1;
			}
			else if (op1Weight < op2Weight)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}

		static int GetArithmeticOperatorWeight(string oprt)
		{
			System.Diagnostics.Trace.Assert(IsCommonArithmeticOperator(oprt));
			if (oprt.Equals("+") || oprt.Equals("-"))
			{
				return 0;
			}
			else if (oprt.Equals("*") || oprt.Equals("/"))
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}

		static string MoveToRightSide(List<MEANING_GROUP> meaning_groups, int operand_idx, ref string right_exp_str, FILE_PARSE_INFO parse_info)
		{
			// 取得运算符
			int oprtIdx;
			string oprtStr = GetOperatorInMeaningGroups(meaning_groups, operand_idx, out oprtIdx);
			string removeExpStr = GetMoveRightOperator(oprtStr) + meaning_groups[operand_idx].Text;
			right_exp_str = "(" + right_exp_str + ")" + removeExpStr;
			if (oprtIdx < operand_idx)
			{
				meaning_groups.RemoveAt(operand_idx);
				meaning_groups.RemoveAt(oprtIdx);
			}
			else
			{
				meaning_groups.RemoveAt(oprtIdx);
				meaning_groups.RemoveAt(operand_idx);
			}
			// 计算右侧常量表达式的值
			right_exp_str = ExpCalc.GetConstExpressionValue(right_exp_str, parse_info).ToString();
			StringBuilder sb = new StringBuilder();
			foreach (var item in meaning_groups)
			{
				sb.Append(item.Text);
			}
			return sb.ToString();
		}
	}
}
