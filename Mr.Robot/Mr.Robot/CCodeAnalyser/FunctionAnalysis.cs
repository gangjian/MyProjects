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
            root.Scope.Start = funInfo.body_start_pos;
            root.Scope.End = funInfo.body_end_pos;

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
			File_Position searchPos = parentNode.Scope.Start;
			// 因为开头第一个字符是左花括号, 所以先要移到下一个位置开始检索
			searchPos = PositionMoveNext(fileInfo.parsedCodeList, searchPos);

			// 循环提取每一条语句(简单语句或者复合语句)
            StatementNode statementNode = null;
            while (true)
			{
                statementNode = GetNextStatement(fileInfo, ref searchPos, parentNode.Scope.End);
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

			if (PositionCompare(searchPos, endPos) >= 0)
			{
				// 对于检索范围超出区块结束位置的判断
				return null;
			}
			// 复合语句
            if (
				("if" == nextIdStr)
				|| ("for" == nextIdStr)
				|| ("while" == nextIdStr)
				|| ("do" == nextIdStr)
				|| ("switch" == nextIdStr)
				|| ("{" == nextIdStr)
				|| ("goto" == nextIdStr))
			{
				// 取得完整的复合语句相关情报
				retNode = GetCompondStatementNode(nextIdStr, fileInfo, ref searchPos, endPos);
				startPos = searchPos;
				return retNode;
			}
            // 否则是简单语句
			else
			{
                // 找到语句结束,也就是分号的位置
                foundPos = FindNextSpecIdentifier(";", fileInfo.parsedCodeList, searchPos);
                if (null != foundPos)
                {
                    string statementStr = LineStringCat(fileInfo.parsedCodeList, startPos, foundPos);
					retNode.Scope.Start = new File_Position(startPos);
					retNode.Scope.End = new File_Position(foundPos);
					retNode.type = StatementNodeType.Simple;

                    startPos = searchPos;
                    return retNode;
                }
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
			StatementNode retNode = new StatementNode();
			retNode.type = GetNodeType(keyWord);

			File_Position searchPos = new File_Position(startPos);
			if (("if" == keyWord)
				|| ("for" == keyWord)
				|| ("while" == keyWord)
				|| ("switch" == keyWord))
			{
				// 取得循环表达式
				string expression = GetCompoundStatementExpression(fileInfo.parsedCodeList, ref searchPos);
				if (string.Empty != expression)
				{
					retNode.expression = expression;
					// 取得语句块起止位置
					File_Scope scope = GetNextStatementsBlockScope(fileInfo.parsedCodeList, ref searchPos);
					if (null != scope)
					{
						startPos = searchPos;
						retNode.Scope = scope;
						return retNode;
					}
				}
			}
			else if ("do" == keyWord)
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
		/// 取得复合语句表达式
		/// </summary>
		static string GetCompoundStatementExpression(List<string> codeList, ref File_Position startPos)
		{
			File_Position searchPos = new File_Position(startPos);
			File_Position foundPos = new File_Position(searchPos);

			string idStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
			if ("(" == idStr)
			{
				File_Position leftBracePos = new File_Position(foundPos);
				searchPos = PositionMoveNext(codeList, leftBracePos);
				File_Position rightBracePos = FindNextMatchSymbol(codeList, searchPos, ')');
				if (null != rightBracePos)
				{
					startPos = searchPos;
					string exp = LineStringCat(codeList, leftBracePos, rightBracePos);
					return exp;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// 取得下一个语句块的范围(起止位置)
		/// </summary>
		/// <param name="codeList"></param>
		/// <param name="startPos"></param>
		/// <returns></returns>
		static File_Scope GetNextStatementsBlockScope(List<string> codeList, ref File_Position startPos)
		{
			File_Scope scope = new File_Scope();
			File_Position searchPos = new File_Position(startPos);
			File_Position foundPos = new File_Position(searchPos);

			string idStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
			if ("{" == idStr)
			{
				// 通常语句块会以花括号括起来
				File_Position leftBrace = new File_Position(foundPos);
				// 找到配对的花括号
				File_Position rightBrace = FindNextMatchSymbol(codeList, searchPos, '}');
				if (null != rightBrace)
				{
					startPos = searchPos;
					scope.Start = leftBrace;
					scope.End = rightBrace;
					return scope;
				}
			}
			else
			{
				// 但是如果没有遇到花括号, 那么可能是语句块只有一条简单语句
				// 提取一条简单语句
				scope.Start = new File_Position(foundPos);
				File_Position ePos;
				if (";" == idStr)
				{
					// 空语句的场合
					ePos = new File_Position(foundPos);
				}
				else
				{
					ePos = FindNextSpecIdentifier(";", codeList, searchPos);
				}
				if (null != ePos)
				{
					startPos = searchPos;
					scope.End = ePos;
					return scope;
				}
			}

			return null;
		}

		static StatementNodeType GetNodeType(string keyWord)
		{
			StatementNodeType retType = StatementNodeType.Invalid;
			switch (keyWord)
			{
				case "if":
					retType = StatementNodeType.Compound_IfElse;
					break;
				case "{":
					retType = StatementNodeType.Compound_Block;
					break;
				case "do":
					retType = StatementNodeType.Compound_DoWhile;
					break;
				case "for":
					retType = StatementNodeType.Compound_For;
					break;
				case "goto":
					retType = StatementNodeType.Compound_GoTo;
					break;
				case "switch":
					retType = StatementNodeType.Compound_SwitchCase;
					break;
				case "while":
					retType = StatementNodeType.Compound_While;
					break;
				default:
					break;
			}
			return retType;
		}

	}
}
