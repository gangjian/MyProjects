using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Mr.Robot.CDeducer;

namespace Mr.Robot
{
	public class COMN_PROC
	{
		/// <summary>
		/// 移除代码注释
		/// </summary>
		public static List<string> RemoveComments(string fileName)
		{
			TextReader tr = new StreamReader(fileName);
			List<string> retList = new List<string>();

			string rdLine = tr.ReadLine();
			if (null == rdLine)
			{
				return retList;
			}
			string wtLine = "";
			do
			{
				int idx1 = rdLine.IndexOf("//");
				int idx2 = rdLine.IndexOf("/*");

				if ((-1 != idx1)
					&& (-1 != idx2)
					&& (idx1 < idx2)
					)
				{
					wtLine = rdLine.Remove(idx1).TrimEnd();
				}
				else if ((-1 != idx1)
						 && (-1 == idx2))
				{
					// 只包含行注释
					wtLine = rdLine.Remove(idx1).TrimEnd();
				}
				else if (-1 != idx2)
				{
					// 只包含块注释
					int idx_s = idx2;
					int idx_e = rdLine.IndexOf("*/", idx_s + 2);
					while (-1 == idx_e)
					{
						if (rdLine.Length > idx_s)
						{
							wtLine = rdLine.Remove(idx_s).TrimEnd();
						}
						retList.Add(wtLine);
						idx_s = 0;

						rdLine = tr.ReadLine();
						if (null == rdLine)
						{
							break;
						}
						idx_e = rdLine.IndexOf("*/");
					}
					if (-1 != idx_e)
					{
						rdLine = rdLine.Remove(idx_s, idx_e - idx_s + 2);
					}
					else
					{
						rdLine = "";
					}
					continue;
				}
				else
				{
					// 不包含注释
					wtLine = rdLine.TrimEnd();
				}
				retList.Add(wtLine);
				rdLine = tr.ReadLine();
				if (null == rdLine)
				{
					break;
				}
			} while (true);
			tr.Close();

			return retList;
		}

		/// <summary>
        /// 从(多行字符)当前位置取得下一个标识符
		/// </summary>
        public static CODE_IDENTIFIER GetNextIdentifier(List<string> codeList, ref CODE_POSITION searchPos, out CODE_POSITION foundPos)
		{
			int lineIdx = searchPos.RowNum;
			int startIdx = searchPos.ColNum;
			foundPos = new CODE_POSITION(searchPos);
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
						default:
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
							//ErrReport();
							//return null;
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
				foundPos = new CODE_POSITION(lineIdx, s_pos);
				if (curIdx >= curLine.Length)
				{
					lineIdx++;
					curIdx = 0;
				}
				startIdx = curIdx;
				searchPos.RowNum = lineIdx;
				searchPos.ColNum = startIdx;
				CODE_IDENTIFIER retId = new CODE_IDENTIFIER(curLine.Substring(s_pos, e_pos - s_pos + 1), foundPos);
				return retId;
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
		public static string GetNextIdentifier2(string statementStr, ref int offset)
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
						if (curChar.Equals('.'))
						{
							// 小数点?成员变量?
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
					case E_CHAR_TYPE.E_CTYPE_SYMBOL:									// 运算符
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

		/// <summary>
		/// 从当前位置开始, 找到下一个指定符号出现的位置
		/// </summary>
        public static CODE_POSITION FindNextSymbol(List<string> codeList, CODE_POSITION searchPos, Char symbol)
		{
			string curLine = "";
			int lineIdx = searchPos.RowNum;
			int startIdx = searchPos.ColNum;
			int curIdx = startIdx;
			bool quoteStart = false;
			Char quoteChar = Char.MinValue;
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
					if ('\'' == curChar
						|| '\"' == curChar)
					{
						if (curIdx > 0
							&& '\\' == curLine[curIdx - 1])
						{
							// 转义字符, 即字符串中的引号 "\""
							continue;
						}
						// 要注意不要算上双引号, 单引号括起来的符号
						if (true == quoteStart
							&& quoteChar == curChar)
						{
							quoteStart = false;
						}
						else if (!quoteStart)
						{
							quoteStart = true;
							quoteChar = curChar;
						}
						else
						{
						}
					}
					// 注意这里暂时没有考虑中间会遇到条件编译分支的情况
					if (symbol == curChar && !quoteStart)
					{
						// 找到了
						CODE_POSITION fpos = new CODE_POSITION(lineIdx, curIdx);
						return fpos;
					}
				}
				// 到达行末
				// 转到下一行开头
				lineIdx++;
				curIdx = 0;
			}
			//ErrReport();
			return null;
		}

		/// <summary>
		/// 找到下一个配对的符号
		/// </summary>
		public static CODE_POSITION FindNextMatchSymbol(FILE_PARSE_INFO parse_info, ref CODE_POSITION searchPos, Char rightSymbol)
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
			CODE_POSITION foundPos = null;
            bool quoteStart = false;
            string quoteStr = string.Empty;
            while (true)
			{
				CODE_IDENTIFIER nextIdtf = GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
				if (null == nextIdtf)
				{
					break;
				}
				else if (IsStandardIdentifier(nextIdtf.Text))
				{
					if (MacroDetectAndExpand_File(nextIdtf.Text, foundPos, parse_info))
					{
						// 判断是否是已定义的宏, 是的话进行宏展开
						// 展开后要返回到原处(展开前的位置), 重新解析展开后的宏
						searchPos = new CODE_POSITION(foundPos);
						continue;
					}
				}
				else if ("\'" == nextIdtf.Text
						|| "\"" == nextIdtf.Text)
                {
					if (nextIdtf.Position.ColNum > 0
						&& '\\' == parse_info.CodeList[nextIdtf.Position.RowNum][nextIdtf.Position.ColNum - 1])
					{
						// 转义字符, 即字符串中的引号 "\""
						continue;
					}
                    // 要注意不要算上双引号, 单引号括起来的符号
                    if (true == quoteStart
						&& quoteStr == nextIdtf.Text)
                    {
                        quoteStart = false;
                    }
                    else if (!quoteStart)
                    {
                        quoteStart = true;
						quoteStr = nextIdtf.Text;
                    }
                    else
                    {
                    }
                }
				else if (!quoteStart && leftSymbol.ToString() == nextIdtf.Text)
				{
					matchCount += 1;
				}
				else if (!quoteStart && rightSymbol.ToString() == nextIdtf.Text)
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
			//ErrReport();
			return null;
		}

