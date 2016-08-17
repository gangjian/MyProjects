﻿using System;
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
        public static AnalysisContext FunctionStatementsAnalysis(StatementNode root,
													             CCodeParseResult parseResult)
		{
            AnalysisContext analysisContext = new AnalysisContext();					// (变量)解析上下文
			analysisContext.parseResult = parseResult;
			// 顺次解析各条语句
			foreach (StatementNode childNode in root.childList)
			{
                StatementAnalysis(childNode, analysisContext);
			}

            return analysisContext;
		}

		public static void StatementAnalysis(StatementNode node,
                                             AnalysisContext analysisContext)
		{
			switch (node.Type)
			{
				case StatementNodeType.Simple:
					SimpleStatementAnalysis(node, analysisContext);
					break;
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
		}

		/// <summary>
		/// 简单语句分析
		/// </summary>
		static void SimpleStatementAnalysis(StatementNode statementNode,
                                            AnalysisContext analysisContext)
		{
			// 按顺序提取出语句各组成部分: 运算数(Operand)和运算符(Operator)
			List<StatementComponent> componentList = GetComponents(statementNode, analysisContext.parseResult);

            ExpressionAnalysis(componentList, analysisContext);
		}

        public static void ExpressionAnalysis(List<StatementComponent> componentList,
											  AnalysisContext analysisContext)
        {
            // 提取含义分组
			List<MeaningGroup> meaningGroupList = GetMeaningGroups(componentList, analysisContext);

            // 含义分组解析
			MeaningGroupsAnalysis(meaningGroupList, analysisContext);
        }

		/// <summary>
		/// 取得语句内各基本成分(运算数或者是运算符)
		/// </summary>
        public static List<StatementComponent> GetComponents(StatementNode statementNode,
                                                             CCodeParseResult parseResult)
        {
            // 取得完整的语句内容
            string statementStr = CommonProcess.LineStringCat(parseResult.SourceParseInfo.parsedCodeList,
                                                statementNode.Scope.Start,
                                                statementNode.Scope.End).Trim();
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
                StatementComponent cpnt = GetOneComponent(ref statementStr, ref offset, parseResult);
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

        public static List<MeaningGroup> GetMeaningGroups(List<StatementComponent> componentList,
														  AnalysisContext analysisContext)
		{
            List<MeaningGroup> meaningGroupList;
            while (true)
            {
                // (1). 首先对语句所有组成部分进行结构分组, 每个组代表一个独立完整的语义结构
				meaningGroupList = GetMeaningGroupList(componentList, analysisContext);
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
		static List<MeaningGroup> GetMeaningGroupList(List<StatementComponent> componentList,
													  AnalysisContext analysisContext)
		{
			List<MeaningGroup> groupList = new List<MeaningGroup>();
			int idx = 0;
			while (true) 
			{
                if (idx >= componentList.Count)
                {
                    break;
                }
				MeaningGroup newGroup = GetOneMeaningGroup(componentList, ref idx, groupList, analysisContext);
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
		static MeaningGroup GetOneMeaningGroup(List<StatementComponent> componentList,
											   ref int idx,
                                               List<MeaningGroup> groupList,
											   AnalysisContext analysisContext)
		{
			MeaningGroup retGroup = null;
			// 是类型名?
			if (null != (retGroup = GetVarTypeGroup(componentList, ref idx, analysisContext.parseResult)))
			{
				return retGroup;
			}
			// 是变量名?
			else if (null != (retGroup = GetVarNameGroup(componentList, ref idx, groupList, analysisContext)))
			{
				return retGroup;
			}
			// 是函数调用?
			else if (null != (retGroup = GetFunctionCallingGroup(componentList, ref idx, analysisContext.parseResult)))
			{
				return retGroup;
			}
			// 是表达式? 或者是强制类型转换运算符
			else if (null != (retGroup = GetExpressionGroup(componentList, ref idx, analysisContext.parseResult)))
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
				retGroup.Type = MeaningGroupType.Unknown;
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
				System.Diagnostics.Trace.Assert(false);
                return null;
			}

			retGroup = new MeaningGroup();
			while (true)
			{
				if (idx > componentList.Count - 1)
				{
					break;
				}
				StatementComponent cpnt = componentList[idx];
				#region MyRegion
				if (StatementComponentType.Operator == cpnt.Type)				        // 如果是操作符
				{
					if ("(" == cpnt.Text)
					{
						List<StatementComponent> braceList = GetBraceComponents(componentList, ref idx);
						if (null != braceList)
						{
							retGroup.ComponentList.AddRange(braceList);
							// TODO: 括号之间如果既有运算符又有运算数的话是表达式
							// 如果括号里是个类型的话,则是"强制类型转换"运算符
							//if (IsVarType(braceList, parseResult))
							{
								// TODO:
							}
						}
					}
					else if ("[" == cpnt.Text)
					{
						List<StatementComponent> braceList = GetBraceComponents(componentList, ref idx);
						if (null != braceList)
						{
							retGroup.ComponentList.AddRange(braceList);
						}
					}
					else if (("." == cpnt.Text)
							 || ("->" == cpnt.Text))
					{
						retGroup.ComponentList.Add(cpnt);
						// TODO: 有成员运算符的话是变量
					}
					else
					{
						if (0 != retGroup.ComponentList.Count)
						{
							break;
						}
						else
						{
							retGroup.ComponentList.Add(cpnt);
							idx++;
							break;
						}
					}
				}
				else
				{																        // 非操作符, 操作数
					// 是类型名?
					// 是函数名?
					// 是变量名?
					if (0 != retGroup.ComponentList.Count)
					{
						StatementComponent lastCpnt = retGroup.ComponentList.Last();
						if (("." == lastCpnt.Text)
							|| ("->" == lastCpnt.Text))
						{
							retGroup.ComponentList.Add(cpnt);
						}
						else
						{
							break;
						}
					}
					else
					{
						retGroup.ComponentList.Add(cpnt);
					}
				}
				#endregion
				idx++;
			}
			foreach (StatementComponent cpnt in retGroup.ComponentList)
			{
				retGroup.Text += cpnt.Text;
			}
			return retGroup;
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
												  CCodeParseResult parseResult)
		{
			string idStr = null;
			int offset_old = -1;
			StatementComponent retSC = new StatementComponent();
			while (true)
			{
				offset_old = offset;
				idStr = GetNextIdentifier(statementStr, ref offset);
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
					if (MacroDetectAndExpand_Function(idStr, ref statementStr, offset, parseResult))
					{
						offset = offset_old;
						continue;
					}
					else
					{
						retSC = new StatementComponent(idStr);
						break;
					}
				}
				else if (IsOperator(idStr, statementStr, ref offset, ref retSC))
				{
					break;
				}
				else
				{
					System.Diagnostics.Trace.Assert(false);
				}
			}

			return retSC;
		}

		/// <summary>
		/// 函数内的宏展开 TODO:以后考虑重构跟MacroDetectAndExpand_File合并
		/// </summary>
		static bool MacroDetectAndExpand_Function(string idStr, ref string statementStr, int offset, CCodeParseResult parseResult)
        {
			// 作成一个所有包含头文件的宏定义的列表
			List<MacroDefineInfo> macroDefineList = new List<MacroDefineInfo>();
			List<TypeDefineInfo> typeDefineList = new List<TypeDefineInfo>();
			foreach (CFileParseInfo hdInfo in parseResult.IncHdParseInfoList)
			{
				macroDefineList.AddRange(hdInfo.macro_define_list);
				typeDefineList.AddRange(hdInfo.type_define_list);
			}
			// 添加上本文件所定义的宏
			macroDefineList.AddRange(parseResult.SourceParseInfo.macro_define_list);
			typeDefineList.AddRange(parseResult.SourceParseInfo.type_define_list);
			// 遍历查找宏名
			foreach (MacroDefineInfo di in macroDefineList)
			{
				// 判断宏名是否一致
				if (idStr == di.name)
				{
					string macroName = di.name;
					string replaceStr = di.value;
					// 判断有无带参数
					if (0 != di.paras.Count)
					{
						// 取得宏参数
						string paraStr = GetNextIdentifier(statementStr, ref offset);
						if ("(" != paraStr)
						{
                            CommonProcess.ErrReport();
							break;
						}
						int leftBracket = offset;
						int rightBracket = statementStr.Substring(offset).IndexOf(')');
						if (-1 == rightBracket)
						{
                            CommonProcess.ErrReport();
							break;
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
							replaceStr = replaceStr.Replace(di.paras[idx], rp);
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
					statementStr = statementStr.Replace(macroName, replaceStr);
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
						offset += 1;
					}
					else
					{
						// "=" : 赋值
						component.Text = idStr;
						component.Priority = 10;
					}
					break;
				case "+":
				case "-":
					if (nextIdStr == idStr)
					{
						// "++", "--" : 自增, 自减
						component.Text = idStr + nextIdStr;
						component.Priority = 2;
						offset += 1;
					}
					else if ("=" == nextIdStr)
					{
						// "+=", "-=" : 加减运算赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 10;
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
					}
					break;
				case "*":														        // 不确定, 可能是乘号, 也可能是指针运算符
				case "/":
					if ("=" == nextIdStr)
					{
						// "*=", "/=" : 乘除运算赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 10;
						offset += 1;
					}
					else
					{
						// "*", "/" : 乘, 除
						component.Text = idStr;
						component.Priority = 3;
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
							offset += 2;
						}
						else
						{
							// ">>", "<<" : 左移, 右移
							component.Text = idStr + nextIdStr;
							component.Priority = 5;
							offset += 1;
						}
					}
					else if ("=" == nextIdStr)
					{
						// ">=", "<=" : 大于等于, 小于等于
						component.Text = idStr + nextIdStr;
						component.Priority = 6;
						offset += 1;
					}
					else
					{
						// ">", "<" : 大于, 小于
						component.Text = idStr;
						component.Priority = 6;
					}
					break;
				case "&":
				case "|":
					if (nextIdStr == idStr)
					{
						// "&&", "||" : 逻辑与, 逻辑或
						component.Text = idStr + nextIdStr;
						component.Priority = 8;
						offset += 1;
					}
					else if ("=" == nextIdStr)
					{
						// "&=", "|=" : 位运算赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 10;
						offset += 1;
					}
					else
					{
						// "&", "|" : 位与, 位或
						component.Text = idStr;
						component.Priority = 7;									        // 不确定, 可能是双目位与&,也可能是取地址符&(优先级2)
					}

					break;
				case "!":
					if ("=" == nextIdStr)
					{
						// "!=" : 不等于
						component.Text = idStr + nextIdStr;
						component.Priority = 6;
						offset += 1;
					}
					else
					{
						// "!" : 逻辑非
						component.Text = idStr;
						component.Priority = 2;
					}
					break;
				case "~":
					// "~" : 按位取反
					component.Text = idStr;
					component.Priority = 2;
					break;
				case "%":
					if ("=" == nextIdStr)
					{
						// "%=" : 取余赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 10;
						offset += 1;
					}
					else
					{
						// "%" : 取余
						component.Text = idStr;
						component.Priority = 3;
					}
					break;
				case "^":
					if ("=" == nextIdStr)
					{
						// "^=" : 位异或赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 10;
						offset += 1;
					}
					else
					{
						// "^" : 位异或
						component.Text = idStr;
						component.Priority = 7;
					}
					break;
				case ",":
					// "," : 逗号
					component.Text = idStr;
					component.Priority = 11;
					break;
				case "?":
				case ":":
					// "?:" : 条件(三目)
					component.Text = idStr;
					component.Priority = 9;
					break;
				default:
					return false;
			}
			return true;
		}

		/// <summary>
		/// 从指定位置开始取得(同一行内)的下一个标识符
		/// </summary>
		/// <returns></returns>
		static string GetNextIdentifier(string statementStr, ref int offset)
		{
            int s_pos = -1, e_pos = -1;     // 标识符的起止位置
            for (; offset < statementStr.Length; offset++)
            {
                char curChar = statementStr[offset];
                E_CHAR_TYPE cType = CommonProcess.GetCharType(curChar);
                switch (cType)
                {
                    case E_CHAR_TYPE.E_CTYPE_WHITE_SPACE:                       // 空格
                        if (-1 == s_pos)
					    {
                            // do nothing
					    }
					    else
					    {
						    // 标识符结束
                            e_pos = offset - 1;
					    }
						break;
                    case E_CHAR_TYPE.E_CTYPE_LETTER:                            // 字母
                    case E_CHAR_TYPE.E_CTYPE_UNDERLINE:                         // 下划线
                    case E_CHAR_TYPE.E_CTYPE_DIGIT:                             // 数字
                        if (-1 == s_pos)
                        {
                            s_pos = offset;
                        }
                        else
                        {
                            // do nothing
                        }
                        break;
                    case E_CHAR_TYPE.E_CTYPE_PUNCTUATION:                       // 标点
                    case E_CHAR_TYPE.E_CTYPE_SYMBOL:                            // 运算符
                        if (-1 == s_pos)
                        {
                            s_pos = offset;
                            e_pos = offset;
                        }
                        else
                        {
                            e_pos = offset - 1;
                        }
                        break;
                    default:
                        CommonProcess.ErrReport();
                        return null;
                }
                if (-1 != s_pos && -1 != e_pos)
                {
                    return statementStr.Substring(s_pos, e_pos - s_pos + 1);
                }
            }
			if (-1 != s_pos && -1 == e_pos)
			{
				e_pos = offset - 1;
				return statementStr.Substring(s_pos, e_pos - s_pos + 1);
			}
			return null;
		}

		/// <summary>
		/// 判断一组标识符是否是一个基本类型名
		/// </summary>
		/// <param name="idStrList"></param>
		static bool IsBasicVarType(List<string> idStrList, ref int count)
		{
			// 开头 "const", "static"等限定符
			List<string> qualifiers = new List<string>();
			List<string> initialParts = new List<string>();
			List<string> lastParts = new List<string>();
			count = 0;
			foreach (string str in idStrList)
			{
				if (("const" == str || "static" == str)
					&& (0 == initialParts.Count)
					&& (0 == lastParts.Count))
				{
					// 前置修饰符
					qualifiers.Add(str);
				}
				else if (("signed" == str || "unsigned" == str)
					     && (0 == lastParts.Count))
				{
					// 类型名开头部分
					initialParts.Add(str);
				}
				else if (	("char" == str)
						 || ("short" == str)
						 || ("int" == str)
						 || ("long" == str)
						 || ("float" == str)
						 || ("double" == str)
						 || ("void" == str)
						 )
				{
					lastParts.Add(str);
				}
				else
				{
					return false;
				}
				count += 1;
			}
			if (0 != lastParts.Count)
			{
				return true;
			}
			else if (0 != qualifiers.Count || 0 != initialParts.Count)
			{
				return false;
			}
			else
			{
				return false;
			}
		}

        /// <summary>
        /// 判断标识符是否是构造类型/用户定义类型
        /// </summary>
		static bool IsUsrDefVarType(List<string> idStrList, CCodeParseResult parseResult, ref int count)
        {
			string idStr = idStrList[0];
			count = 0;
			List<CFileParseInfo> headerList = parseResult.IncHdParseInfoList;
            // 遍历头文件列表
            foreach (CFileParseInfo hfi in headerList)
            {
                // 首先查用户定义类型列表
                foreach (UsrDefTypeInfo udi in hfi.user_def_type_list)
                {
                    foreach (string typeName in udi.nameList)
                    {
						if (typeName.Equals(idStr))
                        {
							count = 1;
                            return true;
                        }
                    }
                }
                // 然后是typedef列表
                foreach (TypeDefineInfo tdi in hfi.type_define_list)
                {
					if (tdi.new_type_name.Equals(idStr))
                    {
						count = 1;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断标识符是否是立即数常量
        /// </summary>
        static bool IsConstantNumber(string idStr)
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
        /// 判断标识符是否是函数名
        /// </summary>
		static bool IsFunctionName(string identifier, CCodeParseResult parsedResult)
        {
			int idx;
			// 遍历头文件列表
			foreach (CFileParseInfo hfi in parsedResult.IncHdParseInfoList)
            {
				if (-1 != (idx = SearchFunctionInfoListByName(identifier, hfi.fun_declare_list)))
				{
					return true;
				}
				else if (-1 != (idx = SearchFunctionInfoListByName(identifier, hfi.fun_define_list)))
				{
					return true;
				}
            }
			// 是否是本文件內前面定义/声明的函数
			if (-1 != (idx = SearchFunctionInfoListByName(identifier, parsedResult.SourceParseInfo.fun_declare_list)))
			{
				return true;
			}
			else if (-1 != (idx = SearchFunctionInfoListByName(identifier, parsedResult.SourceParseInfo.fun_define_list)))
			{
				return true;
			}

            return false;
        }

		static int SearchFunctionInfoListByName(string fun_name, List<CFunctionStructInfo> funInfoList)
		{
			for (int i = 0; i < funInfoList.Count; i++)
			{
				if (funInfoList[i].name.Equals(fun_name))
				{
					return i;
				}
			}
			return -1;
		}

        /// <summary>
        /// 判断标识符是否是全局变量
        /// </summary>
        static bool IsGlobalVariable(string identifier, List<CFileParseInfo> headerList)
        {
			foreach (CFileParseInfo hfi in headerList)
			{
				foreach (VariableInfo vi in hfi.global_var_declare_list)
				{
					if (vi.varName.Equals(identifier))
					{
						return true;
					}
				}
				foreach (VariableInfo vi in hfi.global_var_define_list)
				{
					if (vi.varName.Equals(identifier))
					{
						return true;
					}
				}
			}
            return false;
        }

        /// <summary>
        /// 判断标识符是否是局部(临时)变量
        /// </summary>
		static bool IsLocalVariable(string identifier, AnalysisContext analysisContext)
        {
			foreach (VAR_CTX vctx in analysisContext.local_list)
			{
				if (vctx.name.Equals(identifier))
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
		static bool IsVarType(List<StatementComponent> cpntList, ref int index, CCodeParseResult parseResult)
		{
			// 判断有无类型前缀

			// 判断是否是类型名
			List<string> idStrList = new List<string>();
			for (int i = index; i < cpntList.Count; i++)
			{
				idStrList.Add(cpntList[i].Text);
			}
			int count = 0;
			if (IsBasicVarType(idStrList, ref count))
			{
				index += count;
				return true;
			}
			else if (IsUsrDefVarType(idStrList, parseResult, ref count))
			{
				index += count;
				return true;
			}

			return false;
		}

		static MeaningGroup GetVarTypeGroup(List<StatementComponent> componentList, ref int idx, CCodeParseResult parseResult)
		{
			int old_idx = idx;
			if (IsVarType(componentList, ref idx, parseResult))
			{
                MeaningGroup retGroup = new MeaningGroup();
                retGroup.Type = MeaningGroupType.VariableType;
				for (int i = old_idx; i < idx; i++)
				{
					retGroup.ComponentList.Add(componentList[i]);
					retGroup.Text += componentList[i].Text + " ";
				}
				retGroup.Text = retGroup.Text.Trim();
				return retGroup;
			}
			return null;
		}

		static MeaningGroup GetVarNameGroup(List<StatementComponent> componentList,
                                            ref int idx,
                                            List<MeaningGroup> groupList,
											AnalysisContext analysisContext)
		{
            if (CommonProcess.IsStandardIdentifier(componentList[idx].Text))
            {
                // 是否是函数参数
                // 是否为局部变量
				if (IsLocalVariable(componentList[idx].Text, analysisContext))
                {
                    MeaningGroup retGroup = new MeaningGroup();
                    retGroup.Type = MeaningGroupType.LocalVariable;
                    retGroup.ComponentList.Add(componentList[idx]);
                    retGroup.Text = componentList[idx].Text;
                    GetVarMemberGroup(componentList, ref idx, ref retGroup);
                    return retGroup;
                }
                // 是否为全局变量
				else if (IsGlobalVariable(componentList[idx].Text, analysisContext.parseResult.IncHdParseInfoList))
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
                    retGroup.Type = MeaningGroupType.LocalVariable;
                    retGroup.ComponentList.Add(componentList[idx]);
                    retGroup.Text = componentList[idx].Text;
                    idx++;
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
					retGroup.Type = GetVariableType(braceList, analysisContext.parseResult.IncHdParseInfoList);
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

		static MeaningGroupType GetVariableType(List<StatementComponent> braceList, List<CFileParseInfo> headerList)
		{
			foreach (StatementComponent item in braceList)
			{
				if (CommonProcess.IsStandardIdentifier(item.Text))
				{
					if (IsGlobalVariable(item.Text, headerList))
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
                else
                {
                    break;
                }
            }
            idx = i;
        }

		static MeaningGroup GetFunctionCallingGroup(List<StatementComponent> componentList, ref int idx, CCodeParseResult parseResult)
		{
			if (CommonProcess.IsStandardIdentifier(componentList[idx].Text))
			{
				// 判断是否是函数名
				if (IsFunctionName(componentList[idx].Text, parseResult)
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

		static MeaningGroup GetExpressionGroup(List<StatementComponent> componentList, ref int idx, CCodeParseResult parseResult)
		{
            if ("(" == componentList[idx].Text)
            {
                List<StatementComponent> braceList = GetBraceComponents(componentList, ref idx);
                if (null != braceList)
                {
					MeaningGroup retGroup = new MeaningGroup();
					int tmpIdx = 1;
					if (IsVarType(braceList, ref tmpIdx, parseResult)
						&& tmpIdx == braceList.Count - 1)
					{
						retGroup.Type = MeaningGroupType.TypeCasting;
					}
					else
					{
						retGroup.Type = MeaningGroupType.Expression;
					}
					foreach (StatementComponent item in braceList)
					{
						retGroup.Text += item.Text;
					}
					retGroup.ComponentList.AddRange(braceList);
					idx += 1;
					return retGroup;
				}
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

        static void MeaningGroupsAnalysis(List<MeaningGroup> mgList,
										  AnalysisContext analysisContext)
        {
            // 先检查是否是新定义的局部变量
			VAR_CTX varCtx = null;
			if (null != (varCtx = IsNewDefineVarible(mgList)))
			{
				// 如果是,为此新定义局部变量创建上下文记录项
				analysisContext.local_list.Add(varCtx);
			}
			// 分析左值/右值
			InOutAnalysis.LeftRightValueAnalysis(mgList, analysisContext);
        }

        static VAR_CTX IsNewDefineVarible(List<MeaningGroup> mgList)
        {
            if (mgList.Count >= 2 && mgList[0].Type == MeaningGroupType.VariableType)
            {
				VAR_CTX varCtx = new VAR_CTX();
                varCtx.type = mgList[0].Text;
                varCtx.name = mgList[1].Text;
                return varCtx;
            }
            return null;
        }

	}

	public enum StatementComponentType
	{
		Invalid,				// 无效
		Unknown,				// 未知

        ConstantNumber,         // 数值常量
        String,                 // 字符串
        Char,                   // 字符

		FunctionName,		    // 函数名
        Operator,               // 运算符

		Expression,				// 表达式
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

		private int priority = -1;	// (如果是运算符的话)运算符的优先级

		public int Priority
		{
			get { return priority; }
			set { priority = value; }
		}

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
	}

	/// <summary>
	/// 一组有意义的语句成分构成的分组
	/// </summary>
	public class MeaningGroup
	{
		MeaningGroupType _type = MeaningGroupType.Unknown;

		public MeaningGroupType Type
		{
			get { return _type; }
			set { _type = value; }
		}

		private string _text = string.Empty;
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		private List<StatementComponent> _componentList = new List<StatementComponent>();
		public List<StatementComponent> ComponentList
		{
			get { return _componentList; }
			set { _componentList = value; }
		}
	}

	/// <summary>
	/// 变量上下文
	/// </summary>
	public class VAR_CTX
	{
		public string name = string.Empty;
		public string type = string.Empty;
		public object cur_val = new object();
		public List<VAR_CTX> memberList = new List<VAR_CTX>();
	}

    /// <summary>
    /// 解析上下文
    /// </summary>
    public class AnalysisContext
    {
        // 引数列表
		public List<VAR_CTX> parameter_list = new List<VAR_CTX>();
        // 局部变量列表
		public List<VAR_CTX> local_list = new List<VAR_CTX>();
        // 全局变量列表
		//public List<VAR_CTX> global_list = new List<VAR_CTX>();
		// 入力全局变量列表
		public List<MeaningGroup> inputGlobalList = new List<MeaningGroup>();
		// 出力全局变量列表
		public List<MeaningGroup> outputGlobalList = new List<MeaningGroup>();
		// 调用函数列表
		public List<MeaningGroup> calledFunctionList = new List<MeaningGroup>();

		// 源文件分析结果
		public CCodeParseResult parseResult = null;
	}

}


