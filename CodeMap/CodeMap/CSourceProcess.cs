using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeMap
{
    // 条件编译处理情报
    class CC_INFO
    {
        public string exp = "";
        public bool unidentified_flag = false;
        public bool write_flag = true;
        public bool write_next_flag = false;
        public bool pop_up_flag = false;
    }

    // 文件当中某个内容的位置(行列号)
    public class File_Position
    {
        public int row_num = 0;    // 行号
        public int col_num = 0;    // 列号

        public File_Position(int r, int c)
        {
            row_num = r;
            col_num = c;
        }
    }

    enum E_CHAR_TYPE
    {
        E_CTYPE_WHITE_SPACE,
        E_CTYPE_LETTER,
        E_CTYPE_NUMBER,
        E_CTYPE_UNDERLINE,
        E_CTYPE_PUNCTUATION,
        E_CTYPE_SYMBOL,
        E_CTYPE_UNKNOWN
    }

    public class CFileInfo
    {
        public List<string> include_file_list = new List<string>();
        public List<CFunctionInfo> fun_declare_list = new List<CFunctionInfo>();
        public List<CFunctionInfo> fun_define_list = new List<CFunctionInfo>();
    }

    /// <summary>
    /// C函数的情报
    /// </summary>
    public class CFunctionInfo
    {
        public string name = "";                                            // 函数名称
        public List<string> qualifiers = new List<string>();                // 修饰符列表
        public List<string> paras = new List<string>();                     // 参数列表
        public File_Position body_start_pos = null;                         // 函数体开始位置
        public File_Position body_end_pos = null;                           // 函数体结束位置
    }

    class CSourceProcess
    {
        /// <summary>
        /// ".c"源文件处理
        /// </summary>
        /// <param name="fileName"></param>
        public static CFileInfo CFileProcess(string fileName)
        {
            List<string> wtList = RemoveComments(fileName);
            wtList = RemoveConditionalCompile(wtList);

            int lineIdx = 0, startIdx = 0;
            CFileInfo fi = CodeAnalyze(wtList, ref lineIdx, ref startIdx);

            return fi;
        }

        /// <summary>
        /// ".h"头文件处理
        /// </summary>
        /// <param name="fileName"></param>
        public static void HeaderFileProcess(string fileName)
        {
            List<string> wtList = RemoveComments(fileName);
            wtList = RemoveConditionalCompile(wtList);

            //TextWriter tw = new StreamWriter(fileName);
            //foreach (string wtLine in wtList)
            //{
            //    tw.WriteLine(wtLine);
            //}
            //tw.Close();
        }

        /// <summary>
        /// 移除注释
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

                if (    (-1 != idx1)
                    &&  (-1 != idx2)
                    &&  (idx1 < idx2)
                    )
                {
                    wtLine = rdLine.Remove(idx1).TrimEnd();
                }
                else if (   (-1 != idx1)
                         && (-1 == idx2))
                {
                    // 只包含行注释
                    wtLine = rdLine.Remove(idx1).TrimEnd();
                }
                else if (-1 != idx2)
                {
                    // 只包含块注释
                    int idx_s = idx2;
                    int idx_e = rdLine.IndexOf("*/");
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
        /// 去除条件编译块
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<string> RemoveConditionalCompile(List<string> inputList)
        {
            List<string> retList = new List<string>();
            List<CC_INFO> ccStack = new List<CC_INFO>();    // 条件编译嵌套时, 用堆栈来保存嵌套的条件编译情报参数
            CC_INFO cc_info = new CC_INFO();

            string rdLine = "";
            foreach (string line in inputList)
            {
                rdLine = line.Trim().ToLower();
                if (rdLine.StartsWith("#if"))
                {
                    ccStack.Add(cc_info);
                    cc_info = new CC_INFO();

                    cc_info.exp = rdLine.Remove(0, 3).Trim();
                    if ("0" == cc_info.exp)
                    {
                        cc_info.write_flag = false;
                        cc_info.write_next_flag = false;
                    }
                    else if ("1" == cc_info.exp)
                    {
                        cc_info.write_flag = false;
                        cc_info.write_next_flag = true;
                    }
                    else
                    {
                        cc_info.unidentified_flag = true;
                        cc_info.write_flag = true;
                        cc_info.write_next_flag = false;
                    }
                }
                else if (rdLine.StartsWith("#else"))
                {
                    if (cc_info.unidentified_flag)
                    {
                    }
                    else if (true == cc_info.write_flag)
                    {
                        cc_info.write_flag = false;
                        cc_info.write_next_flag = false;
                    }
                    else
                    {
                        cc_info.write_flag = false;
                        cc_info.write_next_flag = true;
                    }
                }
                else if (rdLine.StartsWith("#endif"))
                {
                    if (cc_info.unidentified_flag)
                    {
                        cc_info.unidentified_flag = false;
                    }
                    else
                    {
                        cc_info.write_flag = false;
                        cc_info.write_next_flag = true;
                    }
                    cc_info.pop_up_flag = true;
                }
                else
                {
                    if (true == cc_info.write_next_flag)
                    {
                        cc_info.write_flag = true;
                        cc_info.write_next_flag = false;
                    }
                }

                if (cc_info.write_flag)
                {
                    retList.Add(line);
                }
                else
                {
                    retList.Add("");
                }

                // 嵌套时弹出堆栈, 恢复之前的情报
                if (true == cc_info.pop_up_flag)
                {
                    int lastIdx = ccStack.Count - 1;
                    cc_info = ccStack[lastIdx];
                    ccStack.RemoveAt(lastIdx);
                }
            }

            return retList;
        }

        /// <summary>
        /// 代码解析
        /// </summary>
        public static CFileInfo CodeAnalyze(List<string> codeList, ref int lineIdx, ref int startIdx)
        {
            System.Diagnostics.Trace.Assert((null != codeList));
            if (0 == codeList.Count)
            {
                return null;
            }

            CFileInfo fi = new CFileInfo();
            List<string> qualifierList = new List<string>();     // 修饰符暂存列表
            string nextId = null;
            File_Position foundPos = null;
            File_Position searchPos = new File_Position(lineIdx, startIdx);
            while (null != (nextId = GetNextIdentifier(codeList, ref searchPos, out foundPos)))
            {
                // 如果是标准标识符(字母,数字,下划线组成且开头不是数字)
                if (IsStandardIdentifier(nextId) 
                    || ("*" == nextId))
                {
                    qualifierList.Add(nextId);
                }
                // 否则可能是各种符号
                else
                {
                    // 遇到小括号了, 可能是碰上函数声明或定义了
                    if (("(" == nextId) && (0 != qualifierList.Count))
                    {
                        CFunctionInfo cfi = FunctionDetection(codeList, qualifierList, ref searchPos);
                        if (null != cfi)
                        {
                            if (null != cfi.body_start_pos)
                            {
                                fi.fun_define_list.Add(cfi);
                            }
                            else
                            {
                                fi.fun_declare_list.Add(cfi);
                            }
                        }
                    }
                    // 井号开头的是预处理命令
                    else if ("#" == nextId)
                    {
                        PreprocessCommandHandling(codeList, ref searchPos, ref fi);
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("艾玛! 不晓得这是啥玩意? line = " + (lineIdx + 1).ToString() + "Col = " + startIdx.ToString());
                    }
                    qualifierList.Clear();
                }
            }

            return fi;
        }

        /// <summary>
        /// 预处理命令处理
        /// </summary>
        static void PreprocessCommandHandling(List<string> codeList, ref File_Position searchPos, ref CFileInfo fi)
        {
            File_Position foundPos = null;
            string cmd = GetNextIdentifier(codeList, ref searchPos, out foundPos);
            // 头文件包含
            if ("include" == cmd.ToLower())
            {
                string incFileName = GetIncludeFileName(codeList, ref searchPos);
                if (null != incFileName)
                {
                    fi.include_file_list.Add(incFileName);
                }
            }
            // 宏定义
            else if ("define" == cmd.ToLower())
            {
            }
            // 条件编译
            else if ("if" == cmd.ToLower())
            {
            }
            else if ("else" == cmd.ToLower())
            {
            }
            else if ("endif" == cmd.ToLower())
            {
            }
            else
            {
            }
        }

        /// <summary>
        /// 函数探测(声明, 定义)
        /// </summary>
        /// <param name="codeList"></param>
        /// <param name="lineIdx"></param>
        /// <param name="startIdx"></param>
        static CFunctionInfo FunctionDetection(List<string> codeList, List<string> qualifierList, ref File_Position searchPos)
        {
            CFunctionInfo cfi = null;

            // 先找匹配的小括号
            File_Position fs = FindNextSymbol(codeList, searchPos, ')');
            if (null == fs)
            {
                return null;
            }
            File_Position bracketLeft = new File_Position(searchPos.row_num, searchPos.col_num);
            File_Position bracketRight = fs;
            List<string> paraList = GetParaList(codeList, bracketLeft, bracketRight);

            // 然后确认小括号后面是否跟着配对的大括号
            searchPos = new File_Position(fs.row_num, fs.col_num + 1);
            File_Position foundPos = null;
            string nextIdStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
            if (";" == nextIdStr)
            {
                // 小括号后面跟着分号说明这是函数声明
                cfi = new CFunctionInfo();
                // 函数名
                cfi.name = qualifierList.Last();
                // 函数修饰符
                qualifierList.RemoveAt(qualifierList.Count - 1);
                cfi.qualifiers = qualifierList;
                // 参数列表
                cfi.paras = paraList;
            }
            else if ("{" == nextIdStr)
            {
                File_Position bodyStartPos = foundPos;
                // 小括号后面跟着配对的大括号说明这是函数定义(带函数体)
                fs = FindNextMatchSymbol(codeList, searchPos, '}');
                if (null == fs)
                {
                    return null;
                }
                searchPos = new File_Position(fs.row_num, fs.col_num + 1);
                cfi = new CFunctionInfo();
                // 函数名
                cfi.name = qualifierList.Last();
                // 函数修饰符
                qualifierList.RemoveAt(qualifierList.Count - 1);
                cfi.qualifiers = qualifierList;
                // 参数列表
                cfi.paras = paraList;
                // 函数体起始位置
                cfi.body_start_pos = bodyStartPos;
                cfi.body_end_pos = fs;
            }
            else
            {
                // 估计是出错了
                return null;
            }
            // 更新index
            return cfi;
        }

        static string GetIncludeFileName(List<string> codeList, ref File_Position searchPos)
        {
            File_Position foundPos = null;
            string quot = GetNextIdentifier(codeList, ref searchPos, out foundPos);
            string retName = quot;
            if ("\"" == quot)
            {
            }
            else if ("<" == quot)
            {
                quot = ">";
            }
            else
            {
                return null;
            }
            string str = GetNextIdentifier(codeList, ref searchPos, out foundPos);
            while (quot != str)
            {
                retName += str;
                str = GetNextIdentifier(codeList, ref searchPos, out foundPos);
            }
            return retName + quot;
        }

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
            foundPos = new File_Position(lineIdx, startIdx);
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
                            return null;
                    }
                }

                // 到达行末
                // 转到下一行开头
                lineIdx++;
                curIdx = 0;
                if (startFlag)
                {
                    goto RET_IDF;
                }
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

            return null;
        }

        /// <summary>
        /// 找到下一个配对的符号
        /// </summary>
        /// <param name="codeList"></param>
        /// <param name="lineIdx"></param>
        /// <param name="startIdx"></param>
        /// <param name="symbol"></param>
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
                System.Diagnostics.Trace.Assert(false);
                return null;
            }

            int matchCount = 1;
            List<int> matchCountStack = new List<int>();    // 应对#ifdef条件编译嵌套时, matchCount的堆栈管理
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
                    if ("ifdef" == idStr.ToLower())
                    {
                        // 压栈
                        matchCountStack.Add(matchCount);
                    }
                    else if ("else" == idStr.ToLower())
                    {
                        // 取值但是不出栈
                        matchCount = matchCountStack[matchCountStack.Count - 1];
                    }
                    else if ("endif" == idStr.ToLower())
                    {
                        // 出栈
                        matchCountStack.RemoveAt(matchCountStack.Count - 1);
                    }
                }
                else
                {
                }
            }
            return null;
        }

        /// <summary>
        /// 取得参数列表
        /// </summary>
        /// <returns></returns>
        static List<string> GetParaList(List<string> codeList, File_Position bracketLeft, File_Position bracketRight)
        {
            string catStr = LineStringCat(codeList, bracketLeft, bracketRight);
            // 去掉右小括号
            if (catStr.EndsWith(")"))
            {
                catStr = catStr.Remove(catStr.LastIndexOf(')'));
            }
            string[] paras = catStr.Split(',');
            if (0 == paras.Length)
            {
                return null;
            }
            List<string> retParaList = new List<string>();
            foreach (string p in paras)
            {
                retParaList.Add(p.Trim());
            }
            return retParaList;
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

    }
}