		/// <summary>
		/// 宏检测与宏展开
		/// </summary>
		public static bool MacroDetectAndExpand_File(string idStr,
													 CODE_POSITION foundPos,
													 FILE_PARSE_INFO parse_info)
		{
			// 遍历查找宏名
			MACRO_DEFINE_INFO mdi = parse_info.FindMacroDefInfo(idStr);
			if (null != mdi
				&& !string.IsNullOrEmpty(mdi.ValStr))
			{
				string macroName = mdi.Name;
				CODE_POSITION macroPos = new CODE_POSITION(foundPos);
				CODE_POSITION removeEndPos = new CODE_POSITION(macroPos.RowNum, macroPos.ColNum + macroName.Length - 1);
				int lineIdx = foundPos.RowNum;
				string replaceStr = mdi.ValStr;
				// 判断有无带参数
				if (0 != mdi.ParaList.Count)
				{
					// 取得实参
					CODE_POSITION sPos = new CODE_POSITION(foundPos.RowNum, foundPos.ColNum + idStr.Length);
					CODE_IDENTIFIER nextIdtf = GetNextIdentifier(parse_info.CodeList, ref sPos, out foundPos);
					if ("(" != nextIdtf.Text)
					{
						//ErrReport();
						// 发现过有一个带参数的宏min(x, y), 另有某一个结构体成员名也叫min
						// 解析至此认为该结构体成员是宏, 取得参数时发现没有参数...
						// ?: 宏与结构体成员重名, 如何处理...
						return false;
					}
					CODE_POSITION leftBracket = foundPos;
					foundPos = FindNextMatchSymbol(parse_info, ref sPos, ')');
					if (null == foundPos)
					{
						ErrReport();
						return false;
					}
					removeEndPos = new CODE_POSITION(foundPos);
					nextIdtf.Text = LineStringCat(parse_info.CodeList, macroPos, foundPos);
					macroName = nextIdtf.Text;
					List<string> realParas = GetParaList(parse_info.CodeList, leftBracket, foundPos);
					if (0 == realParas.Count)
					{
						// 如果宏定义后有括号但是没有参数,加一个空串用以在宏替换时替换掉括号部分
						realParas.Add(string.Empty);
					}
					if (realParas.Count != mdi.ParaList.Count)
					{
						// TODO: 20170111
						// #define NSCAN_uprintf(fmt, ...)
						// 对于可变参数(如上, 实参可以比形参个数少)
						//ErrReport();
						return false;
					}
					// 替换宏值里的形参
					int idx = 0;
					foreach (string rp in realParas)
					{
						if (string.Empty == rp)
						{
							// 参数有可能为空, 即没有参数, 只有一对空的括号里面什么参数也不带
							continue;
						}
						replaceStr = WholeWordSReplace(replaceStr, mdi.ParaList[idx], rp);
						//replaceStr = replaceStr.Replace(mdi.ParaList[idx], rp);
						idx++;
					}
				}
				// 应对宏里面出现的"##"
				string[] seps = { "##" };
				string[] arr = replaceStr.Split(seps, StringSplitOptions.None);
				if (arr.Length > 1)
				{
					string newStr = "";
					foreach (string sepStr in arr)
					{
						newStr += sepStr.Trim();
					}
					replaceStr = newStr;
				}
				// 单个"#"转成字串
				while (replaceStr.Contains('#'))
				{
					int idx = replaceStr.IndexOf("#");
					int search_idx = idx + 1;
					string tokenStr = GetNextIdentifier2(replaceStr, ref search_idx);

					replaceStr = replaceStr.Remove(idx, 1 + tokenStr.Length);
					replaceStr = replaceStr.Insert(idx, "\"" + tokenStr + "\"");
					//ErrReport();
					//return false;
				}
				if (macroName == replaceStr)
				{
					return false;
				}
				RemoveCodeContents(parse_info.CodeList, macroPos, removeEndPos);
				parse_info.CodeList[lineIdx] = parse_info.CodeList[lineIdx].Insert(macroPos.ColNum, replaceStr);
				// 用宏值去替换原来的宏名(宏展开)
				//codeList[lineIdx] = codeList[lineIdx].Replace(macroName, replaceStr);
				return true;
			}
			return false;
		}

		public static string WholeWordSReplace(string input_str, string old_value, string new_value)
		{
			int offset = 0;
			while (true)
			{
				int old_offset = offset;
				if (offset < input_str.Length && Char.IsWhiteSpace(input_str[offset]))
				{
					offset++;
					continue;
				}
				string idStr = GetNextIdentifier2(input_str, ref offset);
				if (null == idStr)
				{
					break;
				}
				if (IsStandardIdentifier(idStr) && idStr == old_value)
				{
					input_str = input_str.Remove(old_offset, idStr.Length);
					input_str = input_str.Insert(old_offset, new_value);
					offset = old_offset + new_value.Length;
				}
				else
				{
					offset = old_offset + idStr.Length;
				}
			}
			return input_str;
		}

		static void RemoveCodeContents(List<string> code_list, CODE_POSITION start_pos, CODE_POSITION end_pos)
		{
			for (int i = start_pos.RowNum; i <= end_pos.RowNum; i++)
			{
				int startIdx = -1;
				int endIdx = -1;
				if (start_pos.RowNum == i)
				{
					startIdx = start_pos.ColNum;
				}
				if (end_pos.RowNum == i)
				{
					endIdx = end_pos.ColNum;
				}

				if (-1 != startIdx && -1 == endIdx)
				{
					code_list[i] = code_list[i].Remove(startIdx);
				}
				else if (-1 == startIdx && -1 != endIdx)
				{
					code_list[i] = code_list[i].Remove(0, endIdx + 1);
				}
				else if (-1 != startIdx && -1 != endIdx)
				{
					code_list[i] = code_list[i].Remove(startIdx, endIdx - startIdx + 1);
				}
			}
		}

		/// <summary>
		/// 取得参数列表
		/// </summary>
		public static List<string> GetParaList(List<string> codeList, CODE_POSITION bracketLeft, CODE_POSITION bracketRight)
		{
			List<string> retParaList = new List<string>();
			string catStr = LineStringCat(codeList, bracketLeft, bracketRight);
			// 去掉小括号
			if (catStr.StartsWith("("))
			{
				catStr = catStr.Remove(0, 1);
			}
			if (catStr.EndsWith(")"))
			{
				catStr = catStr.Remove(catStr.LastIndexOf(')'));
			}
			catStr = catStr.Trim();
			if ("" == catStr)
			{
				return retParaList;
			}
			// 有一种古典写法, 把参数声明列表写在小括号外, 花括号(函数体开始)前
			char[] sepArr = new char[] { ',', ';' };
			string[] paras = catStr.Split(sepArr);
			foreach (string p in paras)
			{
				if (string.Empty != p.Trim())
				{
					retParaList.Add(p.Trim());
				}
			}
			return retParaList;
		}

        /// <summary>
        /// 将起止位置之间的内容连接成一个字符串(去掉换行)
        /// </summary>
		public static string LineStringCat(List<string> codeList, CODE_POSITION startPos, CODE_POSITION endPos)
		{
			int startRow = startPos.RowNum;
			int startCol = startPos.ColNum;
			int endRow = endPos.RowNum;
			int endCol = endPos.ColNum;
			StringBuilder retStr = new StringBuilder();
			while (startRow < endRow)
			{
				retStr.Append(codeList[startRow].Substring(startCol));
				startRow += 1;
				startCol = 0;
			}
			retStr.Append(codeList[startRow].Substring(startCol, endCol - startCol + 1));
			return retStr.ToString();
		}

		/// <summary>
		/// 取得一个表达式
		/// </summary>
		public static string GetPrecompileExpressionStr(List<string> codeList, ref CODE_POSITION searchPos)
		{
			string lineStr = codeList[searchPos.RowNum];
			string exprStr = lineStr.Substring(searchPos.ColNum);
			while (exprStr.EndsWith("\\"))
			{
				exprStr = exprStr.Remove(exprStr.Length - 1);
				searchPos.Move2HeadOfNextRow();
				lineStr = codeList[searchPos.RowNum];
				exprStr += lineStr.Substring(searchPos.ColNum);
			}
			searchPos.Move2HeadOfNextRow();
			return exprStr.Trim();
		}

