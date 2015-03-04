using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeMap
{
    partial class CSourceProcess
    {
        /// <summary>
        /// 从当前位置处取得一个标识符(identifier)
        /// </summary>
        /// <param name="codeList"></param>
        /// <param name="lineIdx"></param>
        /// <param name="startIdx"></param>
        /// <returns></returns>
        static string GetNextIdentifier(List<string> codeList, ref File_Position searchPos, out File_Position foundPos)
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
                        case E_CHAR_TYPE.E_CTYPE_NUMBER:
                            if (false == startFlag)
                            {
                                // 标识符结束, 返回该数字
                                s_pos = curIdx;
                                e_pos = curIdx;
                                curIdx++;
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
                        case E_CHAR_TYPE.E_CTYPE_UNKNOWN:
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

        static E_CHAR_TYPE GetCharType(Char ch)
        {
            if (Char.IsWhiteSpace(ch))
            {
                return E_CHAR_TYPE.E_CTYPE_WHITE_SPACE;
            }
            else if (Char.IsDigit(ch))
            {
                return E_CHAR_TYPE.E_CTYPE_NUMBER;
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
        /// <param name="id"></param>
        /// <returns></returns>
        static bool IsStandardIdentifier(string idStr)
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
        /// <param name="codeList"></param>
        /// <param name="lineIdx"></param>
        /// <param name="startIdx"></param>
        /// <param name="symbol"></param>
        static File_Position FindNextSymbol(List<string> codeList, File_Position searchPos, Char symbol)
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
        /// <param name="codeList"></param>
        /// <param name="searchPos"></param>
        /// <param name="rightSymbol"></param>
        /// <returns></returns>
        static File_Position FindNextMatchSymbol(List<string> codeList, File_Position searchPos, Char rightSymbol)
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

            Stack<int> matchCountStack = new Stack<int>();    // 应对#ifdef条件编译嵌套时, matchCount的堆栈管理
            int matchCount = 1;
            File_Position foundPos = null;
            while (true)
            {
                string idStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
                if (null == idStr)
                {
                    break;
                }
                else if (leftSymbol.ToString() == idStr)
                {
                    matchCount += 1;
                }
                else if (rightSymbol.ToString() == idStr)
                {
                    matchCount -= 1;
                    if (0 == matchCount)
                    {
                        // 找到了
                        return foundPos;
                    }
                }
                else if ("#" == idStr)
                {
                    idStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
                    if (("ifdef" == idStr.ToLower())
                        || ("ifndef" == idStr.ToLower())
                        || ("if" == idStr.ToLower()))
                    {
                        // 压栈
                        matchCountStack.Push(matchCount);
                    }
                    else if ("else" == idStr.ToLower())
                    {
                        // 取值但是不出栈
                        matchCount = matchCountStack.Peek();
                    }
                    else if ("endif" == idStr.ToLower())
                    {
                        // 出栈
                        matchCountStack.Pop();
                    }
                }
                else
                {
                }
            }
            ErrReport();
            return null;
        }

        static string LineStringCat(List<string> codeList, File_Position startPos, File_Position endPos)
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
        /// 从完整路径中取得文件名
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        static string GetFileName(string fullName)
        {
            string retName = fullName;
            int idx = fullName.LastIndexOf('\\');
            if (-1 != idx)
            {
                retName = fullName.Substring(idx + 1).Trim();
            }

            return retName;
        }

        /// <summary>
        /// 取得一个表达式
        /// </summary>
        /// <param name="codeList"></param>
        /// <param name="searchPos"></param>
        /// <param name="foundPos"></param>
        /// <returns></returns>
        static string GetExpressionStr(List<string> codeList, ref File_Position searchPos, out File_Position foundPos)
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
        /// <param name="exp">表达式</param>
        /// <param name="headerList">头文件列表</param>
        /// <param name="defineList">宏定义列表</param>
        /// <returns></returns>
        static MacroDefineInfo JudgeExpressionDefined(string exp, List<CFileInfo> headerList, List<MacroDefineInfo> defineList)
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
        /// <param name="exp">表达式</param>
        /// <param name="headerList">头文件列表</param>
        /// <param name="defineList">宏定义列表</param>
        /// <returns></returns>
        static int JudgeExpressionValue(string exp, List<CFileInfo> headerList, List<MacroDefineInfo> defineList)
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

        static string GetSimpleExpression(string complexExp, int idx)
        {
            return null;
        }

        ////////////////////////////////////////////////

        static void Save2File(List<string> writeList, string saveName)
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
    }

}

