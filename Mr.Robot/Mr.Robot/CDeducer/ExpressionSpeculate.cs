﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot.CDeducer
{
	class LOGIC_EXPRESSION_SIMPLIFY
	{
		public static int ExpressionSpeculate(string expr_str, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			ExpressionSimplify_Phase1(expr_str, parse_info, deducer_ctx);
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
		static string ExpressionSimplify_Phase1(string exp_str, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups2(exp_str, parse_info, deducer_ctx);
			if (1 == meaningGroupList.Count
				&& MeaningGroupType.Constant == meaningGroupList.First().Type)
			{																			// 常量?
			}
			else if (3 == meaningGroupList.Count
					 && MeaningGroupType.OtherOperator == meaningGroupList[1].Type)
			{
				List<string> leftVarList = FindVarsInGroup(meaningGroupList[0], parse_info, deducer_ctx);
				List<string> rightVarList = FindVarsInGroup(meaningGroupList[2], parse_info, deducer_ctx);
				// 现在暂时只考虑一元(一次)不等式, 即只有一个未知数的情况
				System.Diagnostics.Trace.Assert(1 == CheckBothSideVarCount(leftVarList, rightVarList));
			}
			else
			{
				System.Diagnostics.Trace.Assert(false);
			}
			return string.Empty;
		}

		static bool ExpressionSimplify_Phase2(string original_exp, out string remain_exp, out string move_exp, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			remain_exp = string.Empty;
			move_exp = string.Empty;
			// 将原来的表达式拆解为多个group, 包含变量的留在符号左边, 不包含变量的移到符号右边
			List<MEANING_GROUP> meaningGroups = COMN_PROC.GetMeaningGroups2(original_exp, parse_info, deducer_ctx);
			if (1 == meaningGroups.Count)
			{
				if (0 == FindVarsInGroup(meaningGroups.First(), parse_info, deducer_ctx).Count)
				{
					// 移到右边(减)
					move_exp += MoveOtherSideSub(meaningGroups.First().Text);
					return true;
				}
				else
				{
					// 继续留在左边
					remain_exp += meaningGroups.First().Text;
					return false;
				}
			}
			else if (3 == meaningGroups.Count)
			{
				if (meaningGroups[1].Type == MeaningGroupType.OtherOperator)
				{
					
				}
				else
				{
					System.Diagnostics.Trace.Assert(false);
				}
			}
			else
			{
				System.Diagnostics.Trace.Assert(false);
			}
			return false;
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

		/// <summary>
		/// 检查符号左右两侧变量的总共个数
		/// </summary>
		static int CheckBothSideVarCount(List<string> left_var_list, List<string> right_var_list)
		{
			List<string> totalList = new List<string>();
			foreach (var item in left_var_list)
			{
				if (!totalList.Contains(item))
				{
					totalList.Add(item);
				}
			}
			foreach (var item in right_var_list)
			{
				if (!totalList.Contains(item))
				{
					totalList.Add(item);
				}
			}
			return totalList.Count;
		}

		static string MoveOtherSideSub(string exp_str)
		{
			return "-" + "(" + exp_str + ")";
		}
	}
}