		/// <summary>
		/// 判断表达式是否已定义(#if defined)
		/// </summary>
        public static bool JudgeExpressionDefined(string exp, FILE_PARSE_INFO parse_info)
		{
			while (exp.StartsWith("(")
				   && exp.EndsWith(")"))
			{
				exp = exp.Remove(exp.Length - 1);
				exp = exp.Remove(0, 1).Trim();
			}
			if (null != parse_info.FindMacroDefInfo(exp))
			{
				return true;
			}
			else if (IsConstantNumber(exp))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        /// <summary>
        /// 查找下一个指定的标识符
        /// </summary>
        public static CODE_POSITION FindNextSpecIdentifier(string idStr, List<string> codeList, CODE_POSITION searchPos)
		{
			CODE_POSITION foundPos = null;
			while (true)
			{
				CODE_IDENTIFIER nextIdtf = GetNextIdentifier(codeList, ref searchPos, out foundPos);
				if (null == nextIdtf)
				{
					break;
				}
				else if (idStr == nextIdtf.Text)
				{
					return foundPos;
				}
			}
			return null;
		}

        /// <summary>
        /// 移动到指定位置的下一位置
        /// </summary>
        public static CODE_POSITION PositionMoveNext(List<string> codeList, CODE_POSITION thisPos)
		{
			CODE_POSITION nextPos = new CODE_POSITION(thisPos);
            if (thisPos.ColNum == codeList[thisPos.RowNum].Length - 1)
			{
				// 已经是最后一列了, 就移到下一行开头
                if (thisPos.RowNum < codeList.Count - 1)
                {
					nextPos.Move2HeadOfNextRow();
                }
			}
			else
			{
				// 否则移到下一列
				nextPos.ColNum += 1;
			}
			return nextPos;
		}

        /// <summary>
        /// 移动到指定位置的前一位置
        /// </summary>
        public static CODE_POSITION PositionMovePrevious(List<string> codeList, CODE_POSITION thisPos)
        {
            CODE_POSITION prevPos = new CODE_POSITION(thisPos);
            if (0 == thisPos.ColNum)
            {
                // 已经是第一列了, 就移到上一行末尾
                if (thisPos.RowNum > 0)
                {
                    prevPos.RowNum -= 1;
                    prevPos.ColNum = codeList[prevPos.RowNum].Length - 1;
                }
            }
            else
            {
                // 否则移到前一列
                prevPos.ColNum -= 1;
            }
            return prevPos;
        }

		/// <summary>
		/// 比较两个位置 0:一致; 1:前者大(靠后); -1:后者大(靠后);
		/// </summary>
        public static int PositionCompare(CODE_POSITION p1, CODE_POSITION p2)
		{
			if (p1.RowNum > p2.RowNum)
			{
				return 1;
			}
			else if (p1.RowNum < p2.RowNum)
			{
				return -1;
			}
			else if (p1.ColNum > p2.ColNum)
			{
				return 1;
			}
			else if (p1.ColNum < p2.ColNum)
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

		public static bool IsUsrDefTypeName(string type_name, FILE_PARSE_INFO parse_info, out USER_DEFINE_TYPE_INFO usr_def_type_info)
		{
			char[] sep = new char[] {' ', '\t'};
			string[] arr = type_name.Split(sep);
			List<string> tmpList = new List<string>();
			foreach (string item in arr)
			{
				if (!string.IsNullOrEmpty(item.Trim()))
				{
					tmpList.Add(item);
				}
			}
			if (2 == tmpList.Count
				&& IsUsrDefTypeKWD(tmpList[0])
				&& null != (usr_def_type_info = parse_info.FindUsrDefTypeInfo(tmpList[1], tmpList[0])))
			{
				return true;
			}
			else if (1 == tmpList.Count
					 && null != (usr_def_type_info = parse_info.FindUsrDefTypeInfo(tmpList[0], string.Empty)))
			{
				return true;
			}
			else
			{
				usr_def_type_info = null;
				return false;
			}
		}

		/// <summary>
		/// 取得括号括起来的一组操作数
		/// </summary>
		public static List<STATEMENT_COMPONENT> GetBraceComponents(List<STATEMENT_COMPONENT> componentList, ref int idx)
		{
			STATEMENT_COMPONENT cpnt = componentList[idx];
			string matchOp = string.Empty;
			// 
			if ("(" == cpnt.Text)
			{
				matchOp = ")";
			}
			else if ("[" == cpnt.Text)
			{
				matchOp = "]";
			}
			else if ("{" == cpnt.Text)
			{
				matchOp = "}";
			}
			if (string.Empty == matchOp)
			{
				return null;
			}
			List<STATEMENT_COMPONENT> retList = new List<STATEMENT_COMPONENT>();
			retList.Add(cpnt);
			int matchCount = 1;
			for (int j = idx + 1; j < componentList.Count; j++)
			{
				idx = j;
				retList.Add(componentList[j]);
				if (cpnt.Text == componentList[j].Text)
				{
					matchCount += 1;
				}
				else if (matchOp == componentList[j].Text)
				{
					matchCount -= 1;
				}
				if (0 == matchCount)
				{
					break;
				}
			}
			return retList;
		}

		/// <summary>
		/// 取得语句内各基本成分(运算数或者是运算符)
		/// </summary>
		public static List<STATEMENT_COMPONENT> GetComponents(string statementStr,
																FILE_PARSE_INFO parse_info,
																bool replace_empty_macro_def = true)
		{
			// 去掉结尾的分号
			if (statementStr.EndsWith(";"))
			{
				statementStr = statementStr.Remove(statementStr.Length - 1).Trim();
			}
			List<STATEMENT_COMPONENT> componentList = new List<STATEMENT_COMPONENT>();
			int offset = 0;
			do
			{
				// 提取语句的各个组成部分(操作数或者是操作符)
				STATEMENT_COMPONENT cpnt = GetOneComponent(ref statementStr, ref offset, parse_info, replace_empty_macro_def);
				if (string.Empty == cpnt.Text)
				{
					// 语句结束
					break;
				}
				else
				{
					componentList.Add(cpnt);
				}
			} while (true);
			return componentList;
		}

		public static string GetComponentListStr(List<STATEMENT_COMPONENT> component_list)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < component_list.Count; i++)
			{
				if (0 != i
					&& component_list[i].Type == StatementComponentType.Operator
					&& component_list[i - 1].Type == StatementComponentType.Operator)
				{
					// 连续两个符号之间加一个空格防止解析错误, 比如下面的"- -"变成了"--"
					// #if defined(_LARGEFILE64_SOURCE) && -_LARGEFILE64_SOURCE - -1 == 1
					sb.Append(" ");
				}
				sb.Append(component_list[i].Text);
			}
			return sb.ToString();
		}

		/// <summary>
		/// 从语句中提取出一个操作数/操作符
		/// </summary>
		static STATEMENT_COMPONENT GetOneComponent(ref string statementStr,
												  ref int offset,
												  FILE_PARSE_INFO parse_info,
												  bool replace_empty_macro_def)
		{
			string idStr = null;
			int offset_old = -1;
			STATEMENT_COMPONENT retSC = new STATEMENT_COMPONENT();
			while (true)
			{
				offset_old = offset;
				idStr = GetNextIdentifier2(statementStr, ref offset);
				if (null == idStr)
				{
					break;
				}
				else if (IsConstantNumber(idStr))
				{
					retSC.Type = StatementComponentType.ConstantNumber;
					retSC.Text = idStr;
					break;														        // 数字常量
				}
				else if (IsStringOrChar(idStr, statementStr, ref offset))
				{
					break;														        // 字符或者字符串
				}
				else if (IsStandardIdentifier(idStr))						// 标准标识符
				{
					// 如果包含宏, 首先要进行宏展开
					if (null != parse_info
						&& MacroDetectAndExpand_Statement(idStr, ref statementStr, offset, parse_info, replace_empty_macro_def))
					{
						offset = offset_old;
						continue;
					}
					else
					{
						retSC = new STATEMENT_COMPONENT(idStr);
						retSC.Type = StatementComponentType.Identifier;
						break;
					}
				}
				else if (IsOperator(idStr, statementStr, ref offset, ref retSC))
				{
					break;
				}
				else
				{
					retSC = new STATEMENT_COMPONENT(idStr);
					break;
				}
			}
			return retSC;
		}

