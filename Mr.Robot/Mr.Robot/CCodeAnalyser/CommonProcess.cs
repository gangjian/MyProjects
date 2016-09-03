using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Mr.Robot
{
	public class CommonProcess
	{
		/// <summary>
        /// 从(多行字符)当前位置取得下一个标识符
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="lineIdx"></param>
		/// <param varName="startIdx"></param>
		/// <returns></returns>
        public static string GetNextIdentifier(List<string> codeList, ref File_Position searchPos, out File_Position foundPos)
		{
			int lineIdx = searchPos.row_num;
			int startIdx = searchPos.col_num;
			foundPos = new File_Position(searchPos);
			System.Diagnostics.Trace.Assert(lineIdx >= 0);
			if (lineIdx >= codeList.Count)
			{
				return null;
			}

			string curLine = "";
			int curIdx = startIdx;
			int s_pos = -1, e_pos = -1;     // 标识符的起止位置
			while (true)
			{
				// 到list末尾结束跳出
				if (lineIdx >= codeList.Count)
				{
					break;
				}
				curLine = codeList[lineIdx];
				// 如果当前行是空行, 跳到下一行
				if (string.Empty == curLine)
				{
					lineIdx++;
					continue;
				}

				// 从这里开始逐字符进行判断
				bool startFlag = false;
				for (; curIdx < curLine.Length; curIdx++)
				{
					char curChar = curLine[curIdx];
					E_CHAR_TYPE cType = GetCharType(curChar);
					switch (cType)
					{
						case E_CHAR_TYPE.E_CTYPE_WHITE_SPACE:
							if (false == startFlag)
							{
							}
							else
							{
								// 标识符结束
								goto RET_IDF;
							}
							break;
						case E_CHAR_TYPE.E_CTYPE_LETTER:
						case E_CHAR_TYPE.E_CTYPE_UNDERLINE:
							if (false == startFlag)
							{
								// 标识符开始
								startFlag = true;
								s_pos = curIdx;
								e_pos = curIdx;
							}
							else
							{
								// 标识符继续
								e_pos = curIdx;
							}
							break;
						case E_CHAR_TYPE.E_CTYPE_DIGIT:
							if (false == startFlag)
							{
								// 标识符结束, 返回该数字
								s_pos = curIdx;
								e_pos = curIdx;
								curIdx++;
                                // TODO: 这里有疑问, 现在看如果遇到是连续的两位以上数字,不应该返回
								goto RET_IDF;
							}
							else
							{
								// 标识符继续
								e_pos = curIdx;
							}
							break;
						case E_CHAR_TYPE.E_CTYPE_PUNCTUATION:
						case E_CHAR_TYPE.E_CTYPE_SYMBOL:
							if (false == startFlag)
							{
								// 标识符结束, 返回该符号
								s_pos = curIdx;
								e_pos = curIdx;
								curIdx++;
								goto RET_IDF;
							}
							else
							{
								// 标识符结束
								goto RET_IDF;
							}
						default:
							ErrReport();
							return null;
					}
				}

				// 到达行末
				if (startFlag)
				{
					goto RET_IDF;
				}
				// 转到下一行开头
				lineIdx++;
				curIdx = 0;
			}
			return null;
		RET_IDF:
			if (-1 != s_pos && -1 != e_pos)
			{
				foundPos = new File_Position(lineIdx, s_pos);
				if (curIdx >= curLine.Length)
				{
					lineIdx++;
					curIdx = 0;
				}
				startIdx = curIdx;
				searchPos.row_num = lineIdx;
				searchPos.col_num = startIdx;
				return curLine.Substring(s_pos, e_pos - s_pos + 1);
			}
			else
			{
				ErrReport();
				return null;
			}
		}

		/// <summary>
		/// 从指定位置开始取得(同一行内)的下一个标识符
		/// </summary>
		/// <returns></returns>
		public static string GetNextIdentifier2(string statementStr, ref int offset)
		{
			int s_pos = -1, e_pos = -1;     // 标识符的起止位置
			for (; offset < statementStr.Length; offset++)
			{
				char curChar = statementStr[offset];
				E_CHAR_TYPE cType = CommonProcess.GetCharType(curChar);
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
						CommonProcess.ErrReport();
						return null;
				}
				if (-1 != s_pos && -1 != e_pos)
				{
					return statementStr.Substring(s_pos, e_pos - s_pos + 1);
				}
			}
			if (-1 != s_pos && -1 == e_pos)
			{
				e_pos = offset - 1;
				return statementStr.Substring(s_pos, e_pos - s_pos + 1);
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
		/// <param varName="id"></param>
		/// <returns></returns>
        public static bool IsStandardIdentifier(string idStr)
		{
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

		/// <summary>
		/// 从当前位置开始, 找到下一个指定符号出现的位置
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="lineIdx"></param>
		/// <param varName="startIdx"></param>
		/// <param varName="symbol"></param>
        public static File_Position FindNextSymbol(List<string> codeList, File_Position searchPos, Char symbol)
		{
			string curLine = "";
			int lineIdx = searchPos.row_num;
			int startIdx = searchPos.col_num;
			int curIdx = startIdx;
			while (true)
			{
				// 到list末尾结束跳出
				if (lineIdx >= codeList.Count)
				{
					break;
				}
				curLine = codeList[lineIdx];
				// 如果当前行是空行, 跳到下一行
				if (string.Empty == curLine)
				{
					lineIdx++;
					continue;
				}
				for (; curIdx < curLine.Length; curIdx++)
				{
					Char curChar = curLine[curIdx];
					// 注意这里暂时没有考虑中间会遇到条件编译分支的情况
					if (symbol == curChar)
					{
						// 找到了
						File_Position fpos = new File_Position(lineIdx, curIdx);
						return fpos;
					}
				}
				// 到达行末
				// 转到下一行开头
				lineIdx++;
				curIdx = 0;
			}
			ErrReport();
			return null;
		}

		/// <summary>
		/// 找到下一个配对的符号
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="searchPos"></param>
		/// <param varName="rightSymbol"></param>
		/// <returns></returns>
        public static File_Position FindNextMatchSymbol(List<string> codeList, File_Position searchPos, Char rightSymbol)
		{
			Char leftSymbol;
			if ('}' == rightSymbol)
			{
				leftSymbol = '{';
			}
			else if (')' == rightSymbol)
			{
				leftSymbol = '(';
			}
			else
			{
				ErrReport();
				return null;
			}

			int matchCount = 1;
			File_Position foundPos = null;
            bool quoteStart = false;
            string quoteStr = string.Empty;
            while (true)
			{
				string idStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
				if (null == idStr)
				{
					break;
				}
                else if (  "\'" == idStr
                        || "\"" == idStr)
                {
                    // 要注意不要算上双引号, 单引号括起来的符号
                    if (true == quoteStart
                        && quoteStr == idStr)
                    {
                        quoteStart = false;
                    }
                    else if (!quoteStart)
                    {
                        quoteStart = true;
                        quoteStr = idStr;
                    }
                    else
                    {
                    }
                }
                else if (!quoteStart && leftSymbol.ToString() == idStr)
				{
					matchCount += 1;
				}
                else if (!quoteStart && rightSymbol.ToString() == idStr)
				{
					matchCount -= 1;
					if (0 == matchCount)
					{
						// 找到了
						return foundPos;
					}
				}
				else
				{
				}
			}
			ErrReport();
			return null;
		}

        /// <summary>
        /// 将起止位置之间的内容连接成一个字符串(去掉换行)
        /// </summary>
        /// <param varName="codeList"></param>
        /// <param varName="startPos"></param>
        /// <param varName="endPos"></param>
        /// <returns></returns>
		public static string LineStringCat(List<string> codeList, File_Position startPos, File_Position endPos)
		{
			int startRow = startPos.row_num;
			int startCol = startPos.col_num;
			int endRow = endPos.row_num;
			int endCol = endPos.col_num;
			string retStr = "";
			string lineStr = "";
			while (startRow < endRow)
			{
				lineStr = codeList[startRow];
				retStr += lineStr.Substring(startCol);
				startRow += 1;
				startCol = 0;
			}
			lineStr = codeList[startRow];
			retStr += lineStr.Substring(startCol, endCol - startCol + 1);
			return retStr;
		}

		/// <summary>
		/// 取得一个表达式
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="searchPos"></param>
		/// <param varName="foundPos"></param>
		/// <returns></returns>
		public static string GetExpressionStr(List<string> codeList, ref File_Position searchPos, out File_Position foundPos)
		{
			foundPos = new File_Position(searchPos);

			string lineStr = codeList[searchPos.row_num];
			string exprStr = lineStr.Substring(searchPos.col_num).Trim();
			foundPos.col_num = lineStr.IndexOf(exprStr);
			searchPos.row_num += 1;
			searchPos.col_num = 0;

			return exprStr;
		}

		/// <summary>
		/// 判断表达式是否已定义(#if defined)
		/// </summary>
		/// <param varName="exp">表达式</param>
		/// <param varName="headerFileNameList">头文件列表</param>
		/// <param varName="defineList">宏定义列表</param>
		/// <returns></returns>
        public static MacroDefineInfo JudgeExpressionDefined(string exp, List<CFileParseInfo> headerList, List<MacroDefineInfo> defineList)
		{
			foreach (var di in defineList)
			{
				if (exp == di.name)
				{
					return di;
				}
			}
			foreach (var fi in headerList)
			{
				foreach (var di in fi.macro_define_list)
				{
					if (exp == di.name)
					{
						return di;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// 判断表达式的值
		/// </summary>
		/// <param varName="exp">表达式</param>
		/// <param varName="headerFileNameList">头文件列表</param>
		/// <param varName="defineList">宏定义列表</param>
		/// <returns></returns>
        public static int JudgeExpressionValue(string exp, List<CFileParseInfo> headerList, List<MacroDefineInfo> defineList)
		{
			// TODO: 暂不考虑复合表达式的情况
			// 对于复合表达式, 拆分成单独表达式分别求值
			int retVal = 0;
			if (int.TryParse(exp, out retVal))
			{
				return retVal;
			}
			else if (IsStandardIdentifier(exp))
			{
				MacroDefineInfo mdi = JudgeExpressionDefined(exp, headerList, defineList);
				if (null != mdi)
				{
					if (int.TryParse(mdi.value, out retVal))
					{
						return retVal;
					}
				}
			}
			return 0;
		}

        public static string GetSimpleExpression(string complexExp, int idx)
		{
			return null;
		}

        /// <summary>
        /// 查找下一个指定的标识符
        /// </summary>
        /// <param varName="idStr"></param>
        /// <param varName="codeList"></param>
        /// <param varName="searchPos"></param>
        /// <returns></returns>
        public static File_Position FindNextSpecIdentifier(string idStr, List<string> codeList, File_Position searchPos)
		{
			File_Position foundPos = null;
			string retStr = null;
			while (true)
			{
				retStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
				if (idStr == retStr)
				{
					return foundPos;
				}
				else if (null == retStr)
				{
					break;
				}
			}
			return null;
		}

        /// <summary>
        /// 移动到指定位置的下一位置
        /// </summary>
        /// <param varName="codeList"></param>
        /// <param varName="thisPos"></param>
        /// <returns></returns>
        public static File_Position PositionMoveNext(List<string> codeList, File_Position thisPos)
		{
			File_Position nextPos = new File_Position(thisPos);
            if (thisPos.col_num == codeList[thisPos.row_num].Length - 1)
			{
				// 已经是最后一列了, 就移到下一行开头
                if (thisPos.row_num < codeList.Count - 1)
                {
                    nextPos.row_num += 1;
                    nextPos.col_num = 0;
                }
			}
			else
			{
				// 否则移到下一列
				nextPos.col_num += 1;
			}
			return nextPos;
		}

        /// <summary>
        /// 移动到指定位置的前一位置
        /// </summary>
        /// <param varName="codeList"></param>
        /// <param varName="thisPos"></param>
        /// <returns></returns>
        public static File_Position PositionMovePrevious(List<string> codeList, File_Position thisPos)
        {
            File_Position prevPos = new File_Position(thisPos);
            if (0 == thisPos.col_num)
            {
                // 已经是第一列了, 就移到上一行末尾
                if (thisPos.row_num > 0)
                {
                    prevPos.row_num -= 1;
                    prevPos.col_num = codeList[prevPos.row_num].Length - 1;
                }
            }
            else
            {
                // 否则移到前一列
                prevPos.col_num -= 1;
            }
            return prevPos;
        }

		/// <summary>
		/// 比较两个位置 0:一致; 1:前者大(靠后); -1:后者大(靠后);
		/// </summary>
		/// <returns></returns>
        public static int PositionCompare(File_Position p1, File_Position p2)
		{
			if (p1.row_num > p2.row_num)
			{
				return 1;
			}
			else if (p1.row_num < p2.row_num)
			{
				return -1;
			}
			else if (p1.col_num > p2.col_num)
			{
				return 1;
			}
			else if (p1.col_num < p2.col_num)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}

        public static bool IsUsrDefTypeKWD(string keyWord)
        {
            switch (keyWord)
            {
                case "struct":
                case "union":
                case "enum":
                    return true;
                default:
                    break;
            }
            return false;
        }

		////////////////////////////////////////////////

        public static void Save2File(List<string> writeList, string saveName)
		{
			try
			{
				StreamWriter sw = new StreamWriter(saveName);
				foreach (string wtLine in writeList)
				{
					sw.WriteLine(wtLine);
				}
				sw.Close();
			}
			catch (Exception ex)
			{
				ErrReport(ex.ToString());
			}
		}

        public static void ErrReport(string errMsg = "Something is wrong!")
        {
            System.Diagnostics.Trace.WriteLine(errMsg);
            System.Diagnostics.Trace.Assert(false);
        }

		public static string FindTypeDefName(string type_name, List<CFileParseInfo> fpiList)
		{
			foreach (CFileParseInfo fpi in fpiList)
			{
				foreach (TypeDefineInfo tdi in fpi.type_define_list)
				{
					if (tdi.new_type_name.Equals(type_name))
					{
						return tdi.old_type_name;
					}
				}
			}
			fpiList.Clear();
			return string.Empty;
		}
	}
}