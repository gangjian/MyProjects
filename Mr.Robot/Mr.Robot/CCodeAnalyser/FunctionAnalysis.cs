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

            CodeBlockParse(fileInfo.parsedCodeList, root);

			return;
        }

		/// <summary>
		/// 解析一个代码块
		/// </summary>
		/// <param name="fileInfo">文件情报</param>
		/// <param name="startPos">代码块开始位置</param>
		/// <param name="endPos">代码块结束位置</param>
		static void CodeBlockParse(List<string> codeList, StatementNode parentNode)
		{
			File_Position searchPos = parentNode.Scope.Start;
			// 如果开头第一个字符是左花括号"{", 先要移到下一个位置开始检索
			if ('{' == codeList[searchPos.row_num][searchPos.col_num])
			{
				searchPos = PositionMoveNext(codeList, searchPos);
			}

			// 循环提取每一条语句(简单语句或者复合语句)
            StatementNode statementNode = null;
            while (true)
			{
				statementNode = GetNextStatement(codeList, ref searchPos, parentNode.Scope.End);
                if (null == statementNode)
	            {
                    break;
	            }
				statementNode.parent = parentNode;
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
		static StatementNode GetNextStatement(	List<string> codeList,
											    ref File_Position startPos,
												File_Position endPos)
		{
            StatementNode retNode = new StatementNode();
			string nextIdStr = null;
			File_Position foundPos = null;
			nextIdStr = GetNextIdentifier(codeList, ref startPos, out foundPos);
			File_Position searchPos = new File_Position(startPos);
			startPos = new File_Position(foundPos);

			if (PositionCompare(searchPos, endPos) >= 0)
			{
				// 对于检索范围超出区块结束位置的判断
				return null;
			}
			// 复合语句
			if (StatementNodeType.Invalid != GetNodeType(nextIdStr))
			{
				// 取得复合语句节点
				retNode = GetCompondStatementNode(nextIdStr, codeList, ref searchPos);
				startPos = searchPos;
				return retNode;
			}
            // 否则是简单语句
			else
			{
				// 取得简单语句节点
				retNode = GetSimpleStatementNode(codeList, ref searchPos, foundPos);
				startPos = searchPos;
				return retNode;
			}
		}

		/// <summary>
		/// 取得复合语句情报
		/// </summary>
        static StatementNode GetCompondStatementNode(string keyWord,
													 List<string> codeList,
													 ref File_Position startPos)
		{
			StatementNode retNode = null;
			StatementNodeType type = GetNodeType(keyWord);
			File_Position searchPos = new File_Position(startPos);

			switch (type)
			{
				case StatementNodeType.Compound_IfElse:							// if else
					retNode = GetIfElseStatementNode(codeList, ref searchPos);
					startPos = searchPos;
					break;
				case StatementNodeType.Compound_SwitchCase:						// switch case
					retNode = GetSwitchCaseStatementNode(codeList, ref searchPos);
					startPos = searchPos;
					break;
				case StatementNodeType.Compound_While:							// while
				case StatementNodeType.Compound_For:							// for
					retNode = GetForOrWhileStatementNode(type, codeList, ref searchPos);
					startPos = searchPos;
					break;
				case StatementNodeType.Compound_DoWhile:						// do while
					retNode = GetDoWhileStatementNode(codeList, ref searchPos);
					startPos = searchPos;
					break;
				case StatementNodeType.Compound_GoTo:							// go to(未对应)
					System.Diagnostics.Trace.Assert(false);
					break;
				case StatementNodeType.Compound_Block:							// "{"和"}"括起来的语句块
					break;
				default:
					System.Diagnostics.Trace.Assert(false);						// 异常情况
					break;
			}

			return retNode;
		}

		static StatementNode GetSimpleStatementNode(List<string> codeList,
													ref File_Position startPos,
													File_Position foundPos)
		{
			StatementNode retNode = new StatementNode();
			File_Position searchPos = new File_Position(foundPos);
			File_Position oldPos = new File_Position(foundPos);
			// 找到语句结束,也就是分号的位置
			foundPos = FindNextSpecIdentifier(";", codeList, searchPos);
			if (null != foundPos)
			{
				string statementStr = LineStringCat(codeList, oldPos, foundPos);
				retNode.Scope.Start = new File_Position(oldPos);
				retNode.Scope.End = new File_Position(foundPos);
				retNode.type = StatementNodeType.Simple;

				startPos = searchPos;
				return retNode;
			}
			return null;
		}

		/// <summary>
		/// 取得for/while循环的语句节点
		/// </summary>
		static StatementNode GetForOrWhileStatementNode(StatementNodeType type, List<string> codeList, ref File_Position startPos)
		{
			StatementNode retNode = new StatementNode();
			retNode.type = type;
			File_Position searchPos = new File_Position(startPos);
			// 取得循环表达式
			string expression = GetCompoundStatementExpression(codeList, ref searchPos);
			if (string.Empty != expression)
			{
				retNode.expression = expression;
				// 取得语句块起止位置
				File_Scope scope = GetNextStatementsBlockScope(codeList, ref searchPos);
				if (null != scope)
				{
					startPos = searchPos;
					retNode.Scope = scope;
					// 递归解析语句块
					CodeBlockParse(codeList, retNode);
					return retNode;
				}
			}
			return null;
		}

		/// <summary>
		/// 取得if else复合语句节点
		/// </summary>
		static StatementNode GetIfElseStatementNode(List<string> codeList, ref File_Position startPos)
		{
			StatementNode retNode = new StatementNode();
			retNode.type = StatementNodeType.Compound_IfElse;
			File_Position searchPos = new File_Position(startPos);
			// 取得if条件表达式
			string expression = GetCompoundStatementExpression(codeList, ref searchPos);
			if (string.Empty != expression)
			{
				retNode.expression = expression;
				// 取得if分支起止位置
				File_Scope scope = GetNextStatementsBlockScope(codeList, ref searchPos);
				if (null != scope)
				{
					StatementNode ifBranch = new StatementNode();
					ifBranch.parent = retNode;
					ifBranch.expression = expression;
					ifBranch.type = StatementNodeType.Branch_If;
					ifBranch.Scope = scope;
					// 递归解析分支语句块
					CodeBlockParse(codeList, ifBranch);

					retNode.Scope.Start = scope.Start;							// if分支的开始位置, 作为整个if else复合语句的起始位置
					retNode.childList.Add(ifBranch);

					File_Position lastElseEnd = null;							// 最后一个else分支的结束位置, 作为整个if else复合语句的结束位置
					StatementNode elseBranch = GetNextElseBrachNode(codeList, ref searchPos);
					while (null != elseBranch)
					{
						lastElseEnd = elseBranch.Scope.End;
						elseBranch.parent = retNode;
						// 递归解析分支语句块
						CodeBlockParse(codeList, elseBranch);

						retNode.childList.Add(elseBranch);
						if (StatementNodeType.Branch_Else == elseBranch.type)
						{
							// 如果是"else"分支, 代表整个if复合语句结束
							break;
						}
					}
					startPos = searchPos;
					retNode.Scope.End = lastElseEnd;
					return retNode;
				}
			}
			return null;
		}

		/// <summary>
		/// 取得switch case语句节点
		/// </summary>
		static StatementNode GetSwitchCaseStatementNode(List<string> codeList, ref File_Position startPos)
		{
			StatementNode retNode = new StatementNode();
			retNode.type = StatementNodeType.Compound_SwitchCase;
			File_Position searchPos = new File_Position(startPos);
			// 取得switch条件表达式
			string expression = GetCompoundStatementExpression(codeList, ref searchPos);
			if (string.Empty != expression)
			{
				retNode.expression = expression;
				// 取得switch分支起止位置
				File_Position oldPos = PositionMoveNext(codeList, searchPos);	// 移到左{的下一位置
				File_Scope scope = GetNextStatementsBlockScope(codeList, ref searchPos);
				if (null != scope)
				{
					// 取得各case或default分支
					StatementNode caseBranch = GetNextCaseBranchNode(codeList, ref oldPos);
					while (null != caseBranch)
					{
						caseBranch.parent = retNode;
						// 递归解析分支语句块
						CodeBlockParse(codeList, caseBranch);

						retNode.childList.Add(caseBranch);
						if (StatementNodeType.Branch_Default == caseBranch.type)
						{
							// 如果是"default"分支, 代表整个switch case复合语句结束
							break;
						}
					}
					startPos = searchPos;
					return retNode;
				}
			}
			return null;
		}

		static StatementNode GetDoWhileStatementNode(List<string> codeList, ref File_Position startPos)
		{
			return null;
		}

		/// <summary>
		/// 取得下一个else/else if分支节点
		/// </summary>
		static StatementNode GetNextElseBrachNode(List<string> codeList, ref File_Position startPos)
		{
			StatementNode retNode = new StatementNode();
			File_Position searchPos = new File_Position(startPos);
			File_Position foundPos = new File_Position(searchPos);
			string idStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
			if ("else" == idStr)
			{
				File_Position oldPos = new File_Position(searchPos);
				idStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
				if ("if" == idStr)
				{
					// 表示这是一个"else if"分支
					// 取得分支表达式
					string expression = GetCompoundStatementExpression(codeList, ref searchPos);
					if (string.Empty != expression)
					{
						retNode.expression = expression;
						// 确定分支范围
						File_Scope scope = GetNextStatementsBlockScope(codeList, ref searchPos);
						if (null != scope)
						{
							retNode.type = StatementNodeType.Branch_ElseIf;
							retNode.Scope = scope;
							startPos = searchPos;
							return retNode;
						}
					}
				}
				else
				{
					searchPos = oldPos;
					// 否则是"else"分支
					// 确定分支范围
					File_Scope scope = GetNextStatementsBlockScope(codeList, ref searchPos);
					if (null != scope)
					{
						retNode.type = StatementNodeType.Branch_Else;
						retNode.Scope = scope;
						startPos = searchPos;
						return retNode;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 取得下一个case/default分支节点
		/// </summary>
		/// <returns></returns>
		static StatementNode GetNextCaseBranchNode(List<string> codeList, ref File_Position startPos)
		{
			StatementNode retNode = new StatementNode();
			File_Position searchPos = new File_Position(startPos);
			File_Position foundPos = new File_Position(searchPos);
			string idStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
			if ("case" == idStr)
			{
				// 取得case语句, 定位":"的位置
				// 取得整个case分支的范围: 逐一提取语句, 直到遇到"break;"或者下一case/default分支开始
			}
			else if ("default" == idStr)
			{
			}
			return null;
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
		/// 取得switch case语句中case分支的表达式
		/// </summary>
		static string GetCaseBranchExpression(List<string> codeList, ref File_Position startPos)
		{
			File_Position searchPos = new File_Position(startPos);
			File_Position foundPos = new File_Position(searchPos);

			// 找到冒号":"的位置
			foundPos = FindNextSpecIdentifier(":", codeList, searchPos);
			return null;
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

		static File_Scope GetCaseBranchScope(List<string> codeList, ref File_Position startPos)
		{
			return null;
		}

		/// <summary>
		/// 根据关键字取得复合语句节点类型
		/// </summary>
		/// <param name="keyWord"></param>
		/// <returns></returns>
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