		/// <summary>
		/// 判断是否是运算符
		/// </summary>
		static bool IsOperator(string idStr, string statementStr, ref int offset, ref STATEMENT_COMPONENT component)
		{
			if (1 != idStr.Length)
			{
				return false;
			}
			component.Type = StatementComponentType.Operator;
			int startOffset = offset;
			string nextIdStr = string.Empty;
			offset += 1;
			if (offset != statementStr.Length)
			{                                                                           // 不是本语句的末尾,取得下一个位置的字符
				nextIdStr = statementStr.Substring(offset, 1);
			}
			switch (idStr)
			{
				case "(":
				case ")":
				case "[":
				case "]":
				case ".":
					component.Text = idStr;
					component.Priority = 1;
					break;
				case "=":
					if (nextIdStr == idStr)
					{
						// "==" : 等于
						component.Text = idStr + nextIdStr;
						component.Priority = 7;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// "=" : 赋值
						component.Text = idStr;
						component.Priority = 14;
						component.OperandCount = 2;
					}
					break;
				case "+":
				case "-":
					if (nextIdStr == idStr)
					{
						// "++", "--" : 自增, 自减
						component.Text = idStr + nextIdStr;
						component.Priority = 2;
						component.OperandCount = 1;
						offset += 1;
					}
					else if ("=" == nextIdStr)
					{
						// "+=", "-=" : 加减运算赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 14;
						component.OperandCount = 2;
						offset += 1;
					}
					else if ("-" == idStr && ">" == nextIdStr)
					{
						// "->" : 指针成员
						component.Text = idStr + nextIdStr;
						component.Priority = 1;
						offset += 1;
					}
					else
					{
						// "+", "-" : 加, 减
						component.Text = idStr;
						if ("-" == idStr)
						{
							component.Priority = -1;								    // 不确定:可能是减号(优先级4)也可能是单目运算符的负号(优先级2)
						}
						else
						{
							component.Priority = 4;										// 加号
						}
						component.OperandCount = 2;
					}
					break;
				case "*":
				case "/":
					if ("=" == nextIdStr)
					{
						// "*=", "/=" : 乘除运算赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 14;
						component.OperandCount = 2;
						offset += 1;
					}
					else if ("*" == idStr)
					{
						// "*" : 乘
						component.Text = idStr;
						component.Priority = -1;										// 不确定:可能是乘号(优先级3), 也可能是指针运算符(优先级2)
						component.OperandCount = 2;
					}
					else
					{
						// "/" : 除
						component.Text = idStr;
						component.Priority = 3;
						component.OperandCount = 2;
					}
					break;
				case ">":
				case "<":
					if (nextIdStr == idStr)
					{
						string thirdChar = statementStr.Substring(offset + 1, 1);
						if ("=" == thirdChar)
						{
							// ">>=", "<<=" : 位移赋值
							component.Text = idStr + nextIdStr + thirdChar;
							component.Priority = 14;
							component.OperandCount = 2;
							offset += 2;
						}
						else
						{
							// ">>", "<<" : 左移, 右移
							component.Text = idStr + nextIdStr;
							component.Priority = 5;
							component.OperandCount = 2;
							offset += 1;
						}
					}
					else if ("=" == nextIdStr)
					{
						// ">=", "<=" : 大于等于, 小于等于
						component.Text = idStr + nextIdStr;
						component.Priority = 6;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// ">", "<" : 大于, 小于
						component.Text = idStr;
						component.Priority = 6;
						component.OperandCount = 2;
					}
					break;
				case "&":
				case "|":
					if (nextIdStr == idStr)
					{
						// "&&", "||" : 逻辑与, 逻辑或
						component.Text = idStr + nextIdStr;
						if ("&" == idStr)
						{
							component.Priority = 11;
						}
						else
						{
							component.Priority = 12;
						}
						component.OperandCount = 2;
						offset += 1;
					}
					else if ("=" == nextIdStr)
					{
						// "&=", "|=" : 位运算赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 14;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// "&", "|" : 位与, 位或
						component.Text = idStr;
						if ("&" == idStr)
						{
							component.Priority = -1;									// 不确定, 可能是双目位与&(优先级8),也可能是取地址符&(优先级2)
						}
						else
						{
							component.Priority = 10;
						}
						component.OperandCount = 2;
					}

					break;
				case "!":
					if ("=" == nextIdStr)
					{
						// "!=" : 不等于
						component.Text = idStr + nextIdStr;
						component.Priority = 7;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// "!" : 逻辑非
						component.Text = idStr;
						component.Priority = 2;
						component.OperandCount = 1;
					}
					break;
				case "~":
					// "~" : 按位取反
					component.Text = idStr;
					component.Priority = 2;
					component.OperandCount = 1;
					break;
				case "%":
					if ("=" == nextIdStr)
					{
						// "%=" : 取余赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 14;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// "%" : 取余
						component.Text = idStr;
						component.Priority = 3;
						component.OperandCount = 2;
					}
					break;
				case "^":
					if ("=" == nextIdStr)
					{
						// "^=" : 位异或赋值
						component.Text = idStr + nextIdStr;
						component.Priority = 14;
						component.OperandCount = 2;
						offset += 1;
					}
					else
					{
						// "^" : 位异或
						component.Text = idStr;
						component.Priority = 9;
						component.OperandCount = 2;
					}
					break;
				case ",":
					// "," : 逗号
					component.Text = idStr;
					component.Priority = 15;
					component.OperandCount = 2;
					break;
				case "?":
				case ":":
					// "?:" : 条件(三目)
					component.Text = idStr;
					component.Priority = 13;
					component.OperandCount = 3;
					break;
				default:
					return false;
			}
			return true;
		}

		/// <summary>
		/// 判断标识符是否是立即数常量
		/// </summary>
		static bool IsConstantNumber(string idStr)
		{
			int i = 0;
			bool retVal = false;
			for (; i < idStr.Length; i++)
			{
				if (!Char.IsDigit(idStr[i]))
				{
					if (Char.IsLetter(idStr[i]) && (i > 0))
					{
					}
					else if (idStr[i].Equals('.') && (i > 0))
					{
					}
					else
					{
						retVal = false;
						break;
					}
				}
				retVal = true;
			}
			return retVal;
		}

		static bool IsStringOrChar(string idStr, string statementStr, ref int offset)
		{
			if ("\"" == idStr)
			{

			}
			else if ("\'" == idStr)
			{

			}
			else
			{
				return false;
			}
			return false;
		}

