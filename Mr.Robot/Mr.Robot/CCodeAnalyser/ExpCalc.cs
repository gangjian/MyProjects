using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot
{
	public class ExpCalc
	{
		public static int GetLogicalExpressionValue(string exp_str, FileParseInfo parse_info)
		{
			List<StatementComponent> componentList = StatementAnalysis.GetComponents(exp_str, parse_info, false);	// 最后一个参数, 在解析逻辑表达式时, 因为涉及 #if define(XXX) 这样的表达式, 所以如果是空的宏定义, 原样不动不展开
			List<MeaningGroup> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, parse_info, null);
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

		static int GetSingleGroupLogicExpVal(MeaningGroup meaning_group, FileParseInfo parse_info)
		{
			int retVal = 0;
			// 立即数
			if (meaning_group.Type == MeaningGroupType.Constant
				&& GetConstantNumberValue(meaning_group.Text, out retVal))
			{
				return retVal;
			}
			// 宏定义
			else if (meaning_group.Type == MeaningGroupType.Identifier)
			{
				MacroDefineInfo mdi = parse_info.FindMacroDefInfo(meaning_group.Text);
				if (null != mdi)
				{
					return GetLogicalExpressionValue(mdi.Value, parse_info);
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
				return GetLogicalExpressionValue(newExp, parse_info);
			}
			else
			{
				System.Diagnostics.Trace.Assert(false);
			}
			return 0;
		}

		static int CaculateCompondLogicExpVal(List<MeaningGroup> meaningGroupList, FileParseInfo parse_info)
		{
			OPERATION_GROUP opt = GetNextOperationGroup(meaningGroupList);
			if (null == opt)
			{
				System.Diagnostics.Trace.Assert(false);
			}
			int retVal = GetOperationValue(opt, parse_info);
			string firstExpStr = string.Empty;
			for (int i = 0; i < opt.StartIdx; i++)
			{
				firstExpStr += meaningGroupList[i].Text;
			}
			string secondExpStr = string.Empty;
			for (int i = opt.EndIdx + 1; i < meaningGroupList.Count; i++)
			{
				secondExpStr += meaningGroupList[i].Text;
			}
			string nextExpStr = firstExpStr + retVal.ToString() + secondExpStr;
			return GetLogicalExpressionValue(nextExpStr, parse_info);
		}

		static bool GetConstantNumberValue(string const_num_text, out int val)
		{
			string tmpStr = const_num_text.ToUpper();
			int postfixCount = 0;
			for (int i = tmpStr.Length - 1; i >= 0; i--)
			{
				char ch = tmpStr[i];
				if (ch == 'U' || ch == 'L')
				{
					postfixCount += 1;
				}
				else
				{
					break;
				}
			}
			if (0 != postfixCount)
			{
				tmpStr = tmpStr.Remove(tmpStr.Length - postfixCount);
			}
			if (tmpStr.StartsWith("0X"))
			{
				tmpStr = tmpStr.Remove(0, 2);
				if (int.TryParse(tmpStr, System.Globalization.NumberStyles.HexNumber, null, out val))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (int.TryParse(tmpStr, out val))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		// 运算符
		class OPERATOR
		{
			public string Text = string.Empty;
			public int GroupIdx = -1;
		}

		// 运算数
		class OPERAND
		{
			public string Text = string.Empty;
			public int GroupIdx = -1;
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
		}

		/// <summary>
		/// 取得表达式下一个运算组
		/// </summary>
		static OPERATION_GROUP GetNextOperationGroup(List<MeaningGroup> meaningGroupList)
		{
			MeaningGroup operatorGroup = null;
			OPERATION_GROUP retGroup = null;
			int operatorIdx = -1;
			for (int i = 0; i < meaningGroupList.Count; i++)
			{
				if ("defined" == meaningGroupList[i].Text)								// 把"defined"关键字当作'特殊的'运算符处理
				{
					retGroup = new OPERATION_GROUP();
					retGroup._Operator.Text = meaningGroupList[i].Text;
					retGroup._Operator.GroupIdx = i;
					OPERAND oprnd = new OPERAND();
					oprnd.Text = meaningGroupList[i + 1].Text;
					oprnd.GroupIdx = i + 1;
					retGroup._OperandList.Add(oprnd);
					retGroup.GetStartEndIdx();
					return retGroup;
				}
				else if (meaningGroupList[i].Type == MeaningGroupType.OtherOperator
					|| meaningGroupList[i].Type == MeaningGroupType.EqualMark)
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
			retGroup._Operator.Text = operatorGroup.Text;
			retGroup._Operator.GroupIdx = operatorIdx;
			retGroup._OperandList = GetOperandList(meaningGroupList, operatorIdx, operatorGroup.ComponentList[0].OperandCount);
			retGroup.GetStartEndIdx();
			return retGroup;
		}

		static List<OPERAND> GetOperandList(List<MeaningGroup> meaningGroupList, int operator_idx, int operand_count)
		{
			List<OPERAND> retList = new List<OPERAND>();
			if (1 == operand_count)
			{
				if (operator_idx > 0
					&& meaningGroupList[operator_idx - 1].Type != MeaningGroupType.OtherOperator
					&& meaningGroupList[operator_idx - 1].Type != MeaningGroupType.EqualMark)
				{
					OPERAND operand = new OPERAND();
					operand.Text = meaningGroupList[operator_idx - 1].Text;
					operand.GroupIdx = operator_idx - 1;
					retList.Add(operand);
				}
				else if (operator_idx < meaningGroupList.Count - 1
						 && meaningGroupList[operator_idx + 1].Type != MeaningGroupType.OtherOperator
						 && meaningGroupList[operator_idx + 1].Type != MeaningGroupType.EqualMark)
				{
					OPERAND operand = new OPERAND();
					operand.Text = meaningGroupList[operator_idx + 1].Text;
					operand.GroupIdx = operator_idx + 1;
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
					&& meaningGroupList[operator_idx - 1].Type != MeaningGroupType.EqualMark
					&& meaningGroupList[operator_idx + 1].Type != MeaningGroupType.OtherOperator
					&& meaningGroupList[operator_idx + 1].Type != MeaningGroupType.EqualMark)
				{
					OPERAND operand = new OPERAND();
					operand.Text = meaningGroupList[operator_idx - 1].Text;
					operand.GroupIdx = operator_idx - 1;
					retList.Add(operand);
					operand = new OPERAND();
					operand.Text = meaningGroupList[operator_idx + 1].Text;
					operand.GroupIdx = operator_idx + 1;
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
		static int GetOperationValue(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			int retVal = 0;
			switch (oper_group._Operator.Text)
			{
				case "==":																// 逻辑等
					retVal = CalcLogical_Equal(oper_group, parse_info);
					break;
				case "!=":																// 逻辑不等
					retVal = CalcLogical_Unequal(oper_group, parse_info);
					break;
				case "||":																// 逻辑或
					retVal = CalcLogical_Or(oper_group, parse_info);
					break;
				case "&&":																// 逻辑与
					retVal = CalcLogical_And(oper_group, parse_info);
					break;
				case "defined":															// 宏是否定义
					retVal = CalcLogical_Defined(oper_group, parse_info);
					break;
				case "%":																// 取余
					retVal = CalcLogical_Remainder(oper_group, parse_info);
					break;
				case "!":																// 逻辑非
					retVal = CalcLogical_Not(oper_group, parse_info);
					break;
				case ">=":																// 大于等于
					retVal = CalcLogical_GreaterOrEqual(oper_group, parse_info);
					break;
				case "<=":																// 小于等于
					retVal = CalcLogical_LessOrEqual(oper_group, parse_info);
					break;
				case ">":																// 大于
					retVal = CalcLogical_Greater(oper_group, parse_info);
					break;
				case "<":																// 小于
					retVal = CalcLogical_Less(oper_group, parse_info);
					break;
				case "+":
					retVal = CalcArithmetic_Add(oper_group, parse_info);				// 加
					break;
				case "-":
					retVal = CalcArithmetic_Sub(oper_group, parse_info);				// 减
					break;
				case "*":
					retVal = CalcArithmetic_Multiply(oper_group, parse_info);			// 乘
					break;
				case "/":
					retVal = CalcArithmetic_Divide(oper_group, parse_info);				// 除
					break;
				default:
					throw new System.NotImplementedException();
			}
			return retVal;
		}

		/// <summary>
		/// 逻辑等运算("==")
		/// </summary>
		static int CalcLogical_Equal(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value == operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_Unequal(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value == operand2Value)
			{
				return 0;
			}
			else
			{
				return 1;
			}
		}

		static int CalcLogical_Or(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
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

		static int CalcLogical_And(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
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

		static int CalcLogical_Defined(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 1);
			string operand = oper_group._OperandList.First().Text;
			while (operand.StartsWith("(")
					&& operand.EndsWith(")"))
			{
				operand = operand.Remove(operand.Length - 1);
				operand = operand.Remove(0, 1).Trim();
			}
			if (null != parse_info.FindMacroDefInfo(operand))
			{
				return 1;
			}
			else if (StatementAnalysis.IsConstantNumber(operand))
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_Remainder(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
			return (operand1Value % operand2Value);
		}

		static int CalcLogical_Not(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 1);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			if (0 == operand1Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_GreaterOrEqual(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value >= operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_LessOrEqual(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value <= operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_Greater(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value > operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcLogical_Less(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value < operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		static int CalcArithmetic_Add(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
			return operand1Value + operand2Value;
		}

		static int CalcArithmetic_Sub(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
			return operand1Value - operand2Value;
		}

		static int CalcArithmetic_Multiply(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
			return operand1Value * operand2Value;
		}

		static int CalcArithmetic_Divide(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetLogicalExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetLogicalExpressionValue(oper_group._OperandList[1].Text, parse_info);
			return operand1Value / operand2Value;
		}
	}
}
