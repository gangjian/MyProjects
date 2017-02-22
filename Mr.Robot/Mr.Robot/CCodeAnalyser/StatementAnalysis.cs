using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Robot
{
	public static partial class StatementAnalysis
	{
		/// <summary>
		/// 语句分析
		/// </summary>
        public static FuncAnalysisContext FunctionStatementsAnalysis(StatementNode root,
																	 FileParseInfo p_result)
		{
            FuncAnalysisContext fCtx = new FuncAnalysisContext();					// (变量)解析上下文
			// 顺次解析各条语句
			foreach (StatementNode childNode in root.childList)
			{
				StatementAnalyze(childNode, p_result, fCtx);
			}

            return fCtx;
		}

		public static void StatementAnalyze(StatementNode s_node,
											FileParseInfo parse_info,
                                            FuncAnalysisContext func_ctx)
		{
			switch (s_node.Type)
			{
				case StatementNodeType.Simple:
					// 取得完整的语句内容
					List<string> codeList = parse_info.CodeList;
					string statementStr = GetStatementStr(codeList, s_node.Scope);
					SimpleStatementAnalyze(statementStr, parse_info, func_ctx);
					break;
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
		}

		/// <summary>
		/// 简单语句分析(函数内)
		/// </summary>
		public static void SimpleStatementAnalyze(	string statement_str,
													FileParseInfo parse_info,
													FuncAnalysisContext func_ctx)
		{
			// 按顺序提取出语句各组成部分: 运算数(Operand)和运算符(Operator)
			List<StatementComponent> componentList = GetComponents(statement_str, parse_info);
			if (statement_str.StartsWith("typedef"))
			{
				TypeDefProc(componentList, parse_info, func_ctx);
			}
			else
			{
				ExpressionAnalysis(componentList, parse_info, func_ctx);
			}
		}

		public static string GetStatementStr(List<string> code_list, CodeScope code_scope)
		{
			return CommonProcess.LineStringCat(code_list, code_scope.Start, code_scope.End).Trim();
		}

        public static void ExpressionAnalysis(	List<StatementComponent> componentList,
												FileParseInfo parse_info,
												FuncAnalysisContext func_ctx)
        {
            // 提取含义分组
			List<MeaningGroup> meaningGroupList = GetMeaningGroups(componentList, parse_info, func_ctx);

            // 含义分组解析
			MeaningGroupsAnalysis(meaningGroupList, parse_info, func_ctx);
        }

		/// <summary>
		/// 取得语句内各基本成分(运算数或者是运算符)
		/// </summary>
		public static List<StatementComponent> GetComponents(string statementStr,
                                                             FileParseInfo parse_info,
															 bool replace_empty_macro_def = true)
        {
			// 去掉结尾的分号
			if (statementStr.EndsWith(";"))
			{
				statementStr = statementStr.Remove(statementStr.Length - 1).Trim();
			}
            List<StatementComponent> componentList = new List<StatementComponent>();
            int offset = 0;
            do
            {
                // 提取语句的各个组成部分(操作数或者是操作符)
                StatementComponent cpnt = GetOneComponent(ref statementStr, ref offset, parse_info, replace_empty_macro_def);
                if (string.Empty == cpnt.Text)
                {
                    // 语句结束
                    break;
                }
                else
                {
                    componentList.Add(cpnt);
                }
            } while (true);

            return componentList;
        }

        public static List<MeaningGroup> GetMeaningGroups(	List<StatementComponent> componentList,
															FileParseInfo parse_info,
															FuncAnalysisContext func_ctx)
		{
            List<MeaningGroup> meaningGroupList;
            while (true)
            {
                // (1). 首先对语句所有组成部分进行结构分组, 每个组代表一个独立完整的语义结构
				meaningGroupList = GetMeaningGroupList(componentList, parse_info, func_ctx);
                if (1 == meaningGroupList.Count
                    && "(" == meaningGroupList[0].ComponentList.First().Text
                    && ")" == meaningGroupList[0].ComponentList.Last().Text)
                {
                    componentList.RemoveAt(0);
                    componentList.RemoveAt(componentList.Count - 1);
                    continue;
                }
                else
                {
                    break;
                }
            }

			// 如果以类型开头那应该是变量定义;
			// 然后找有没有赋值运算符(优先级14), ++/--也可能表示有左值
			// 是表达式的话要进一步递归解析
			int a, b, c = 20;
			(a) = b = c + 3;

            return meaningGroupList;
		}

		/// <summary>
		/// 对语句所有构成成分进行结构分组
		/// </summary>
		static List<MeaningGroup> GetMeaningGroupList(	List<StatementComponent> componentList,
														FileParseInfo parse_info,
														FuncAnalysisContext func_ctx)
		{
			List<MeaningGroup> groupList = new List<MeaningGroup>();
			int idx = 0;
			while (true) 
			{
                if (idx >= componentList.Count)
                {
                    break;
                }
				MeaningGroup newGroup = GetOneMeaningGroup(componentList, ref idx, groupList, parse_info, func_ctx);
				if (0 != newGroup.ComponentList.Count)
				{
					groupList.Add(newGroup);
				}
				else
				{
					break;
				}
			}
			return groupList;
		}

		/// <summary>
		/// 取得一个构成分组
		/// </summary>
		static MeaningGroup GetOneMeaningGroup(	List<StatementComponent> componentList,
												ref int idx,
												List<MeaningGroup> groupList,
												FileParseInfo parse_info,
												FuncAnalysisContext func_ctx)
		{
			MeaningGroup retGroup = null;
			// 是类型名?
			if (null != (retGroup = GetVarTypeGroup(componentList, ref idx, parse_info)))
			{
				return retGroup;
			}
			// 基本类型名以外的关键字?
			else if (null != (retGroup = GetSingleKeywordGroup(componentList, ref idx, parse_info)))
			{
				return retGroup;
			}
			// 是变量名?
			else if (null != (retGroup = GetVarNameGroup(componentList, ref idx, groupList, parse_info, func_ctx)))
			{
				return retGroup;
			}
			// 是函数调用?
			else if (null != (retGroup = GetFunctionCallingGroup(componentList, ref idx, parse_info)))
			{
				return retGroup;
			}
			// 是表达式? 或者是强制类型转换运算符
			else if (null != (retGroup = GetExpressionGroup(componentList, ref idx, parse_info)))
			{
				return retGroup;
			}
			// "{和}括起的代码块"
			else if (null != (retGroup = GetCodeBlockGroup(componentList, ref idx, parse_info)))
			{
				return retGroup;
			}
			// 双引号""括起的字符串
			else if (null != (retGroup = GetStringBlockGroup(componentList, ref idx, parse_info)))
			{
				return retGroup;
			}
			// 单引号''括起的单个字符
			else if (null != (retGroup = GetCharBlockGroup(componentList, ref idx, parse_info)))
			{
				return retGroup;
			}
			// 是运算符
			else if (null != (retGroup = GetOperatorGroup(componentList, ref idx)))
			{
				return retGroup;
			}
            else if (CommonProcess.IsStandardIdentifier(componentList[idx].Text))
			{
				retGroup = new MeaningGroup();
				if (componentList[idx].Type == StatementComponentType.Identifier)
				{
					retGroup.Type = MeaningGroupType.Identifier;
				}
				else
				{
					retGroup.Type = MeaningGroupType.Unknown;
				}
				retGroup.ComponentList.Add(componentList[idx]);
				retGroup.Text = componentList[idx].Text;
				idx += 1;
				return retGroup;
			}
			else if (IsConstantNumber(componentList[idx].Text))
			{
				retGroup = new MeaningGroup();
				retGroup.Type = MeaningGroupType.Constant;
				retGroup.ComponentList.Add(componentList[idx]);
				retGroup.Text = componentList[idx].Text;
				idx += 1;
				return retGroup;
			}
			else
			{
				// What the hell is this?
				System.Windows.Forms.MessageBox.Show("=== GetOneMeaningGroup === Assert!!!");
				System.Diagnostics.Trace.Assert(false);
                return null;
			}
		}

		/// <summary>
		/// 取得括号括起来的一组操作数
		/// </summary>
		/// <returns></returns>
		static List<StatementComponent> GetBraceComponents(List<StatementComponent> componentList, ref int idx)
		{
			StatementComponent cpnt = componentList[idx];
			string matchOp = string.Empty;
			// 
			if ("(" == cpnt.Text)
			{
				matchOp = ")";
			}
			else if ("[" == cpnt.Text)
			{
				matchOp = "]";
			}
			else if ("{" == cpnt.Text)
			{
				matchOp = "}";
			}
			if (string.Empty == matchOp)
			{
				return null;
			}
			List<StatementComponent> retList = new List<StatementComponent>();
			retList.Add(cpnt);
			int matchCount = 1;
			for (int j = idx + 1; j < componentList.Count; j++)
			{
				idx = j;
				retList.Add(componentList[j]);
				if (cpnt.Text == componentList[j].Text)
				{
					matchCount += 1;
				}
				else if (matchOp == componentList[j].Text)
				{
					matchCount -= 1;
				}
				if (0 == matchCount)
				{
					break;
				}
			}
			return retList;
		}

		/// <summary>
		/// 从语句中提取出一个操作数/操作符
		/// </summary>
		static StatementComponent GetOneComponent(ref string statementStr,
												  ref int offset,
												  FileParseInfo parse_info,
												  bool replace_empty_macro_def)
		{
			string idStr = null;
			int offset_old = -1;
			StatementComponent retSC = new StatementComponent();
			while (true)
			{
				offset_old = offset;
				idStr = CommonProcess.GetNextIdentifier2(statementStr, ref offset);
				if (null == idStr)
				{
					break;
				}
				else if (IsConstantNumber(idStr))
				{
                    retSC.Type = StatementComponentType.ConstantNumber;
                    retSC.Text = idStr;
					break;														        // 数字常量
				}
				else if (IsStringOrChar(idStr, statementStr, ref offset))
				{
					break;														        // 字符或者字符串
				}
                else if (CommonProcess.IsStandardIdentifier(idStr))						// 标准标识符
				{
					// 如果包含宏, 首先要进行宏展开
					if (null != parse_info
						&& MacroDetectAndExpand_Statement(idStr, ref statementStr, offset, parse_info, replace_empty_macro_def))
					{
						offset = offset_old;
						continue;
					}
					else
					{
						retSC = new StatementComponent(idStr);
						retSC.Type = StatementComponentType.Identifier;
						break;
					}
				}
				else if (IsOperator(idStr, statementStr, ref offset, ref retSC))
				{
					break;
				}
				else
				{
					retSC = new StatementComponent(idStr);
					break;
				}
			}

			return retSC;
		}

		/// <summary>
		/// 函数内的宏展开 TODO:以后考虑重构跟MacroDetectAndExpand_File合并
		/// </summary>
		public static bool MacroDetectAndExpand_Statement(	string idStr,
															ref string statementStr,
															int offset,
															FileParseInfo parse_info,
															bool replace_empty_macro_def)
        {
			// 遍历查找宏名
			MacroDefineInfo mdi = parse_info.FindMacroDefInfo(idStr);
			if (null != mdi)
			{
				if (string.IsNullOrEmpty(mdi.Value)
					&& false == replace_empty_macro_def)
				{
				}
				else
				{
					string macroName = mdi.Name;
					string replaceStr = mdi.Value;
					// 判断有无带参数
					if (0 != mdi.ParaList.Count)
					{
						// 取得宏参数
						string paraStr = CommonProcess.GetNextIdentifier2(statementStr, ref offset);
						if ("(" != paraStr)
						{
							//CommonProcess.ErrReport();
							return false;
						}
						int leftBracket = offset;
						int rightBracket = statementStr.Substring(offset).IndexOf(')');
						if (-1 == rightBracket)
						{
							CommonProcess.ErrReport();
							return false;
						}
						paraStr = statementStr.Substring(leftBracket + 1, rightBracket - 1).Trim();
						macroName += statementStr.Substring(leftBracket, rightBracket + 1);
						string[] realParas = paraStr.Split(',');
						// 然后用实参去替换宏值里的形参
						int idx = 0;
						foreach (string rp in realParas)
						{
							if (string.Empty == rp)
							{
								// 参数有可能为空, 即没有参数, 只有一对空的括号里面什么参数也不带
								continue;
							}
							replaceStr = CommonProcess.WholeWordSReplace(replaceStr, mdi.ParaList[idx], rp);
							//replaceStr = replaceStr.Replace(mdi.ParaList[idx], rp);
							idx++;
						}
					}
					// 应对宏里面出现的"##"
					string[] seps = { "##" };
					string[] arr = replaceStr.Split(seps, StringSplitOptions.None);
					if (arr.Length > 1)
					{
						string newStr = "";
						foreach (string sepStr in arr)
						{
							newStr += sepStr.Trim();
						}
						replaceStr = newStr;
					}
					// 单个"#"转成字串的情况暂未对应, 以后遇到再说, 先出个error report作为保护
					if (replaceStr.Contains('#'))
					{
						CommonProcess.ErrReport();
						return false;
					}
					// 用宏值去替换原来的宏名(宏展开)
					if (idStr == replaceStr)
					{
						return false;
					}
					int macroIdx = offset - idStr.Length;
					statementStr = statementStr.Remove(macroIdx, idStr.Length);
					statementStr = statementStr.Insert(macroIdx, replaceStr);
					return true;
				}
			}
			return false;
        }

		/// <summary>
		/// 判断是否是运算符
		/// </summary>
		static bool IsOperator(string idStr, string statementStr, ref int offset, ref StatementComponent component)
		{
			if (1 != idStr.Length)
			{
				return false;
			}
			component.Type = StatementComponentType.Operator;
			int startOffset = offset;
            string nextIdStr = string.Empty;
            offset += 1;
            if (offset != statementStr.Length)
            {                                                                           // 不是本语句的末尾,取得下一个位置的字符
                nextIdStr = statementStr.Substring(offset, 1);
            }
			switch (idStr)
			{
				case "(":
				case ")":
				case "[":
				case "]":
				case ".":
					component.Text = idStr;
					component.Priority = 1;
					break;
				case "=":
					if (nextIdStr == idStr)
					{
						// "==" : 等于
						component.Text = idStr + nextIdStr;
						component.Priority = 6;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// "=" : 赋值
						component.Text = idStr;
						component.Priority = 10;
						component.OperandCount = 2;
					}
					break;
				case "+":
				case "-":
					if (nextIdStr == idStr)
					{
						// "++", "--" : 自增, 自减
						component.Text = idStr + nextIdStr;
						component.Priority = 2;
						component.OperandCount = 1;
						offset += 1;
					}
					else if ("=" == nextIdStr)
					{
						// "+=", "-=" : 加减运算赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 10;
						component.OperandCount = 2;
						offset += 1;
					}
					else if ( "-" == idStr && ">" == nextIdStr)
					{
						// "->" : 指针成员
						component.Text = idStr + nextIdStr;
						component.Priority = 1;
						offset += 1;
					}
					else
					{
						// "+", "-" : 加, 减
						component.Text = idStr;
						component.Priority = 4;									        // 不确定, 也可能是单目运算符的正负号(优先级为2)
						component.OperandCount = 2;
					}
					break;
				case "*":														        // 不确定, 可能是乘号, 也可能是指针运算符
				case "/":
					if ("=" == nextIdStr)
					{
						// "*=", "/=" : 乘除运算赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 10;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// "*", "/" : 乘, 除
						component.Text = idStr;
						component.Priority = 3;
						component.OperandCount = 2;
					}
					break;
				case ">":
				case "<":
					if (nextIdStr == idStr)
					{
						string thirdChar = statementStr.Substring(offset + 1, 1);
						if ("=" == thirdChar)
						{
							// ">>=", "<<=" : 位移赋值
							component.Text = idStr + nextIdStr + thirdChar;
							component.Priority = 10;
							component.OperandCount = 2;
							offset += 2;
						}
						else
						{
							// ">>", "<<" : 左移, 右移
							component.Text = idStr + nextIdStr;
							component.Priority = 5;
							component.OperandCount = 2;
							offset += 1;
						}
					}
					else if ("=" == nextIdStr)
					{
						// ">=", "<=" : 大于等于, 小于等于
						component.Text = idStr + nextIdStr;
						component.Priority = 6;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// ">", "<" : 大于, 小于
						component.Text = idStr;
						component.Priority = 6;
						component.OperandCount = 2;
					}
					break;
				case "&":
				case "|":
					if (nextIdStr == idStr)
					{
						// "&&", "||" : 逻辑与, 逻辑或
						component.Text = idStr + nextIdStr;
						component.Priority = 8;
						component.OperandCount = 2;
						offset += 1;
					}
					else if ("=" == nextIdStr)
					{
						// "&=", "|=" : 位运算赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 10;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// "&", "|" : 位与, 位或
						component.Text = idStr;
						component.Priority = 7;									        // 不确定, 可能是双目位与&,也可能是取地址符&(优先级2)
						component.OperandCount = 2;
					}

					break;
				case "!":
					if ("=" == nextIdStr)
					{
						// "!=" : 不等于
						component.Text = idStr + nextIdStr;
						component.Priority = 6;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// "!" : 逻辑非
						component.Text = idStr;
						component.Priority = 2;
						component.OperandCount = 1;
					}
					break;
				case "~":
					// "~" : 按位取反
					component.Text = idStr;
					component.Priority = 2;
					component.OperandCount = 1;
					break;
				case "%":
					if ("=" == nextIdStr)
					{
						// "%=" : 取余赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 10;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// "%" : 取余
						component.Text = idStr;
						component.Priority = 3;
						component.OperandCount = 2;
					}
					break;
				case "^":
					if ("=" == nextIdStr)
					{
						// "^=" : 位异或赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 10;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// "^" : 位异或
						component.Text = idStr;
						component.Priority = 7;
						component.OperandCount = 2;
					}
					break;
				case ",":
					// "," : 逗号
					component.Text = idStr;
					component.Priority = 11;
					component.OperandCount = 2;
					break;
				case "?":
				case ":":
					// "?:" : 条件(三目)
					component.Text = idStr;
					component.Priority = 9;
					component.OperandCount = 3;
					break;
				default:
					return false;
			}
			return true;
		}

        /// <summary>
        /// 判断标识符是否是构造类型/用户定义类型
        /// </summary>
		static bool IsUsrDefVarType(List<string> idStrList, FileParseInfo parse_info, ref int count)
        {
			string categoryStr = string.Empty;
			string idStr = idStrList[0];
			count = 1;
			if (CommonProcess.IsUsrDefTypeKWD(idStr))
			{
				categoryStr = idStr;
				if (idStrList.Count > 1)
				{
					idStr = idStrList[1];
					count = 2;
				}
				else
				{
					return false;
				}
				if (null != parse_info.FindUsrDefTypeInfo(idStr, categoryStr))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (null != parse_info.FindTypeDefInfo(idStr))
			{
				return true;
			}
			else
			{
				return false;
			}
        }

        /// <summary>
        /// 判断标识符是否是立即数常量
        /// </summary>
        public static bool IsConstantNumber(string idStr)
        {
            int i = 0;
            bool retVal = false;
            for (; i < idStr.Length; i++)
            {
                if (!Char.IsDigit(idStr[i]))
                {
                    if (Char.IsLetter(idStr[i]) && (i > 0))
                    {
                    }
                    else
                    {
                        retVal = false;
                        break;
                    }
                }
                retVal = true;
            }
            return retVal;
        }

        /// <summary>
        /// 判断标识符是否是局部(临时)变量
        /// </summary>
		static bool IsLocalVariable(string identifier, List<VAR_CTX>local_var_list)
        {
			foreach (VAR_CTX vctx in local_var_list)
			{
				if (vctx.Name.Equals(identifier))
				{
					return true;
				}
			}
            return false;
        }

		static bool IsStringOrChar(string idStr, string statementStr, ref int offset)
		{
            if ("\"" == idStr)
            {
                
            }
            else if ("\'" == idStr)
            {

            }
            else
            {
                return false;
            }
			return false;
		}

		/// <summary>
		/// 判断基本成分列表构成的是否是一个变量类型
		/// </summary>
		static bool IsVarType(List<StatementComponent> cpntList, ref int index, FileParseInfo parse_info)
		{
			// 判断是否是类型名
			List<string> idStrList = new List<string>();
			for (int i = index; i < cpntList.Count; i++)
			{
				idStrList.Add(cpntList[i].Text);
			}
			int count = 0;
			if (CommonProcess.IsBasicTypeName(idStrList, ref count))
			{
				index += count;
				return true;
			}
			else if (IsUsrDefVarType(idStrList, parse_info, ref count))
			{
				index += count;
				return true;
			}

			return false;
		}

		/// <summary>
		/// 是否是类型前缀
		/// </summary>
		/// <returns></returns>
		static bool IsTypePrefix(string identifier)
		{
			switch (identifier)
			{
				case "extern":
				case "const":
				case "static":
				case "register":
				case "volatile":
					return true;
				default:
					break;
			}
			return false;
		}

		/// <summary>
		/// 是否是类型后缀
		/// </summary>
		/// <returns></returns>
		static bool IsTypeSuffix(string identifier)
		{
			switch (identifier)
			{
				case "*":
				case "const":
					return true;
				default:
					break;
			}
			return false;
		}

		static MeaningGroup GetVarTypeGroup(List<StatementComponent> componentList, ref int idx, FileParseInfo parse_info)
		{
			MeaningGroup retGroup = null;
			List<StatementComponent> prefixList = new List<StatementComponent>();
			for (int i = idx; i < componentList.Count; i++)
			{
				int old_idx = idx;
				// 判断有无类型前缀
				if (null == retGroup && IsTypePrefix(componentList[i].Text))
				{
					prefixList.Add(componentList[i]);
					idx++;
				}
				else if (null == retGroup && IsVarType(componentList, ref idx, parse_info))
				{
					retGroup = new MeaningGroup();
					retGroup.Type = MeaningGroupType.VariableType;
					for (int j = 0; j < prefixList.Count; j++)
					{
						retGroup.ComponentList.Add(prefixList[j]);
						retGroup.Text += prefixList[j].Text + " ";
					}
					retGroup.PrefixCount = prefixList.Count;
					for (int j = old_idx; j < idx; j++)
					{
						retGroup.ComponentList.Add(componentList[j]);
						retGroup.Text += componentList[j].Text + " ";
					}
					i = idx - 1;
					retGroup.Text = retGroup.Text.Trim();
				}
				else if (null != retGroup
						 && IsTypeSuffix(componentList[i].Text))
				{
					retGroup.ComponentList.Add(componentList[i]);
					retGroup.Text += " " + componentList[i].Text;
					retGroup.SuffixCount++;
					idx++;
				}
				else
				{
					break;
				}
			}
			return retGroup;
		}

		static MeaningGroup GetSingleKeywordGroup(List<StatementComponent> componentList, ref int idx, FileParseInfo parse_info)
		{
			if ("typedef" == componentList[idx].Text
				|| "auto" == componentList[idx].Text
				|| "break" == componentList[idx].Text
				|| "case" == componentList[idx].Text
				|| "continue" == componentList[idx].Text
				|| "default" == componentList[idx].Text
				|| "goto" == componentList[idx].Text
				|| "return" == componentList[idx].Text
				|| "sizeof" == componentList[idx].Text)
			{
				MeaningGroup retGroup = new MeaningGroup();
				retGroup.Type = MeaningGroupType.SingleKeyword;
				retGroup.ComponentList.Add(componentList[idx]);
				retGroup.Text = componentList[idx].Text;
				idx += 1;
				return retGroup;
			}
			return null;
		}

		static MeaningGroup GetVarNameGroup(List<StatementComponent> componentList,
                                            ref int idx,
                                            List<MeaningGroup> groupList,
											FileParseInfo parse_info,
											FuncAnalysisContext func_ctx)
		{
            if (CommonProcess.IsStandardIdentifier(componentList[idx].Text))
            {
                // 是否是函数参数
                // 是否为局部变量
				if (null != func_ctx
					&& IsLocalVariable(componentList[idx].Text, func_ctx.LocalVarList))
                {
                    MeaningGroup retGroup = new MeaningGroup();
                    retGroup.Type = MeaningGroupType.LocalVariable;
                    retGroup.ComponentList.Add(componentList[idx]);
                    retGroup.Text = componentList[idx].Text;
                    GetVarMemberGroup(componentList, ref idx, ref retGroup);
                    return retGroup;
                }
                // 是否为全局变量
				else if (null != parse_info.FindGlobalVarInfoByName(componentList[idx].Text))
                {
                    MeaningGroup retGroup = new MeaningGroup();
                    retGroup.Type = MeaningGroupType.GlobalVariable;
                    retGroup.ComponentList.Add(componentList[idx]);
                    retGroup.Text = componentList[idx].Text;
                    GetVarMemberGroup(componentList, ref idx, ref retGroup);
                    return retGroup;
                }
                // 如果前面是类型名且是开头,那么可能是新定义变量
                else if (1 == groupList.Count
                    && groupList[0].Type == MeaningGroupType.VariableType)
                {
                    MeaningGroup retGroup = new MeaningGroup();
					if (null == func_ctx)
					{
						retGroup.Type = MeaningGroupType.GlobalVariable;
					}
					else
					{
						retGroup.Type = MeaningGroupType.LocalVariable;
					}
                    retGroup.ComponentList.Add(componentList[idx]);
                    retGroup.Text = componentList[idx].Text;
					GetVarMemberGroup(componentList, ref idx, ref retGroup);
                    return retGroup;
                }
            }
            else if ("(" == componentList[idx].Text)
            {
                int tmp_idx = idx;
                List<StatementComponent> braceList = GetBraceComponents(componentList, ref tmp_idx);
                if (null != braceList
                    && (tmp_idx != componentList.Count - 1)
                    && ("." == componentList[tmp_idx + 1].Text
                        || "->" == componentList[tmp_idx + 1].Text))
                {
                    MeaningGroup retGroup = new MeaningGroup();
					retGroup.Type = GetVariableType(braceList, parse_info);
                    foreach (StatementComponent item in braceList)
                    {
                        retGroup.ComponentList.Add(item);
                        retGroup.Text += item.Text;
                    }
                    GetVarMemberGroup(componentList, ref tmp_idx, ref retGroup);
                    idx = tmp_idx;
                    return retGroup;
                }
            }
            return null;
		}

		static MeaningGroupType GetVariableType(List<StatementComponent> braceList, FileParseInfo parse_info)
		{
			foreach (StatementComponent item in braceList)
			{
				if (CommonProcess.IsStandardIdentifier(item.Text))
				{
					if (null != parse_info.FindGlobalVarInfoByName(item.Text))
					{
						return MeaningGroupType.GlobalVariable;
					}
					else
					{
						return MeaningGroupType.LocalVariable;
					}
				}
			}
			return MeaningGroupType.LocalVariable;
		}

        static void GetVarMemberGroup(List<StatementComponent> componentList,
                                      ref int idx,
                                      ref MeaningGroup retGroup)
        {
            int i = idx + 1;
            for (i = idx + 1; i < componentList.Count; i++)
            {
                if ("." == componentList[i].Text
                    || "->" == componentList[i].Text)
                {
                    retGroup.ComponentList.Add(componentList[i]);
                    retGroup.Text += componentList[i].Text;
                    i += 1;
                    retGroup.ComponentList.Add(componentList[i]);
                    retGroup.Text += componentList[i].Text;
                }
				else if ("[" == componentList[i].Text)
				{
					idx = i;
					List<StatementComponent> braceList = GetBraceComponents(componentList, ref idx);
					retGroup.ComponentList.AddRange(braceList);
					foreach (var item in braceList)
					{
						retGroup.Text += item.Text;
					}
					i = idx;
				}
                else
                {
                    break;
                }
            }
            idx = i;
        }

		static MeaningGroup GetFunctionCallingGroup(List<StatementComponent> componentList, ref int idx, FileParseInfo parse_info)
		{
			if (CommonProcess.IsStandardIdentifier(componentList[idx].Text))
			{
				// 判断是否是函数名
				if (null != parse_info.FindFuncParseInfo(componentList[idx].Text)
					&& "(" == componentList[idx + 1].Text)
				{
					MeaningGroup retGroup = new MeaningGroup();
					retGroup.Type = MeaningGroupType.FunctionCalling;
					retGroup.ComponentList.Add(componentList[idx]);
					retGroup.Text += componentList[idx].Text;
					idx += 1;
					List<StatementComponent> braceList = GetBraceComponents(componentList, ref idx);
					if (null != braceList)
					{
						foreach (StatementComponent item in braceList)
						{
							retGroup.Text += item.Text;
						}
						retGroup.ComponentList.AddRange(braceList);
						idx += 1;
						return retGroup;
					}
				}
			}
			return null;
		}

		static MeaningGroup GetExpressionGroup(List<StatementComponent> componentList, ref int idx, FileParseInfo parse_info)
		{
            if ("(" == componentList[idx].Text)
            {
                List<StatementComponent> braceList = GetBraceComponents(componentList, ref idx);
                if (null != braceList)
                {
					MeaningGroup retGroup = new MeaningGroup();
					int tmpIdx = 1;
					if (IsVarType(braceList, ref tmpIdx, parse_info)
						&& tmpIdx == braceList.Count - 1)
					{
						retGroup.Type = MeaningGroupType.TypeCasting;
					}
					else
					{
						retGroup.Type = MeaningGroupType.Expression;
					}
					StringBuilder sb = new StringBuilder();
					foreach (StatementComponent item in braceList)
					{
						sb.Append(item.Text);
					}
					retGroup.Text = sb.ToString();
					retGroup.ComponentList.AddRange(braceList);
					idx += 1;
					return retGroup;
				}
            }
			return null;
		}

		static MeaningGroup GetCodeBlockGroup(List<StatementComponent> cpnt_list, ref int idx, FileParseInfo parse_result)
		{
			if ("{" == cpnt_list[idx].Text)
			{
				List<StatementComponent> braceList = GetBraceComponents(cpnt_list, ref idx);
				if (null != braceList)
				{
					MeaningGroup retGroup = new MeaningGroup();
					retGroup.Type = MeaningGroupType.CodeBlock;
					retGroup.ComponentList.AddRange(braceList);
					StringBuilder groupTextBuilder = new StringBuilder();
					for (int i = 0; i < braceList.Count; i++)
					{
						groupTextBuilder.Append(braceList[i].Text);
					}
					retGroup.Text = groupTextBuilder.ToString();
					idx += 1;
					return retGroup;
				}
			}
			return null;
		}

		static MeaningGroup GetStringBlockGroup(List<StatementComponent> cpnt_list, ref int idx, FileParseInfo parse_result)
		{
			if ("\"" == cpnt_list[idx].Text)
			{
				MeaningGroup retGroup = new MeaningGroup();
				retGroup.Type = MeaningGroupType.StringBlock;
				retGroup.ComponentList.Add(cpnt_list[idx]);
				StringBuilder sb = new StringBuilder();
				sb.Append(cpnt_list[idx].Text);
				for (int i = idx + 1; i < cpnt_list.Count; i++)
				{
					retGroup.ComponentList.Add(cpnt_list[i]);
					sb.Append(cpnt_list[idx].Text);
					if ("\"" == cpnt_list[i].Text && "\\" != cpnt_list[i - 1].Text)
					{
						idx = i + 1;
						break;
					}
				}
				retGroup.Text = sb.ToString();
				return retGroup;
			}
			return null;
		}

		static MeaningGroup GetCharBlockGroup(List<StatementComponent> cpnt_list, ref int idx, FileParseInfo parse_result)
		{
			if ("\'" == cpnt_list[idx].Text
				&& idx < cpnt_list.Count - 3
				&& "\'" == cpnt_list[idx + 2].Text)
			{
				MeaningGroup retGroup = new MeaningGroup();
				retGroup.Type = MeaningGroupType.CharBlock;
				retGroup.ComponentList.Add(cpnt_list[idx]);
				retGroup.Text += cpnt_list[idx].Text;
				retGroup.ComponentList.Add(cpnt_list[idx + 1]);
				retGroup.Text += cpnt_list[idx + 1].Text;
				retGroup.ComponentList.Add(cpnt_list[idx + 2]);
				retGroup.Text += cpnt_list[idx + 2].Text;
				idx += 3;
				return retGroup;
			}
			return null;
		}

		static MeaningGroup GetOperatorGroup(List<StatementComponent> componentList, ref int idx)
		{
            MeaningGroup retGroup = null;
            if (componentList[idx].Type == StatementComponentType.Operator)
            {
                retGroup = new MeaningGroup();
                retGroup.ComponentList.Add(componentList[idx]);
                retGroup.Text = componentList[idx].Text;
                if ("=" == componentList[idx].Text)
                {
                    retGroup.Type = MeaningGroupType.EqualMark;
                }
                else
                {
                    retGroup.Type = MeaningGroupType.OtherOperator;
                }
                idx += 1;
            }
            return retGroup;
		}

        static void MeaningGroupsAnalysis(	List<MeaningGroup> mgList,
											FileParseInfo parse_info,
											FuncAnalysisContext func_ctx)
        {
            // 先检查是否是新定义的局部变量
			VAR_CTX varCtx = null;
			if (null != (varCtx = IsNewDefineVarible(mgList, parse_info, func_ctx)))
			{
				// 如果是,为此新定义局部变量创建上下文记录项
				if (null != func_ctx)
				{
					func_ctx.LocalVarList.Add(varCtx);
				}
				else
				{
					if (0 != varCtx.Type.PrefixList.Count
						&& "extern" == varCtx.Type.PrefixList[0])
					{
						parse_info.GlobalDeclareList.Add(varCtx);						// 全局变量声明
					}
					else
					{
						parse_info.GlobalDefineList.Add(varCtx);						// 全局变量定义
					}
				}
			}
			// 分析左值/右值
			InOutAnalysis.LeftRightValueAnalysis(mgList, parse_info, func_ctx);
        }

		public static VAR_CTX IsNewDefineVarible(List<MeaningGroup> mgList, FileParseInfo parse_info, FuncAnalysisContext func_ctx)
        {
            if (mgList.Count >= 2 && mgList[0].Type == MeaningGroupType.VariableType)
            {
				VAR_CTX varCtx = InOutAnalysis.GetVarCtxByName(mgList[1].Text, parse_info, func_ctx);
				if (null != varCtx)
				{
					// 在当前上下文中已存在! (作用域覆盖? 正常情况下不应该出现!)
					return varCtx;
				}
				else
				{
					// 创建变量上下文
					varCtx = InOutAnalysis.CreateVarCtx(mgList[0], mgList[1].Text, parse_info);
					return varCtx;
				}
            }
            return null;
        }

		public static void TypeDefProc(	List<StatementComponent> component_list,
										FileParseInfo parse_info,
										FuncAnalysisContext func_ctx)
		{
			// 提取含义分组
			List<MeaningGroup> meaningGroupList = GetMeaningGroups(component_list, parse_info, func_ctx);
			if (3 == meaningGroupList.Count
				&& "typedef" == meaningGroupList[0].Text
				&& meaningGroupList[1].Type == MeaningGroupType.VariableType
				&& meaningGroupList[2].Type == MeaningGroupType.Identifier)
			{
				TypeDefineInfo tdi = new TypeDefineInfo();
				tdi.OldName = meaningGroupList[1].Text;
				tdi.NewName = meaningGroupList[2].Text;
				if (tdi.OldName != tdi.NewName)
				{
					parse_info.TypeDefineList.Add(tdi);
				}
			}
		}
	}

	public enum StatementComponentType
	{
		Invalid,				// 无效
		Identifier,				// 其它标识符
        ConstantNumber,         // 数值常量
        String,                 // 字符串
        Char,                   // 字符
		FunctionName,		    // 函数名
        Operator,               // 运算符
	}

	public class StatementComponent
	{
		private StatementComponentType type = StatementComponentType.Invalid;

		public StatementComponentType Type
		{
			get { return type; }
			set { type = value; }
		}
		private string text = "";

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		public int Priority = -1;	// (如果是运算符的话)运算符的优先级
		public int OperandCount = 0;// (如果是运算符的话)操作数的个数

		public StatementComponent()
		{
		}

		public StatementComponent(string str)
		{
			Text = str;
		}
	}

	public enum MeaningGroupType
	{
		Unknown,

		VariableType,				// 类型名
		LocalVariable,				// 局部变量
        GlobalVariable,				// 全局变量
        FunctionCalling,			// 函数调用
		Expression,					// 表达式
		EqualMark,					// 赋值符号
		TypeCasting,				// 强制类型转换
		OtherOperator,				// 其它运算符
		Constant,					// 常量
		CodeBlock,					// "{"和"}"括起的代码段
		StringBlock,				// 双引号""括起的字符串
		CharBlock,					// 单引号''括起的单字符
		SingleKeyword,				// 基本类型名以外的单个的关键字(保留字)
		Identifier,					// 其它标识符
	}

	/// <summary>
	/// 一组有意义的语句成分构成的分组
	/// </summary>
	public class MeaningGroup
	{
		public MeaningGroupType Type = MeaningGroupType.Unknown;

		public string Text = string.Empty;

		public List<StatementComponent> ComponentList = new List<StatementComponent>();

		public int PrefixCount = 0;														// 如果分组是类型的话, 还包括前后缀的个数
		public int SuffixCount = 0;
	}

    /// <summary>
    /// 解析上下文
    /// </summary>
    public class FuncAnalysisContext
    {
        // 引数列表
		public List<VAR_CTX> ParameterList = new List<VAR_CTX>();
        // 局部变量列表
		public List<VAR_CTX> LocalVarList = new List<VAR_CTX>();
		// 入力全局变量列表
		public List<VAR_CTX> InputGlobalList = new List<VAR_CTX>();
		// 出力全局变量列表
		public List<VAR_CTX> OutputGlobalList = new List<VAR_CTX>();
		// 其它未确定入出力的全局变量(比如函数调用读出值)
		public List<VAR_CTX> OtherGlobalList = new List<VAR_CTX>();
		// 调用函数列表
		public List<CalledFunction> CalledFunctionList = new List<CalledFunction>();
	}

	/// <summary>
	/// 函数调用
	/// </summary>
	public class CalledFunction
	{
		public string FunctionName = string.Empty;										// 函数名
		public MeaningGroup MeaningGroup = new MeaningGroup();
		public List<ActualParaInfo> ActualParaInfoList = new List<ActualParaInfo>();	// 实参情报列表
		public string ReturnValType = string.Empty;
	}
}
