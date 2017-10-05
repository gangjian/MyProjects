using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mr.Robot;

namespace Mr.Robot.CDeducer
{
	public partial class C_DEDUCER
	{
		FILE_PARSE_INFO m_SourceParseInfo = null;
		string m_FunctionName = string.Empty;

		public C_DEDUCER(FILE_PARSE_INFO src_parse_info, string func_name)
		{
			this.m_SourceParseInfo = src_parse_info;
			this.m_FunctionName = func_name;
		}

		/// <summary>
		/// 语句分析
		/// </summary>
        public static FUNC_CONTEXT DeducerStart(STATEMENT_NODE root, FILE_PARSE_INFO parse_info)
		{
            FUNC_CONTEXT fCtx = new FUNC_CONTEXT();
			DEDUCER_CONTEXT dCtx = new DEDUCER_CONTEXT();
			// 顺次解析各条语句
			foreach (STATEMENT_NODE childNode in root.ChildNodeList)
			{
				StatementProc(childNode, parse_info, fCtx, dCtx);
			}
            return fCtx;
		}

		DEDUCER_CONTEXT m_DeducerContext = new DEDUCER_CONTEXT();
		STATEMENT_NODE m_LastStepNode = null;

		public void DeducerStart2()
		{
			// 函数根节点
			STATEMENT_NODE func_root = C_FUNC_LOCATOR.FuncLocatorStart2(this.m_SourceParseInfo, this.m_FunctionName);
			// 处理参数列表
			FuncParaListProc();

			while (DeducerRun(func_root, this.m_SourceParseInfo))
			{
				// 每次遍历一条路径, 所有能走的路径都走完了, 结束遍历
			}
		}

		void FuncParaListProc()
		{
			FUNCTION_PARSE_INFO funcInfo = FILE_PARSE_INFO.SearchFunctionInfoList(this.m_FunctionName, this.m_SourceParseInfo.FunDefineList);
			if (null == funcInfo
				|| 0 == funcInfo.ParaList.Count)
			{
				return;
			}
			foreach (var item in funcInfo.ParaList)
			{
				
			}
		}

		bool DeducerRun(STATEMENT_NODE root_node, FILE_PARSE_INFO parse_info)
		{
			bool procFlag = false;
			this.m_LastStepNode = null;
			while (true)
			{
				STATEMENT_NODE nextStep = GetNextStep(root_node);
				if (null == nextStep)
				{
					break;
				}
				else
				{
					// 处理语句
					StatementProc(nextStep, parse_info, null, this.m_DeducerContext);
					nextStep.IsPassed = true;
					this.m_LastStepNode = nextStep;
					procFlag = true;
				}
			}
			return procFlag;
		}

		const string FOR_ENTER = "ForEnter";
		const string FOR_NOT_ENTER = "ForNotEnter";
		const string FOR_OUT = "ForOut";
		STATEMENT_NODE GetNextStep(STATEMENT_NODE root_node)
		{
			if (null == this.m_LastStepNode)
			{
				return FindFirstUnreachedNode(root_node);
			}
			else
			{
				switch (this.m_LastStepNode.Type)
				{
					case E_STATEMENT_TYPE.Simple:
						return GetNextBrotherOrParent(this.m_LastStepNode);
					case E_STATEMENT_TYPE.Compound_IfElse:
						// 根据if-else复合语句的状态,判定进入分支还是跳过,注意子语句为空的情况也算进入
						if (FOR_ENTER == this.m_LastStepNode.StatusStr)
						{
							if (null != this.m_LastStepNode.ChildNodeList.First())
							{
								return this.m_LastStepNode.ChildNodeList.First();
							}
							else
							{
								this.m_LastStepNode.StatusStr = FOR_OUT;
								return this.m_LastStepNode;
							}
						}
						else if (FOR_NOT_ENTER == this.m_LastStepNode.StatusStr)
						{
							return GetNextBrotherOrParent(this.m_LastStepNode);
						}
						else
						{
							System.Diagnostics.Trace.Assert(false);
						}
						break;
					case E_STATEMENT_TYPE.Compound_SwitchCase:
						break;
					case E_STATEMENT_TYPE.Compound_While:
						break;
					case E_STATEMENT_TYPE.Compound_For:
						break;
					case E_STATEMENT_TYPE.Compound_DoWhile:
						break;
					case E_STATEMENT_TYPE.Compound_GoTo:
						break;
					case E_STATEMENT_TYPE.Compound_Block:
						break;
					case E_STATEMENT_TYPE.Branch_If:
						break;
					case E_STATEMENT_TYPE.Branch_ElseIf:
						break;
					case E_STATEMENT_TYPE.Branch_Else:
						break;
					case E_STATEMENT_TYPE.Branch_Case:
						break;
					case E_STATEMENT_TYPE.Branch_Default:
						break;
					default:
						break;
				}
			}
			return null;
		}

