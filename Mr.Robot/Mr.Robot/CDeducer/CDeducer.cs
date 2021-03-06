﻿using System;
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

		DEDUCER_CONTEXT m_DeducerContext = null;

		public void DeducerStart2()
		{
			// 函数根节点
			STATEMENT_NODE func_root = C_FUNC_LOCATOR.FuncLocatorStart2(this.m_SourceParseInfo, this.m_FunctionName);

			// 创建上下文
			this.m_DeducerContext = new DEDUCER_CONTEXT();

			// 处理参数列表
			FuncParaListProc(this.m_FunctionName, this.m_SourceParseInfo, this.m_DeducerContext);

			while (true)
			{
				// 每次遍历一条路径
				DeducerRun(func_root, this.m_SourceParseInfo);
				if (0 == this.m_DeducerContext.ForkPointList.Count)
				{
					// 跑完一趟后发现没有分歧点了,说明所有能走的路径都走完了, 结束遍历
					break;
				}
			}
		}

		void FuncParaListProc(string func_name, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			FunctionParseInfo funcInfo = FILE_PARSE_INFO.SearchFunctionInfoList(func_name, parse_info.FunDefineList);
			if (null == funcInfo
				|| 0 == funcInfo.ParaList.Count)
			{
				return;
			}
			foreach (var item in funcInfo.ParaList)
			{
				List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups2(item, parse_info, deducer_ctx);
				MEANING_GROUP typeGroup = meaningGroupList.First();
				meaningGroupList.RemoveAt(0);
				VAR_CTX2 varCtx2 = D_COMMON.CreateVarCtx2(typeGroup, meaningGroupList, parse_info, string.Empty, VAR_CATEGORY.FUNC_PARA);
				deducer_ctx.VarCtxList.Add(varCtx2);
			}
		}

		void DeducerRun(STATEMENT_NODE root_node, FILE_PARSE_INFO parse_info)
		{
			STATEMENT_NODE nextStep = null;
			if (0 == this.m_DeducerContext.ForkPointList.Count)
			{
				// 歧路点列表为空,说明是首次跑,返回第一条语句节点
				nextStep = root_node.ChildNodeList.First();
			}
			else
			{
				// TODO:取得列表里最后一个歧路点的下一个分支
				STATEMENT_NODE lastFork = this.m_DeducerContext.ForkPointList.Last();
				nextStep = lastFork.GetNextBrother();
				this.m_DeducerContext.ForkPointList.RemoveAt(this.m_DeducerContext.ForkPointList.Count - 1);
				if (null == nextStep.GetNextBrother())
				{
					// 如果该分支是该歧路点的最后一条分支, 要删掉该歧路点
				}
				else
				{
					// 否则将该分支记入歧路点List
					this.m_DeducerContext.ForkPointList.Add(nextStep);
				}
				// 恢复上下文状态到歧路点位置的状态
				this.m_DeducerContext.RestoreStatus(nextStep.ParentNode.StepMarkStr);
			}
			while (true)
			{
				// 处理语句
				StatementProc(nextStep, parse_info, null, this.m_DeducerContext);
				nextStep = GetNextStep(this.m_DeducerContext);
				if (null == nextStep)
				{
					break;
				}
			}
		}

		STATEMENT_NODE GetNextStep(DEDUCER_CONTEXT deducer_ctx)
		{
			System.Diagnostics.Trace.Assert(null != deducer_ctx.LastStepNode);
			STATEMENT_NODE retNode = null;
			retNode = deducer_ctx.LastStepNode.GetNextBrother();
			if (null != retNode)
			{
				return retNode;
			}
			else
			{
				// 没有下一个兄弟节点,说明走到了分支尽头,要回到上层节点继续向下走
				return GetUpperNext(deducer_ctx.LastStepNode);
			}
		}

		STATEMENT_NODE GetUpperNext(STATEMENT_NODE cur_node)
		{
			while (true)
			{
				STATEMENT_NODE parentNode = cur_node.ParentNode;
				switch (parentNode.GetCategory())
				{
					case STATEMENT_CATEGORY.ROOT:
						return null;
					case STATEMENT_CATEGORY.SIMPLE:
					case STATEMENT_CATEGORY.COMPLEX:
						return parentNode.GetNextBrother();
					case STATEMENT_CATEGORY.BRANCH:
						cur_node = parentNode;
						break;
					default:
						System.Diagnostics.Trace.Assert(false);
						break;
				}
			}
		}

		public static void StatementProc(STATEMENT_NODE s_node, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx, DEDUCER_CONTEXT deducer_ctx)
		{
			System.Diagnostics.Trace.WriteLine("StatementProc ---> " + s_node.StepMarkStr);		// 打印语句节点编号
			if (s_node.Type == STATEMENT_TYPE.Simple)
			{
				// 简单语句
				string statementStr = COMN_PROC.GetStatementStr(parse_info.CodeList, s_node.Scope);
				SimpleStatementProc(statementStr, parse_info, func_ctx, deducer_ctx, s_node);
				if (null != deducer_ctx)
				{
					deducer_ctx.LastStepNode = s_node;
				}
			}
			else
			{
				// 复合语句
				ComplexStatementProc(s_node, parse_info, deducer_ctx);
			}
		}

		public static void ComplexStatementProc(STATEMENT_NODE s_node, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			deducer_ctx.LastStepNode = s_node;
			switch (s_node.Type)
			{
				case STATEMENT_TYPE.Complex_IfElse:
					ComplexIfElseStatementProc(s_node, parse_info, deducer_ctx);
					break;
				case STATEMENT_TYPE.Complex_SwitchCase:
					break;
				case STATEMENT_TYPE.Complex_While:
					break;
				case STATEMENT_TYPE.Complex_For:
					break;
				case STATEMENT_TYPE.Complex_DoWhile:
					break;
				case STATEMENT_TYPE.Complex_GoTo:
					break;
				case STATEMENT_TYPE.Complex_Block:
					break;
				case STATEMENT_TYPE.Branch_If:
					break;
				case STATEMENT_TYPE.Branch_ElseIf:
					break;
				case STATEMENT_TYPE.Branch_Else:
					ElseBranchProc(s_node, parse_info, deducer_ctx);
					break;
				case STATEMENT_TYPE.Branch_Case:
					break;
				case STATEMENT_TYPE.Branch_Default:
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

		static void ComplexIfElseStatementProc(STATEMENT_NODE s_node, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			// 取得第一条没走过的branch
			STATEMENT_NODE ifBanch = s_node.ChildNodeList.First();
			IfBranchProc(ifBanch, parse_info, deducer_ctx);
		}

		static void IfBranchProc(STATEMENT_NODE branch, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			SIMPLIFIED_EXPRESSION enterExpr = branch.GetIfBranchExpression(parse_info, deducer_ctx);
			if (null != enterExpr)
			{
				// 记录分歧点
				deducer_ctx.ForkPointList.Add(branch);
				// 进入分支
				branch.EnterExpression = enterExpr;
				VAR_CTX2 varCtx = deducer_ctx.FindVarCtxByName(enterExpr.VarName);
				System.Diagnostics.Trace.Assert(null != varCtx);				// 在入力值的上下文中标记取值限定的分支号
				varCtx.ValueEvolveList.Add(new VAR_RECORD(VAR_BEHAVE.VALUE_LIMIT, branch.StepMarkStr));
				if (0 != branch.ChildNodeList.Count)
				{
					branch.BranchStatus = BRANCH_STATUS.PASSING;
					StatementProc(branch.ChildNodeList.First(), parse_info, null, deducer_ctx);
				}
				else
				{
					branch.BranchStatus = BRANCH_STATUS.PASSED;
				}
			}
			else
			{
				// 进不去?
				branch.BranchStatus = BRANCH_STATUS.CAN_NOT_ENTER;
			}
		}

		/// <summary>
		/// else分支处理
		/// </summary>
		static void ElseBranchProc(STATEMENT_NODE s_node, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			// 取得前面if, else if各分支的表达式方程组
			STATEMENT_NODE parentNode = s_node.ParentNode;
			System.Diagnostics.Trace.Assert(parentNode.Type == STATEMENT_TYPE.Complex_IfElse);
			List<SIMPLIFIED_EXPRESSION> ifBranchExpStrList = new List<SIMPLIFIED_EXPRESSION>();
			for (int i = 0; i < parentNode.ChildNodeList.Count - 1; i++)
			{
				SIMPLIFIED_EXPRESSION expStr = parentNode.ChildNodeList[i].GetIfBranchExpression(parse_info, deducer_ctx);
				if (null != expStr)
				{
					ifBranchExpStrList.Add(expStr);
				}
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

		public static bool RunUnitTestAll = true;

        static void MeaningGroupsAnalysis(List<MEANING_GROUP> group_list, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx, DEDUCER_CONTEXT deducer_ctx, STATEMENT_NODE s_node)
        {
            // 先检查是否是新定义的局部变量
			VAR_CTX varCtx = null;
			if (null != (varCtx = IsNewDefineVarible(group_list, parse_info, func_ctx)))
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
			//InOutAnalysis.LeftRightValueAnalysis(group_list, parse_info, func_ctx);

			if (RunUnitTestAll)
			{
				return;
			}
			/////////////////////// 以下是新方案 /////////////////////
			MEANING_GROUP typeGroup = null;
			List<List<MEANING_GROUP>> varGroupList = null;
			if (IfNewDefVar2(group_list, parse_info, out typeGroup, out varGroupList)
				&& null != typeGroup
				&& null != varGroupList)
			{																			// 变量声明
				foreach (var varGroup in varGroupList)
				{
					string stepMarkStr = s_node.StepMarkStr;
					if (null != s_node && null != deducer_ctx)
					{
						VAR_CTX2 varCtx2 = D_COMMON.CreateVarCtx2(typeGroup, varGroup, parse_info, stepMarkStr, VAR_CATEGORY.LOCAL);
						deducer_ctx.VarCtxList.Add(varCtx2);
					}
				}
			}
			else if (IsVarAssignment(group_list))
			{																			// 赋值
				VAR_CTX2 varCtx2 = deducer_ctx.FindVarCtxByName(group_list.First().TextStr);
				if (null != varCtx2)
				{
					varCtx2.ValueEvolveList.Add(new VAR_RECORD(VAR_BEHAVE.ASSIGNMENT, s_node.StepMarkStr));
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
				VAR_CTX varCtx = InOutAnalysis.GetVarCtxByName(mgList[1].TextStr, parse_info, func_ctx);
				if (null != varCtx)
				{
					// 在当前上下文中已存在! (作用域覆盖? 正常情况下不应该出现!)
					return varCtx;
				}
				else
				{
					// 创建变量上下文
					MEANING_GROUP initGroup = null;
					if (mgList.Count >= 4 && mgList[2].TextStr.Equals("="))
					{
						initGroup = new MEANING_GROUP();
						for (int i = 3; i < mgList.Count; i++)
						{
							initGroup.ComponentList.AddRange(mgList[i].ComponentList);
							initGroup.TextStr += " " + mgList[i].TextStr;
						}
						initGroup.TextStr = initGroup.TextStr.Trim();
						if (4 == mgList.Count)
						{
							initGroup.Type = mgList[3].Type;
						}
						else
						{
							initGroup.Type = MeaningGroupType.Expression;
						}
					}
					varCtx = InOutAnalysis.CreateVarCtx(mgList[0], mgList[1].TextStr, initGroup, parse_info);
					return varCtx;
				}
            }
            return null;
        }

		static bool IfNewDefVar2(	List<MEANING_GROUP> mgList, FILE_PARSE_INFO parse_info,
									out MEANING_GROUP var_type_group,
									out List<List<MEANING_GROUP>> var_group_list)
		{
			var_type_group = null;
			var_group_list = null;
			if (mgList.Count < 2
				|| MeaningGroupType.VariableType != mgList[0].Type)
			{
				return false;
			}
			else
			{
				var_type_group = mgList[0];
				var_group_list = new List<List<MEANING_GROUP>>();
				List<MEANING_GROUP> tmp_list = new List<MEANING_GROUP>();
				for (int i = 1; i < mgList.Count; i++)
				{
					if (mgList[i].TextStr.Equals(","))
					{
						var_group_list.Add(tmp_list);
						tmp_list = new List<MEANING_GROUP>();
					}
					else
					{
						tmp_list.Add(mgList[i]);
					}
				}
				var_group_list.Add(tmp_list);
				return true;
			}
		}

		static bool IsVarAssignment(List<MEANING_GROUP> group_list)
		{
			if (null != group_list
				&& group_list.Count > 2
				&& COMN_PROC.IsStandardIdentifier(group_list.First().TextStr)
				&& group_list[1].Type == MeaningGroupType.AssignmentMark)
			{
				return true;
			}
			else
			{
				return false;
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
		FuncPara,					// 函数参数
        GlobalVariable,				// 全局变量
        FunctionCalling,			// 函数调用
		Expression,					// 表达式
		AssignmentMark,				// 赋值符号
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
		public string TextStr = string.Empty;
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
