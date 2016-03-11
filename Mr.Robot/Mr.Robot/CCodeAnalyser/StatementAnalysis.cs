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
		static public void FunctionStatementsAnalysis(StatementNode root, List<string> codeList, List<CFileParseInfo> headerList)
		{
			// 顺次解析各条语句
			foreach (StatementNode childNode in root.childList)
			{
				StatementAnalysis(childNode, codeList, headerList);
			}
		}

		static void StatementAnalysis(StatementNode node, List<string> codeList, List<CFileParseInfo> headerList)
		{
			switch (node.Type)
			{
				case StatementNodeType.Simple:
					SimpleStatementAnalysis(node, codeList, headerList);
					break;
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
		}

		/// <summary>
		/// 简单语句分析
		/// </summary>
		static void SimpleStatementAnalysis(StatementNode statementNode, List<string> codeList, List<CFileParseInfo> headerList)
		{
			// 取得完整的语句内容
			string statementStr = LineStringCat(codeList, statementNode.Scope.Start, statementNode.Scope.End);

			// 依次按顺序取出语句各组成部分
			List<StatementOperand> operandList = new List<StatementOperand>();
			int offset = 0;
			do
			{
				StatementOperand cpnt = Get1OperandOrOperator(statementStr, ref offset, headerList);
				if (null == cpnt)
				{
					break;
				}
				operandList.Add(cpnt);
			} while (true);

            // 对各组成部分进行分析
		}

		/// <summary>
		/// 从语句中提取出一个操作数/操作符
		/// </summary>
        static StatementOperand Get1OperandOrOperator(string statementStr, ref int offset, List<CFileParseInfo> headerList)
		{
			string idStr = null;
            StatementOperand retSO = null;
			if (null != (idStr = GetNextIdentifier(statementStr, ref offset)))
			{
				if (IsStandardIdentifier(idStr))
				{
                    // 如果包含宏, 首先要进行宏展开

                    retSO = new StatementOperand();
                    retSO.Text = idStr;
                    retSO.Type = GetIdentifierType(idStr, headerList);
				}
				else if ("(" == idStr)
				{
					
				}
				else if ("=" == idStr)
				{
					
				}
                else if ("*" == idStr)
                {

                }
			}

            return retSO;
		}

        static void MacroDetectAndExpand_Function()
        {

        }

        static StatementOperandType GetIdentifierType(string identifier, List<CFileParseInfo> headerList)
        {
            // 可能是基本类型名
            if (IsBasicVarType(identifier))
            {
                return StatementOperandType.BasicVarType;
            }
            // 可能是用户定义类型名
            else if (IsUsrDefVarType(identifier, headerList))
            {
                return StatementOperandType.UsrDefVarType;
            }
            // 可能是常量
            else if (IsConstantNumber(identifier))
            {
                return StatementOperandType.Constant;
            }
            // 可能是函数名
            else if (IsFunctionName(identifier, headerList))
            {
                return StatementOperandType.FunctionName;
            }
            // 可能是全局变量
            else if (IsGlobalVariable(identifier, headerList))
            {
                return StatementOperandType.GlobalVariable;
            }
            // 可能是局部变量名
            else if (IsLocalVariable(identifier, headerList))
            {
                return StatementOperandType.LocalVariable;
            }
            else
            {
                return StatementOperandType.Unknown;
            }
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
                foreach (UsrDefineTypeInfo udi in hfi.user_def_type_list)
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
        static bool IsConstantNumber(string identifier)
        {
            for (int i = 0; i < identifier.Length; i++)
            {
                if (!Char.IsDigit(identifier[i]))
                {
                    return false;
                }
            }
            return true;
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
            return false;
        }

        /// <summary>
        /// 判断标识符是否是局部(临时)变量
        /// </summary>
        static bool IsLocalVariable(string identifier, List<CFileParseInfo> headerList)
        {
            return false;
        }
	}

	public class StatementOperand
	{
		string text = string.Empty;

		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		StatementOperandType type = StatementOperandType.Unknown;

		public StatementOperandType Type
		{
			get { return type; }
			set { type = value; }
		}
	}

	public enum StatementOperandType
	{
		Unknown,				// 未知

		BasicVarType,			// 基本数据类型
        UsrDefVarType,			// 用户定义数据类型/构造类型
		GlobalVariable,			// 全局变量名
		LocalVariable,			// 局部变量名
        Constant,               // 常量
        String,                 // 字符串
        Char,                   // 字符

		FunctionName,		    // 函数名
		EqualMark,				// 等号(赋值符号)
        StarMark,               // 星号
        Symbol,                 // 其它运算符

		Expression,				// 表达式
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


