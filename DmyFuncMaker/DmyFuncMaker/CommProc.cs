using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DmyFuncMaker
{
	public class CodeIdentifier
	{
		public int Offset = -1;
		public string Text = string.Empty;

		public CodeIdentifier(int offset, string text)
		{
			this.Offset = offset;
			this.Text = text;
		}
	}

	public class VarInfo
	{
		public string TypeStr = string.Empty;
		public string Name = string.Empty;

		public VarInfo(string type, string name)
		{
			this.TypeStr = type;
			this.Name = name;
		}
	}

	public class CommProc
	{
		public static CodeIdentifier GetNextIdentifier(string statementStr, ref int offset)
		{
			int s_pos = -1, e_pos = -1;     // 标识符的起止位置
			for (; offset < statementStr.Length; offset++)
			{
				char curChar = statementStr[offset];
				E_CHAR_TYPE cType = GetCharType(curChar);
				switch (cType)
				{
					case E_CHAR_TYPE.E_CTYPE_WHITE_SPACE:								// 空格
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
					case E_CHAR_TYPE.E_CTYPE_LETTER:									// 字母
					case E_CHAR_TYPE.E_CTYPE_UNDERLINE:									// 下划线
					case E_CHAR_TYPE.E_CTYPE_DIGIT:										// 数字
						if (-1 == s_pos)
						{
							s_pos = offset;
						}
						else
						{
							// do nothing
						}
						break;
					case E_CHAR_TYPE.E_CTYPE_PUNCTUATION:								// 标点
					case E_CHAR_TYPE.E_CTYPE_SYMBOL:									// 运算符
						if (-1 == s_pos)
						{
							s_pos = offset;
							e_pos = offset;
							offset += 1;
						}
						else
						{
							e_pos = offset - 1;
						}
						break;
					default:
						return null;
				}
				if (-1 != s_pos && -1 != e_pos)
				{
					break;
				}
			}
			if (-1 != s_pos)
			{
				if (-1 == e_pos)
				{
					e_pos = offset - 1;
				}
				string text = statementStr.Substring(s_pos, e_pos - s_pos + 1);
				return new CodeIdentifier(s_pos, text);
			}
			return null;
		}

		public static E_CHAR_TYPE GetCharType(Char ch)
		{
			if (Char.IsWhiteSpace(ch))
			{
				return E_CHAR_TYPE.E_CTYPE_WHITE_SPACE;
			}
			else if (Char.IsDigit(ch))
			{
				return E_CHAR_TYPE.E_CTYPE_DIGIT;
			}
			else if (Char.IsLetter(ch))
			{
				return E_CHAR_TYPE.E_CTYPE_LETTER;
			}
			else if (ch.Equals('_'))
			{
				return E_CHAR_TYPE.E_CTYPE_UNDERLINE;
			}
			else if (Char.IsPunctuation(ch))
			{
				return E_CHAR_TYPE.E_CTYPE_PUNCTUATION;
			}
			else if (Char.IsSymbol(ch))
			{
				return E_CHAR_TYPE.E_CTYPE_SYMBOL;
			}
			else
			{
				return E_CHAR_TYPE.E_CTYPE_UNKNOWN;
			}
		}

		/// <summary>
		/// 判断是否是"标准"标识符
		/// 字母数字下划线组成且开头不是数字
		/// </summary>
		public static bool IsStandardIdentifier(string idStr)
		{
			if (string.IsNullOrEmpty(idStr))
			{
				return false;
			}
			int cnt = 0;
			foreach (Char ch in idStr)
			{
				if (!Char.IsDigit(ch) && !Char.IsLetter(ch) && ('_' != ch))
				{
					return false;
				}
				if (0 == cnt)
				{
					// 开头不能是数字
					if (Char.IsDigit(ch))
					{
						return false;
					}
				}
				cnt++;
			}
			return true;
		}

		public enum E_CHAR_TYPE
		{
			E_CTYPE_WHITE_SPACE,
			E_CTYPE_LETTER,
			E_CTYPE_DIGIT,
			E_CTYPE_UNDERLINE,
			E_CTYPE_PUNCTUATION,
			E_CTYPE_SYMBOL,
			E_CTYPE_UNKNOWN
		}
	}
}
