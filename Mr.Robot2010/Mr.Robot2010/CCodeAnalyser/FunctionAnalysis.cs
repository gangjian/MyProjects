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
            StatementNode root = new StatementNode();
            root.type = StatementNodeType.Root;
            root.startPos = funInfo.body_start_pos;
            root.endPos = funInfo.body_end_pos;

            CodeBlockParse(fileInfo, root);
        }

		/// <summary>
		/// 解析一个代码块
		/// </summary>
		/// <param name="fileInfo">文件情报</param>
		/// <param name="startPos">代码块开始位置</param>
		/// <param name="endPos">代码块结束位置</param>
        static void CodeBlockParse(CFileParseInfo fileInfo, StatementNode parentNode)
		{
			File_Position searchPos = parentNode.startPos;
			// 因为开头第一个字符是左花括号, 所以先要移到下一个位置开始检索
			searchPos = PositionMoveNext(fileInfo.parsedCodeList, searchPos);

			// 循环提取每一条语句(简单语句或者复合语句)
            StatementNode statementNode = null;
            while (true)
			{
                statementNode = GetNextStatement(fileInfo, ref searchPos, parentNode.endPos);
                if (null == statementNode)
	            {
                    break;
	            }
                parentNode.childList.Add(statementNode);
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
        static StatementNode GetNextStatement(  CFileParseInfo fileInfo,
											    ref File_Position startPos,
												File_Position endPos)
		{
            StatementNode retNode = new StatementNode();
			string nextIdStr = null;
			File_Position foundPos = null;
			nextIdStr = GetNextIdentifier(fileInfo.parsedCodeList, ref startPos, out foundPos);
			File_Position searchPos = new File_Position(startPos);
			startPos = new File_Position(foundPos);
            bool bStart = false;    // 标识语句的开头

			while (null != nextIdStr)
			{
				if (PositionCompare(searchPos, endPos) >= 0)
				{
					// 对于检索范围超出区块结束位置的判断
					break;
				}
				// 复合语句
                if (
                    (false == bStart)
                    && (   ("if" == nextIdStr)
                        || ("for" == nextIdStr)
						|| ("while" == nextIdStr)
                        || ("do" == nextIdStr)
                        || ("switch" == nextIdStr)
                        || ("{" == nextIdStr)
                        || ("goto" == nextIdStr))
                    )
				{
					// 取得完整的复合语句相关情报
					retNode = GetCompondStatementNode(nextIdStr, fileInfo, ref searchPos, endPos);
				}
                // 否则是简单语句
				else
				{
                    // 找到语句结束,也就是分号的位置
                    foundPos = FindNextSpecIdentifier(";", fileInfo.parsedCodeList, searchPos);
                    if (null != foundPos)
                    {
                        string statementStr = LineStringCat(fileInfo.parsedCodeList, startPos, foundPos);
						retNode.startPos = new File_Position(startPos);
						retNode.endPos = new File_Position(foundPos);

                        startPos = searchPos;
                        return retNode;
                    }
				}
                bStart = true;
				nextIdStr = GetNextIdentifier(fileInfo.parsedCodeList, ref searchPos, out foundPos);
			}
			return null;
		}

		/// <summary>
		/// 取得复合语句情报
		/// </summary>
        static StatementNode GetCompondStatementNode(  string keyWord,
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
