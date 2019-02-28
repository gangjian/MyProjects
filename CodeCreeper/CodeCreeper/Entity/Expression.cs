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
		int GetOperatorLength(string exp_str, int offset, out int priority)
		{
			priority = -1;
			char ch = exp_str[offset];
			if (!char.IsSymbol(ch))
			{
				return 0;
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
					if (next_ch == '=')
					{
						// "==" : 等于
					}
					else
					{
						// "=" : 赋值
					}
					break;
				default:
					break;
			}
			return 0;
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
