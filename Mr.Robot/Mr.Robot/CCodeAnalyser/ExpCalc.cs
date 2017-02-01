using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot
{
	public class ExpCalc
	{
		public static int GetExpressionValue(string exp_str, FileParseInfo parse_info)
		{
			List<StatementComponent> componentList = StatementAnalysis.GetComponents(exp_str, parse_info);
			List<MeaningGroup> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, parse_info, null);
			int retVal = 0;
			if (1 == meaningGroupList.Count)
			{
				retVal = GetSingleGroupExpVal(meaningGroupList.First(), parse_info);
			}
			else
			{
				retVal = CaculateCompondExpVal(meaningGroupList, parse_info);
			}
			return retVal;
		}

		static int GetSingleGroupExpVal(MeaningGroup meaning_group, FileParseInfo parse_info)
		{
			int retVal = 0;
			MacroDefineInfo mdi = null;
			// 立即数
			if (meaning_group.Type == MeaningGroupType.Constant
				&& GetConstantNumberValue(meaning_group.Text, out retVal))
			{
				return retVal;
			}
			// 宏定义
			else if (meaning_group.Type == MeaningGroupType.Identifier
					 && null != (mdi = parse_info.FindMacroDefInfo(meaning_group.Text)))
			{
				return GetExpressionValue(mdi.Value, parse_info);
			}
			// 表达式
			else if (meaning_group.Type == MeaningGroupType.Expression
					 && meaning_group.Text.Trim().StartsWith("(")
					 && meaning_group.Text.Trim().EndsWith(")"))
			{
				string newExp = meaning_group.Text.Trim();
				newExp = newExp.Remove(newExp.Length - 1);
				newExp = newExp.Remove(0, 1);
				return GetExpressionValue(newExp, parse_info);
			}
			else
			{
				System.Diagnostics.Trace.Assert(false);
			}
			return 0;
		}

		static int CaculateCompondExpVal(List<MeaningGroup> meaningGroupList, FileParseInfo parse_info)
		{
			OPERATION_GROUP opt = GetNextOperationGroup(meaningGroupList);
			if (null == opt)
			{
				System.Diagnostics.Trace.Assert(false);
			}
			int retVal = GetOperationValue(opt, parse_info);
			return retVal;
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
		}

		// 运算数
		class OPERAND
		{
			public string Text = string.Empty;
		}

		// 运算组
		class OPERATION_GROUP
		{
			public OPERATOR _Operator = new OPERATOR();
			public List<OPERAND> _OperandList = new List<OPERAND>();
		}

		/// <summary>
		/// 取得表达式下一个运算组
		/// </summary>
		static OPERATION_GROUP GetNextOperationGroup(List<MeaningGroup> meaningGroupList)
		{
			MeaningGroup operatorGroup = null;
			int operatorIdx = -1;
			for (int i = 0; i < meaningGroupList.Count; i++)
			{
				if (meaningGroupList[i].Type == MeaningGroupType.OtherOperator
					|| meaningGroupList[i].Type == MeaningGroupType.EqualMark)
				{
					if (null == operatorGroup
						|| meaningGroupList[i].ComponentList[0].Priority > operatorGroup.ComponentList[0].Priority)
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

			OPERATION_GROUP retGroup = new OPERATION_GROUP();
			retGroup._Operator.Text = operatorGroup.Text;
			retGroup._OperandList = GetOperandList(meaningGroupList, operatorIdx, operatorGroup.ComponentList[0].OperandCount);

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
					retList.Add(operand);
				}
				else if (operator_idx < meaningGroupList.Count - 1
						 && meaningGroupList[operator_idx + 1].Type != MeaningGroupType.OtherOperator
						 && meaningGroupList[operator_idx + 1].Type != MeaningGroupType.EqualMark)
				{
					OPERAND operand = new OPERAND();
					operand.Text = meaningGroupList[operator_idx + 1].Text;
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
					retList.Add(operand);
					operand = new OPERAND();
					operand.Text = meaningGroupList[operator_idx + 1].Text;
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
				case "==":
					retVal = CalcLogicalEqual(oper_group, parse_info);
					break;
				default:
					throw new System.NotImplementedException();
			}
			return retVal;
		}

		/// <summary>
		/// 逻辑等运算("==")
		/// </summary>
		static int CalcLogicalEqual(OPERATION_GROUP oper_group, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(oper_group._OperandList.Count == 2);
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[0].Text));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(oper_group._OperandList[1].Text));
			int operand1Value = GetExpressionValue(oper_group._OperandList[0].Text, parse_info);
			int operand2Value = GetExpressionValue(oper_group._OperandList[1].Text, parse_info);
			if (operand1Value == operand2Value)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}
	}
}
