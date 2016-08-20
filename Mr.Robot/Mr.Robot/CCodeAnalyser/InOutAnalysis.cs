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
				if (item.Type == MeaningGroupType.GlobalVariable)						// 全局变量
				{
					analysisContext.inputGlobalList.Add(item);
				}
				else if (item.Type == MeaningGroupType.FunctionCalling)					// 函数调用
				{
					CalledFunctionProcess(item, analysisContext);
				}
				else if (item.Type == MeaningGroupType.Expression)						// 表达式
				{
					CCodeAnalyser.ExpressionAnalysis(item.ComponentList, analysisContext);
				}
			}
		}

		static void CalledFunctionProcess(MeaningGroup mg, AnalysisContext ctx)
		{
			CalledFunction cf = new CalledFunction();
			cf.meaningGroup = mg;
			// 根据函数名查找函数声明(暂时不考虑C++的函数重载)
			CFunctionStructInfo fi = ctx.parseResult.GetFunctionParseInfoByName(mg.ComponentList[0].Text);
			if (null == fi)
			{
				// 不应该找不到
				System.Windows.Forms.MessageBox.Show("CalledFunctionProcess(...) : 没找到调用函数的解析结果!");
				return;
			}
			// 分别判断各引数是值类型还是引用类型
			foreach (var pds in fi.paras)
			{
				cf.paraTypeList.Add(JudgeParaType(pds));
			}

			ctx.calledFunctionList.Add(cf);
		}

		// 判断引数的种别(值类型或者是引用类型)
		static FunParaType JudgeParaType(string para_def)
		{
			int offset = 0;
			int old_offset = 0;
			// 前缀 + 类型部分 + 引数名
			//while (true)
			//{
			//	old_offset = offset;
			//	string idStr = CommonProcess.GetNextIdentifier2(para_def, ref offset);
			//	if (string.IsNullOrEmpty(idStr))
			//	{
			//		break;
			//	}
			//	else
			//	{
			//		offset = old_offset + idStr.Length;
			//	}
			//}
			return FunParaType.Value;
		}
	}

	/// <summary>
	/// 函数引数的种别(引用类型或者值类型)
	/// </summary>
	public enum FunParaType
	{
		Reference,
		Value,
	}
}
