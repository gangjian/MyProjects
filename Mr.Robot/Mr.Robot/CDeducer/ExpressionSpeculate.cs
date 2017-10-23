using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot.CDeducer
{
	class EXPRESSION_SPECULATE
	{
		public static int ExpressionSpeculate(string expr_str, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups2(expr_str, parse_info, deducer_ctx);
			if (1 == meaningGroupList.Count)
			{
				return SingleGroupExpressionSpeculate(meaningGroupList.First(), parse_info, deducer_ctx);
			}
			else
			{
				return CompoundGroupExpressionSpeculate(meaningGroupList, parse_info, deducer_ctx);
			}
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
		static void ExpressionSimplify_Phase1(string exp_str, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups2(exp_str, parse_info, deducer_ctx);
			if (1 == meaningGroupList.Count)
			{
				if (meaningGroupList.First().Type == MeaningGroupType.Expression
					&& meaningGroupList.First().Text.Trim().StartsWith("(")
					&& meaningGroupList.First().Text.Trim().EndsWith(")"))
				{																		// 表达式
					string newExp = meaningGroupList.First().Text.Trim();
					newExp = newExp.Remove(newExp.Length - 1);
					newExp = newExp.Remove(0, 1);
					ExpressionSimplify_Phase1(newExp, parse_info, deducer_ctx);
				}
				else if (meaningGroupList.First().Type == MeaningGroupType.Constant)
				{																		// 常量
					
				}
				else
				{
					System.Diagnostics.Trace.Assert(false);
				}
			}
			else
			{
				// 判断关系运算符的位置
				int oprtIdx = -1;
				for (int i = 0; i < meaningGroupList.Count; i++)
				{
					if (meaningGroupList[i].Type == MeaningGroupType.OtherOperator)
					{
						oprtIdx = i;
					}
				}
				if (-1 == oprtIdx)
				{
					System.Diagnostics.Trace.Assert(false);
				}
				int varGroupIdx = -1;
				// 分别判断关系运算符左右两侧各个group, 包含变量的表达式放到运算符左侧, 不包含变量的表达式(常量表达式)放到右侧
				for (int i = 0; i < oprtIdx; i++)
				{
					List<string> varList = FindVarsInGroup(meaningGroupList[i], parse_info, deducer_ctx);
					if (0 != varList.Count)
					{
						if (-1 == varGroupIdx)
						{
							varGroupIdx = i;
						}
						else
						{																// 多个group包含变量的场合
							System.Diagnostics.Trace.Assert(false);
						}
					}
				}
				for (int i = oprtIdx + 1; i < meaningGroupList.Count; i++)
				{
					
				}
			}
		}

		/// <summary>
		/// 找出一个MeaningGroup内的变量
		/// </summary>
		static List<string> FindVarsInGroup(MEANING_GROUP meaning_group, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			List<string> varList = new List<string>();
			if (meaning_group.Type == MeaningGroupType.Expression
				&& meaning_group.Text.Trim().StartsWith("(")
				&& meaning_group.Text.Trim().EndsWith(")"))
			{																			// 表达式
				string newExp = meaning_group.Text.Trim();
				newExp = newExp.Remove(newExp.Length - 1);
				newExp = newExp.Remove(0, 1);
				List<MEANING_GROUP> groupList = COMN_PROC.GetMeaningGroups2(newExp, parse_info, deducer_ctx);
				foreach (var group in groupList)
				{
					List<string> tmpList = FindVarsInGroup(group, parse_info, deducer_ctx);
					varList.AddRange(tmpList);
				}
			}
			else if (meaning_group.Type == MeaningGroupType.Identifier)
			{																			// 标识符
				MACRO_DEFINE_INFO mdi = null;
				if (null != deducer_ctx.SearchByName(meaning_group.Text))
				{																		// 在上下文中查找
					varList.Add(meaning_group.Text);
				}
				else if (null != parse_info.FindGlobalVarInfoByName(meaning_group.Text))
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
						varList.AddRange(tmpList);
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
	}
}
