using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Robot
{
	public static partial class CCodeAnalyser
	{
		static void FunctionsAnalyze(CFileParseInfo fileInfo)
		{
			foreach (CFunctionInfo func in fileInfo.fun_define_list)
			{
				CodeBlockParse(fileInfo, func.body_start_pos, func.body_end_pos);
			}
		}

		/// <summary>
		/// 解析一个代码块
		/// </summary>
		/// <param name="fileInfo">文件情报</param>
		/// <param name="startPos">代码块开始位置</param>
		/// <param name="endPos">代码块结束位置</param>
		static void CodeBlockParse(CFileParseInfo fileInfo, File_Position startPos, File_Position endPos)
		{
			bool isSimple;
			string statementStr;
			List<string> statementsList = new List<string>();
			File_Position searchPos = startPos;
			searchPos.col_num += 1;
			// 循环提取每一条语句(简单语句或者复合语句)
			while (string.Empty != (statementStr = GetNextStatement(fileInfo, ref searchPos, endPos, out isSimple)))
			{
				if (isSimple)
				{
					// 简单语句
					statementsList.Add(statementStr);
				}
				else
				{
					// 复合语句
				}
			}
		}

		/// <summary>
		/// 从当前位置提取一条语句(简单语句或者是复合语句)
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <param name="startPos"></param>
		/// <param name="endPos"></param>
		/// <param name="isSimple">true: 是简单语句; false: 是复合语句</param>
		/// <returns></returns>
		static string GetNextStatement(CFileParseInfo fileInfo, ref File_Position startPos,
									   File_Position endPos, out bool isSimple)
		{
			isSimple = true;
			List<string> qualifierList = new List<string>();     // 修饰符暂存列表
			string nextId = null;
			File_Position foundPos = null;
			nextId = GetNextIdentifier(fileInfo.parsedCodeList, ref startPos, out foundPos);
			File_Position searchPos = new File_Position(startPos);
			startPos = new File_Position(foundPos);
			while (null != nextId)
			{
				if ((searchPos.row_num > endPos.row_num)
					|| ((searchPos.row_num == endPos.row_num) && (searchPos.col_num > endPos.col_num)))
				{
					// 对于超出范围的判断
					break;
				}
				// 如果是标准标识符(字母,数字,下划线组成且开头不是数字)
				if (IsStandardIdentifier(nextId)
					|| ("*" == nextId))
				{
					// 复合语句
					if (("if" == nextId) || ("for" == nextId)
						|| ("while" == nextId) || ("do" == nextId)
						|| ("switch" == nextId))
					{
						isSimple = false;
						// 取得完整的复合语句相关情报
						GetCompondStatementsInfo(nextId, fileInfo, ref searchPos, endPos);
					}
				}
				// 否则可能是各种符号
				else
				{
					if (";" == nextId)
					{
						// 分号是简单语句的结束标志
						// 确定起始, 终了位置, 提取语句内容
						string statementStr = "";
						for (int i = startPos.row_num; i <= foundPos.row_num; i++)
						{
							int sIdx = 0;
							int eIdx = -1;
							if (startPos.row_num == i)
							{
								sIdx = startPos.col_num;
							}
							if (endPos.row_num == i)
							{
								eIdx = endPos.col_num;
								statementStr += fileInfo.parsedCodeList[i].Substring(sIdx, eIdx - sIdx + 1);
							}
							else
							{
								statementStr += fileInfo.parsedCodeList[i].Substring(sIdx);
							}
						}
						startPos = searchPos;
						return statementStr;
					}
					else if ("{" == nextId)
					{
						// 左花括号, 内嵌的代码块
						isSimple = false;
						File_Position fp = FindNextMatchSymbol(fileInfo.parsedCodeList, searchPos, '}');
						if (null != fp)
						{
							
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
				nextId = GetNextIdentifier(fileInfo.parsedCodeList, ref searchPos, out foundPos);
			}
			return string.Empty;
		}

		static void GetCompondStatementsInfo(string keyWord, CFileParseInfo fileInfo,
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
					return;
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
