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

		static int GetOperationLimitCondition()
		{
			return 0;
		}
	}
}
