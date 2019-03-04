using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper
{
	public class Expression
	{
		CreeperContext _ContextRef = null;
		public Expression(CreeperContext ctx)
		{
			Trace.Assert(null != ctx);
			this._ContextRef = ctx;
		}

		/// <summary>
		/// 表达式求值
		/// </summary>
		public object Evaluate(string expression_str)
		{
			List<CodeComponent> cpnt_list = GetComponents(expression_str);
			return null;
		}

		List<CodeComponent> GetComponents(string exp_str)
		{
			List<CodeComponent> ret_list = new List<CodeComponent>();
			int offset = 0;
			while (true)
			{
				CodeComponent cpnt = GetOneComponent(exp_str, ref offset);
				if (null == cpnt)
				{
					break;
				}
				ret_list.Add(cpnt);
			}
			return ret_list;
		}

		CodeComponent GetOneComponent(string exp_str, ref int offset)
		{
			int s_pos = -1, e_pos = -1;													// 标识符的起止位置
			for (; offset < exp_str.Length; offset++)
			{
				char curChar = exp_str[offset];
				E_CHAR_TYPE cType = GetCharType(curChar);
				switch (cType)
				{
					case E_CHAR_TYPE.WhiteSpace:
						if (-1 != s_pos)
						{
							// 标识符结束
							e_pos = offset - 1;
						}
						break;
					case E_CHAR_TYPE.Letter:
					case E_CHAR_TYPE.Digit:
					case E_CHAR_TYPE.Underline:
						if (-1 == s_pos)
						{
							s_pos = offset;
						}
						break;
					case E_CHAR_TYPE.Punctuation:
						if (curChar.Equals('.'))
						{
							// 小数点?成员变量?
						}
						else if (curChar.Equals('"'))
						{
							s_pos = offset;
							e_pos = exp_str.Substring(s_pos + 1).IndexOf('"');
							Trace.Assert(-1 != e_pos);
						}
						else if (curChar.Equals('\''))
						{
							s_pos = offset;
							e_pos = exp_str.Substring(s_pos + 1).IndexOf('\'');
							Trace.Assert(-1 != e_pos);
						}
						else
						{
							if (-1 == s_pos)
							{
								s_pos = offset;
								e_pos = offset;
							}
							else
							{
								e_pos = offset - 1;
							}
						}
						break;
					case E_CHAR_TYPE.SYMBOL:
						if (-1 == s_pos)
						{
							s_pos = offset;
							e_pos = offset;
							CodeComponent oprt_cpnt = GetOperatorComponent(exp_str, ref offset);
							return oprt_cpnt;
						}
						else
						{
							e_pos = offset - 1;
						}
						break;
					default:
						Trace.Assert(false);
						break;
				}
				if (-1 != s_pos && -1 != e_pos)
				{
					string cpnt_txt = exp_str.Substring(s_pos, e_pos - s_pos + 1);
					return new CodeComponent(cpnt_txt);
				}
			}
			if (-1 != s_pos && -1 == e_pos)
			{
				e_pos = offset - 1;
				string cpnt_txt = exp_str.Substring(s_pos, e_pos - s_pos + 1);
				return new CodeComponent(cpnt_txt);
			}
			else
			{
				return null;
			}
		}
		E_CHAR_TYPE GetCharType(Char ch)
		{
			if (Char.IsWhiteSpace(ch))
			{
				return E_CHAR_TYPE.WhiteSpace;
			}
			else if (Char.IsDigit(ch))
			{
				return E_CHAR_TYPE.Digit;
			}
			else if (Char.IsLetter(ch))
			{
				return E_CHAR_TYPE.Letter;
			}
			else if (ch.Equals('_'))
			{
				return E_CHAR_TYPE.Underline;
			}
			else if (Char.IsPunctuation(ch))
			{
				return E_CHAR_TYPE.Punctuation;
			}
			else if (Char.IsSymbol(ch))
			{
				return E_CHAR_TYPE.SYMBOL;
			}
			else
			{
				return E_CHAR_TYPE.Unknown;
			}
		}
		CodeComponent GetOperatorComponent(string exp_str, ref int offset)
		{
			int priority = -1;
			int operand_cnt = 0;
			int len = 1;
			char ch = exp_str[offset];
			if (!char.IsSymbol(ch))
			{
				return null;
			}
			char? next_ch = null;
			if (offset != exp_str.Length - 1)
			{
				next_ch = exp_str[offset + 1];
			}
			switch (ch)
			{
				case '(':
				case ')':
				case '[':
				case ']':
				case '.':
					priority = 1;
					break;
				case '=':
					if ('=' == next_ch)
					{
						// "==" : 等于
						priority = 7;
						operand_cnt = 2;
						len = 2;
					}
					else
					{
						// "=" : 赋值
						priority = 14;
						operand_cnt = 2;
					}
					break;
				case '+':
				case '-':
					if (next_ch == ch)
					{
						// "++", "--" : 自增, 自减
						priority = 2;
						operand_cnt = 1;
						len = 2;
					}
					else if ('=' == next_ch)
					{
						// "+=", "-=" : 加减运算赋值
						priority = 14;
						operand_cnt = 2;
						len = 2;
					}
					else if ('-' == ch && '>' == next_ch)
					{
						// "->" : 指针成员
						priority = 1;
						len = 2;
					}
					else
					{
						// "+", "-" : 加, 减(优先级4), 也有可能是正负号(优先级2)
						priority = -1;
						operand_cnt = 2;
					}
					break;
				case '*':
				case '/':
					if ('=' == next_ch)
					{
						// "*=", "/=" : 乘除运算赋值
						priority = 14;
						operand_cnt = 2;
						len = 2;
					}
					else
					{
						// 乘除(优先级3), 也可能是指针取值运算符(优先级2)
						priority = 3;
						operand_cnt = 2;
						if ('*' == ch)
						{
							priority = -1;
							operand_cnt = 0;
						}
					}
					break;
				case '>':
				case '<':
					if (next_ch == ch)
					{
						char? third_ch = null;
						if (offset < exp_str.Length - 2)
						{
							third_ch = exp_str[offset + 2];
						}
						if ('=' == third_ch)
						{
							// ">>=", "<<=" : 位移赋值
							len = 3;
							priority = 14;
							operand_cnt = 2;
						}
						else
						{
							// ">>", "<<" : 左移, 右移
							len = 2;
							priority = 5;
							operand_cnt = 2;
						}
					}
					else if ('=' == next_ch)
					{
						// ">=", "<=" : 大于等于, 小于等于
						priority = 6;
						operand_cnt = 2;
						len = 2;
					}
					else
					{
						// ">", "<" : 大于, 小于
						priority = 6;
						operand_cnt = 2;
					}
					break;
				case '&':
				case '|':
					if (next_ch == ch)
					{
						// "&&", "||" : 逻辑与, 逻辑或
						if ('&' == ch)
						{
							priority = 11;
						}
						else
						{
							priority = 12;
						}
						operand_cnt = 2;
						len = 2;
					}
					else if ('=' == next_ch)
					{
						// "&=", "|=" : 位运算赋值
						priority = 14;
						operand_cnt = 2;
						len = 2;
					}
					else
					{
						// "&", "|" : 位与, 位或
						if ('&' == ch)
						{
							// 不确定, 可能是双目位与&(优先级8),也可能是取地址符&(优先级2)
							priority = -1;
						}
						else
						{
							priority = 10;
							operand_cnt = 2;
						}
					}
					break;
				case '!':
					if ('=' == next_ch)
					{
						// "!=" : 不等于
						priority = 7;
						operand_cnt = 2;
						len = 2;
					}
					else
					{
						// "!" : 逻辑非
						priority = 2;
						operand_cnt = 1;
					}
					break;
				case '~':
					// "~" : 按位取反
					priority = 2;
					operand_cnt = 1;
					break;
				case '%':
					if ('=' == next_ch)
					{
						// "%=" : 取余赋值
						priority = 14;
						operand_cnt = 2;
						len = 2;
					}
					else
					{
						// "%" : 取余
						priority = 3;
						operand_cnt = 2;
					}
					break;
				case '^':
					if ('=' == next_ch)
					{
						// "^=" : 位异或赋值
						priority = 14;
						operand_cnt = 2;
						len = 2;
					}
					else
					{
						// "^" : 位异或
						priority = 9;
						operand_cnt = 2;
					}
					break;
				case ',':
					// "," : 逗号
					priority = 15;
					operand_cnt = 2;
					break;
				case '?':
				case ':':
					// "?:" : 条件(三目)
					priority = 13;
					operand_cnt = 3;
					break;
				default:
					Trace.Assert(false);
					break;
			}
			string oprt_txt = exp_str.Substring(offset, len);
			offset += len;
			CodeComponent ret_cpnt = new CodeComponent(oprt_txt);
			ret_cpnt.Priority = priority;
			ret_cpnt.OperandCount = operand_cnt;
			ret_cpnt.Type = ComponentType.Operator;
			return ret_cpnt;
		}
	}

	enum ComponentType
	{
		Invalid,				// 无效
		Identifier,				// 标识符
		ConstantNumber,         // 数值常量
		String,                 // 字符串
		Char,                   // 字符
		FunctionName,		    // 函数名
		Operator,               // 运算符
		Punctuation,			// 标点符号
	}

	class CodeComponent
	{
		private ComponentType type = ComponentType.Invalid;
		public ComponentType Type
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

		public int Priority = -1;			// (如果是运算符的话)运算符的优先级
		public int OperandCount = 0;		// (如果是运算符的话)操作数的个数

		public CodeComponent()
		{
		}

		public CodeComponent(string str)
		{
			Text = str;
		}
		void SetComponetType()
		{
			if (this.Type == ComponentType.Operator)
			{
			}
			else if (this.Text.StartsWith("\"") && this.Text.EndsWith("\""))
			{
				this.Type = ComponentType.String;
			}
			else if (this.Text.StartsWith("\'") && this.Text.EndsWith("\'"))
			{
				this.Type = ComponentType.Char;
			}
			else if (CommProc.GetNumberStrLength(this.Text, 0) == this.Text.Length)
			{
				this.Type = ComponentType.ConstantNumber;
			}
			else if (CommProc.GetIdentifierStringLength(this.Text, 0) == this.Text.Length)
			{
				this.Type = ComponentType.Identifier;
			}
			else if (1 == this.Text.Length && char.IsPunctuation(this.Text[0]))
			{
				this.Type = ComponentType.Punctuation;
			}
			else
			{

			}
		}
	}

	enum E_CHAR_TYPE
	{
		WhiteSpace,
		Letter,
		Digit,
		Underline,
		Punctuation,
		SYMBOL,
		Unknown
	}
}
