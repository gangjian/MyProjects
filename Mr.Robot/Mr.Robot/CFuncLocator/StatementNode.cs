using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot
{
    public class STATEMENT_NODE
    {
		// 第一部分: CFuncLocator用到的
        // 1.语句范围(起止位置)
		public CODE_SCOPE Scope = null;

        // 2.父语句节点引用
        public STATEMENT_NODE ParentNode = null;
        // 3.子语句节点列表
        public List<STATEMENT_NODE> ChildNodeList = new List<STATEMENT_NODE>();
        // 4.语句类型
		public E_STATEMENT_TYPE Type = E_STATEMENT_TYPE.Invalid;

        // 5.条件表达式(可以为空,表示恒成立)
        public string Expression = string.Empty;

		// 第二部分: CDeducer用到的
		public List<VAR_CTX> VarCtxList = new List<VAR_CTX>();
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