		/// <summary>
		/// 函数内的宏展开 TODO:以后考虑重构跟MacroDetectAndExpand_File合并
		/// </summary>
		static bool MacroDetectAndExpand_Statement(string idStr, ref string statementStr, int offset, FILE_PARSE_INFO parse_info, bool replace_empty_macro_def)
		{
			// 遍历查找宏名
			MACRO_DEFINE_INFO mdi = parse_info.FindMacroDefInfo(idStr);
			if (null != mdi)
			{
				if (string.IsNullOrEmpty(mdi.ValStr)
					&& false == replace_empty_macro_def)
				{
				}
				else
				{
					string macroName = mdi.Name;
					string replaceStr = mdi.ValStr;
					// 判断有无带参数
					if (0 != mdi.ParaList.Count)
					{
						// 取得宏参数
						string paraStr = GetNextIdentifier2(statementStr, ref offset);
						if ("(" != paraStr)
						{
							//CommonProcess.ErrReport();
							return false;
						}
						int leftBracket = offset;
						int rightBracket = statementStr.Substring(offset).IndexOf(')');
						if (-1 == rightBracket)
						{
							ErrReport();
							return false;
						}
						paraStr = statementStr.Substring(leftBracket + 1, rightBracket - 1).Trim();
						macroName += statementStr.Substring(leftBracket, rightBracket + 1);
						string[] realParas = paraStr.Split(',');
						// 然后用实参去替换宏值里的形参
						int idx = 0;
						foreach (string rp in realParas)
						{
							if (string.Empty == rp)
							{
								// 参数有可能为空, 即没有参数, 只有一对空的括号里面什么参数也不带
								continue;
							}
							replaceStr = WholeWordSReplace(replaceStr, mdi.ParaList[idx], rp);
							//replaceStr = replaceStr.Replace(mdi.ParaList[idx], rp);
							idx++;
						}
					}
					// 应对宏里面出现的"##"
					string[] seps = { "##" };
					string[] arr = replaceStr.Split(seps, StringSplitOptions.None);
					if (arr.Length > 1)
					{
						string newStr = "";
						foreach (string sepStr in arr)
						{
							newStr += sepStr.Trim();
						}
						replaceStr = newStr;
					}
					// 单个"#"转成字串的情况暂未对应, 以后遇到再说, 先出个error report作为保护
					if (replaceStr.Contains('#'))
					{
						ErrReport();
						return false;
					}
					// 用宏值去替换原来的宏名(宏展开)
					if (idStr == replaceStr)
					{
						return false;
					}
					int macroIdx = offset - idStr.Length;
					statementStr = statementStr.Remove(macroIdx, idStr.Length);
					statementStr = statementStr.Insert(macroIdx, replaceStr);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 取得自定义类型表示成员的字符串
		/// </summary>
		public static string GetUsrDefTypeMemberStr(string in_mem_str, FILE_PARSE_INFO source_info)
		{
			string memberStr = in_mem_str;
			string idStr;
			int offset = 0, old_offset;
			// 可能包含宏, 所有要检测宏, 有的话要做宏展开
			while (true)
			{
				old_offset = offset;
				idStr = GetNextIdentifier2(memberStr, ref offset);
				if (null == idStr)
				{
					break;
				}
				else if (IsStandardIdentifier(idStr)
						 && MacroDetectAndExpand_Statement(idStr, ref memberStr, offset, source_info, true))
				{
					offset = old_offset;
					continue;
				}
				else
				{
					offset += idStr.Length;
				}
			}
			return memberStr.Trim();
		}

		public static string GetStatementStr(List<string> code_list, CODE_SCOPE code_scope)
		{
			return LineStringCat(code_list, code_scope.Start, code_scope.End).Trim();
		}

		static bool IsTypePrefix(string identifier)
		{
			switch (identifier)
			{
				case "extern":
				case "const":
				case "static":
				case "register":
				case "volatile":
					return true;
				default:
					break;
			}
			return false;
		}

		static bool IsTypeSuffix(string identifier)
		{
			switch (identifier)
			{
				case "*":
				case "const":
					return true;
				default:
					break;
			}
			return false;
		}

		static bool IsUsrDefVarType(List<string> idStrList, FILE_PARSE_INFO parse_info, ref int count)
		{
			string categoryStr = string.Empty;
			string idStr = idStrList[0];
			count = 1;
			if (IsUsrDefTypeKWD(idStr))
			{
				categoryStr = idStr;
				if (idStrList.Count > 1)
				{
					idStr = idStrList[1];
					count = 2;
				}
				else
				{
					return false;
				}
				if (null != parse_info.FindUsrDefTypeInfo(idStr, categoryStr))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (null != parse_info.FindTypeDefInfo(idStr))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		static bool IsVarType(List<STATEMENT_COMPONENT> cpntList, ref int index, FILE_PARSE_INFO parse_info)
		{
			// 判断是否是类型名
			List<string> idStrList = new List<string>();
			for (int i = index; i < cpntList.Count; i++)
			{
				idStrList.Add(cpntList[i].Text);
			}
			int count = 0;
			if (BasicTypeProc.IsBasicTypeName(idStrList, ref count))
			{
				index += count;
				return true;
			}
			else if (IsUsrDefVarType(idStrList, parse_info, ref count))
			{
				index += count;
				return true;
			}
			return false;
		}

		static MEANING_GROUP GetCodeBlockGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_result)
		{
			if ("{" == cpnt_list[idx].Text)
			{
				List<STATEMENT_COMPONENT> braceList = GetBraceComponents(cpnt_list, ref idx);
				if (null != braceList)
				{
					MEANING_GROUP retGroup = new MEANING_GROUP();
					retGroup.Type = MeaningGroupType.CodeBlock;
					retGroup.ComponentList.AddRange(braceList);
					retGroup.TextStr = GetComponentListStr(braceList);
					idx += 1;
					return retGroup;
				}
			}
			return null;
		}

		static MEANING_GROUP GetStringBlockGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_result)
		{
			if ("\"" == cpnt_list[idx].Text)
			{
				MEANING_GROUP retGroup = new MEANING_GROUP();
				retGroup.Type = MeaningGroupType.StringBlock;
				retGroup.ComponentList.Add(cpnt_list[idx]);
				StringBuilder sb = new StringBuilder();
				sb.Append(cpnt_list[idx].Text);
				for (int i = idx + 1; i < cpnt_list.Count; i++)
				{
					retGroup.ComponentList.Add(cpnt_list[i]);
					sb.Append(cpnt_list[idx].Text);
					if ("\"" == cpnt_list[i].Text && "\\" != cpnt_list[i - 1].Text)
					{
						idx = i + 1;
						break;
					}
				}
				retGroup.TextStr = sb.ToString();
				return retGroup;
			}
			return null;
		}

		static MEANING_GROUP GetCharBlockGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_result)
		{
			if ("\'" == cpnt_list[idx].Text
				&& idx < cpnt_list.Count - 3
				&& "\'" == cpnt_list[idx + 2].Text)
			{
				MEANING_GROUP retGroup = new MEANING_GROUP();
				retGroup.Type = MeaningGroupType.CharBlock;
				retGroup.ComponentList.Add(cpnt_list[idx]);
				retGroup.TextStr += cpnt_list[idx].Text;
				retGroup.ComponentList.Add(cpnt_list[idx + 1]);
				retGroup.TextStr += cpnt_list[idx + 1].Text;
				retGroup.ComponentList.Add(cpnt_list[idx + 2]);
				retGroup.TextStr += cpnt_list[idx + 2].Text;
				idx += 3;
				return retGroup;
			}
			return null;
		}

		static MEANING_GROUP GetOperatorGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx)
		{
			MEANING_GROUP retGroup = null;
			if (cpnt_list[idx].Type == StatementComponentType.Operator)
			{
				retGroup = new MEANING_GROUP();
				retGroup.ComponentList.Add(cpnt_list[idx]);
				retGroup.TextStr = cpnt_list[idx].Text;
				if ("=" == cpnt_list[idx].Text)
				{
					retGroup.Type = MeaningGroupType.AssignmentMark;
				}
				else
				{
					retGroup.Type = MeaningGroupType.OtherOperator;
				}
				idx += 1;
			}
			return retGroup;
		}

		static MEANING_GROUP GetVarTypeGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_info)
		{
			MEANING_GROUP retGroup = null;
			List<STATEMENT_COMPONENT> prefixList = new List<STATEMENT_COMPONENT>();
			for (int i = idx; i < cpnt_list.Count; i++)
			{
				int old_idx = idx;
				// 判断有无类型前缀
				if (null == retGroup && IsTypePrefix(cpnt_list[i].Text))
				{
					prefixList.Add(cpnt_list[i]);
					idx++;
				}
				else if (null == retGroup && IsVarType(cpnt_list, ref idx, parse_info))
				{
					retGroup = new MEANING_GROUP();
					retGroup.Type = MeaningGroupType.VariableType;
					for (int j = 0; j < prefixList.Count; j++)
					{
						retGroup.ComponentList.Add(prefixList[j]);
						retGroup.TextStr += prefixList[j].Text + " ";
					}
					retGroup.PrefixCount = prefixList.Count;
					for (int j = old_idx; j < idx; j++)
					{
						retGroup.ComponentList.Add(cpnt_list[j]);
						retGroup.TextStr += cpnt_list[j].Text + " ";
					}
					i = idx - 1;
					retGroup.TextStr = retGroup.TextStr.Trim();
				}
				else if (null != retGroup
						 && IsTypeSuffix(cpnt_list[i].Text))
				{
					retGroup.ComponentList.Add(cpnt_list[i]);
					retGroup.TextStr += " " + cpnt_list[i].Text;
					retGroup.SuffixCount++;
					idx++;
				}
				else
				{
					break;
				}
			}
			return retGroup;
		}

		static MEANING_GROUP GetSingleKeywordGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_info)
		{
			if ("typedef" == cpnt_list[idx].Text
				|| "auto" == cpnt_list[idx].Text
				|| "break" == cpnt_list[idx].Text
				|| "case" == cpnt_list[idx].Text
				|| "continue" == cpnt_list[idx].Text
				|| "default" == cpnt_list[idx].Text
				|| "goto" == cpnt_list[idx].Text
				|| "return" == cpnt_list[idx].Text
				|| "sizeof" == cpnt_list[idx].Text)
			{
				MEANING_GROUP retGroup = new MEANING_GROUP();
				retGroup.Type = MeaningGroupType.SingleKeyword;
				retGroup.ComponentList.Add(cpnt_list[idx]);
				retGroup.TextStr = cpnt_list[idx].Text;
				idx += 1;
				return retGroup;
			}
			return null;
		}

