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
        /// 函数解析
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="funcName"></param>
        /// <param name="parsedResultList"></param>
        static public void FunctionAnalyze(string fullPath, string funcName, List<CCodeParseResult> parsedResultList)
        {
            CFunctionInfo funInfo = null;
            CFileParseInfo fileInfo = null;
            // 根据文件名, 函数名取得函数情报的引用
            foreach (CCodeParseResult result in parsedResultList)
            {
                if (result.SourceParseInfo.full_name.Equals(fullPath))
                {
                    foreach (CFunctionInfo fi in result.SourceParseInfo.fun_define_list)
                    {
                        if (fi.name.Equals(funcName))
                        {
                            funInfo = fi;
                            fileInfo = result.SourceParseInfo;
                            break;
                        }
                    }
                    break;
                }
            }
            if (null == funInfo)
            {
                return;
            }

            // 函数语句树
            StatementTree stree = new StatementTree();
            CodeBlockParse(fileInfo, funInfo.body_start_pos, funInfo.body_end_pos);
        }

		/// <summary>
		/// 解析一个代码块
		/// </summary>
		/// <param name="fileInfo">文件情报</param>
		/// <param name="startPos">代码块开始位置</param>
		/// <param name="endPos">代码块结束位置</param>
        static List<StatementNode> CodeBlockParse(CFileParseInfo fileInfo, File_Position startPos, File_Position endPos)
		{
            List<StatementNode> retList = new List<StatementNode>();
			File_Position searchPos = startPos;
			// 因为开头第一个字符是左花括号, 所以先要移到下一个位置开始检索
			searchPos = PositionMoveNext(fileInfo.parsedCodeList, searchPos);

			// 循环提取每一条语句(简单语句或者复合语句)
            StatementNode statementNode = GetNextStatement(fileInfo, ref searchPos, endPos);
            while (null != statementNode)
			{
                retList.Add(statementNode);
                statementNode = GetNextStatement(fileInfo, ref searchPos, endPos);
			}
            return retList;
		}

		/// <summary>
		/// 从当前位置提取一条语句(简单语句或者是复合语句)
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <param name="startPos"></param>
		/// <param name="endPos"></param>
		/// <param name="isSimpleStatement">true: 是简单语句; false: 是复合语句</param>
		/// <returns></returns>
        static StatementNode GetNextStatement(  CFileParseInfo fileInfo,
											    ref File_Position startPos,
												File_Position endPos)
		{
            StatementNode retStatementInfo = new StatementNode();
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
						// 确定起始, 终了位置, 提取语句内容
						string statementStr = LineStringCat(fileInfo.parsedCodeList, startPos, foundPos);
						startPos = searchPos;
						//retStatementInfo.keyStr = statementStr;
						retStatementInfo.startPos = new File_Position(startPos);
						retStatementInfo.endPos = new File_Position(foundPos);
						return retStatementInfo;
					}
					else if ("{" == nextIdStr)
					{
						// 左花括号, 内嵌的代码块
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
        static StatementNode GetCompondStatementsInfo(  string keyWord,
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
