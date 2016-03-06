using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Robot
{
	public static partial class CCodeAnalyser
	{
		/// <summary>
		/// 语句分析
		/// </summary>
		static public void FunctionStatementsAnalysis(StatementNode root, List<string> codeList, List<CFileParseInfo> headerList)
		{
			// 顺次解析各条语句
			foreach (StatementNode childNode in root.childList)
			{
				StatementAnalysis(childNode, codeList, headerList);
			}
		}

		static void StatementAnalysis(StatementNode node, List<string> codeList, List<CFileParseInfo> headerList)
		{
			switch (node.Type)
			{
				case StatementNodeType.Simple:
					SimpleStatementAnalysis(node, codeList, headerList);
					break;
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
		}

		/// <summary>
		/// 简单语句分析
		/// </summary>
		static void SimpleStatementAnalysis(StatementNode statementNode, List<string> codeList, List<CFileParseInfo> headerList)
		{
			// 取得完整的语句内容
			string statementStr = LineStringCat(codeList, statementNode.Scope.Start, statementNode.Scope.End);

			// 依次按顺序取出语句各组成部分
			List<StatementOperator> componentList = new List<StatementOperator>();
			int offset = 0;
			do
			{
				StatementOperator cpnt = GetSingleStatementOperator(statementStr, ref offset);
				if (null == cpnt)
				{
					break;
				}
				componentList.Add(cpnt);
			} while (true);
		}

		/// <summary>
		/// 从语句中提取出一个操作数
		/// </summary>
		static StatementOperator GetSingleStatementOperator(string statementStr, ref int offset)
		{
			string idStr = null;
			if (null != (idStr = GetNextIdentifier(statementStr, ref offset)))
			{
				if (IsStandardIdentifier(idStr))
				{
					// 如果是标准标识符
				}
				else if ("(" == idStr)
				{
					
				}
				else if ("=" == idStr)
				{
					
				}
			}
			return null;
		}

		/// <summary>
		/// 从指定位置开始取得语句中的下一个标识符
		/// </summary>
		/// <returns></returns>
		static string GetNextIdentifier(string statementStr, ref int offset)
		{
			return null;
		}
	}

	public class StatementOperator
	{
		string text = string.Empty;

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		StatementComponentType type = StatementComponentType.Inavalid;

		public StatementComponentType Type
		{
			get { return type; }
			set { type = value; }
		}
	}

	public enum StatementComponentType
	{
		Inavalid,				// 无效

		Type,					// 类型
		GlobalVariable,			// 全局变量
		LocalVariable,			// 局部变量

		FunctionCalling,		// 函数调用
		EqualMark,				// 等号(赋值符号)

		Expression,				// 表达式
	}
}