		static MEANING_GROUP GetExpressionGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_info)
		{
			if ("(" == cpnt_list[idx].Text)
			{
				List<STATEMENT_COMPONENT> braceList = GetBraceComponents(cpnt_list, ref idx);
				if (null != braceList)
				{
					MEANING_GROUP retGroup = new MEANING_GROUP();
					int tmpIdx = 1;
					if (IsVarType(braceList, ref tmpIdx, parse_info)
						&& tmpIdx == braceList.Count - 1)
					{
						retGroup.Type = MeaningGroupType.TypeCasting;
					}
					else
					{
						retGroup.Type = MeaningGroupType.Expression;
					}
					retGroup.ComponentList.AddRange(braceList);
					retGroup.TextStr = GetComponentListStr(braceList);
					idx += 1;
					return retGroup;
				}
			}
			return null;
		}

		static MEANING_GROUP GetFunctionCallingGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_info)
		{
			if (IsStandardIdentifier(cpnt_list[idx].Text))
			{
				// 判断是否是函数名
				if (null != parse_info.FindFuncParseInfo(cpnt_list[idx].Text)
					&& "(" == cpnt_list[idx + 1].Text)
				{
					MEANING_GROUP retGroup = new MEANING_GROUP();
					retGroup.Type = MeaningGroupType.FunctionCalling;
					retGroup.ComponentList.Add(cpnt_list[idx]);
					retGroup.TextStr += cpnt_list[idx].Text;
					idx += 1;
					List<STATEMENT_COMPONENT> braceList = GetBraceComponents(cpnt_list, ref idx);
					if (null != braceList)
					{
						retGroup.ComponentList.AddRange(braceList);
						retGroup.TextStr += GetComponentListStr(braceList);
						idx += 1;
						return retGroup;
					}
				}
			}
			return null;
		}

		static MeaningGroupType GetVariableType(List<STATEMENT_COMPONENT> braceList, FILE_PARSE_INFO parse_info)
		{
			foreach (STATEMENT_COMPONENT item in braceList)
			{
				if (IsStandardIdentifier(item.Text))
				{
					if (null != parse_info.FindGlobalVarInfoByName(item.Text))
					{
						return MeaningGroupType.GlobalVariable;
					}
					else
					{
						return MeaningGroupType.LocalVariable;
					}
				}
			}
			return MeaningGroupType.LocalVariable;
		}

		static void GetVarMemberGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, ref MEANING_GROUP ret_group)
		{
			int i = idx + 1;
			for (i = idx + 1; i < cpnt_list.Count; i++)
			{
				if ("." == cpnt_list[i].Text
					|| "->" == cpnt_list[i].Text)
				{
					ret_group.ComponentList.Add(cpnt_list[i]);
					ret_group.TextStr += cpnt_list[i].Text;
					i += 1;
					ret_group.ComponentList.Add(cpnt_list[i]);
					ret_group.TextStr += cpnt_list[i].Text;
				}
				else if ("[" == cpnt_list[i].Text)
				{
					idx = i;
					List<STATEMENT_COMPONENT> braceList = GetBraceComponents(cpnt_list, ref idx);
					ret_group.ComponentList.AddRange(braceList);
					ret_group.TextStr += GetComponentListStr(braceList);
					i = idx;
				}
				else
				{
					break;
				}
			}
			idx = i;
		}

		static MEANING_GROUP GetVarNameGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, List<MEANING_GROUP> group_list, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
		{
			MEANING_GROUP retGroup = null;
			if (IsStandardIdentifier(cpnt_list[idx].Text))
			{
				// 是否是函数参数
				// 是否为局部变量
				if (null != func_ctx
					&& func_ctx.IsLocalVariable(cpnt_list[idx].Text))
				{
					retGroup = new MEANING_GROUP();
					retGroup.Type = MeaningGroupType.LocalVariable;
					retGroup.ComponentList.Add(cpnt_list[idx]);
					retGroup.TextStr = cpnt_list[idx].Text;
					GetVarMemberGroup(cpnt_list, ref idx, ref retGroup);
					return retGroup;
				}
				// 是否为全局变量
				else if (null != parse_info.FindGlobalVarInfoByName(cpnt_list[idx].Text))
				{
					retGroup = new MEANING_GROUP();
					retGroup.Type = MeaningGroupType.GlobalVariable;
					retGroup.ComponentList.Add(cpnt_list[idx]);
					retGroup.TextStr = cpnt_list[idx].Text;
					GetVarMemberGroup(cpnt_list, ref idx, ref retGroup);
					return retGroup;
				}
				// 如果前面是类型名且是开头,那么可能是新定义变量
				else if (1 == group_list.Count
					&& group_list[0].Type == MeaningGroupType.VariableType)
				{
					retGroup = new MEANING_GROUP();
					if (null == func_ctx)
					{
						retGroup.Type = MeaningGroupType.GlobalVariable;
					}
					else
					{
						retGroup.Type = MeaningGroupType.LocalVariable;
					}
					retGroup.ComponentList.Add(cpnt_list[idx]);
					retGroup.TextStr = cpnt_list[idx].Text;
					GetVarMemberGroup(cpnt_list, ref idx, ref retGroup);
					return retGroup;
				}
			}
			else if (null != (retGroup = GetVarNameGroupWithSubMember(cpnt_list, ref idx, parse_info)))
			{
				return retGroup;
			}
			return null;
		}

		static MEANING_GROUP GetVarNameGroupWithSubMember(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_info)
		{
			if ("(" == cpnt_list[idx].Text)
			{
				int tmp_idx = idx;
				List<STATEMENT_COMPONENT> braceList = GetBraceComponents(cpnt_list, ref tmp_idx);
				if (null != braceList
					&& (tmp_idx != cpnt_list.Count - 1)
					&& ("." == cpnt_list[tmp_idx + 1].Text
						|| "->" == cpnt_list[tmp_idx + 1].Text))
				{
					MEANING_GROUP retGroup = new MEANING_GROUP();
					retGroup.Type = GetVariableType(braceList, parse_info);
					retGroup.ComponentList.AddRange(braceList);
					retGroup.TextStr = GetComponentListStr(braceList);
					GetVarMemberGroup(cpnt_list, ref tmp_idx, ref retGroup);
					idx = tmp_idx;
					return retGroup;
				}
			}
			return null;
		}

		static MEANING_GROUP GetVarNameGroup2(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			MEANING_GROUP retGroup = null;
			if (IsStandardIdentifier(cpnt_list[idx].Text))
			{
				// 首先在当前上下文中查找这个标识符
				VAR_CTX2 varCtx = deducer_ctx.FindVarCtxByName(cpnt_list[idx].Text);
				if (null != varCtx)
				{
					MeaningGroupType type = MeaningGroupType.Unknown;
					switch (varCtx.VarCategory)
					{
						case VAR_CATEGORY.FUNC_PARA:
							type = MeaningGroupType.FuncPara;
							break;
						case VAR_CATEGORY.LOCAL:
							type = MeaningGroupType.LocalVariable;
							break;
						case VAR_CATEGORY.GLOBAL:
							type = MeaningGroupType.GlobalVariable;
							break;
						default:
							break;
					}
					if (type != MeaningGroupType.Unknown)
					{
						retGroup = new MEANING_GROUP();
						retGroup.Type = type;
						retGroup.ComponentList.Add(cpnt_list[idx]);
						retGroup.TextStr = cpnt_list[idx].Text;
						GetVarMemberGroup(cpnt_list, ref idx, ref retGroup);
						return retGroup;
					}
				}
			}
			else if (null != (retGroup = GetVarNameGroupWithSubMember(cpnt_list, ref idx, parse_info)))
			{
				return retGroup;
			}
			return null;
		}

		/// <summary>
		/// 取得一个构成分组
		/// </summary>
		public static MEANING_GROUP GetOneMeaningGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, List<MEANING_GROUP> group_list, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
		{
			MEANING_GROUP retGroup = null;
			// 是变量名?
			if (null != (retGroup = GetVarNameGroup(cpnt_list, ref idx, group_list, parse_info, func_ctx)))
			{
				return retGroup;
			}
			else if (null != (retGroup = GetOtherMeaningGroup(cpnt_list, ref idx, parse_info)))
			{
				return retGroup;
			}
			else if (IsStandardIdentifier(cpnt_list[idx].Text))
			{
				retGroup = new MEANING_GROUP();
				if (cpnt_list[idx].Type == StatementComponentType.Identifier)
				{
					retGroup.Type = MeaningGroupType.Identifier;
				}
				else
				{
					retGroup.Type = MeaningGroupType.Unknown;
				}
				retGroup.ComponentList.Add(cpnt_list[idx]);
				retGroup.TextStr = cpnt_list[idx].Text;
				idx += 1;
				if ("defined" == retGroup.TextStr)
				{
					MEANING_GROUP nextGroup = GetOneMeaningGroup(cpnt_list, ref idx, group_list, parse_info, func_ctx);
					if (null != nextGroup)
					{
						MEANING_GROUP defGroup = new MEANING_GROUP();
						defGroup.ComponentList.AddRange(retGroup.ComponentList);
						defGroup.ComponentList.AddRange(nextGroup.ComponentList);
						defGroup.TextStr = retGroup.TextStr + " " + nextGroup.TextStr;
						defGroup.Type = MeaningGroupType.Expression;
						return defGroup;
					}
				}
				else if (idx < cpnt_list.Count
						 && "[" == cpnt_list[idx].Text)									// 标识符后面跟着一个中括号(数组?)
				{
					List<STATEMENT_COMPONENT> braceList = GetBraceComponents(cpnt_list, ref idx);
					retGroup.ComponentList.AddRange(braceList);
					retGroup.TextStr += GetComponentListStr(braceList);
					retGroup.Type = MeaningGroupType.Expression;
					idx += 1;
				}
				return retGroup;
			}
			else
			{
				// What the hell is this?
				System.Windows.Forms.MessageBox.Show("=== GetOneMeaningGroup === Assert!!!");
				System.Diagnostics.Trace.Assert(false);
				return null;
			}
		}

		public static MEANING_GROUP GetOneMeaningGroup2(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			MEANING_GROUP retGroup = null;
			// 是变量名?
			if (null != (retGroup = GetVarNameGroup2(cpnt_list, ref idx, parse_info, deducer_ctx)))
			{
				return retGroup;
			}
			else if (null != (retGroup = GetOtherMeaningGroup(cpnt_list, ref idx, parse_info)))
			{
				return retGroup;
			}
			else if (IsStandardIdentifier(cpnt_list[idx].Text))
			{
				retGroup = new MEANING_GROUP();
				if (cpnt_list[idx].Type == StatementComponentType.Identifier)
				{
					retGroup.Type = MeaningGroupType.Identifier;
				}
				else
				{
					retGroup.Type = MeaningGroupType.Unknown;
				}
				retGroup.ComponentList.Add(cpnt_list[idx]);
				retGroup.TextStr = cpnt_list[idx].Text;
				idx += 1;
				if ("defined" == retGroup.TextStr)
				{
					MEANING_GROUP nextGroup = GetOneMeaningGroup2(cpnt_list, ref idx, parse_info, deducer_ctx);
					if (null != nextGroup)
					{
						MEANING_GROUP defGroup = new MEANING_GROUP();
						defGroup.ComponentList.AddRange(retGroup.ComponentList);
						defGroup.ComponentList.AddRange(nextGroup.ComponentList);
						defGroup.TextStr = retGroup.TextStr + " " + nextGroup.TextStr;
						defGroup.Type = MeaningGroupType.Expression;
						return defGroup;
					}
				}
				else if (idx < cpnt_list.Count
						 && "[" == cpnt_list[idx].Text)									// 标识符后面跟着一个中括号(数组?)
				{
					List<STATEMENT_COMPONENT> braceList = GetBraceComponents(cpnt_list, ref idx);
					retGroup.ComponentList.AddRange(braceList);
					retGroup.TextStr += GetComponentListStr(braceList);
					retGroup.Type = MeaningGroupType.Expression;
					idx += 1;
				}
				return retGroup;
			}
			else
			{
				// What the hell is this?
				System.Windows.Forms.MessageBox.Show("=== GetOneMeaningGroup2 === Assert!!!");
				System.Diagnostics.Trace.Assert(false);
				return null;
			}
		}

		static MEANING_GROUP GetOtherMeaningGroup(List<STATEMENT_COMPONENT> cpnt_list, ref int idx, FILE_PARSE_INFO parse_info)
		{
			MEANING_GROUP retGroup = null;
			// 是类型名?
			if (null != (retGroup = GetVarTypeGroup(cpnt_list, ref idx, parse_info)))
			{
				return retGroup;
			}
			// 基本类型名以外的关键字?
			else if (null != (retGroup = GetSingleKeywordGroup(cpnt_list, ref idx, parse_info)))
			{
				return retGroup;
			}
			// 是函数调用?
			else if (null != (retGroup = GetFunctionCallingGroup(cpnt_list, ref idx, parse_info)))
			{
				return retGroup;
			}
			// 是表达式? 或者是强制类型转换运算符
			else if (null != (retGroup = GetExpressionGroup(cpnt_list, ref idx, parse_info)))
			{
				return retGroup;
			}
			// "{和}括起的代码块"
			else if (null != (retGroup = GetCodeBlockGroup(cpnt_list, ref idx, parse_info)))
			{
				return retGroup;
			}
			// 双引号""括起的字符串
			else if (null != (retGroup = GetStringBlockGroup(cpnt_list, ref idx, parse_info)))
			{
				return retGroup;
			}
			// 单引号''括起的单个字符
			else if (null != (retGroup = GetCharBlockGroup(cpnt_list, ref idx, parse_info)))
			{
				return retGroup;
			}
			// 是运算符
			else if (null != (retGroup = GetOperatorGroup(cpnt_list, ref idx)))
			{
				return retGroup;
			}
			else if (IsConstantNumber(cpnt_list[idx].Text))
			{
				retGroup = new MEANING_GROUP();
				retGroup.Type = MeaningGroupType.Constant;
				retGroup.ComponentList.Add(cpnt_list[idx]);
				retGroup.TextStr = cpnt_list[idx].Text;
				idx += 1;
				return retGroup;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 如果MeaningGroup里有多于一个以上的运算符,把高优先级的运算符跟对应的运算数结合成表达式
		/// </summary>
		static bool CombineHighPriorityOperatorToExpression(List<MEANING_GROUP> in_list)
		{
			int operatorCount = 0;	// 运算符的个数
			int highestPriority = int.MaxValue;
			int idx = -1;
			// TODO: 20170823 如果有多个相同(最高)优先级的运算符且连续,要根据运算符的结合方向进行结合
			for (int i = in_list.Count - 1; i >= 0; i--)
			{
				if (in_list[i].Type == MeaningGroupType.OtherOperator
					|| in_list[i].Type == MeaningGroupType.AssignmentMark)
				{
					if (in_list[i].ComponentList[0].Priority < highestPriority)
					{
						highestPriority = in_list[i].ComponentList[0].Priority;
						idx = i;
					}
					operatorCount++;
				}
			}
			if (operatorCount > 1
				&& "," != in_list[idx].TextStr)
			{
				int operandCount = in_list[idx].ComponentList[0].OperandCount;
				if (2 == operandCount)
				{
					List<STATEMENT_COMPONENT> newList = new List<STATEMENT_COMPONENT>();
					string newText = in_list[idx - 1].TextStr + " " + in_list[idx].TextStr + " " + in_list[idx + 1].TextStr;
					newList.AddRange(in_list[idx - 1].ComponentList);
					newList.AddRange(in_list[idx].ComponentList);
					newList.AddRange(in_list[idx + 1].ComponentList);
					in_list.RemoveRange(idx - 1, 3);
					MEANING_GROUP newGroup = new MEANING_GROUP();
					newGroup.ComponentList = newList;
					newGroup.TextStr = newText;
					newGroup.Type = MeaningGroupType.Expression;
					in_list.Insert(idx - 1, newGroup);
				}
				else if (1 == operandCount)
				{
					List<STATEMENT_COMPONENT> newList = new List<STATEMENT_COMPONENT>();
					string newText = in_list[idx].TextStr + in_list[idx + 1].TextStr;
					newList.AddRange(in_list[idx].ComponentList);
					newList.AddRange(in_list[idx + 1].ComponentList);
					in_list.RemoveRange(idx, 2);
					MEANING_GROUP newGroup = new MEANING_GROUP();
					newGroup.ComponentList = newList;
					newGroup.TextStr = newText;
					newGroup.Type = MeaningGroupType.Expression;
					in_list.Insert(idx, newGroup);
				}
				else
				{
					System.Diagnostics.Trace.Assert(false);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// 确定未明确的运算符(比如-是减号还是负号)
		/// </summary>
		static void ConfirmUncertainPriorityOperator(List<MEANING_GROUP> mg_list)
		{
			for (int i = 0; i < mg_list.Count; i++)
			{
				if (mg_list[i].Type == MeaningGroupType.OtherOperator
					&& mg_list[i].ComponentList[0].Priority == -1)
				{
					string oprStr = mg_list[i].TextStr;
					int oprCnt = ConfirmUnaryOrBinaryOperator(mg_list, i);
					switch (oprStr)
					{
						case "&":														// 单目取地址或者双目位与
							if (1 == oprCnt)
							{
								mg_list[i].ComponentList[0].Priority = 2;
								mg_list[i].ComponentList[0].OperandCount = 1;
							}
							else if (2 == oprCnt)
							{
								mg_list[i].ComponentList[0].Priority = 8;
								mg_list[i].ComponentList[0].OperandCount = 2;
							}
							break;
						case "-":														// 单目负号或者双目减号
							if (1 == oprCnt)
							{
								mg_list[i].ComponentList[0].Priority = 2;
								mg_list[i].ComponentList[0].OperandCount = 1;
							}
							else if (2 == oprCnt)
							{
								mg_list[i].ComponentList[0].Priority = 4;
								mg_list[i].ComponentList[0].OperandCount = 2;
							}
							break;
						case "*":														// 单目指针取值或者双目乘号
							if (1 == oprCnt)
							{
								mg_list[i].ComponentList[0].Priority = 2;
								mg_list[i].ComponentList[0].OperandCount = 1;
							}
							else if (2 == oprCnt)
							{
								mg_list[i].ComponentList[0].Priority = 3;
								mg_list[i].ComponentList[0].OperandCount = 2;
							}
							break;
						default:
							System.Diagnostics.Trace.Assert(false);
							break;
					}
				}
			}
		}

		/// <summary>
		/// 判定是单目运算符还是双目运算符
		/// </summary>
		static int ConfirmUnaryOrBinaryOperator(List<MEANING_GROUP> mg_list, int opr_idx)
		{
			System.Diagnostics.Trace.Assert(opr_idx >= 0 && mg_list.Count > opr_idx);
			if (0 == opr_idx)
			{
				return 1;
			}
			else if (mg_list[opr_idx - 1].Type == MeaningGroupType.OtherOperator
					|| mg_list[opr_idx - 1].Type == MeaningGroupType.AssignmentMark)
			{
				return 1;
			}
			else
			{
				return 2;
			}
		}

		/// <summary>
		/// 对语句所有构成成分进行结构分组
		/// </summary>
		public static List<MEANING_GROUP> GetMeaningGroups(List<STATEMENT_COMPONENT> cpnt_list, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
		{
			string statementStr = GetComponentListStr(cpnt_list);
			List<MEANING_GROUP> groupList = new List<MEANING_GROUP>();
			int idx = 0;
			while (true)
			{
				if (idx >= cpnt_list.Count)
				{
					break;
				}
				MEANING_GROUP newGroup = GetOneMeaningGroup(cpnt_list, ref idx, groupList, parse_info, func_ctx);
				if (0 != newGroup.ComponentList.Count)
				{
					groupList.Add(newGroup);
				}
				else
				{
					break;
				}
			}
			return MeaningGroupListAdjust(groupList, parse_info, func_ctx);
		}

		static List<MEANING_GROUP> MeaningGroupListAdjust(List<MEANING_GROUP> group_list, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
		{
			ConfirmUncertainPriorityOperator(group_list);
			while (CombineHighPriorityOperatorToExpression(group_list))
			{
			}
			while (true)
			{
				if (1 == group_list.Count
					&& group_list[0].ComponentList.Count >= 2
					&& "(" == group_list[0].ComponentList.First().Text
					&& ")" == group_list[0].ComponentList.Last().Text)
				{
					List<STATEMENT_COMPONENT> cpntList = group_list[0].ComponentList;
					cpntList.RemoveAt(0);
					cpntList.RemoveAt(cpntList.Count - 1);
					group_list = GetMeaningGroups(cpntList, parse_info, func_ctx);
				}
				else
				{
					break;
				}
			}
			return group_list;
		}

		static List<MEANING_GROUP> MeaningGroupListAdjust2(List<MEANING_GROUP> group_list, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			ConfirmUncertainPriorityOperator(group_list);
			while (CombineHighPriorityOperatorToExpression(group_list))
			{
			}
			while (true)
			{
				if (1 == group_list.Count
					&& group_list[0].ComponentList.Count >= 2
					&& "(" == group_list[0].ComponentList.First().Text
					&& ")" == group_list[0].ComponentList.Last().Text)
				{
					List<STATEMENT_COMPONENT> cpntList = group_list[0].ComponentList;
					cpntList.RemoveAt(0);
					cpntList.RemoveAt(cpntList.Count - 1);
					group_list = GetMeaningGroups2(cpntList, parse_info, deducer_ctx);
				}
				else
				{
					break;
				}
			}
			return group_list;
		}

		public static List<MEANING_GROUP> GetMeaningGroups2(string statement_str, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			List<STATEMENT_COMPONENT> componentList = GetComponents(statement_str, parse_info);
			return GetMeaningGroups2(componentList, parse_info, deducer_ctx);
		}

		public static List<MEANING_GROUP> GetMeaningGroups2(List<STATEMENT_COMPONENT> cpnt_list, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			List<MEANING_GROUP> groupList = new List<MEANING_GROUP>();
			int idx = 0;
			while (true)
			{
				if (idx >= cpnt_list.Count)
				{
					break;
				}
				MEANING_GROUP newGroup = GetOneMeaningGroup2(cpnt_list, ref idx, parse_info, deducer_ctx);
				if (0 != newGroup.ComponentList.Count)
				{
					groupList.Add(newGroup);
				}
				else
				{
					break;
				}
			}
			return MeaningGroupListAdjust2(groupList, parse_info, deducer_ctx);
		}

		public static void TypeDefProc(List<STATEMENT_COMPONENT> cpnt_list, FILE_PARSE_INFO parse_info, FUNC_CONTEXT func_ctx)
		{
			// 提取含义分组
			List<MEANING_GROUP> meaningGroupList = GetMeaningGroups(cpnt_list, parse_info, func_ctx);
			if (3 == meaningGroupList.Count
				&& "typedef" == meaningGroupList[0].TextStr
				&& meaningGroupList[1].Type == MeaningGroupType.VariableType
				&& meaningGroupList[2].Type == MeaningGroupType.Identifier)
			{
				TYPE_DEFINE_INFO tdi = new TYPE_DEFINE_INFO();
				tdi.OldName = meaningGroupList[1].TextStr;
				tdi.NewName = meaningGroupList[2].TextStr;
				if (tdi.OldName != tdi.NewName)
				{
					parse_info.TypeDefineList.Add(tdi);
				}
			}
		}

		public static bool GetConstantNumberValue(string const_num_text, out int val)
		{
			string tmpStr = const_num_text.ToUpper();
			int postfixCount = 0;
			for (int i = tmpStr.Length - 1; i >= 0; i--)
			{
				char ch = tmpStr[i];
				if (ch == 'U' || ch == 'L')
				{
					postfixCount += 1;
				}
				else
				{
					break;
				}
			}
			if (0 != postfixCount)
			{
				tmpStr = tmpStr.Remove(tmpStr.Length - postfixCount);
			}
			if (tmpStr.StartsWith("0X"))
			{
				tmpStr = tmpStr.Remove(0, 2);
				if (int.TryParse(tmpStr, System.Globalization.NumberStyles.HexNumber, null, out val))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else if (int.TryParse(tmpStr, out val))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool Int2Bool(int val)
		{
			if (0 != val)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public class CODE_IDENTIFIER
	{
		public string Text = string.Empty;
		public CODE_POSITION Position = null;

		public CODE_IDENTIFIER(string text, CODE_POSITION pos)
		{
			this.Text = text;
			this.Position = new CODE_POSITION(pos);
		}
	}
}
