using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mr.Robot.CDeducer;

namespace Mr.Robot
{
    public class STATEMENT_NODE
    {
        // 1.语句范围(起止位置)
		public CODE_SCOPE Scope = null;

        // 2.父语句节点引用
        public STATEMENT_NODE ParentNode = null;
        // 3.子语句节点列表
        public List<STATEMENT_NODE> ChildNodeList = new List<STATEMENT_NODE>();
        // 4.语句类型
		public E_STATEMENT_TYPE Type = E_STATEMENT_TYPE.Invalid;

        // 5.条件表达式(可以为空,表示恒成立)
        public string ExpressionStr = string.Empty;

		// 6.语句标号
		public string StepMarkStr = string.Empty;

		// 7.分支状态
		public BRANCH_STATUS BranchStatus = BRANCH_STATUS.NOT_PASSED;

		// 8.进入该Branch的入力取值限制条件表达式
		public SIMPLIFIED_EXPRESSION EnterExpression = null;

		/// <summary>
		/// 从当前节点出发,找到指定的节点
		/// </summary>
		public STATEMENT_NODE GetOtherNode(string step_mark)
		{
			STATEMENT_NODE curNode = this;
			while (null != curNode.ParentNode)
			{
				curNode = curNode.ParentNode;
			}
			string[] stepArr = step_mark.Split(',');
			string stepStr = string.Empty;
			foreach (var step in stepArr)
			{
				if (!string.IsNullOrEmpty(stepStr))
				{
					stepStr += ",";
				}
				stepStr += step;
				bool findChild = false;
				foreach (var child in curNode.ChildNodeList)
				{
					if (child.StepMarkStr.Equals(stepStr))
					{
						curNode = child;
						findChild = true;
						break;
					}
				}
				System.Diagnostics.Trace.Assert(findChild);
			}
			return curNode;
		}

		public SIMPLIFIED_EXPRESSION GetBranchEnterExpression(FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			// 取得分支条件表达式
			string expr_str = this.GetBranchExprStr();
			while (true)
			{
				// 表达式化简
				SIMPLIFIED_EXPRESSION splfExp = LOGIC_EXPRESSION_SIMPLIFY.ExpressionSimplify_Phase1(expr_str, parse_info, deducer_ctx);
				VAR_CTX2 varCtx = deducer_ctx.FindVarCtxByName(splfExp.VarName);
				if (varCtx.VarCategory == VAR_CATEGORY.FUNC_PARA
					|| varCtx.VarCategory == VAR_CATEGORY.GLOBAL)
				{
					// 如果是入力(函数参数,全局量或者函数调用返回值/读出参数)
					// 取得入力量相对当前表达式的约束条件,并判断跟既存约束条件是否兼容
					if (varCtx.CheckValueLimitPossible(new VAL_LIMIT_EXPR(splfExp.OprtStr, splfExp.ValStr)))
					{
						return splfExp;
					}
					else
					{
						return null;
					}
				}
				else if (varCtx.VarCategory == VAR_CATEGORY.LOCAL)
				{
					// 如果是临时变量,要根据赋值记录进一步代入化简
					VAR_RECORD record = varCtx.GetLastAssignmentRecord();
					System.Diagnostics.Trace.Assert(null != record);
					// 取得赋值节点
					STATEMENT_NODE assignmentNode = this.GetOtherNode(record.StepMarkStr);
					System.Diagnostics.Trace.Assert(null != assignmentNode);
					// 取得赋值表达式,用赋值表达式替换原变量继续化简
					string assignmentStr = GetAssignmentStr(assignmentNode.ExpressionStr, parse_info, deducer_ctx);
					expr_str = assignmentStr + splfExp.OprtStr + splfExp.ValStr;
				}
			}
		}

		/// <summary>
		/// 取得进入分支的表达式
		/// </summary>
		string GetBranchExprStr()
		{
			return this.ExpressionStr;
		}

		string GetAssignmentStr(string expr_str, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			List<MEANING_GROUP> meaningGroups = COMN_PROC.GetMeaningGroups2(expr_str, parse_info, deducer_ctx);
			// 注意:除了等号"="赋值运算符, 还有可能是"+=", "|="等其它赋值运算符
			System.Diagnostics.Trace.Assert(3 == meaningGroups.Count && meaningGroups[1].TextStr.Equals("="));
			return meaningGroups[2].TextStr;
		}
    }

    // 语句节点类型枚举
    public enum E_STATEMENT_TYPE
    {
        Invalid,
        Root,                 // 根节点(函数体)
        Simple,               // 简单句

        // 以下是复合语句
        Compound_IfElse,      // if-else分支语句
        Compound_SwitchCase,  // switch-case分支语句
        Compound_While,       // while循环语句
        Compound_For,         // for循环语句
        Compound_DoWhile,     // do-while循环语句
        Compound_GoTo,        // goto跳转语句
        Compound_Block,       // {}括起来的语句块

        // 以下是复合语句的分支语句
        Branch_If,            // if分支
        Branch_ElseIf,        // else if分支
        Branch_Else,          // else分支
        Branch_Case,          // case分支
        Branch_Default,       // default分支
    }

	public enum BRANCH_STATUS
	{
		NOT_PASSED,
		PASSED,
		PASSING,
		CAN_NOT_ENTER,
	}
}