		STATEMENT_NODE FindFirstUnreachedNode(STATEMENT_NODE root_node)
		{
			foreach (var child in root_node.ChildNodeList)
			{
				if (!child.IsPassed)
				{
					STATEMENT_NODE subNode = FindFirstUnreachedNode(child);
					if (null != subNode)
					{
						return subNode;
					}
					else
					{
						return child;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 根据语句标号找到指定的语句节点
		/// </summary>
		static STATEMENT_NODE GetTargetNodeByStepMark(STATEMENT_NODE root_node, string step_mark)
		{
			foreach (var item in root_node.ChildNodeList)
			{
				if (item.StepMarkStr.Equals(step_mark))
				{
					return item;
				}
				if (step_mark.StartsWith(item.StepMarkStr))
				{
					return GetTargetNodeByStepMark(item, step_mark);
				}
			}
			return null;
		}

		/// <summary>
		/// 找到指定节点的下一个兄弟节点
		/// </summary>
		static STATEMENT_NODE GetNextBrotherNode(STATEMENT_NODE target_node)
		{
			string curStepMark = target_node.StepMarkStr;
			STATEMENT_NODE parentNode = target_node.ParentNode;
			for (int i = 0; i < parentNode.ChildNodeList.Count; i++)
			{
				if (parentNode.ChildNodeList[i].StepMarkStr.Equals(curStepMark)
					&& i < parentNode.ChildNodeList.Count - 1)
				{
					return parentNode.ChildNodeList[i + 1];
				}
			}
			return null;
		}

		static STATEMENT_NODE GetNextBrotherOrParent(STATEMENT_NODE target_node)
		{
			STATEMENT_NODE retNode = GetNextBrotherNode(target_node);
			if (null != retNode)
			{
				return retNode;
			}
			else
			{
				return target_node.ParentNode;
			}
		}

		/// <summary>
		/// 还原Deducer上下文到指定节点位置
		/// </summary>
		static void RestoreDeducerContext(DEDUCER_CONTEXT d_ctx, string step_mark)
		{
			// TODO
		}

		public static void StatementProc(STATEMENT_NODE s_node, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx, DEDUCER_CONTEXT deducer_ctx)
		{
			switch (s_node.Type)
			{
				case E_STATEMENT_TYPE.Simple:
					// 取得完整的语句内容
					string statementStr = COMN_PROC.GetStatementStr(parse_info.CodeList, s_node.Scope);
					SimpleStatementProc(statementStr, parse_info, func_ctx, deducer_ctx, s_node);
					break;
				case E_STATEMENT_TYPE.Compound_IfElse:
					break;
				case E_STATEMENT_TYPE.Compound_For:
					break;
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
		}

		/// <summary>
		/// 简单语句分析(函数内)
		/// </summary>
		public static void SimpleStatementProc(string statement_str, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx, DEDUCER_CONTEXT deducer_ctx, STATEMENT_NODE s_node)
		{
			// 按顺序提取出语句各组成部分: 运算数(Operand)和运算符(Operator)
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statement_str, parse_info);
			if (statement_str.StartsWith("typedef"))
			{
				COMN_PROC.TypeDefProc(componentList, parse_info, func_ctx);
			}
			else
			{
				ExpressionProc(componentList, parse_info, func_ctx, deducer_ctx, s_node);
			}
		}

        public static void ExpressionProc(List<STATEMENT_COMPONENT> component_list, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx, DEDUCER_CONTEXT deducer_ctx, STATEMENT_NODE s_node)
        {
            // 提取含义分组
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(component_list, parse_info, func_ctx);

            // 含义分组解析
			MeaningGroupsAnalysis(meaningGroupList, parse_info, func_ctx, deducer_ctx, s_node);
			//int a, b, c = 20;
			//(a) = b = c + 3;
		}

        static void MeaningGroupsAnalysis(List<MEANING_GROUP> mgList, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx, DEDUCER_CONTEXT deducer_ctx, STATEMENT_NODE s_node)
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

			/////////////////////// 以下是新方案 /////////////////////
			if (IfNewDefVar(mgList, parse_info))
			{
				string stepMarkStr = string.Empty;
				if (null != s_node)
				{
					stepMarkStr = s_node.StepMarkStr;
				}
				VAR_CTX2 varCtx2 = D_COMMON.CreateVarCtx2(mgList, parse_info, stepMarkStr);
				if (null != deducer_ctx)
				{
					deducer_ctx.VarCtxList.Add(varCtx2);
				}
			}
			else
			{
			}
        }

		static VAR_CTX IsNewDefineVarible(List<MEANING_GROUP> mgList, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
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

		static bool IfNewDefVar(List<MEANING_GROUP> mgList, FILE_PARSE_INFO parse_info)
		{
			if (mgList.Count < 2
				|| MeaningGroupType.VariableType != mgList[0].Type)
			{
				return false;
			}
			return true;
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
    public class FUNC_CONTEXT
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

		public bool IsLocalVariable(string identifier)
		{
			foreach (VAR_CTX vctx in this.LocalVarList)
			{
				if (vctx.Name.Equals(identifier))
				{
					return true;
				}
			}
			return false;
		}
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
