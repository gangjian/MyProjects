﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mr.Robot.CDeducer;

namespace Mr.Robot
{
	public class ExpCalc
	{
		public static int GetConstExpressionValue(string exp_str, FILE_PARSE_INFO parse_info)
		{
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(exp_str, parse_info, false);	// 最后一个参数, 在解析逻辑表达式时, 因为涉及 #if define(XXX) 这样的表达式, 所以如果是空的宏定义, 原样不动不展开
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, parse_info, null);
			int retVal = 0;
			if (1 == meaningGroupList.Count)
			{
				retVal = GetSingleGroupLogicExpVal(meaningGroupList.First(), parse_info);
			}
			else
			{
				retVal = CaculateCompondLogicExpVal(meaningGroupList, parse_info);
			}
			return retVal;
		}

		static int GetSingleGroupLogicExpVal(MEANING_GROUP meaning_group, FILE_PARSE_INFO parse_info)
		{
			int retVal = 0;
			// 立即数
			if (meaning_group.Type == MeaningGroupType.Constant
				&& COMN_PROC.GetConstantNumberValue(meaning_group.TextStr, out retVal))
			{
				return retVal;
			}
			// 宏定义
			else if (meaning_group.Type == MeaningGroupType.Identifier)
			{
				MacroDefineInfo mdi = parse_info.FindMacroDefInfo(meaning_group.TextStr);
				if (null != mdi)
				{
					return GetConstExpressionValue(mdi.ValStr, parse_info);
				}
				else
				{
					return 0;
				}
			}
			// 表达式
			else if (meaning_group.Type == MeaningGroupType.Expression
					 && meaning_group.TextStr.Trim().StartsWith("(")
					 && meaning_group.TextStr.Trim().EndsWith(")"))
			{
				string newExp = meaning_group.TextStr.Trim();
				newExp = newExp.Remove(newExp.Length - 1);
				newExp = newExp.Remove(0, 1);
				return GetConstExpressionValue(newExp, parse_info);
			}
			else if (meaning_group.Type == MeaningGroupType.Expression
					 && meaning_group.ComponentList[0].Text == "defined")
			{
				string newExp = meaning_group.TextStr.Remove(0, "defined".Length).Trim();
				if (COMN_PROC.JudgeExpressionDefined(newExp, parse_info))
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
			else
			{
				System.Diagnostics.Trace.Assert(false);
			}
			return 0;
		}

		static int CaculateCompondLogicExpVal(List<MEANING_GROUP> meaningGroupList, FILE_PARSE_INFO parse_info)
		{
			OPERATION_GROUP opt = GetNextOperationGroup(meaningGroupList);
			if (null == opt)
			{
				throw new MyException("GetNextOperationGroup return null!");
			}
			int retVal = GetOperationValue(opt, parse_info);
			string firstExpStr = string.Empty;
			for (int i = 0; i < opt.StartIdx; i++)
			{
				firstExpStr += meaningGroupList[i].TextStr;
			}
			string secondExpStr = string.Empty;
			for (int i = opt.EndIdx + 1; i < meaningGroupList.Count; i++)
			{
				secondExpStr += meaningGroupList[i].TextStr;
			}
			if (!string.IsNullOrEmpty(firstExpStr)
				|| !string.IsNullOrEmpty(secondExpStr))
			{
				string nextExpStr = firstExpStr + retVal.ToString() + secondExpStr;
				return GetConstExpressionValue(nextExpStr, parse_info);
			}
			else
			{
				return retVal;
			}
		}

		/// <summary>
		/// 取得表达式下一个运算组
		/// </summary>
		static OPERATION_GROUP GetNextOperationGroup(List<MEANING_GROUP> meaningGroupList)
		{
			MEANING_GROUP operatorGroup = null;
			OPERATION_GROUP retGroup = null;
			int operatorIdx = -1;
			for (int i = 0; i < meaningGroupList.Count; i++)
			{
				if ("defined" == meaningGroupList[i].TextStr								// 把"defined"关键字当作'特殊的'运算符处理
					|| meaningGroupList[i].Type == MeaningGroupType.TypeCasting)		// 强制类型转换
				{
					retGroup = new OPERATION_GROUP();
					if (meaningGroupList[i].Type == MeaningGroupType.TypeCasting)
					{
						retGroup._Operator.Type = OPERATOR_TYPE.TYPE_CASTING;
					}
					retGroup._Operator.Text = meaningGroupList[i].TextStr;
					retGroup._Operator.GroupIdx = i;
					OPERAND oprnd = new OPERAND(meaningGroupList[i + 1].TextStr, i + 1);
					retGroup._OperandList.Add(oprnd);
					retGroup.GetStartEndIdx();
					return retGroup;
				}
				else if (meaningGroupList[i].Type == MeaningGroupType.OtherOperator
					|| meaningGroupList[i].Type == MeaningGroupType.AssignmentMark)
				{
					if (null == operatorGroup
						|| meaningGroupList[i].ComponentList[0].Priority < operatorGroup.ComponentList[0].Priority)
					{
						operatorGroup = meaningGroupList[i];
						operatorIdx = i;
					}
				}
			}
			if (null == operatorGroup)
			{
				return null;
			}

			retGroup = new OPERATION_GROUP();
			retGroup._Operator.Text = operatorGroup.TextStr;
			retGroup._Operator.GroupIdx = operatorIdx;
			retGroup._OperandList = GetOperandList(meaningGroupList, operatorIdx, operatorGroup.ComponentList[0].OperandCount);
			retGroup.GetStartEndIdx();
			return retGroup;
		}

		static List<OPERAND> GetOperandList(List<MEANING_GROUP> meaningGroupList, int operator_idx, int operand_count)
		{
			List<OPERAND> retList = new List<OPERAND>();
			if (1 == operand_count)
			{
				if (operator_idx > 0
					&& meaningGroupList[operator_idx - 1].Type != MeaningGroupType.OtherOperator
					&& meaningGroupList[operator_idx - 1].Type != MeaningGroupType.AssignmentMark)
				{
					OPERAND operand = new OPERAND(meaningGroupList[operator_idx - 1].TextStr, operator_idx - 1);
					retList.Add(operand);
				}
				else if (operator_idx < meaningGroupList.Count - 1
						 && meaningGroupList[operator_idx + 1].Type != MeaningGroupType.OtherOperator
						 && meaningGroupList[operator_idx + 1].Type != MeaningGroupType.AssignmentMark)
				{
					OPERAND operand = new OPERAND(meaningGroupList[operator_idx + 1].TextStr, operator_idx + 1);
					retList.Add(operand);
				}
				else
				{
					System.Diagnostics.Trace.Assert(false);
				}
			}
			else if (2 == operand_count)
			{
				if (operator_idx > 0
					&& operator_idx < meaningGroupList.Count - 1
					&& meaningGroupList[operator_idx - 1].Type != MeaningGroupType.OtherOperator
					&& meaningGroupList[operator_idx - 1].Type != MeaningGroupType.AssignmentMark
					&& meaningGroupList[operator_idx + 1].Type != MeaningGroupType.OtherOperator
					&& meaningGroupList[operator_idx + 1].Type != MeaningGroupType.AssignmentMark)
				{
					OPERAND operand = new OPERAND(meaningGroupList[operator_idx - 1].TextStr, operator_idx - 1);
					retList.Add(operand);
					operand = new OPERAND(meaningGroupList[operator_idx + 1].TextStr, operator_idx + 1);
					retList.Add(operand);
				}
			}
			else if (3 == operand_count)
			{
				throw new System.NotImplementedException();
			}
			return retList;
		}

		/// <summary>
		/// 取得运算值
		/// </summary>
		static int GetOperationValue(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			int retVal = 0;
			if (OPERATOR_TYPE.TYPE_CASTING == oper_group._Operator.Type)
			{
				retVal = CalcTypeCasting(oper_group, parse_info);
			}
			else
			{
				switch (oper_group._Operator.Text)
				{
					case "==":															// 逻辑等
						retVal = CalcLogical_Equal(oper_group, parse_info);
						break;
					case "!=":															// 逻辑不等
						retVal = CalcLogical_Unequal(oper_group, parse_info);
						break;
					case "||":															// 逻辑或
						retVal = CalcLogical_Or(oper_group, parse_info);
						break;
					case "&&":															// 逻辑与
						retVal = CalcLogical_And(oper_group, parse_info);
						break;
					case "defined":														// 宏是否定义
						retVal = CalcLogical_Defined(oper_group, parse_info);
						break;
					case "%":															// 取余
						retVal = CalcLogical_Remainder(oper_group, parse_info);
						break;
					case "!":															// 逻辑非
						retVal = CalcLogical_Not(oper_group, parse_info);
						break;
					case ">=":															// 大于等于
						retVal = CalcLogical_GreaterOrEqual(oper_group, parse_info);
						break;
					case "<=":															// 小于等于
						retVal = CalcLogical_LessOrEqual(oper_group, parse_info);
						break;
					case ">":															// 大于
						retVal = CalcLogical_Greater(oper_group, parse_info);
						break;
					case "<":															// 小于
						retVal = CalcLogical_Less(oper_group, parse_info);
						break;
					case "+":
						retVal = CalcArithmetic_Add(oper_group, parse_info);			// 加
						break;
					case "-":
						retVal = CalcArithmetic_Sub(oper_group, parse_info);			// 减
						break;
					case "*":
						retVal = CalcArithmetic_Multiply(oper_group, parse_info);		// 乘
						break;
					case "/":
						retVal = CalcArithmetic_Divide(oper_group, parse_info);			// 除
						break;
					case "&":
						retVal = Calculate_BitAnd(oper_group, parse_info);				// 位与
						break;
					case "|":
						retVal = Calculate_BitOr(oper_group, parse_info);				// 位或
						break;
					default:
						// TODO: LowDA_三回目\ALL\gerdaC_dd\zlib\zconf.h 443行
						//		#if defined(_LARGEFILE64_SOURCE) && -_LARGEFILE64_SOURCE - -1 == 1
						//		"- -1"被当成了"--"
						//		重构自动测试后要改正
						throw new System.NotImplementedException();
				}
			}
			return retVal;
		}

		/// <summary>
		/// 逻辑等运算("==")
		/// </summary>
		static int CalcLogical_Equal(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value == operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_Unequal(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value == operand2Value)
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}

		static int CalcLogical_Or(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (0 != operand1Value
				|| 0 != operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_And(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (0 != operand1Value
				&& 0 != operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_Defined(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 1);
			string operand = oper_group._OperandList.First().Text;
			if (COMN_PROC.JudgeExpressionDefined(operand, parse_info))
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_Remainder(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			return (operand1Value % operand2Value);
		}

		static int CalcLogical_Not(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 1);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			if (0 == operand1Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_GreaterOrEqual(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value >= operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_LessOrEqual(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value <= operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_Greater(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value > operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_Less(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value < operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcArithmetic_Add(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			return operand1Value + operand2Value;
		}

		static int CalcArithmetic_Sub(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			if (2 == oper_group._OperandList.Count)
			{
				// 减号
				System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
				System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
				int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
				int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
				return operand1Value - operand2Value;
			}
			else if (1 == oper_group._OperandList.Count)
			{
				// 负号
				System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
				int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
				return -operand1Value;
			}
			else
			{
				System.Diagnostics.Trace.Assert(false);
				return 0;
			}
		}

		static int CalcArithmetic_Multiply(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			return operand1Value * operand2Value;
		}

		static int CalcArithmetic_Divide(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			return operand1Value / operand2Value;
		}

		static int Calculate_BitAnd(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			return operand1Value & operand2Value;
		}

		static int Calculate_BitOr(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetConstExpressionValue(oper_group._OperandList[1].Text, parse_info);
			return operand1Value | operand2Value;
		}

		static int CalcTypeCasting(OPERATION_GROUP oper_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 1);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));

			int operandValue = GetConstExpressionValue(oper_group._OperandList[0].Text, parse_info);
			string typeName = GetCastingTypeName(oper_group._Operator, parse_info);
			if (BasicTypeProc.IsBasicTypeName(typeName))
			{
				int retVal = Convert.ToInt32(BasicTypeProc.CalcTypeCastingValue(typeName, operandValue));
				return retVal;
			}
			else
			{
				throw new System.NotImplementedException("非基本类型的强制转换 未实现!");
			}
		}

		static string GetCastingTypeName(OPERATOR oper, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(oper.Type == OPERATOR_TYPE.TYPE_CASTING);
			string typeStr = oper.Text.Trim();
			if (typeStr.StartsWith("(") && typeStr.EndsWith(")"))
			{
				typeStr = typeStr.Remove(typeStr.Length - 1).Trim();
				typeStr = typeStr.Remove(0, 1).Trim();
				typeStr = parse_info.GetOriginalTypeName(typeStr);
				return typeStr;
			}
			return null;
		}
	}

	enum OPERATOR_TYPE
	{
		OTHER,
		TYPE_CASTING,
	}

	// 运算符
	class OPERATOR
	{
		public string Text = string.Empty;
		public int GroupIdx = -1;
		public OPERATOR_TYPE Type = OPERATOR_TYPE.OTHER;

		public OPERATOR()
		{
		}
		public OPERATOR(string opt_str, int group_idx)
		{
			this.Text = opt_str;
			this.GroupIdx = group_idx;
		}
	}

	// 运算数
	class OPERAND
	{
		public string Text = string.Empty;
		public int GroupIdx = -1;

		public OPERAND(string opd_str, int group_idx)
		{
			this.Text = opd_str;
			this.GroupIdx = group_idx;
		}
	}

	// 运算组
	class OPERATION_GROUP
	{
		public OPERATOR _Operator = new OPERATOR();
		public List<OPERAND> _OperandList = new List<OPERAND>();
		public int StartIdx = -1;
		public int EndIdx = -1;

		public void GetStartEndIdx()
		{
			this.StartIdx = this._Operator.GroupIdx;
			this.EndIdx = this._Operator.GroupIdx;
			foreach (OPERAND oprd in this._OperandList)
			{
				if (oprd.GroupIdx < this.StartIdx)
				{
					this.StartIdx = oprd.GroupIdx;
				}
				if (oprd.GroupIdx > this.EndIdx)
				{
					this.EndIdx = oprd.GroupIdx;
				}
			}
		}
		public OPERATION_GROUP()
		{
		}
		public OPERATION_GROUP(OPERATOR oprt, OPERAND operand_1, OPERAND operand_2)
		{
			this._Operator = oprt;
			this._OperandList.Add(operand_1);
			this._OperandList.Add(operand_2);
			this.GetStartEndIdx();
		}
	}
}
