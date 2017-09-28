using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mr.Robot;

namespace Mr.Robot.CDeducer
{
	public static partial class C_DEDUCER
	{
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

		public static void StatementProc(	STATEMENT_NODE s_node,
											FILE_PARSE_INFO parse_info,
                                            FUNC_CONTEXT func_ctx,
											DEDUCER_CONTEXT deducer_ctx)
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
		public static void SimpleStatementProc(	string statement_str,
												FILE_PARSE_INFO parse_info,
												FUNC_CONTEXT func_ctx,
												DEDUCER_CONTEXT deducer_ctx,
												STATEMENT_NODE s_node)
		{
			// 按顺序提取出语句各组成部分: 运算数(Operand)和运算符(Operator)
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statement_str, parse_info);
			if (statement_str.StartsWith("typedef"))
			{
				COMN_PROC.TypeDefProc(componentList, parse_info, func_ctx);
			}
			else
			{
				ExpressionProc(componentList, parse_info, func_ctx, s_node);
			}
		}

        public static void ExpressionProc(	List<STATEMENT_COMPONENT> component_list,
											FILE_PARSE_INFO parse_info,
											FUNC_CONTEXT func_ctx,
											STATEMENT_NODE s_node)
        {
            // 提取含义分组
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(component_list, parse_info, func_ctx);

            // 含义分组解析
			MeaningGroupsAnalysis(meaningGroupList, parse_info, func_ctx, s_node);
			//int a, b, c = 20;
			//(a) = b = c + 3;
		}

        static void MeaningGroupsAnalysis(	List<MEANING_GROUP> mgList,
											FILE_PARSE_INFO parse_info,
											FUNC_CONTEXT func_ctx,
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

		public static VAR_CTX IsNewDefineVarible(List<MEANING_GROUP> mgList, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
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
