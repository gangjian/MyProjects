using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot
{
    class StatementTree
    {
        List<StatementNode> statementList = new List<StatementNode>();
    }

    class StatementNode
    {
        // 1.语句起止位置
        public File_Position startPos;
        public File_Position endPos;

        // 2.父语句节点引用
        public StatementNode parent;
        // 3.子语句节点列表
        public List<StatementNode> childList = new List<StatementNode>();
        // 4.语句类型
        public StatementType type = StatementType.StatementType_Simple;

        // 5.条件表达式(可以为空,表示恒成立)
        public string expression = string.Empty;
    }

    // 语句类型枚举
    enum StatementType
    {
        StatementType_Simple,               // 简单句
                                            // 以下是复合语句
        StatementType_Compound_IfElse,      // if-else分支语句
        StatementType_Compound_SwitchCase,  // switch-case分支语句
        StatementType_Compound_While,       // while循环语句
        StatementType_Compound_For,         // for循环语句
        StatementType_Compound_DoWhile,     // do-while循环语句
        StatementType_Compound_GoTo,        // goto跳转语句

        // 以下是复合语句的分支语句
        StatementType_Branch_If,            // if分支
        StatementType_Branch_ElseIf,        // else if分支
        StatementType_Branch_Else,          // else分支
        StatementType_Branch_Case,          // case分支
        StatementType_Branch_Default,       // default分支
    }
}
