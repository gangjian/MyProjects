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
						string paraStr = CommonProcess.GetNextIdentifier2(statementStr, ref offset);
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
        /// 判断标识符是否是构造类型/用户定义类型
        /// </summary>
		static bool IsUsrDefVarType(List<string> idStrList, CCodeParseResult parseResult)
        {
			string idStr = idStrList[0];
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
                            return true;
                        }
                    }
                }
                // 然后是typedef列表
                foreach (TypeDefineInfo tdi in hfi.type_define_list)
                {
					if (tdi.new_type_name.Equals(idStr))
                    {
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
        /// 判断标识符是否是局部(临时)变量
        /// </summary>
		static bool IsLocalVariable(string identifier, AnalysisContext analysisContext)
        {
			foreach (VAR_CTX vctx in analysisContext.local_list)
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
			if (CommonProcess.IsBasicVarType(idStrList, ref count))
			{
				index += count;
				return true;
			}
			else if (IsUsrDefVarType(idStrList, parseResult))
			{
				index += 1;
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
				else if (null != analysisContext.parseResult.FindGlobalVarInfoByName(componentList[idx].Text))
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
					retGroup.Type = GetVariableType(braceList, analysisContext.parseResult);
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

		static MeaningGroupType GetVariableType(List<StatementComponent> braceList, CCodeParseResult parseResult)
		{
			foreach (StatementComponent item in braceList)
			{
				if (CommonProcess.IsStandardIdentifier(item.Text))
				{
					if (null != parseResult.FindGlobalVarInfoByName(item.Text))
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
				if (null != parseResult.FindFuncParseInfoByName(componentList[idx].Text)
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
										  AnalysisContext ctx)
        {
            // 先检查是否是新定义的局部变量
			VAR_CTX varCtx = null;
			if (null != (varCtx = IsNewDefineVarible(mgList, ctx)))
			{
				// 如果是,为此新定义局部变量创建上下文记录项
				ctx.local_list.Add(varCtx);
			}
			// 分析左值/右值
			InOutAnalysis.LeftRightValueAnalysis(mgList, ctx);
        }

		static VAR_CTX IsNewDefineVarible(List<MeaningGroup> mgList, AnalysisContext ctx)
        {
            if (mgList.Count >= 2 && mgList[0].Type == MeaningGroupType.VariableType)
            {
				VAR_CTX varCtx = InOutAnalysis.GetVarCtx(mgList[1].Text, ctx, mgList[0].Text);
				if (string.Empty == varCtx.Type)
				{
					varCtx.Type = mgList[0].Text;
				}
				string orgTypeName;
				if (string.Empty != (orgTypeName = IsTypeDefTypeName(mgList[0], ctx)))
				{
					varCtx.RealType = orgTypeName;
				}
                return varCtx;
            }
            return null;
        }

		/// <summary>
		/// 判断类型名是否是一个typedef定义的类型别名
		/// </summary>
		/// <returns>返回原类型名</returns>
		static string IsTypeDefTypeName(MeaningGroup type_name_group, AnalysisContext ctx)
		{
			foreach (StatementComponent cpnt in type_name_group.ComponentList)
			{
				if (CommonProcess.IsStandardIdentifier(cpnt.Text))
				{
					string real_type;
					List<CFileParseInfo> fpiList = new List<CFileParseInfo>();
					fpiList.AddRange(ctx.parseResult.IncHdParseInfoList);
					fpiList.Add(ctx.parseResult.SourceParseInfo);
					if (string.Empty != (real_type = CommonProcess.FindTypeDefName(cpnt.Text, fpiList)))
					{
						return real_type;
					}
				}
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
    /// 解析上下文
    /// </summary>
    public class AnalysisContext
    {
        // 引数列表
		public List<VAR_CTX> parameter_list = new List<VAR_CTX>();
        // 局部变量列表
		public List<VAR_CTX> local_list = new List<VAR_CTX>();
		// 入力全局变量列表
		public List<VAR_CTX> inputGlobalList = new List<VAR_CTX>();
		// 出力全局变量列表
		public List<VAR_CTX> outputGlobalList = new List<VAR_CTX>();
		// 其它未确定入出力的全局变量(比如函数调用读出值)
		public List<VAR_CTX> otherGlobalList = new List<VAR_CTX>();
		// 调用函数列表
		public List<CalledFunction> calledFunctionList = new List<CalledFunction>();

		// 源文件分析结果
		public CCodeParseResult parseResult = null;
	}

	/// <summary>
	/// 函数调用
	/// </summary>
	public class CalledFunction
	{
		public string functionName = string.Empty;										// 函数名
		public MeaningGroup meaningGroup = new MeaningGroup();
		public List<ActualParaInfo> actParaInfoList = new List<ActualParaInfo>();		// 实参情报列表
		public string returnValType = string.Empty;
	}
}


