using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		// 7.语句状态(for, if-else等条件判定状态等, 待定)
		public string StatusStr = string.Empty;

		// 8.该语句是否完成遍历
		public bool IsPassed = false;

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
}
