using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Robot
{
	public static partial class C_DEDUCER
	{
		/// <summary>
		/// 语句分析
		/// </summary>
        public static FUNC_INFO DeducerStart(STATEMENT_NODE root, FILE_PARSE_INFO parse_info)
		{
            FUNC_INFO fCtx = new FUNC_INFO();
			// 顺次解析各条语句
			foreach (STATEMENT_NODE childNode in root.ChildNodeList)
			{
				StatementProc(childNode, parse_info, fCtx);
			}
            return fCtx;
		}

		public static void StatementProc(	STATEMENT_NODE s_node,
											FILE_PARSE_INFO parse_info,
                                            FUNC_INFO func_ctx)
		{
			switch (s_node.Type)
			{
				case E_STATEMENT_TYPE.Simple:
					// 取得完整的语句内容
					string statementStr = COMN_PROC.GetStatementStr(parse_info.CodeList, s_node.Scope);
					SimpleStatementProc(statementStr, parse_info, func_ctx, s_node);
					break;
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
		}

		/// <summary>
		/// 简单语句分析(函数内)
		/// </summary>
		public static void SimpleStatementProc(	string statement_str,
												FILE_PARSE_INFO parse_info,
												FUNC_INFO func_ctx,
												STATEMENT_NODE s_node)
		{
			// 按顺序提取出语句各组成部分: 运算数(Operand)和运算符(Operator)
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statement_str, parse_info);
			if (statement_str.StartsWith("typedef"))
			{
				TypeDefProc(componentList, parse_info, func_ctx);
			}
			else
			{
				ExpressionProc(componentList, parse_info, func_ctx, s_node);
			}
		}

        public static void ExpressionProc(	List<STATEMENT_COMPONENT> component_list,
											FILE_PARSE_INFO parse_info,
											FUNC_INFO func_ctx,
											STATEMENT_NODE s_node)
        {
            // 提取含义分组
			List<MEANING_GROUP> meaningGroupList = GetMeaningGroups(component_list, parse_info, func_ctx);

            // 含义分组解析
			MeaningGroupsAnalysis(meaningGroupList, parse_info, func_ctx, s_node);
			//int a, b, c = 20;
			//(a) = b = c + 3;
		}

		/// <summary>
		/// 对语句所有构成成分进行结构分组
		/// </summary>
		public static List<MEANING_GROUP> GetMeaningGroups(	List<STATEMENT_COMPONENT> component_list,
															FILE_PARSE_INFO parse_info,
															FUNC_INFO func_ctx)
		{
			string statementStr = COMN_PROC.GetComponentListStr(component_list);
			List<MEANING_GROUP> groupList = new List<MEANING_GROUP>();
			int idx = 0;
			while (true) 
			{
                if (idx >= component_list.Count)
                {
                    break;
                }
				MEANING_GROUP newGroup = GetOneMeaningGroup(component_list, ref idx, groupList, parse_info, func_ctx);
				if (0 != newGroup.ComponentList.Count)
				{
					groupList.Add(newGroup);
				}
				else
				{
					break;
				}
			}
			ConfirmUncertainPriorityOperator(groupList);
			while (CombineHighPriorityOperatorToExpression(groupList))
			{
			}
			while (true)
			{
				if (1 == groupList.Count
					&& groupList[0].ComponentList.Count >= 2
					&& "(" == groupList[0].ComponentList.First().Text
					&& ")" == groupList[0].ComponentList.Last().Text)
				{
					List<STATEMENT_COMPONENT> cpntList = groupList[0].ComponentList;
					cpntList.RemoveAt(0);
					cpntList.RemoveAt(cpntList.Count - 1);
					groupList = GetMeaningGroups(cpntList, parse_info, func_ctx);
				}
				else
				{
					break;
				}
			}
			return groupList;
		}

		/// <summary>
		/// 如果MeaningGroup里有多于一个以上的运算符,把高优先级的运算符跟对应的运算数结合成表达式
		/// </summary>
		static bool CombineHighPriorityOperatorToExpression(List<MEANING_GROUP> in_list)
		{
			int operatorCount = 0;	// 运算符的个数
			int highestPriority = int.MaxValue;
			int idx = -1;
			// TODO: 20170823 如果有多个相同(最高)优先级的运算符且连续,要根据运算符的结合方向进行结合
			for (int i = in_list.Count - 1; i >= 0; i--)
			{
				if (in_list[i].Type == MeaningGroupType.OtherOperator
					|| in_list[i].Type == MeaningGroupType.EqualMark)
				{
					if (in_list[i].ComponentList[0].Priority < highestPriority)
					{
						highestPriority = in_list[i].ComponentList[0].Priority;
						idx = i;
					}
					operatorCount++;
				}
			}
			if (operatorCount > 1
				&& "," != in_list[idx].Text)
			{
				int operandCount = in_list[idx].ComponentList[0].OperandCount;
				if (2 == operandCount)
				{
					List<STATEMENT_COMPONENT> newList = new List<STATEMENT_COMPONENT>();
					string newText = in_list[idx - 1].Text + in_list[idx].Text + in_list[idx + 1].Text;
					newList.AddRange(in_list[idx - 1].ComponentList);
					newList.AddRange(in_list[idx].ComponentList);
					newList.AddRange(in_list[idx + 1].ComponentList);
					in_list.RemoveRange(idx - 1, 3);
					MEANING_GROUP newGroup = new MEANING_GROUP();
					newGroup.ComponentList = newList;
					newGroup.Text = newText;
					newGroup.Type = MeaningGroupType.Expression;
					in_list.Insert(idx - 1, newGroup);
				}
				else if (1 == operandCount)
				{
					List<STATEMENT_COMPONENT> newList = new List<STATEMENT_COMPONENT>();
					string newText = in_list[idx].Text + in_list[idx + 1].Text;
					newList.AddRange(in_list[idx].ComponentList);
					newList.AddRange(in_list[idx + 1].ComponentList);
					in_list.RemoveRange(idx, 2);
					MEANING_GROUP newGroup = new MEANING_GROUP();
					newGroup.ComponentList = newList;
					newGroup.Text = newText;
					newGroup.Type = MeaningGroupType.Expression;
					in_list.Insert(idx, newGroup);
				}
				else
				{
					System.Diagnostics.Trace.Assert(false);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// 确定未明确的运算符(比如-是减号还是负号)
		/// </summary>
		static void ConfirmUncertainPriorityOperator(List<MEANING_GROUP> mg_list)
		{
			for (int i = 0; i < mg_list.Count; i++)
			{
				if (mg_list[i].Type == MeaningGroupType.OtherOperator
					&& mg_list[i].ComponentList[0].Priority == -1)
				{
					string oprStr = mg_list[i].Text;
					int oprCnt = ConfirmUnaryOrBinaryOperator(mg_list, i);
					switch (oprStr)
					{
						case "&":														// 单目取地址或者双目位与
							if (1 == oprCnt)
							{
								mg_list[i].ComponentList[0].Priority = 2;
								mg_list[i].ComponentList[0].OperandCount = 1;
							}
							else if (2 == oprCnt)
							{
								mg_list[i].ComponentList[0].Priority = 8;
								mg_list[i].ComponentList[0].OperandCount = 2;
							}
							break;
						case "-":														// 单目负号或者双目减号
							if (1 == oprCnt)
							{
								mg_list[i].ComponentList[0].Priority = 2;
								mg_list[i].ComponentList[0].OperandCount = 1;
							}
							else if (2 == oprCnt)
							{
								mg_list[i].ComponentList[0].Priority = 4;
								mg_list[i].ComponentList[0].OperandCount = 2;
							}
							break;
						case "*":														// 单目指针取值或者双目乘号
							if (1 == oprCnt)
							{
								mg_list[i].ComponentList[0].Priority = 2;
								mg_list[i].ComponentList[0].OperandCount = 1;
							}
							else if (2 == oprCnt)
							{
								mg_list[i].ComponentList[0].Priority = 3;
								mg_list[i].ComponentList[0].OperandCount = 2;
							}
							break;
						default:
							System.Diagnostics.Trace.Assert(false);
							break;
					}
				}
			}
		}

		/// <summary>
		/// 判定是单目运算符还是双目运算符
		/// </summary>
		/// <returns></returns>
		static int ConfirmUnaryOrBinaryOperator(List<MEANING_GROUP> mg_list, int opr_idx)
		{
			System.Diagnostics.Trace.Assert(opr_idx >= 0 && mg_list.Count > opr_idx);
			if (0 == opr_idx)
			{
				return 1;
			}
			else if (mg_list[opr_idx - 1].Type == MeaningGroupType.OtherOperator
					|| mg_list[opr_idx - 1].Type == MeaningGroupType.EqualMark)
			{
				return 1;
			}
			else
			{
				return 2;
			}
		}

		/// <summary>
		/// 取得一个构成分组
		/// </summary>
		static MEANING_GROUP GetOneMeaningGroup(List<STATEMENT_COMPONENT> componentList,
												ref int idx,
												List<MEANING_GROUP> groupList,
												FILE_PARSE_INFO parse_info,
												FUNC_INFO func_ctx)
		{
			MEANING_GROUP retGroup = null;
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
            else if (COMN_PROC.IsStandardIdentifier(componentList[idx].Text))
			{
				retGroup = new MEANING_GROUP();
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
				if ("defined" == retGroup.Text)
				{
					MEANING_GROUP nextGroup = GetOneMeaningGroup(componentList, ref idx, groupList, parse_info, func_ctx);
					if (null != nextGroup)
					{
						MEANING_GROUP defGroup = new MEANING_GROUP();
						defGroup.ComponentList.AddRange(retGroup.ComponentList);
						defGroup.ComponentList.AddRange(nextGroup.ComponentList);
						defGroup.Text = retGroup.Text + " " + nextGroup.Text;
						defGroup.Type = MeaningGroupType.Expression;
						return defGroup;
					}
				}
				else if (idx < componentList.Count
						 && "[" == componentList[idx].Text)								// 标识符后面跟着一个中括号(数组?)
				{
					List<STATEMENT_COMPONENT> braceList = COMN_PROC.GetBraceComponents(componentList, ref idx);
					retGroup.ComponentList.AddRange(braceList);
					retGroup.Text += COMN_PROC.GetComponentListStr(braceList);
					retGroup.Type = MeaningGroupType.Expression;
					idx += 1;
				}
				return retGroup;
			}
			else if (COMN_PROC.IsConstantNumber(componentList[idx].Text))
			{
				retGroup = new MEANING_GROUP();
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
        /// 判断标识符是否是构造类型/用户定义类型
        /// </summary>
		static bool IsUsrDefVarType(List<string> idStrList, FILE_PARSE_INFO parse_info, ref int count)
        {
			string categoryStr = string.Empty;
			string idStr = idStrList[0];
			count = 1;
			if (COMN_PROC.IsUsrDefTypeKWD(idStr))
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

		/// <summary>
		/// 判断基本成分列表构成的是否是一个变量类型
		/// </summary>
		static bool IsVarType(List<STATEMENT_COMPONENT> cpntList, ref int index, FILE_PARSE_INFO parse_info)
		{
			// 判断是否是类型名
			List<string> idStrList = new List<string>();
			for (int i = index; i < cpntList.Count; i++)
			{
				idStrList.Add(cpntList[i].Text);
			}
			int count = 0;
			if (BasicTypeProc.IsBasicTypeName(idStrList, ref count))
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

		static MEANING_GROUP GetVarTypeGroup(List<STATEMENT_COMPONENT> componentList, ref int idx, FILE_PARSE_INFO parse_info)
		{
			MEANING_GROUP retGroup = null;
			List<STATEMENT_COMPONENT> prefixList = new List<STATEMENT_COMPONENT>();
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
					retGroup = new MEANING_GROUP();
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

		static MEANING_GROUP GetSingleKeywordGroup(List<STATEMENT_COMPONENT> componentList, ref int idx, FILE_PARSE_INFO parse_info)
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
				MEANING_GROUP retGroup = new MEANING_GROUP();
				retGroup.Type = MeaningGroupType.SingleKeyword;
				retGroup.ComponentList.Add(componentList[idx]);
				retGroup.Text = componentList[idx].Text;
				idx += 1;
				return retGroup;
			}
			return null;
		}

		static MEANING_GROUP GetVarNameGroup(List<STATEMENT_COMPONENT> componentList,
                                            ref int idx,
                                            List<MEANING_GROUP> groupList,
											FILE_PARSE_INFO parse_info,
											FUNC_INFO func_ctx)
		{
            if (COMN_PROC.IsStandardIdentifier(componentList[idx].Text))
            {
                // 是否是函数参数
                // 是否为局部变量
				if (null != func_ctx
					&& IsLocalVariable(componentList[idx].Text, func_ctx.LocalVarList))
                {
                    MEANING_GROUP retGroup = new MEANING_GROUP();
                    retGroup.Type = MeaningGroupType.LocalVariable;
                    retGroup.ComponentList.Add(componentList[idx]);
                    retGroup.Text = componentList[idx].Text;
                    GetVarMemberGroup(componentList, ref idx, ref retGroup);
                    return retGroup;
                }
                // 是否为全局变量
				else if (null != parse_info.FindGlobalVarInfoByName(componentList[idx].Text))
                {
                    MEANING_GROUP retGroup = new MEANING_GROUP();
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
                    MEANING_GROUP retGroup = new MEANING_GROUP();
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
				List<STATEMENT_COMPONENT> braceList = COMN_PROC.GetBraceComponents(componentList, ref tmp_idx);
                if (null != braceList
                    && (tmp_idx != componentList.Count - 1)
                    && ("." == componentList[tmp_idx + 1].Text
                        || "->" == componentList[tmp_idx + 1].Text))
                {
                    MEANING_GROUP retGroup = new MEANING_GROUP();
					retGroup.Type = GetVariableType(braceList, parse_info);
					retGroup.ComponentList.AddRange(braceList);
					retGroup.Text = COMN_PROC.GetComponentListStr(braceList);
                    GetVarMemberGroup(componentList, ref tmp_idx, ref retGroup);
                    idx = tmp_idx;
                    return retGroup;
                }
            }
            return null;
		}

		static MeaningGroupType GetVariableType(List<STATEMENT_COMPONENT> braceList, FILE_PARSE_INFO parse_info)
		{
			foreach (STATEMENT_COMPONENT item in braceList)
			{
				if (COMN_PROC.IsStandardIdentifier(item.Text))
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

        static void GetVarMemberGroup(List<STATEMENT_COMPONENT> componentList,
                                      ref int idx,
                                      ref MEANING_GROUP retGroup)
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
					List<STATEMENT_COMPONENT> braceList = COMN_PROC.GetBraceComponents(componentList, ref idx);
					retGroup.ComponentList.AddRange(braceList);
					retGroup.Text += COMN_PROC.GetComponentListStr(braceList);
					i = idx;
				}
                else
                {
                    break;
                }
            }
            idx = i;
        }

		static MEANING_GROUP GetFunctionCallingGroup(List<STATEMENT_COMPONENT> componentList, ref int idx, FILE_PARSE_INFO parse_info)
		{
			if (COMN_PROC.IsStandardIdentifier(componentList[idx].Text))
			{
				// 判断是否是函数名
				if (null != parse_info.FindFuncParseInfo(componentList[idx].Text)
					&& "(" == componentList[idx + 1].Text)
				{
					MEANING_GROUP retGroup = new MEANING_GROUP();
					retGroup.Type = MeaningGroupType.FunctionCalling;
					retGroup.ComponentList.Add(componentList[idx]);
					retGroup.Text += componentList[idx].Text;
					idx += 1;
					List<STATEMENT_COMPONENT> braceList = COMN_PROC.GetBraceComponents(componentList, ref idx);
					if (null != braceList)
					{
						retGroup.ComponentList.AddRange(braceList);
						retGroup.Text += COMN_PROC.GetComponentListStr(braceList);
						idx += 1;
						return retGroup;
					}
				}
			}
			return null;
		}

		static MEANING_GROUP GetExpressionGroup(List<STATEMENT_COMPONENT> componentList, ref int idx, FILE_PARSE_INFO parse_info)
		{
            if ("(" == componentList[idx].Text)
            {
				List<STATEMENT_COMPONENT> braceList = COMN_PROC.GetBraceComponents(componentList, ref idx);
                if (null != braceList)
                {
					MEANING_GROUP retGroup = new MEANING_GROUP();
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
					retGroup.ComponentList.AddRange(braceList);
					retGroup.Text = COMN_PROC.GetComponentListStr(braceList);
					idx += 1;
					return retGroup;
				}
            }
			return null;
		}

		static MEANING_GROUP GetCodeBlockGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_result)
		{
			if ("{" == cpnt_list[idx].Text)
			{
				List<STATEMENT_COMPONENT> braceList = COMN_PROC.GetBraceComponents(cpnt_list, ref idx);
				if (null != braceList)
				{
					MEANING_GROUP retGroup = new MEANING_GROUP();
					retGroup.Type = MeaningGroupType.CodeBlock;
					retGroup.ComponentList.AddRange(braceList);
					retGroup.Text = COMN_PROC.GetComponentListStr(braceList);
					idx += 1;
					return retGroup;
				}
			}
			return null;
		}

		static MEANING_GROUP GetStringBlockGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_result)
		{
			if ("\"" == cpnt_list[idx].Text)
			{
				MEANING_GROUP retGroup = new MEANING_GROUP();
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

		static MEANING_GROUP GetCharBlockGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_result)
		{
			if ("\'" == cpnt_list[idx].Text
				&& idx < cpnt_list.Count - 3
				&& "\'" == cpnt_list[idx + 2].Text)
			{
				MEANING_GROUP retGroup = new MEANING_GROUP();
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

		static MEANING_GROUP GetOperatorGroup(List<STATEMENT_COMPONENT> componentList, ref int idx)
		{
            MEANING_GROUP retGroup = null;
            if (componentList[idx].Type == StatementComponentType.Operator)
            {
                retGroup = new MEANING_GROUP();
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

        static void MeaningGroupsAnalysis(	List<MEANING_GROUP> mgList,
											FILE_PARSE_INFO parse_info,
											FUNC_INFO func_ctx,
											STATEMENT_NODE s_node)
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

		public static VAR_CTX IsNewDefineVarible(List<MEANING_GROUP> mgList, FILE_PARSE_INFO parse_info, FUNC_INFO func_ctx)
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
					MEANING_GROUP initGroup = null;
					if (mgList.Count >= 4 && mgList[2].Text.Equals("="))
					{
						initGroup = new MEANING_GROUP();
						for (int i = 3; i < mgList.Count; i++)
						{
							initGroup.ComponentList.AddRange(mgList[i].ComponentList);
							initGroup.Text += " " + mgList[i].Text;
						}
						initGroup.Text = initGroup.Text.Trim();
						if (4 == mgList.Count)
						{
							initGroup.Type = mgList[3].Type;
						}
						else
						{
							initGroup.Type = MeaningGroupType.Expression;
						}
					}
					varCtx = InOutAnalysis.CreateVarCtx(mgList[0], mgList[1].Text, initGroup, parse_info);
					return varCtx;
				}
            }
            return null;
        }

		public static void TypeDefProc(	List<STATEMENT_COMPONENT> component_list,
										FILE_PARSE_INFO parse_info,
										FUNC_INFO func_ctx)
		{
			// 提取含义分组
			List<MEANING_GROUP> meaningGroupList = GetMeaningGroups(component_list, parse_info, func_ctx);
			if (3 == meaningGroupList.Count
				&& "typedef" == meaningGroupList[0].Text
				&& meaningGroupList[1].Type == MeaningGroupType.VariableType
				&& meaningGroupList[2].Type == MeaningGroupType.Identifier)
			{
				TYPE_DEFINE_INFO tdi = new TYPE_DEFINE_INFO();
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

	public class STATEMENT_COMPONENT
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

		public STATEMENT_COMPONENT()
		{
		}

		public STATEMENT_COMPONENT(string str)
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
	public class MEANING_GROUP
	{
		public MeaningGroupType Type = MeaningGroupType.Unknown;

		public string Text = string.Empty;

		public List<STATEMENT_COMPONENT> ComponentList = new List<STATEMENT_COMPONENT>();

		public int PrefixCount = 0;														// 如果分组是类型的话, 还包括前后缀的个数
		public int SuffixCount = 0;
	}

    /// <summary>
    /// 解析上下文
    /// </summary>
    public class FUNC_INFO
    {
        // 引数列表
		public List<VAR_CTX> ParameterList = new List<VAR_CTX>();
        // 局部变量列表
		public List<VAR_CTX> LocalVarList = new List<VAR_CTX>();
		// 入力全局变量列表
		public List<VAR_DESCRIPTION> InputGlobalList = new List<VAR_DESCRIPTION>();
		// 出力全局变量列表
		public List<VAR_DESCRIPTION> OutputGlobalList = new List<VAR_DESCRIPTION>();
		// 其它未确定入出力的全局变量(比如函数调用读出值)
		public List<VAR_CTX> OtherGlobalList = new List<VAR_CTX>();
		// 调用函数列表
		public List<CALLED_FUNCTION> CalledFunctionList = new List<CALLED_FUNCTION>();
	}

	/// <summary>
	/// 函数调用
	/// </summary>
	public class CALLED_FUNCTION
	{
		public string FunctionName = string.Empty;										// 函数名
		public MEANING_GROUP MeaningGroup = new MEANING_GROUP();
		public List<ACTUAL_PARA_INFO> ActualParaInfoList = new List<ACTUAL_PARA_INFO>();// 实参情报列表
		public string ReturnValType = string.Empty;
	}
}
