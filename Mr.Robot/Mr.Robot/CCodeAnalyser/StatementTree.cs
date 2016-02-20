using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot
{
    class StatementNode
    {
        // 1.语句范围(起止位置)
		private File_Scope scope = new File_Scope();

		public File_Scope Scope
		{
			get { return scope; }
			set { scope = value; }
		}

        // 2.父语句节点引用
        public StatementNode parent = null;
        // 3.子语句节点列表
        public List<StatementNode> childList = new List<StatementNode>();
        // 4.语句类型
        public StatementNodeType type = StatementNodeType.Invalid;

        // 5.条件表达式(可以为空,表示恒成立)
        public string expression = string.Empty;
    }

    // 语句节点类型枚举
    enum StatementNodeType
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
