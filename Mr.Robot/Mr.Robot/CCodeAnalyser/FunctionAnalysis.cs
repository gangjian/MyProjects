using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Robot
{
	/// <summary>
	/// 语句块情报类
	/// </summary>
	public class StatementsBlock
	{
		string keyStr = string.Empty;											// 关键字
		string ExpressionStr = "";												// 表达式
		File_Position sPos = null;												// 语句块起止位置
		File_Position ePos = null;
		List<StatementsBlock> childBlocks = new List<StatementsBlock>();
	}
	/// <summary>
	/// 复合语句情报类
	/// </summary>
	public class CompondStatementsInfo
	{
		// 复合语句关键字: if, for, switch, while, do while
		public string keyStr = string.Empty;
		// 整个复合语句的起止位置
		public File_Position startPos = null;
		public File_Position endPos = null;
		// 各子语句块
		public List<StatementsBlock> blocks = new List<StatementsBlock>();
	}

	public static partial class CCodeAnalyser
	{
		static void FunctionAnalyze(CFileParseInfo fileInfo, CFunctionInfo funcInfo)
		{
			// 入出力列表

			// 最外层, 整个函数体
			CodeBlockParse(fileInfo, funcInfo.body_start_pos, funcInfo.body_end_pos);
		}

		/// <summary>
		/// 解析一个代码块
		/// </summary>
		/// <param name="fileInfo">文件情报</param>
		/// <param name="startPos">代码块开始位置</param>
		/// <param name="endPos">代码块结束位置</param>
		static void CodeBlockParse(CFileParseInfo fileInfo, File_Position startPos, File_Position endPos)
		{
			bool isSimpleStatement;
			List<string> statementsList = new List<string>();
			File_Position searchPos = startPos;
			// 因为开头第一个字符是左花括号, 所以先要移到下一个位置开始检索
			searchPos = PositionMoveNext(fileInfo.parsedCodeList, searchPos);

			// 循环提取每一条语句(简单语句或者复合语句)
			CompondStatementsInfo statementInfo = GetNextStatement(fileInfo, ref searchPos, endPos, out isSimpleStatement);
			while (null != statementInfo)
			{
				if (isSimpleStatement)
				{
					// 简单语句
					statementsList.Add(statementInfo.keyStr);
				}
				else
				{
					// 复合语句
				}
				statementInfo = GetNextStatement(fileInfo, ref searchPos, endPos, out isSimpleStatement);
			}
		}

		/// <summary>
		/// 从当前位置提取一条语句(简单语句或者是复合语句)
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <param name="startPos"></param>
		/// <param name="endPos"></param>
		/// <param name="isSimpleStatement">true: 是简单语句; false: 是复合语句</param>
		/// <returns></returns>
		static CompondStatementsInfo GetNextStatement(CFileParseInfo fileInfo,
													  ref File_Position startPos,
													  File_Position endPos,
													  out bool isSimple)
		{
			CompondStatementsInfo retStatementInfo = new CompondStatementsInfo();
			isSimple = true;
			List<string> qualifierList = new List<string>();     // 修饰符暂存列表
			string nextIdStr = null;
			File_Position foundPos = null;
			nextIdStr = GetNextIdentifier(fileInfo.parsedCodeList, ref startPos, out foundPos);
			File_Position searchPos = new File_Position(startPos);
			startPos = new File_Position(foundPos);
			while (null != nextIdStr)
			{
				if (PositionCompare(searchPos, endPos) >= 0)
				{
					// 对于检索范围超出区块结束位置的判断
					break;
				}
				// 如果是标准标识符(字母,数字,下划线组成且开头不是数字)
				if (IsStandardIdentifier(nextIdStr)
					|| ("*" == nextIdStr))
				{
					// 复合语句
					if (("if" == nextIdStr) || ("for" == nextIdStr)
						|| ("while" == nextIdStr) || ("do" == nextIdStr)
						|| ("switch" == nextIdStr))
					{
						isSimple = false;
						// 取得完整的复合语句相关情报
						retStatementInfo = GetCompondStatementsInfo(nextIdStr, fileInfo, ref searchPos, endPos);
					}
					else
					{
						qualifierList.Add(nextIdStr);
					}
				}
				// 否则可能是各种符号
				else
				{
					if (";" == nextIdStr)
					{
						// 分号是简单语句的结束标志
						isSimple = true;
						// 确定起始, 终了位置, 提取语句内容
						string statementStr = LineStringCat(fileInfo.parsedCodeList, startPos, foundPos);
						startPos = searchPos;
						retStatementInfo.keyStr = statementStr;
						retStatementInfo.startPos = new File_Position(startPos);
						retStatementInfo.endPos = new File_Position(foundPos);
						return retStatementInfo;
					}
					else if ("{" == nextIdStr)
					{
						// 左花括号, 内嵌的代码块
						isSimple = false;
						File_Position fp = FindNextMatchSymbol(fileInfo.parsedCodeList, searchPos, '}');
						if (null != fp)
						{
							;
						}
						else
						{
							break;
						}
					}
					else
					{
					}
				}
				nextIdStr = GetNextIdentifier(fileInfo.parsedCodeList, ref searchPos, out foundPos);
			}
			return null;
		}

		/// <summary>
		/// 取得复合语句情报
		/// </summary>
		static CompondStatementsInfo GetCompondStatementsInfo(string keyWord,
														CFileParseInfo fileInfo,
														ref File_Position startPos,
														File_Position endPos)
		{
			File_Position searchPos = new File_Position(startPos);
			File_Position foundPos;
			if ("if" == keyWord)
			{
				// 取得条件表达式
				string conditionalExpression = GetConditionalExpression(fileInfo.parsedCodeList, ref searchPos);
				if (string.Empty == conditionalExpression)
				{
					return null;
				}
				// 取得语句块
				string idStr = GetNextIdentifier(fileInfo.parsedCodeList, ref searchPos, out foundPos);
				if ("{" == idStr)
				{
					// 通常语句块会以花括号括起来
					// 找到配对的花括号
				}
				else
				{
					// 但是如果没有遇到花括号, 那么可能是语句块只有一条简单语句
					// 提取一条简单语句
				}
			}
			else if (("for" == keyWord) || "while" == keyWord)
			{

			}
			else if ("do" == keyWord)
			{

			}
			else if ("switch" == keyWord)
			{

			}
			else if ("{" == keyWord)
			{

			}
			else
			{
			}
			return null;
		}

		/// <summary>
		/// 解析一条简单语句
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <param name="startPos"></param>
		/// <param name="endPos"></param>
		static void ParseSimpleStatement(CFileParseInfo fileInfo, File_Position startPos, File_Position endPos)
		{

		}

		/// <summary>
		/// 取得条件表达式
		/// </summary>
		static string GetConditionalExpression(List<string> codeList, ref File_Position searchPos)
		{
			return string.Empty;
		}

	}
}
