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
				if (item.Type == MeaningGroupType.GlobalVariable)
				{
					analysisContext.inputGlobalList.Add(item);
				}
				else if (item.Type == MeaningGroupType.FunctionCalling)
				{
					analysisContext.calledFunctionList.Add(item);
				}
				else if (item.Type == MeaningGroupType.Expression)
				{
					CCodeAnalyser.ExpressionAnalysis(item.ComponentList, analysisContext);
				}
			}
		}
	}
}
