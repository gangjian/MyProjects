using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Robot
{
	public partial class C_FUNC_LOCATOR
	{
		public static STATEMENT_NODE FuncLocatorStart2(FILE_PARSE_INFO src_parse_info, string func_name)
		{
			FUNCTION_PARSE_INFO funcInfo = FILE_PARSE_INFO.SearchFunctionInfoList(func_name, src_parse_info.FunDefineList);
			if (null == funcInfo)
			{
				return null;
			}
			STATEMENT_NODE root = new STATEMENT_NODE();
			root.Type = STATEMENT_TYPE.Root;
			root.Scope = funcInfo.Scope;
			GetFunctionStatmentsNodeTree(src_parse_info, ref root);
			SetStatementNodeTreeStepMark(root);											// 设置语句标号
			return root;
		}

		/// <summary>
		/// 解析一个代码块,提取语句树结构
		/// </summary>
		public static void GetFunctionStatmentsNodeTree(FILE_PARSE_INFO parse_info, ref STATEMENT_NODE root)
		{
			CODE_POSITION searchPos = new CODE_POSITION(root.Scope.Start);
			// 如果开头第一个字符是左花括号"{", 先要移到下一个位置开始检索
			if ('{' == parse_info.CodeList[searchPos.RowNum][searchPos.ColNum])
			{
				searchPos = COMN_PROC.PositionMoveNext(parse_info.CodeList, searchPos);
			}

			// 循环提取每一条语句(简单语句或者复合语句)
            STATEMENT_NODE statementNode = null;
            while (true)
			{
				statementNode = GetNextStatement(parse_info, ref searchPos, root.Scope.End);
                if (null == statementNode)
	            {
                    break;
	            }
				statementNode.ParentNode = root;
                root.ChildNodeList.Add(statementNode);
			}
		}

		/// <summary>
		/// 从当前位置提取一条语句(简单语句或者是复合语句)
		/// </summary>
		static STATEMENT_NODE GetNextStatement(	FILE_PARSE_INFO parse_info,
											    ref CODE_POSITION startPos,
												CODE_POSITION endPos)
		{
            STATEMENT_NODE retNode = new STATEMENT_NODE();
			CODE_POSITION foundPos = null;
			CODE_IDENTIFIER nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref startPos, out foundPos);
			CODE_POSITION searchPos = new CODE_POSITION(startPos);
			startPos = new CODE_POSITION(foundPos);

            if (null != endPos
                && COMN_PROC.PositionCompare(searchPos, endPos) >= 0)
			{
				// 对于检索范围超出区块结束位置的判断
				return null;
			}
			// 复合语句
			if (STATEMENT_TYPE.Invalid != GetNodeType(nextIdtf.Text))
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
				startPos = COMN_PROC.PositionMoveNext(parse_info.CodeList, retNode.Scope.End);
				return retNode;
			}
		}

		/// <summary>
		/// 取得复合语句情报
		/// </summary>
        static STATEMENT_NODE GetCompondStatementNode(string keyWord, FILE_PARSE_INFO parse_info, ref CODE_POSITION startPos)
		{
			STATEMENT_NODE retNode = null;
			STATEMENT_TYPE type = GetNodeType(keyWord);
			CODE_POSITION searchPos = new CODE_POSITION(startPos);

			switch (type)
			{
				case STATEMENT_TYPE.Complex_IfElse:									// if else
					retNode = GetIfElseStatementNode(parse_info, ref searchPos);
					startPos = searchPos;
					break;
				case STATEMENT_TYPE.Complex_SwitchCase:								// switch case
					retNode = GetSwitchCaseStatementNode(parse_info, ref searchPos);
					startPos = searchPos;
					break;
				case STATEMENT_TYPE.Complex_While:									// while
				case STATEMENT_TYPE.Complex_For:										// for
					retNode = GetForOrWhileStatementNode(type, parse_info, ref searchPos);
					startPos = searchPos;
					break;
				case STATEMENT_TYPE.Complex_DoWhile:									// do while
					retNode = GetDoWhileStatementNode(parse_info, ref searchPos);
					startPos = searchPos;
					break;
				case STATEMENT_TYPE.Complex_GoTo:									// go to(未对应)
					System.Diagnostics.Trace.Assert(false);
					break;
				case STATEMENT_TYPE.Complex_Block:									// "{"和"}"括起来的语句块
					break;
				default:
					System.Diagnostics.Trace.Assert(false);								// 异常情况
					break;
			}
			return retNode;
		}

        /// <summary>
        /// 取得简单语句的语句节点对象
        /// </summary>
		static STATEMENT_NODE GetSimpleStatementNode(FILE_PARSE_INFO parse_info, ref CODE_POSITION startPos, CODE_POSITION foundPos)
		{
			STATEMENT_NODE retNode = new STATEMENT_NODE();
			CODE_POSITION searchPos = new CODE_POSITION(foundPos);
			CODE_POSITION oldPos = new CODE_POSITION(foundPos);
			// 找到语句结束,也就是分号的位置
			foundPos = COMN_PROC.FindNextSpecIdentifier(";", parse_info.CodeList, searchPos);
			if (null != foundPos)
			{
				string statementStr = COMN_PROC.LineStringCat(parse_info.CodeList, oldPos, foundPos);
				retNode.Scope = new CODE_SCOPE(oldPos, foundPos);
				retNode.Type = STATEMENT_TYPE.Simple;
				retNode.ExpressionStr = statementStr;
				startPos = searchPos;
				return retNode;
			}
			return null;
		}

		/// <summary>
		/// 取得for/while循环语句的语句节点对象
		/// </summary>
		static STATEMENT_NODE GetForOrWhileStatementNode(STATEMENT_TYPE type, FILE_PARSE_INFO parse_info, ref CODE_POSITION startPos)
		{
			STATEMENT_NODE retNode = new STATEMENT_NODE();
			retNode.Type = type;
			CODE_POSITION searchPos = new CODE_POSITION(startPos);
			// 取得循环表达式
			string expression = GetComplexStatementExpression(parse_info, ref searchPos);
			if (string.Empty != expression)
			{
				retNode.ExpressionStr = expression;
				// 取得分支范围
				CODE_SCOPE scope = GetBranchScope(parse_info, ref searchPos);
				if (null != scope)
				{
					startPos = searchPos;
					retNode.Scope = scope;
					// 递归解析语句块
					GetFunctionStatmentsNodeTree(parse_info, ref retNode);
					return retNode;
				}
			}
			return null;
		}

        /// <summary>
        /// 取得do-while循环语句的语句节点对象
        /// </summary>
		static STATEMENT_NODE GetDoWhileStatementNode(FILE_PARSE_INFO parse_info, ref CODE_POSITION startPos)
        {
            STATEMENT_NODE retNode = new STATEMENT_NODE();
            retNode.Type = STATEMENT_TYPE.Complex_DoWhile;
            CODE_POSITION searchPos = new CODE_POSITION(startPos);
            // 取得分支范围
			CODE_SCOPE scope = GetBranchScope(parse_info, ref searchPos);
            if (null != scope)
            {
                retNode.Scope = scope;
				searchPos = COMN_PROC.PositionMoveNext(parse_info.CodeList, scope.End);
                CODE_POSITION foundPos;
				CODE_IDENTIFIER nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
				if ("while" == nextIdtf.Text)
                {
					string expression = GetComplexStatementExpression(parse_info, ref searchPos);
					nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
                    if (string.Empty != expression
						&& ";" == nextIdtf.Text)
                    {
                        retNode.ExpressionStr = expression;
                        // 递归解析语句块
						GetFunctionStatmentsNodeTree(parse_info, ref retNode);
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
		static STATEMENT_NODE GetIfElseStatementNode(FILE_PARSE_INFO parse_info, ref CODE_POSITION startPos)
		{
			STATEMENT_NODE retNode = new STATEMENT_NODE();
			retNode.Type = STATEMENT_TYPE.Complex_IfElse;
			CODE_POSITION searchPos = new CODE_POSITION(startPos);
			// 取得if条件表达式
			string expression = GetComplexStatementExpression(parse_info, ref searchPos);
			if (string.Empty != expression)
			{
				retNode.ExpressionStr = expression;
				// 取得if分支范围
				CODE_SCOPE scope = GetBranchScope(parse_info, ref searchPos);
				if (null != scope)
				{
					STATEMENT_NODE ifBranch = new STATEMENT_NODE();
					ifBranch.ParentNode = retNode;
					ifBranch.ExpressionStr = expression;
					ifBranch.Type = STATEMENT_TYPE.Branch_If;
					ifBranch.Scope = scope;
					// 递归解析分支语句块
					GetFunctionStatmentsNodeTree(parse_info, ref ifBranch);

					CODE_POSITION firstIfStart = scope.Start;							// if分支的开始位置, 作为整个if else复合语句的起始位置
					retNode.ChildNodeList.Add(ifBranch);

					CODE_POSITION lastElseEnd = ifBranch.Scope.End;						// 最后一个else分支的结束位置, 作为整个if else复合语句的结束位置
					STATEMENT_NODE elseBranch = GetNextElseBrachNode(parse_info, ref searchPos);
					while (null != elseBranch)
					{
						lastElseEnd = elseBranch.Scope.End;
						elseBranch.ParentNode = retNode;
						// 递归解析分支语句块
						GetFunctionStatmentsNodeTree(parse_info, ref elseBranch);

						retNode.ChildNodeList.Add(elseBranch);
						if (STATEMENT_TYPE.Branch_Else == elseBranch.Type)
						{
							// 如果是"else"分支, 代表整个if复合语句结束
							break;
						}
						elseBranch = GetNextElseBrachNode(parse_info, ref searchPos);
					}
					startPos = searchPos;
					retNode.Scope = new CODE_SCOPE(firstIfStart, lastElseEnd);
					return retNode;
				}
			}
			return null;
		}

		/// <summary>
		/// 取得switch case语句节点
		/// </summary>
		static STATEMENT_NODE GetSwitchCaseStatementNode(FILE_PARSE_INFO parse_info, ref CODE_POSITION startPos)
		{
			STATEMENT_NODE retNode = new STATEMENT_NODE();
			retNode.Type = STATEMENT_TYPE.Complex_SwitchCase;
			CODE_POSITION searchPos = new CODE_POSITION(startPos);
			// 取得switch条件表达式
			string expression = GetComplexStatementExpression(parse_info, ref searchPos);
			if (string.Empty != expression)
			{
				retNode.ExpressionStr = expression;
				// 取得switch分支范围
				CODE_POSITION oldPos = COMN_PROC.PositionMoveNext(parse_info.CodeList, searchPos);	// 移到左{的位置
				CODE_SCOPE scope = GetBranchScope(parse_info, ref searchPos);
                searchPos = oldPos;
				if (null != scope)
				{
                    retNode.Scope = scope;
					searchPos = COMN_PROC.PositionMoveNext(parse_info.CodeList, searchPos);			// 移到左{的下一个位置
					// 取得各case或default分支
					STATEMENT_NODE caseBranch = GetNextCaseBranchNode(parse_info, ref searchPos);
					while (null != caseBranch)
					{
						caseBranch.ParentNode = retNode;
						// 递归解析分支语句块
						GetFunctionStatmentsNodeTree(parse_info, ref caseBranch);

						retNode.ChildNodeList.Add(caseBranch);
						if (STATEMENT_TYPE.Branch_Default == caseBranch.Type)
						{
							// 如果是"default"分支, 代表整个switch case复合语句结束
							break;
						}
						caseBranch = GetNextCaseBranchNode(parse_info, ref searchPos);
					}
					startPos = COMN_PROC.PositionMoveNext(parse_info.CodeList, retNode.Scope.End);
					return retNode;
				}
			}
			return null;
		}

		/// <summary>
		/// 取得下一个else/else if分支节点
		/// </summary>
		static STATEMENT_NODE GetNextElseBrachNode(FILE_PARSE_INFO parse_info, ref CODE_POSITION startPos)
		{
			STATEMENT_NODE retNode = new STATEMENT_NODE();
			CODE_POSITION searchPos = new CODE_POSITION(startPos);
			CODE_POSITION foundPos = new CODE_POSITION(searchPos);
			CODE_IDENTIFIER nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
			if ("else" == nextIdtf.Text)
			{
				CODE_POSITION oldPos = new CODE_POSITION(searchPos);
				nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
				if ("if" == nextIdtf.Text)
				{
					// 表示这是一个"else if"分支
					// 取得分支表达式
					string expression = GetComplexStatementExpression(parse_info, ref searchPos);
					if (string.Empty != expression)
					{
						retNode.ExpressionStr = expression;
						// 确定分支范围
						CODE_SCOPE scope = GetBranchScope(parse_info, ref searchPos);
						if (null != scope)
						{
							retNode.Type = STATEMENT_TYPE.Branch_ElseIf;
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
					CODE_SCOPE scope = GetBranchScope(parse_info, ref searchPos);
					if (null != scope)
					{
						retNode.Type = STATEMENT_TYPE.Branch_Else;
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
		static STATEMENT_NODE GetNextCaseBranchNode(FILE_PARSE_INFO parse_info, ref CODE_POSITION startPos)
		{
			STATEMENT_NODE retNode = new STATEMENT_NODE();
			CODE_POSITION searchPos = new CODE_POSITION(startPos);
			CODE_POSITION foundPos = new CODE_POSITION(searchPos);
            CODE_POSITION oldPos = null;
			CODE_IDENTIFIER nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);

            // 定位"case"或"default"关键字的位置
            // 定位分号":"的位置
            oldPos = new CODE_POSITION(foundPos);
			foundPos = COMN_PROC.FindNextSpecIdentifier(":", parse_info.CodeList, searchPos);
            if (null != foundPos)
            {
				string caseStr = COMN_PROC.LineStringCat(parse_info.CodeList, oldPos, foundPos);
                retNode.ExpressionStr = caseStr;
				retNode.Type = GetNodeType(nextIdtf.Text);
                if (STATEMENT_TYPE.Invalid == retNode.Type)
                {
                    return null;
                }
				retNode.Scope.Start = COMN_PROC.PositionMoveNext(parse_info.CodeList, foundPos);
                retNode.Scope.End = retNode.Scope.Start;
                // 取得整个case分支的范围: 逐一提取子语句, 直到遇到"break;"或者下一case/default分支开始
                // 注意也可能没有子语句
                while (true)
                {
					STATEMENT_NODE sn = GetNextStatement(parse_info, ref searchPos, null);
                    if (null != sn)
                    {
                        //retNode.childList.Add(sn);
                        retNode.Scope.End = sn.Scope.End;
                        oldPos = new CODE_POSITION(searchPos);
						nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
                        searchPos = oldPos;
                        // 分支结束的判断
						if ("case" == nextIdtf.Text
							|| "default" == nextIdtf.Text
							|| "}" == nextIdtf.Text)
                        {
                            // 移到前一位
							searchPos = COMN_PROC.PositionMovePrevious(parse_info.CodeList, foundPos);
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
		static string GetComplexStatementExpression(FILE_PARSE_INFO parse_info, ref CODE_POSITION startPos)
		{
			CODE_POSITION searchPos = new CODE_POSITION(startPos);
			CODE_POSITION foundPos = new CODE_POSITION(searchPos);
			CODE_IDENTIFIER nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
			if ("(" == nextIdtf.Text)
			{
				CODE_POSITION leftBracePos = new CODE_POSITION(foundPos);
				searchPos = COMN_PROC.PositionMoveNext(parse_info.CodeList, leftBracePos);
				CODE_POSITION rightBracePos = COMN_PROC.FindNextMatchSymbol(parse_info, ref searchPos, ')');
				if (null != rightBracePos)
				{
					startPos = searchPos;
					string exp = COMN_PROC.LineStringCat(parse_info.CodeList, leftBracePos, rightBracePos);
					return exp;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// 取得下一个语句块的范围(起止位置)
		/// </summary>
		static CODE_SCOPE GetBranchScope(FILE_PARSE_INFO parse_info, ref CODE_POSITION startPos)
		{
			CODE_POSITION searchPos = new CODE_POSITION(startPos);
			CODE_POSITION foundPos = new CODE_POSITION(searchPos);
			CODE_IDENTIFIER nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
			if ("{" == nextIdtf.Text)
			{
				// 通常语句块会以花括号括起来
				CODE_POSITION leftBrace = new CODE_POSITION(foundPos);
				// 找到配对的花括号
				CODE_POSITION rightBrace = COMN_PROC.FindNextMatchSymbol(parse_info, ref searchPos, '}');
				if (null != rightBrace)
				{
					startPos = searchPos;
					CODE_SCOPE scope = new CODE_SCOPE(leftBrace, rightBrace);
					scope.Start = leftBrace;
					scope.End = rightBrace;
					return scope;
				}
			}
			else
			{
				// 但是如果没有遇到花括号, 那么可能是语句块只有一条语句(※可能是简单语句也可能是复合语句)
				// 提取一条语句
				STATEMENT_NODE sn = GetNextStatement(parse_info, ref startPos, null);
                if (null != sn)
                {
					CODE_SCOPE scope = sn.Scope;
                    return scope;
                }
			}
			return null;
		}

		/// <summary>
		/// 根据关键字取得复合语句节点类型
		/// </summary>
		static STATEMENT_TYPE GetNodeType(string keyWord)
		{
			STATEMENT_TYPE retType = STATEMENT_TYPE.Invalid;
			switch (keyWord)
			{
				case "if":
					retType = STATEMENT_TYPE.Complex_IfElse;
					break;
				case "{":
					retType = STATEMENT_TYPE.Complex_Block;
					break;
				case "do":
					retType = STATEMENT_TYPE.Complex_DoWhile;
					break;
				case "for":
					retType = STATEMENT_TYPE.Complex_For;
					break;
				case "goto":
					retType = STATEMENT_TYPE.Complex_GoTo;
					break;
				case "switch":
					retType = STATEMENT_TYPE.Complex_SwitchCase;
					break;
				case "while":
					retType = STATEMENT_TYPE.Complex_While;
					break;

                case "case":
                    retType = STATEMENT_TYPE.Branch_Case;
                    break;
                case "default":
                    retType = STATEMENT_TYPE.Branch_Default;
                    break;
				default:
					break;
			}
			return retType;
		}

		/// <summary>
		/// 设置语句树各节点的标号
		/// </summary>
		static void SetStatementNodeTreeStepMark(STATEMENT_NODE root_node)
		{
			string parentMarkStr = root_node.StepMarkStr;
			if (!string.IsNullOrEmpty(parentMarkStr))
			{
				parentMarkStr += ",";
			}
			for (int i = 0; i < root_node.ChildNodeList.Count; i++)
			{
				string childMarkStr = string.Empty;
				switch (root_node.ChildNodeList[i].Type)
				{
					case STATEMENT_TYPE.Root:
					case STATEMENT_TYPE.Simple:
					case STATEMENT_TYPE.Complex_IfElse:
					case STATEMENT_TYPE.Complex_SwitchCase:
					case STATEMENT_TYPE.Complex_While:
					case STATEMENT_TYPE.Complex_For:
					case STATEMENT_TYPE.Complex_DoWhile:
					case STATEMENT_TYPE.Complex_GoTo:
					case STATEMENT_TYPE.Complex_Block:
						childMarkStr = (i + 1).ToString();
						break;
					case STATEMENT_TYPE.Branch_If:
					case STATEMENT_TYPE.Branch_ElseIf:
					case STATEMENT_TYPE.Branch_Else:
					case STATEMENT_TYPE.Branch_Case:
					case STATEMENT_TYPE.Branch_Default:
						childMarkStr = "-" + (i + 1).ToString();
						break;
					default:
						break;
				}
				root_node.ChildNodeList[i].StepMarkStr = parentMarkStr + childMarkStr;
				SetStatementNodeTreeStepMark(root_node.ChildNodeList[i]);
			}
		}
	}
}
