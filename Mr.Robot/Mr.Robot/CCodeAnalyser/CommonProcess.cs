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
        public static CodeIdentifier GetNextIdentifier(List<string> codeList, ref CodePosition searchPos, out CodePosition foundPos)
		{
			int lineIdx = searchPos.RowNum;
			int startIdx = searchPos.ColNum;
			foundPos = new CodePosition(searchPos);
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
				foundPos = new CodePosition(lineIdx, s_pos);
				if (curIdx >= curLine.Length)
				{
					lineIdx++;
					curIdx = 0;
				}
				startIdx = curIdx;
				searchPos.RowNum = lineIdx;
				searchPos.ColNum = startIdx;
				CodeIdentifier retId = new CodeIdentifier(curLine.Substring(s_pos, e_pos - s_pos + 1), foundPos);
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
        public static CodePosition FindNextSymbol(List<string> codeList, CodePosition searchPos, Char symbol)
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
						CodePosition fpos = new CodePosition(lineIdx, curIdx);
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
		/// <param varName="codeList"></param>
		/// <param varName="searchPos"></param>
		/// <param varName="rightSymbol"></param>
		/// <returns></returns>
		public static CodePosition FindNextMatchSymbol(FileParseInfo parse_info, ref CodePosition searchPos, Char rightSymbol)
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
			CodePosition foundPos = null;
            bool quoteStart = false;
            string quoteStr = string.Empty;
            while (true)
			{
				CodeIdentifier nextIdtf = GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
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
						searchPos = new CodePosition(foundPos);
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
													 CodePosition foundPos,
													 FileParseInfo parse_info)
		{
			// 遍历查找宏名
			MacroDefineInfo mdi = parse_info.FindMacroDefInfo(idStr);
			if (null != mdi
				&& !string.IsNullOrEmpty(mdi.Value))
			{
				string macroName = mdi.Name;
				CodePosition macroPos = new CodePosition(foundPos);
				CodePosition removeEndPos = new CodePosition(macroPos.RowNum, macroPos.ColNum + macroName.Length - 1);
				int lineIdx = foundPos.RowNum;
				string replaceStr = mdi.Value;
				// 判断有无带参数
				if (0 != mdi.ParaList.Count)
				{
					// 取得实参
					CodePosition sPos = new CodePosition(foundPos.RowNum, foundPos.ColNum + idStr.Length);
					CodeIdentifier nextIdtf = GetNextIdentifier(parse_info.CodeList, ref sPos, out foundPos);
					if ("(" != nextIdtf.Text)
					{
						//ErrReport();
						// 发现过有一个带参数的宏min(x, y), 另有某一个结构体成员名也叫min
						// 解析至此认为该结构体成员是宏, 取得参数时发现没有参数...
						// ?: 宏与结构体成员重名, 如何处理...
						return false;
					}
					CodePosition leftBracket = foundPos;
					foundPos = FindNextMatchSymbol(parse_info, ref sPos, ')');
					if (null == foundPos)
					{
						ErrReport();
						return false;
					}
					removeEndPos = new CodePosition(foundPos);
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

		static void RemoveCodeContents(List<string> code_list, CodePosition start_pos, CodePosition end_pos)
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
		/// <returns></returns>
		public static List<string> GetParaList(List<string> codeList, CodePosition bracketLeft, CodePosition bracketRight)
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
		public static string LineStringCat(List<string> codeList, CodePosition startPos, CodePosition endPos)
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
		public static string GetPrecompileExpressionStr(List<string> codeList, ref CodePosition searchPos, out CodePosition foundPos)
		{
			foundPos = new CodePosition(searchPos);

			string lineStr = codeList[searchPos.RowNum];
			string exprStr = lineStr.Substring(searchPos.ColNum);
			foundPos.ColNum = lineStr.IndexOf(exprStr);
			while (exprStr.EndsWith("\\"))
			{
				exprStr = exprStr.Remove(exprStr.Length - 1);
				searchPos.RowNum += 1;
				searchPos.ColNum = 0;
				lineStr = codeList[searchPos.RowNum];
				exprStr += lineStr.Substring(searchPos.ColNum);
			}
			searchPos.RowNum += 1;
			searchPos.ColNum = 0;

			return exprStr.Trim();
		}

		/// <summary>
		/// 判断表达式是否已定义(#if defined)
		/// </summary>
        public static bool JudgeExpressionDefined(string exp, FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert(CommonProcess.IsStandardIdentifier(exp));
			if (null != parse_info.FindMacroDefInfo(exp))
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
        public static CodePosition FindNextSpecIdentifier(string idStr, List<string> codeList, CodePosition searchPos)
		{
			CodePosition foundPos = null;
			while (true)
			{
				CodeIdentifier nextIdtf = GetNextIdentifier(codeList, ref searchPos, out foundPos);
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
        public static CodePosition PositionMoveNext(List<string> codeList, CodePosition thisPos)
		{
			CodePosition nextPos = new CodePosition(thisPos);
            if (thisPos.ColNum == codeList[thisPos.RowNum].Length - 1)
			{
				// 已经是最后一列了, 就移到下一行开头
                if (thisPos.RowNum < codeList.Count - 1)
                {
                    nextPos.RowNum += 1;
                    nextPos.ColNum = 0;
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
        public static CodePosition PositionMovePrevious(List<string> codeList, CodePosition thisPos)
        {
            CodePosition prevPos = new CodePosition(thisPos);
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
		/// <returns></returns>
        public static int PositionCompare(CodePosition p1, CodePosition p2)
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

		public static string FindTypeDefName(string type_name, FileParseInfo fpi)
		{
			foreach (TypeDefineInfo tdi in fpi.TypeDefineList)
			{
				if (tdi.NewName.Equals(type_name))
				{
					return tdi.OldName;
				}
			}
			return string.Empty;
		}

		public static bool IsUsrDefTypeName(string type_name, FileParseInfo parse_info, out UsrDefTypeInfo usr_def_type_info)
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
	}

	public class CodeIdentifier
	{
		public string Text = string.Empty;
		public CodePosition Position = null;

		public CodeIdentifier(string text, CodePosition pos)
		{
			this.Text = text;
			this.Position = new CodePosition(pos);
		}
	}
}
