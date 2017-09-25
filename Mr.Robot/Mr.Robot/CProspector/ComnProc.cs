using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
				&& !string.IsNullOrEmpty(mdi.Value))
			{
				string macroName = mdi.Name;
				CODE_POSITION macroPos = new CODE_POSITION(foundPos);
				CODE_POSITION removeEndPos = new CODE_POSITION(macroPos.RowNum, macroPos.ColNum + macroName.Length - 1);
				int lineIdx = foundPos.RowNum;
				string replaceStr = mdi.Value;
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
				// 如果没有参数, 只有一对空的小括号, 那么要加入一个空字串""参数
				retParaList.Add("");
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

		public static string GetComponentListStr(List<STATEMENT_COMPONENT> componentList)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var item in componentList)
			{
				sb.Append(item.Text);
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
		public static bool IsConstantNumber(string idStr)
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
		public static bool MacroDetectAndExpand_Statement(	string idStr,
															ref string statementStr,
															int offset,
															FILE_PARSE_INFO parse_info,
															bool replace_empty_macro_def)
		{
			// 遍历查找宏名
			MACRO_DEFINE_INFO mdi = parse_info.FindMacroDefInfo(idStr);
			if (null != mdi)
			{
				if (string.IsNullOrEmpty(mdi.Value)
					&& false == replace_empty_macro_def)
				{
				}
				else
				{
					string macroName = mdi.Name;
					string replaceStr = mdi.Value;
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

		public static string GetStatementStr(List<string> code_list, CODE_SCOPE code_scope)
		{
			return LineStringCat(code_list, code_scope.Start, code_scope.End).Trim();
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
