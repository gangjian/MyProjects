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
		static public void FunctionStatementsAnalysis(StatementNode root,
													  CCodeParseResult parseResult)
		{
			List<VariableInfo> localVarList = new List<VariableInfo>();			// 局部变量列表
			// 顺次解析各条语句
			foreach (StatementNode childNode in root.childList)
			{
				StatementAnalysis(childNode, parseResult, localVarList);
			}
		}

		static void StatementAnalysis(StatementNode node,
									  CCodeParseResult parseResult,
									  List<VariableInfo> localVarList)
		{
			switch (node.Type)
			{
				case StatementNodeType.Simple:
					SimpleStatementAnalysis(node, parseResult, localVarList);
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
											CCodeParseResult parseResult,
											List<VariableInfo> localVarList)
		{

			// (1).按顺序取出语句各组成部分: 运算数(Operand)和运算符(Operator)
            List<StatementComponent> componentList = GetStatementComponents(statementNode, parseResult);

            // (2).对各组成部分进行分析
			StatementComponentsAnalysis(componentList);
		}

        static List<StatementComponent> GetStatementComponents(StatementNode statementNode,
                                                               CCodeParseResult parseResult)
        {
            // 取得完整的语句内容
            string statementStr = LineStringCat(parseResult.SourceParseInfo.parsedCodeList,
                                                statementNode.Scope.Start,
                                                statementNode.Scope.End);

            List<StatementComponent> componentList = new List<StatementComponent>();
            int offset = 0;
            do
            {
                // 提取语句的各个组成部分(操作数或者是操作符)
                StatementComponent cpnt = GetSingleComponent(ref statementStr, ref offset, parseResult);
                if (StatementComponentType.StatementEnd == cpnt.Type)
                {
                    // 语句结束
                    break;
                }
                else if (StatementComponentType.Invalid == cpnt.Type)
                {
                    ErrReport();
                    break;
                }
                else
                {
                    componentList.Add(cpnt);
                }
            } while (true);

            return componentList;
        }

		static void StatementComponentsAnalysis(List<StatementComponent> componentList)
		{
			// (1). 首先对语句所有组成部分进行结构分组, 每个组代表一个独立完整的语义结构
			List<ComponentsGroup> cpntsGroupList = GetComponentsGroupList(componentList);

			return;
		}

		/// <summary>
		/// 对语句所有构成成分进行结构分组
		/// </summary>
		static List<ComponentsGroup> GetComponentsGroupList(List<StatementComponent> componentList)
		{
			List<ComponentsGroup> groupList = new List<ComponentsGroup>();
			int idx = 0;
			while (true) 
			{
				ComponentsGroup newGroup = GetOneComponentsGroup(componentList, ref idx);
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
		static ComponentsGroup GetOneComponentsGroup(List<StatementComponent> componentList, ref int idx)
		{
			ComponentsGroup retGroup = new ComponentsGroup();
			while (true)
			{
				if (idx > componentList.Count - 1)
				{
					break;
				}
				StatementComponent cpnt = componentList[idx];
				if (StatementComponentType.Operator == cpnt.Type)				// 如果是操作符
				{
					if (	("(" == cpnt.Text)
						||	("[" == cpnt.Text)	)
					{
						List<StatementComponent> braceList = GetBraceComponents(componentList, ref idx);
						if (null != braceList)
						{
							retGroup.ComponentList.AddRange(braceList);
						}
					}
					else if (	("." == cpnt.Text)
							 || ("->" == cpnt.Text)	)
					{
						retGroup.ComponentList.Add(cpnt);
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
				{																// 非操作符, 操作数
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
		static StatementComponent GetSingleComponent(ref string statementStr,
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
				if (";" == idStr)
				{
					retSC.Type = StatementComponentType.StatementEnd;			// 语句结束
					break;
				}
				else if (IsConstantNumber(idStr))
				{
                    retSC.Type = StatementComponentType.ConstantNumber;
                    retSC.Text = idStr;
					break;														// 数字常量
				}
				else if (IsStringOrChar(idStr, statementStr, ref offset))
				{
					break;														// 字符或者字符串
				}
				else if (IsStandardIdentifier(idStr))							// 标准标识符
				{
					// 如果包含宏, 首先要进行宏展开
					if (MacroDetectAndExpand_Function(idStr, ref statementStr, offset, parseResult))
					{
						offset = offset_old;
						continue;
					}
					else
					{
						retSC = GetStandardIdentifierComponent(idStr, statementStr, ref offset, parseResult);
						break;
					}
				}
				else if (IsOperator(idStr, statementStr, ref offset, ref retSC))
				{
					break;
				}
				else
				{
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
							ErrReport();
							break;
						}
						int leftBracket = offset;
						int rightBracket = statementStr.Substring(offset).IndexOf(')');
						if (-1 == rightBracket)
						{
							ErrReport();
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
						ErrReport();
						return false;
					}
					// 用宏值去替换原来的宏名(宏展开)
					statementStr = statementStr.Replace(macroName, replaceStr);
					return true;
				}
			}

			return false;
        }

		static StatementComponentType GetStandardIdentifierType(string identifier,
																ref string statementStr,
																int offset,
																List<CFileParseInfo> headerList)
        {
            // 可能是基本类型名
            if (IsBasicVarType(identifier))
            {
                return StatementComponentType.BasicVarType;
            }
            // 可能是用户定义类型名
            else if (IsUsrDefVarType(identifier, headerList))
            {
                return StatementComponentType.UsrDefVarType;
            }
            // 可能是函数名
            else if (IsFunctionName(identifier, headerList))
            {
                return StatementComponentType.FunctionName;
            }
            // 可能是全局变量
            else if (IsGlobalVariable(identifier, headerList))
            {
                return StatementComponentType.GlobalVariable;
            }
            else
            {
                return StatementComponentType.Unknown;
            }
        }

		static StatementComponent GetStandardIdentifierComponent(string idStr,
																 string statementStr,
																 ref int offset,
																 CCodeParseResult parseResult)
		{
			int startOffset = offset;
			StatementComponent retSC = new StatementComponent();
			StatementComponentType idType = StatementComponentType.Invalid;

			retSC.Text = idStr;
			idType = GetStandardIdentifierType(idStr, ref statementStr, offset, parseResult.IncHdParseInfoList);
			if (StatementComponentType.BasicVarType == idType
				|| StatementComponentType.UsrDefVarType == idType)
			{
				// 如果是类型名开头, 可能是变量定义
				retSC.Type = idType;
			}
			else if (StatementComponentType.FunctionName == idType)
			{
				// 函数名
				retSC.Type = StatementComponentType.FunctionName;
			}
			else
			{
				retSC.Type = StatementComponentType.Unknown;
			}
			return retSC;
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
			offset += 1;
			string nextIdStr = statementStr.Substring(offset, 1);
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
						component.Priority = 4;									// 不确定, 也可能是单目运算符的正负号(优先级为2)
					}
					break;
				case "*":														// 不确定, 可能是乘号, 也可能是指针运算符
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
						component.Priority = 7;									// 不确定, 可能是双目位与&,也可能是取地址符&(优先级2)
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
                E_CHAR_TYPE cType = GetCharType(curChar);
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
                        ErrReport();
                        return null;
                }
                if (-1 != s_pos && -1 != e_pos)
                {
                    return statementStr.Substring(s_pos, e_pos - s_pos + 1);
                }
            }
			return null;
		}

        /// <summary>
        /// 判断标识符是否是系统基本类型
        /// </summary>
        static bool IsBasicVarType(string identifier)
        {
            if (   ("int" == identifier)
                || ("char" == identifier)
                || ("short" == identifier)
                || ("long" == identifier)
                || ("double" == identifier)
                || ("float" == identifier)
                || ("signed" == identifier)
                || ("unsigned" == identifier)
                || ("void" == identifier)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断标识符是否是构造类型/用户定义类型
        /// </summary>
        static bool IsUsrDefVarType(string identifier, List<CFileParseInfo> headerList)
        {
            // 遍历头文件列表
            foreach (CFileParseInfo hfi in headerList)
            {
                // 首先查用户定义类型列表
                foreach (UsrDefTypeInfo udi in hfi.user_def_type_list)
                {
                    foreach (string typeName in udi.nameList)
                    {
                        if (typeName.Equals(identifier))
                        {
                            return true;
                        }
                    }
                }
                // 然后是typedef列表
                foreach (TypeDefineInfo tdi in hfi.type_define_list)
                {
                    if (tdi.new_type_name.Equals(identifier))
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
        /// 判断标识符是否是函数名
        /// </summary>
        static bool IsFunctionName(string identifier, List<CFileParseInfo> headerList)
        {
            // 遍历头文件列表
            foreach (CFileParseInfo hfi in headerList)
            {
                // 首先是函数声明列表
                foreach (CFunctionStructInfo fi in hfi.fun_declare_list)
                {
                    if (fi.name.Equals(identifier))
                    {
                        return true;
                    }
                }
                // 然后是函数定义列表
                foreach (CFunctionStructInfo fi in hfi.fun_define_list)
                {
                    if (fi.name.Equals(identifier))
                    {
                        return true;
                    }
                }
            }
            return false;
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
        static bool IsLocalVariable(string identifier, List<VariableInfo> localVarList)
        {
			foreach (VariableInfo vi in localVarList)
			{
				if (vi.varName.Equals(identifier))
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
	}

	public enum StatementComponentType
	{
		Invalid,				// 无效
		Unknown,				// 未知

		BasicVarType,			// 基本数据类型
        UsrDefVarType,			// 用户定义数据类型/构造类型
		GlobalVariable,			// 全局变量名
		LocalVariable,			// 局部变量名
        ConstantNumber,         // 数值常量
        String,                 // 字符串
        Char,                   // 字符

		FunctionName,		    // 函数名
		StatementEnd,			// 语句结束(分号)
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
	}

	public enum StatementGroupType
	{
		Invalid,
		VarType,
		Variable,
		FunctionCalling,
	}

	/// <summary>
	/// 一组有意义的语句成分构成的分组
	/// </summary>
	public class ComponentsGroup
	{
		StatementGroupType Type = StatementGroupType.Invalid;

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
    /// C函数解析情报类
    /// </summary>
    public class CFunctionAnalysisInfo
    {
        string name = "";
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        // 参数列表
        public List<VariableInfo> parameter_list = new List<VariableInfo>();
        // 入力列表
        public List<VariableInfo> input_list = new List<VariableInfo>();
        // 出力列表
        public List<VariableInfo> output_list = new List<VariableInfo>();

    }

}


