using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Robot
{
	public partial class CCodeAnalyser
	{
        /// <summary>
        /// 函数解析
        /// </summary>
        /// <param varName="fullPath"></param>
        /// <param varName="funcName"></param>
        /// <param varName="parsedResultList"></param>
        public static void FunctionAnalysis(string fullPath, string funcName, List<FileParseInfo> parsedInfoList)
        {
            // 从全部解析结果列表中根据指定文件名和函数名找到相应的文件和函数解析结果
            FileParseInfo CSourceParseInfo = null;
            FuncParseInfo funInfo = GetFuncInfoFromParseResult(fullPath, funcName, parsedInfoList, out CSourceParseInfo);

            // 函数语句树结构的分析提取
            StatementNode root = new StatementNode();
            root.Type = StatementNodeType.Root;
            root.Scope = funInfo.Scope;
            GetFuncBlockStruct(CSourceParseInfo, ref root);

			// 函数语句分析: 分析入出力
			StatementAnalysis.FunctionStatementsAnalysis(root, CSourceParseInfo);
        }

        public static FuncParseInfo GetFuncInfoFromParseResult(string fileName,
																string funcName,
																List<FileParseInfo> parseInfoList,
                                                                out FileParseInfo parseInfo)
        {
            FuncParseInfo funInfo = null;
            parseInfo = null;
            // 根据文件名, 函数名取得函数情报的引用
            foreach (FileParseInfo pi in parseInfoList)
            {
                if (pi.SourceName.Equals(fileName))
                {                                                                       // 找到指定的文件(根据文件名)
                    foreach (FuncParseInfo fi in pi.FunDefineList)
                    {
                        if (fi.Name.Equals(funcName))
                        {                                                               // 找到指定的函数(根据函数名)
                            funInfo = fi;
                            parseInfo = pi;
                            break;
                        }
                    }
                    break;
                }
            }
            return funInfo;
        }

		/// <summary>
		/// 解析一个代码块,提取语句树结构
		/// </summary>
		public static void GetFuncBlockStruct(FileParseInfo parse_info, ref StatementNode root)
		{
			CodePosition searchPos = new CodePosition(root.Scope.Start);
			// 如果开头第一个字符是左花括号"{", 先要移到下一个位置开始检索
			if ('{' == parse_info.CodeList[searchPos.RowNum][searchPos.ColNum])
			{
				searchPos = CommonProcess.PositionMoveNext(parse_info.CodeList, searchPos);
			}

			// 循环提取每一条语句(简单语句或者复合语句)
            StatementNode statementNode = null;
            while (true)
			{
				statementNode = GetNextStatement(parse_info, ref searchPos, root.Scope.End);
                if (null == statementNode)
	            {
                    break;
	            }
				statementNode.parent = root;
                root.childList.Add(statementNode);
			}
		}

		/// <summary>
		/// 从当前位置提取一条语句(简单语句或者是复合语句)
		/// </summary>
		/// <param varName="fileInfo"></param>
		/// <param varName="startPos"></param>
		/// <param varName="endPos"></param>
		/// <param varName="isSimpleStatement">true: 是简单语句; false: 是复合语句</param>
		/// <returns></returns>
		static StatementNode GetNextStatement(	FileParseInfo parse_info,
											    ref CodePosition startPos,
												CodePosition endPos)
		{
            StatementNode retNode = new StatementNode();
			CodePosition foundPos = null;
			CodeIdentifier nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref startPos, out foundPos);
			CodePosition searchPos = new CodePosition(startPos);
			startPos = new CodePosition(foundPos);

            if (null != endPos
                && CommonProcess.PositionCompare(searchPos, endPos) >= 0)
			{
				// 对于检索范围超出区块结束位置的判断
				return null;
			}
			// 复合语句
			if (StatementNodeType.Invalid != GetNodeType(nextIdtf.Text))
			{
				// 取得复合语句节点
				retNode = GetCompondStatementNode(nextIdtf.Text, parse_info, ref searchPos);
				startPos = searchPos;
				return retNode;
			}
            // 否则是简单语句
			else
			{
				// 取得简单语句节点
				retNode = GetSimpleStatementNode(parse_info, ref searchPos, foundPos);
				startPos = CommonProcess.PositionMoveNext(parse_info.CodeList, retNode.Scope.End);
				return retNode;
			}
		}

		/// <summary>
		/// 取得复合语句情报
		/// </summary>
        static StatementNode GetCompondStatementNode(string keyWord,
													 FileParseInfo parse_info,
													 ref CodePosition startPos)
		{
			StatementNode retNode = null;
			StatementNodeType type = GetNodeType(keyWord);
			CodePosition searchPos = new CodePosition(startPos);

			switch (type)
			{
				case StatementNodeType.Compound_IfElse:							// if else
					retNode = GetIfElseStatementNode(parse_info, ref searchPos);
					startPos = searchPos;
					break;
				case StatementNodeType.Compound_SwitchCase:						// switch case
					retNode = GetSwitchCaseStatementNode(parse_info, ref searchPos);
					startPos = searchPos;
					break;
				case StatementNodeType.Compound_While:							// while
				case StatementNodeType.Compound_For:							// for
					retNode = GetForOrWhileStatementNode(type, parse_info, ref searchPos);
					startPos = searchPos;
					break;
				case StatementNodeType.Compound_DoWhile:						// do while
					retNode = GetDoWhileStatementNode(parse_info, ref searchPos);
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

        /// <summary>
        /// 取得简单语句的语句节点对象
        /// </summary>
		static StatementNode GetSimpleStatementNode(FileParseInfo parse_info,
													ref CodePosition startPos,
													CodePosition foundPos)
		{
			StatementNode retNode = new StatementNode();
			CodePosition searchPos = new CodePosition(foundPos);
			CodePosition oldPos = new CodePosition(foundPos);
			// 找到语句结束,也就是分号的位置
			foundPos = CommonProcess.FindNextSpecIdentifier(";", parse_info.CodeList, searchPos);
			if (null != foundPos)
			{
				string statementStr = CommonProcess.LineStringCat(parse_info.CodeList, oldPos, foundPos);
				retNode.Scope = new CodeScope(oldPos, foundPos);
				retNode.Type = StatementNodeType.Simple;

				startPos = searchPos;
				return retNode;
			}
			return null;
		}

		/// <summary>
		/// 取得for/while循环语句的语句节点对象
		/// </summary>
		static StatementNode GetForOrWhileStatementNode(StatementNodeType type, FileParseInfo parse_info, ref CodePosition startPos)
		{
			StatementNode retNode = new StatementNode();
			retNode.Type = type;
			CodePosition searchPos = new CodePosition(startPos);
			// 取得循环表达式
			string expression = GetCompoundStatementExpression(parse_info, ref searchPos);
			if (string.Empty != expression)
			{
				retNode.expression = expression;
				// 取得分支范围
				CodeScope scope = GetBranchScope(parse_info, ref searchPos);
				if (null != scope)
				{
					startPos = searchPos;
					retNode.Scope = scope;
					// 递归解析语句块
					GetFuncBlockStruct(parse_info, ref retNode);
					return retNode;
				}
			}
			return null;
		}

        /// <summary>
        /// 取得do-while循环语句的语句节点对象
        /// </summary>
		static StatementNode GetDoWhileStatementNode(FileParseInfo parse_info, ref CodePosition startPos)
        {
            StatementNode retNode = new StatementNode();
            retNode.Type = StatementNodeType.Compound_DoWhile;
            CodePosition searchPos = new CodePosition(startPos);
            // 取得分支范围
			CodeScope scope = GetBranchScope(parse_info, ref searchPos);
            if (null != scope)
            {
                retNode.Scope = scope;
				searchPos = CommonProcess.PositionMoveNext(parse_info.CodeList, scope.End);
                CodePosition foundPos;
				CodeIdentifier nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
				if ("while" == nextIdtf.Text)
                {
					string expression = GetCompoundStatementExpression(parse_info, ref searchPos);
					nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
                    if (string.Empty != expression
						&& ";" == nextIdtf.Text)
                    {
                        retNode.expression = expression;
                        // 递归解析语句块
						GetFuncBlockStruct(parse_info, ref retNode);
                        startPos = searchPos;
                        return retNode;
                    }
                }
            }

            return null;
        }

		/// <summary>
		/// 取得if else复合语句节点
		/// </summary>
		static StatementNode GetIfElseStatementNode(FileParseInfo parse_info, ref CodePosition startPos)
		{
			StatementNode retNode = new StatementNode();
			retNode.Type = StatementNodeType.Compound_IfElse;
			CodePosition searchPos = new CodePosition(startPos);
			// 取得if条件表达式
			string expression = GetCompoundStatementExpression(parse_info, ref searchPos);
			if (string.Empty != expression)
			{
				retNode.expression = expression;
				// 取得if分支范围
				CodeScope scope = GetBranchScope(parse_info, ref searchPos);
				if (null != scope)
				{
					StatementNode ifBranch = new StatementNode();
					ifBranch.parent = retNode;
					ifBranch.expression = expression;
					ifBranch.Type = StatementNodeType.Branch_If;
					ifBranch.Scope = scope;
					// 递归解析分支语句块
					GetFuncBlockStruct(parse_info, ref ifBranch);

					retNode.Scope.Start = scope.Start;							// if分支的开始位置, 作为整个if else复合语句的起始位置
					retNode.childList.Add(ifBranch);

					CodePosition lastElseEnd = ifBranch.Scope.End;             // 最后一个else分支的结束位置, 作为整个if else复合语句的结束位置
					StatementNode elseBranch = GetNextElseBrachNode(parse_info, ref searchPos);
					while (null != elseBranch)
					{
						lastElseEnd = elseBranch.Scope.End;
						elseBranch.parent = retNode;
						// 递归解析分支语句块
						GetFuncBlockStruct(parse_info, ref elseBranch);

						retNode.childList.Add(elseBranch);
						if (StatementNodeType.Branch_Else == elseBranch.Type)
						{
							// 如果是"else"分支, 代表整个if复合语句结束
							break;
						}
						elseBranch = GetNextElseBrachNode(parse_info, ref searchPos);
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
		static StatementNode GetSwitchCaseStatementNode(FileParseInfo parse_info, ref CodePosition startPos)
		{
			StatementNode retNode = new StatementNode();
			retNode.Type = StatementNodeType.Compound_SwitchCase;
			CodePosition searchPos = new CodePosition(startPos);
			// 取得switch条件表达式
			string expression = GetCompoundStatementExpression(parse_info, ref searchPos);
			if (string.Empty != expression)
			{
				retNode.expression = expression;
				// 取得switch分支范围
				CodePosition oldPos = CommonProcess.PositionMoveNext(parse_info.CodeList, searchPos);	// 移到左{的位置
				CodeScope scope = GetBranchScope(parse_info, ref searchPos);
                searchPos = oldPos;
				if (null != scope)
				{
                    retNode.Scope = scope;
					searchPos = CommonProcess.PositionMoveNext(parse_info.CodeList, searchPos);          // 移到左{的下一个位置
					// 取得各case或default分支
					StatementNode caseBranch = GetNextCaseBranchNode(parse_info, ref searchPos);
					while (null != caseBranch)
					{
						caseBranch.parent = retNode;
						// 递归解析分支语句块
						GetFuncBlockStruct(parse_info, ref caseBranch);

						retNode.childList.Add(caseBranch);
						if (StatementNodeType.Branch_Default == caseBranch.Type)
						{
							// 如果是"default"分支, 代表整个switch case复合语句结束
							break;
						}
						caseBranch = GetNextCaseBranchNode(parse_info, ref searchPos);
					}
					startPos = CommonProcess.PositionMoveNext(parse_info.CodeList, retNode.Scope.End);
					return retNode;
				}
			}
			return null;
		}

		/// <summary>
		/// 取得下一个else/else if分支节点
		/// </summary>
		static StatementNode GetNextElseBrachNode(FileParseInfo parse_info, ref CodePosition startPos)
		{
			StatementNode retNode = new StatementNode();
			CodePosition searchPos = new CodePosition(startPos);
			CodePosition foundPos = new CodePosition(searchPos);
			CodeIdentifier nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
			if ("else" == nextIdtf.Text)
			{
				CodePosition oldPos = new CodePosition(searchPos);
				nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
				if ("if" == nextIdtf.Text)
				{
					// 表示这是一个"else if"分支
					// 取得分支表达式
					string expression = GetCompoundStatementExpression(parse_info, ref searchPos);
					if (string.Empty != expression)
					{
						retNode.expression = expression;
						// 确定分支范围
						CodeScope scope = GetBranchScope(parse_info, ref searchPos);
						if (null != scope)
						{
							retNode.Type = StatementNodeType.Branch_ElseIf;
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
					CodeScope scope = GetBranchScope(parse_info, ref searchPos);
					if (null != scope)
					{
						retNode.Type = StatementNodeType.Branch_Else;
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
		static StatementNode GetNextCaseBranchNode(FileParseInfo parse_info, ref CodePosition startPos)
		{
			StatementNode retNode = new StatementNode();
			CodePosition searchPos = new CodePosition(startPos);
			CodePosition foundPos = new CodePosition(searchPos);
            CodePosition oldPos = null;
			CodeIdentifier nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);

            // 定位"case"或"default"关键字的位置
            // 定位分号":"的位置
            oldPos = new CodePosition(foundPos);
			foundPos = CommonProcess.FindNextSpecIdentifier(":", parse_info.CodeList, searchPos);
            if (null != foundPos)
            {
				string caseStr = CommonProcess.LineStringCat(parse_info.CodeList, oldPos, foundPos);
                retNode.expression = caseStr;
				retNode.Type = GetNodeType(nextIdtf.Text);
                if (StatementNodeType.Invalid == retNode.Type)
                {
                    return null;
                }
				retNode.Scope.Start = CommonProcess.PositionMoveNext(parse_info.CodeList, foundPos);
                retNode.Scope.End = retNode.Scope.Start;
                // 取得整个case分支的范围: 逐一提取子语句, 直到遇到"break;"或者下一case/default分支开始
                // 注意也可能没有子语句
                while (true)
                {
					StatementNode sn = GetNextStatement(parse_info, ref searchPos, null);
                    if (null != sn)
                    {
                        //retNode.childList.Add(sn);
                        retNode.Scope.End = sn.Scope.End;
                        oldPos = new CodePosition(searchPos);
						nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
                        searchPos = oldPos;
                        // 分支结束的判断
						if ("case" == nextIdtf.Text
							|| "default" == nextIdtf.Text
							|| "}" == nextIdtf.Text)
                        {
                            // 移到前一位
							searchPos = CommonProcess.PositionMovePrevious(parse_info.CodeList, foundPos);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                startPos = searchPos;
                return retNode;
            }

            return null;
		}

		/// <summary>
		/// 取得复合语句表达式
		/// </summary>
		static string GetCompoundStatementExpression(FileParseInfo parse_info, ref CodePosition startPos)
		{
			CodePosition searchPos = new CodePosition(startPos);
			CodePosition foundPos = new CodePosition(searchPos);

			CodeIdentifier nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
			if ("(" == nextIdtf.Text)
			{
				CodePosition leftBracePos = new CodePosition(foundPos);
				searchPos = CommonProcess.PositionMoveNext(parse_info.CodeList, leftBracePos);
				CodePosition rightBracePos = CommonProcess.FindNextMatchSymbol(parse_info, ref searchPos, ')');
				if (null != rightBracePos)
				{
					startPos = searchPos;
					string exp = CommonProcess.LineStringCat(parse_info.CodeList, leftBracePos, rightBracePos);
					return exp;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// 取得switch case语句中case分支的表达式
		/// </summary>
		static string GetCaseBranchExpression(FileParseInfo parse_info, ref CodePosition startPos)
		{
			CodePosition searchPos = new CodePosition(startPos);
			CodePosition foundPos = new CodePosition(searchPos);

			// 找到冒号":"的位置
			foundPos = CommonProcess.FindNextSpecIdentifier(":", parse_info.CodeList, searchPos);
			return null;
		}

		/// <summary>
		/// 取得下一个语句块的范围(起止位置)
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="startPos"></param>
		/// <returns></returns>
		static CodeScope GetBranchScope(FileParseInfo parse_info, ref CodePosition startPos)
		{
			CodePosition searchPos = new CodePosition(startPos);
			CodePosition foundPos = new CodePosition(searchPos);

			CodeIdentifier nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
			if ("{" == nextIdtf.Text)
			{
				// 通常语句块会以花括号括起来
				CodePosition leftBrace = new CodePosition(foundPos);
				// 找到配对的花括号
				CodePosition rightBrace = CommonProcess.FindNextMatchSymbol(parse_info, ref searchPos, '}');
				if (null != rightBrace)
				{
					startPos = searchPos;
					CodeScope scope = new CodeScope(leftBrace, rightBrace);
					scope.Start = leftBrace;
					scope.End = rightBrace;
					return scope;
				}
			}
			else
			{
				// 但是如果没有遇到花括号, 那么可能是语句块只有一条语句(※可能是简单语句也可能是复合语句)
				// 提取一条语句
				StatementNode sn = GetNextStatement(parse_info, ref startPos, null);
                if (null != sn)
                {
					CodeScope scope = sn.Scope;
                    return scope;
                }
			}

			return null;
		}

		/// <summary>
		/// 根据关键字取得复合语句节点类型
		/// </summary>
		/// <param varName="keyWord"></param>
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

                case "case":
                    retType = StatementNodeType.Branch_Case;
                    break;
                case "default":
                    retType = StatementNodeType.Branch_Default;
                    break;
				default:
					break;
			}
			return retType;
		}

	}
}
