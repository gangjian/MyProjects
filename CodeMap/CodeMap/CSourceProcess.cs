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
        /// ".c"源文件处理
        /// </summary>
        /// <param name="fileName"></param>
        public static List<CFileParseInfo> CFileListProcess(List<string> srcFileList, List<string> hdFileList)
        {
            List<CFileParseInfo> retList = new List<CFileParseInfo>();
            foreach (string hdFile in hdFileList)
            {
                CFileProcess(hdFile, ref retList, hdFileList);
            }
            foreach (string srcFile in srcFileList)
            {
                CFileProcess(srcFile, ref retList, hdFileList);
            }

            return retList;
        }

        static CFileParseInfo CFileProcess(string fileName, ref List<CFileParseInfo> parsedList, List<string> headerList)
        {
            // 去掉注释
            List<string> codeList = RemoveComments(fileName);
            CFileParseInfo fi = new CFileParseInfo(fileName);
            // 预编译处理
            codeList = PrecompileProcess(codeList, ref fi, ref parsedList, headerList);
//          Save2File(codeList, fileName + ".bak");

            // 从头开始解析
            File_Position sPos = new File_Position(0, 0);
            // 文件解析
            CodeAnalyze(codeList, ref sPos, ref fi);
            parsedList.Add(fi);
//          XmlProcess.SaveCFileInfo2XML(fi);

            return fi;
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
        /// 预编译处理
        /// </summary>
        /// <param name="codeList"></param>
        /// <param name="fi"></param>
        /// <param name="parsedList">已解析的文件情报列表</param>
        /// <param name="headerList">头文件名列表</param>
        /// <returns></returns>
        public static List<string> PrecompileProcess(List<string> codeList, ref CFileParseInfo fi,
                                                     ref List<CFileParseInfo> parsedList, List<string> headerList)
        {
            List<string> retList = new List<string>();
            Stack<CC_INFO> ccStack = new Stack<CC_INFO>();              // 条件编译嵌套时, 用堆栈来保存嵌套的条件编译情报参数
            CC_INFO cc_info = new CC_INFO();
            List<CFileParseInfo> includeHeaderList = new List<CFileParseInfo>();  // 该文件包含的头文件的解析情报List

            string rdLine = "";
            for (int idx = 0; idx < codeList.Count; idx++)
            {
                rdLine = codeList[idx].Trim();
                if (rdLine.StartsWith("#"))
                {
                    File_Position searchPos = new File_Position(idx, codeList[idx].IndexOf("#") + 1);
                    File_Position foundPos = null;
                    string idStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
                    if ("include" == idStr.ToLower())
                    {
                        if (false != cc_info.write_flag)
                        {
                            // 取得include文件名
                            string incFileName = GetIncludeFileName(codeList, ref searchPos);
                            System.Diagnostics.Trace.Assert(null != incFileName);
                            fi.include_file_list.Add(incFileName);
                            if (incFileName.StartsWith("\"") && incFileName.EndsWith("\""))
                            {
                                // 去掉引号
                                incFileName = incFileName.Substring(1, incFileName.Length - 2).Trim();
                                // 取得头文件的解析情报
                                CFileParseInfo incInfo = GetIncFileParsedInfo(incFileName, ref parsedList, headerList);
                                if (null != incInfo)
                                {
                                    includeHeaderList.Add(incInfo);
                                }
                            }
                        }
                    }
                    else if ("define" == idStr.ToLower())
                    {
                        if (false == cc_info.write_flag)
                        {
                            continue;
                        }
                        DefineProcess(codeList, ref searchPos, ref fi);
                    }
                    else
                    {
                        string exprStr = "";            // 表达式字符串
                        if ("if" == idStr.ToLower())
                        {
                            bool lastFlag = cc_info.write_flag;
                            ccStack.Push(cc_info);
                            cc_info = new CC_INFO();
                            if (false == lastFlag)
                            {
                                cc_info.write_flag = false;
                                cc_info.write_next_flag = false;
                            }
                            else
                            {
                                exprStr = GetExpressionStr(codeList, ref searchPos, out foundPos);
                                // 判断表达式的值
                                if (0 != JudgeExpressionValue(exprStr, includeHeaderList, fi.macro_define_list))
                                {
                                    cc_info.write_flag = false;
                                    cc_info.write_next_flag = true;
                                }
                                else
                                {
                                    cc_info.write_flag = false;
                                    cc_info.write_next_flag = false;
                                }
                            }
                        }
                        else if ("ifdef" == idStr.ToLower())
                        {
                            bool lastFlag = cc_info.write_flag;
                            ccStack.Push(cc_info);
                            cc_info = new CC_INFO();
                            if (false == lastFlag)
                            {
                                cc_info.write_flag = false;
                                cc_info.write_next_flag = false;
                            }
                            else
                            {
                                exprStr = GetExpressionStr(codeList, ref searchPos, out foundPos);
                                // 判断表达式是否已定义
                                if (null != JudgeExpressionDefined(exprStr, includeHeaderList, fi.macro_define_list))
                                {
                                    cc_info.write_flag = false;
                                    cc_info.write_next_flag = true;
                                }
                                else
                                {
                                    cc_info.write_flag = false;
                                    cc_info.write_next_flag = false;
                                }
                            }
                        }
                        else if ("ifndef" == idStr.ToLower())
                        {
                            bool lastFlag = cc_info.write_flag;
                            ccStack.Push(cc_info);
                            cc_info = new CC_INFO();
                            if (false == lastFlag)
                            {
                                cc_info.write_flag = false;
                                cc_info.write_next_flag = false;
                            }
                            else
                            {
                                exprStr = GetExpressionStr(codeList, ref searchPos, out foundPos);
                                // 判断表达式是否已定义
                                if (null != JudgeExpressionDefined(exprStr, includeHeaderList, fi.macro_define_list))
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
                        }
                        else if ("else" == idStr.ToLower())
                        {
                            bool lastFlag = true;
                            if (ccStack.Count > 0)
                            {
                                lastFlag = ccStack.Peek().write_flag;
                            }
                            if (false == lastFlag)
                            {
                                cc_info.write_flag = false;
                                cc_info.write_next_flag = false;
                            }
                            else
                            {
                                if (true == cc_info.write_flag)
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
                        }
                        else if ("elif" == idStr.ToLower())
                        {
                            bool lastFlag = true;
                            if (ccStack.Count > 0)
                            {
                                lastFlag = ccStack.Peek().write_flag;
                            }
                            if (false == lastFlag)
                            {
                                cc_info.write_flag = false;
                                cc_info.write_next_flag = false;
                            }
                            else
                            {
                                // 跟"if"一样, 但是因为不是嵌套所以不用压栈
                                exprStr = GetExpressionStr(codeList, ref searchPos, out foundPos);
                                // 判断表达式的值
                                if (0 != JudgeExpressionValue(exprStr, includeHeaderList, fi.macro_define_list))
                                {
                                    cc_info.write_flag = false;
                                    cc_info.write_next_flag = true;
                                }
                                else
                                {
                                    cc_info.write_flag = false;
                                    cc_info.write_next_flag = false;
                                }
                            }
                        }
                        else if ("endif" == idStr.ToLower())
                        {
                            bool lastFlag = true;
                            if (ccStack.Count > 0)
                            {
                                lastFlag = ccStack.Peek().write_flag;
                            }
                            if (false == lastFlag)
                            {
                                cc_info.write_flag = false;
                                cc_info.write_next_flag = false;
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
                        }
                    }
                }

                if (cc_info.write_flag)
                {
                    retList.Add(codeList[idx]);
                }
                else
                {
                    retList.Add("");
                }
                if (true == cc_info.write_next_flag)
                {
                    cc_info.write_flag = true;
                    cc_info.write_next_flag = false;
                }

                // 嵌套时弹出堆栈, 恢复之前的情报
                if (true == cc_info.pop_up_flag)
                {
                    cc_info = ccStack.Pop();
                }
            }

            return retList;
        }

        static CFileParseInfo GetIncFileParsedInfo(string incFileName, ref List<CFileParseInfo> parsedList, List<string> headerList)
        {
            // 先在已解析过的文件list里找
            foreach (var pi in parsedList)
            {
                string fName = GetFileName(pi.full_name);
                if (fName.ToLower() == incFileName.ToLower())
                {
                    // 如果找到了, 直接返回
                    return pi;
                }
            }

            // 如果上一步没找到, 证明还没被解析, 则在全部头文件list里找
            foreach (var hd_name in headerList)
            {
                string fName = GetFileName(hd_name);
                if (fName.ToLower() == incFileName.ToLower())
                {
                    // 如果找到了, 则要先解析这个头文件
                    CFileParseInfo fi = CFileProcess(hd_name, ref parsedList, headerList);
                    // TODO: 注意当有多个同名文件符合条件时的情况应对

                    return fi;
                }
            }
            // 头文件没找到
            // ErrReport(incFileName + " 头文件没找到!");

            return null;
        }

        /// <summary>
        /// 代码解析
        /// </summary>
        public static void CodeAnalyze(List<string> codeList, ref File_Position searchPos, ref CFileParseInfo fi)
        {
            System.Diagnostics.Trace.Assert((null != codeList));
            if (0 == codeList.Count)
            {
                ErrReport();
                return;
            }

            List<string> qualifierList = new List<string>();     // 修饰符暂存列表
            string nextId = null;
            File_Position foundPos = null;
            while (null != (nextId = GetNextIdentifier(codeList, ref searchPos, out foundPos)))
            {
                // 如果是标准标识符(字母,数字,下划线组成且开头不是数字)
                if (IsStandardIdentifier(nextId) 
                    || ("*" == nextId))
                {
                    if (MacroDetectAndExpand(nextId, codeList, foundPos, fi.macro_define_list))
                    {
                        // 判断是否是已定义的宏, 是的话进行宏展开
                        // 展开后要返回到原处(展开前的位置), 重新解析展开后的宏
                        searchPos = new File_Position(foundPos);
                        continue;
                    }

                    qualifierList.Add(nextId);
                    if (("struct" == nextId)
                        || ("enum" == nextId)
                        || ("union" == nextId)
                        )
                    {
                        // 用户定义类型处理
                        UsrDefineTypeInfo udti = UserDefineTypeProcess(codeList, qualifierList, ref searchPos);
                        if (null != udti)
                        {
                            // 如果是匿名类型, 要给加个名字
                            if (0 == udti.nameList.Count)
                            {
                                udti.nameList.Add("USR_DEFINE_TYPE_ANONYMOUS_" + fi.user_def_type_list.Count.ToString());
                            }
                            fi.user_def_type_list.Add(udti);
                            qualifierList.Clear();
                            qualifierList.Add(udti.nameList[0]);
                        }
                    }
                }
                // 否则可能是各种符号
                else
                {
                    // 遇到小括号了, 可能是碰上函数声明或定义了
                    if (("(" == nextId) && (0 != qualifierList.Count))
                    {
                        CFunctionInfo cfi = FunctionDetectProcess(codeList, qualifierList, ref searchPos, foundPos);
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
                    else if ("#" == nextId)
                    {
                        // 预编译命令, 因为已经处理过了, 不在这里解析, 跳到宏定义结束
                        while (codeList[searchPos.row_num].EndsWith("\\"))
                        {
                            searchPos.row_num += 1;
                        }
                        searchPos.col_num = codeList[searchPos.row_num].Length;
                    }
                    // 全局量(包含全局数组)
                    else if ("[" == nextId)
                    {
                        // 到下一个"]"出现的位置是数组长度
                        File_Position fp = FindNextSymbol(codeList, searchPos, ']');
                        if (null != fp)
                        {
                            string arraySize = LineStringCat(codeList, foundPos, fp);
                            qualifierList.Add(arraySize);
                            fp.col_num += 1;
                            searchPos = fp;
                            continue;
                        }
                    }
                    else if ("=" == nextId)
                    {
                        // 直到下一个分号出现的位置, 都是初始化语句
                        qualifierList.Add(nextId);
                        File_Position fp = FindNextSymbol(codeList, searchPos, ';');
                        if (null != fp)
                        {
                            foundPos.col_num += 1;
                            string initialStr = LineStringCat(codeList, foundPos, fp);
                            qualifierList.Add(initialStr.Trim());
                            searchPos = fp;
                            continue;
                        }
                    }
                    else if (";" == nextId)
                    {
                        // typedef类型定义
                        if (   (0 != qualifierList.Count)
                            && ("typedef" == qualifierList[0]))
                        {
                            TypeDefProcess(codeList, qualifierList, ref fi);
                        }
                        // 注意用户定义类型后面的分号不是全局量
                        else if (2 <= qualifierList.Count)
                        {
                            GlobalVarProcess(qualifierList, ref fi);
                        }
                    }
                    else if ("," == nextId)
                    {
                        GlobalVarProcess(qualifierList, ref fi);
                        continue;
                    }
                    else
                    {
                        // Do Nothing
                    }
                    qualifierList.Clear();
                }
            }
        }

        /// <summary>
        /// 函数探测(声明, 定义)
        /// </summary>
        /// <param name="codeList"></param>
        /// <param name="lineIdx"></param>
        /// <param name="startIdx"></param>
        static CFunctionInfo FunctionDetectProcess(List<string> codeList, List<string> qualifierList, ref File_Position searchPos, File_Position bracketLeft)
        {
            CFunctionInfo cfi = new CFunctionInfo();

            // 先找匹配的小括号
            File_Position bracketRight = FindNextMatchSymbol(codeList, searchPos, ')');
            if (null == bracketRight)
            {
                ErrReport();
                return null;
            }
            if (codeList[bracketLeft.row_num].Substring(bracketLeft.col_num + 1).Trim().StartsWith("*"))
            {
                // 吗呀, 这不是传说中的函数指针嘛...
                File_Position sp = bracketLeft;
                File_Position ep = bracketRight;
                sp.col_num += 1;
                ep.col_num -= 1;
                bracketLeft = FindNextSymbol(codeList, bracketRight, '(');
                if (null == searchPos)
                {
                    ErrReport();
                    return null;
                }
                bracketRight = FindNextSymbol(codeList, searchPos, ')');
                if (null == bracketRight)
                {
                    ErrReport();
                    return null;
                }
                string nameStr = LineStringCat(codeList, sp, ep);
                cfi.name = nameStr;
            }
            List<string> paraList = GetParaList(codeList, bracketLeft, bracketRight);

            // 然后确认小括号后面是否跟着配对的大括号
            searchPos = new File_Position(bracketRight.row_num, bracketRight.col_num + 1);
            File_Position foundPos = null;
            string nextIdStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
            if (";" == nextIdStr)
            {
                // 小括号后面跟着分号说明这是函数声明
                // 函数名
                if ("" == cfi.name)
                {
                    cfi.name = qualifierList.Last();
                    // 函数修饰符
                    qualifierList.RemoveAt(qualifierList.Count - 1);
                }
                cfi.qualifiers = new List<string>(qualifierList);
                // 参数列表
                cfi.paras = paraList;
            }
            else if ("{" == nextIdStr)
            {
                File_Position bodyStartPos = foundPos;
                // 小括号后面跟着配对的大括号说明这是函数定义(带函数体)
                File_Position fp = FindNextMatchSymbol(codeList, searchPos, '}');
                if (null == fp)
                {
                    ErrReport();
                    return null;
                }
                searchPos = new File_Position(fp.row_num, fp.col_num + 1);
                // 函数名
                cfi.name = qualifierList.Last();
                // 函数修饰符
                qualifierList.RemoveAt(qualifierList.Count - 1);
                cfi.qualifiers = qualifierList;
                // 参数列表
                cfi.paras = paraList;
                // 函数体起始位置
                cfi.body_start_pos = bodyStartPos;
                cfi.body_end_pos = fp;
            }
            else
            {
                // 估计是出错了
                ErrReport();
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
                ErrReport();
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
        /// 取得参数列表
        /// </summary>
        /// <returns></returns>
        static List<string> GetParaList(List<string> codeList, File_Position bracketLeft, File_Position bracketRight)
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
            string[] paras = catStr.Split(',');
            foreach (string p in paras)
            {
                retParaList.Add(p.Trim());
            }
            return retParaList;
        }

        /// <summary>
        /// 用户定义类型处理
        /// </summary>
        /// <param name="codeList"></param>
        /// <param name="startPos"></param>
        /// <param name="qualifierList"></param>
        /// <returns></returns>
        static UsrDefineTypeInfo UserDefineTypeProcess(List<string> codeList, List<string> qualifierList, ref File_Position startPos)
        {
            if (0 == qualifierList.Count)
            {
                ErrReport();
                return null;
            }
            string keyStr = qualifierList.Last();
            string fstStr = qualifierList.First();

            UsrDefineTypeInfo retUsrTypeInfo = new UsrDefineTypeInfo();
            File_Position searchPos = new File_Position(startPos);
            File_Position foundPos = null;
            string nextIdStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
            if ("{" != nextIdStr)
            {
                if (IsStandardIdentifier(nextIdStr))
                {
                    retUsrTypeInfo.nameList.Add(nextIdStr);
                    nextIdStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
                    if ("{" != nextIdStr)
                    {
                        // 没找到最大括号, 说明这不是一个新的用户定义类型
                        if ((0 != qualifierList.Count)
                            && ("typedef" == qualifierList[0]))
                        {
                            // 说明这是一个typedef类型定义, 这里返回不做处理, 等后面解析到分号";"的时候再进行typedef类型定义处理
                        }
                        return null;
                    }
                }
                else
                {
                    ErrReport();
                    return null;
                }
            }
            retUsrTypeInfo.body_start_pos = foundPos;
            foundPos = FindNextMatchSymbol(codeList, searchPos, '}');
            if (null == foundPos)
            {
                ErrReport();
                return null;
            }
            retUsrTypeInfo.body_end_pos = foundPos;
            string catStr = LineStringCat(codeList, retUsrTypeInfo.body_start_pos, retUsrTypeInfo.body_end_pos);
            if (catStr.StartsWith("{"))
            {
                catStr = catStr.Remove(0, 1);
            }
            if (catStr.EndsWith("}"))
            {
                catStr = catStr.Remove(catStr.LastIndexOf('}'));
            }
            char sepStr = ';';
            retUsrTypeInfo.type = keyStr;
            if ("enum" == keyStr)
            {
                sepStr = ',';
            }
            string[] members = catStr.Split(sepStr);
            if (0 == members.Length)
            {
                ErrReport();
                return null;
            }
            foreach (string m in members)
            {
                string memStr = m.Trim();
                if (string.Empty != memStr)
                {
                    retUsrTypeInfo.memberList.Add(m.Trim());
                }
            }

            if ("typedef" == fstStr)
            {
                searchPos = foundPos;   // 右侧大括号
                searchPos.col_num += 1;
                File_Position sPos = new File_Position(searchPos);
                foundPos = FindNextSpecIdentifier(";", codeList, searchPos);    // 找到分号; 也就是typedef结束的位置
                if (null == foundPos)
                {
                    return null;
                }
                retUsrTypeInfo.nameList.Clear();
                string nameStr = LineStringCat(codeList, sPos, foundPos);
                string[] nameArr = nameStr.Split(',');
                foreach (string name in nameArr)
                {
                    if (IsStandardIdentifier(name.Trim()))
                    {
                        retUsrTypeInfo.nameList.Add(name.Trim());
                    }
                }
                foundPos.col_num -= 1;
                searchPos = foundPos;
            }
            startPos = searchPos;

            return retUsrTypeInfo;
        }

        /// <summary>
        /// 全局变量处理
        /// </summary>
        /// <param name="codeList"></param>
        /// <param name="qualifierList"></param>
        /// <param name="searchPos"></param>
        /// <param name="cfi"></param>
        static void GlobalVarProcess(List<string> qualifierList, ref CFileParseInfo cfi)
        {
            GlobalVarInfo gvi = new GlobalVarInfo();

            // 判断是否有初始化语句
            int idx = -1;
            if (-1 != (idx = qualifierList.IndexOf("=")))
            {
                gvi.initial_string += qualifierList[idx + 1];
                for (int i = qualifierList.Count - 1; i >= idx; i--)
                {
                    qualifierList.RemoveAt(i);
                }
            }

            // 判断是否是数组
            idx = qualifierList.Count - 1;
            string qlfStr = qualifierList[idx].Trim();
            if (qlfStr.StartsWith("[") && qlfStr.EndsWith("]"))
            {
                qlfStr = qlfStr.Substring(1, qlfStr.Length - 2).Trim();
                gvi.array_size_string = qlfStr;
                qualifierList.RemoveAt(idx);
            }

            // 变量名
            idx = qualifierList.Count - 1;
            qlfStr = qualifierList[idx].Trim();
            if (IsStandardIdentifier(qlfStr))
            {
                gvi.name = qlfStr;
            }
            else
            {
                return;
            }
            qualifierList.RemoveAt(idx);

            // 类型名
            idx = qualifierList.Count - 1;
            qlfStr = qualifierList[idx].Trim();
            string type_name = "";
            while (false == IsStandardIdentifier(qlfStr))
            {
                type_name = qlfStr + type_name;
                idx--;
                qlfStr = qualifierList[idx].Trim();
            }
            type_name = qlfStr + type_name;
            gvi.type = type_name;

            // 剩余的都放到修饰符列表里去
            for (int i = 0; i < idx; i++)
            {
                gvi.qualifiers.Add(qualifierList[i].Trim());
            }
            if ((0 != gvi.qualifiers.Count)
                && ("extern" == gvi.qualifiers.First().Trim().ToLower()))
            {
                cfi.global_var_declare_list.Add(gvi);
            }
            else
            {
                cfi.global_var_define_list.Add(gvi);
            }
        }

        static void DefineProcess(List<string> codeList, ref File_Position searchPos, ref CFileParseInfo cfi)
        {
            File_Position sPos, fPos;
            sPos = new File_Position(searchPos);
            string nextIdStr = GetNextIdentifier(codeList, ref sPos, out fPos);
            if (!IsStandardIdentifier(nextIdStr))
            {
                return;
            }
            MacroDefineInfo mdi = new MacroDefineInfo();
            mdi.name = nextIdStr;

            string leftStr = codeList[sPos.row_num].Substring(sPos.col_num);
            if (leftStr.StartsWith("("))
            {
                File_Position ePos = FindNextSymbol(codeList, sPos, ')');
                if (null == ePos)
                {
                    return;
                }
                mdi.paras = GetParaList(codeList, sPos, ePos);
                sPos = ePos;
                sPos.col_num += 1;
            }

            string defineValStr = codeList[sPos.row_num].Substring(sPos.col_num);
            while (defineValStr.EndsWith(@"\"))
            {
                defineValStr = defineValStr.Remove(defineValStr.Length - 1);
                sPos.row_num += 1;
                sPos.col_num = 0;
                defineValStr += codeList[sPos.row_num].Substring(sPos.col_num);
            }
            mdi.value = defineValStr.Trim();
            cfi.macro_define_list.Add(mdi);

            sPos.row_num += 1;
            sPos.col_num = 0;
            searchPos = new File_Position(sPos);
        }

        static bool MacroDetectAndExpand(string idStr, List<string> codeList, File_Position foundPos, List<MacroDefineInfo> defineList)
        {
            if (!IsStandardIdentifier(idStr))
            {
                return false;
            }
            foreach (MacroDefineInfo di in defineList)
            {
                // 判断宏名是否一致
                if (idStr == di.name)
                {
                    string macroName = di.name;
                    File_Position macroPos = new File_Position(foundPos);
                    int lineIdx = foundPos.row_num;
                    string replaceStr = di.value;
                    // 判断有无带参数
                    if (0 != di.paras.Count)
                    {
                        // 取得实参
                        File_Position sPos = new File_Position(foundPos.row_num, foundPos.col_num + idStr.Length);
                        string paraStr = GetNextIdentifier(codeList, ref sPos, out foundPos);
                        if ("(" != paraStr)
                        {
                            ErrReport();
                            break;
                        }
                        File_Position leftBracket = foundPos;
                        foundPos = FindNextSymbol(codeList, sPos, ')');
                        if (null == foundPos)
                        {
                            ErrReport();
                            break;
                        }
                        paraStr = LineStringCat(codeList, macroPos, foundPos);
                        macroName = paraStr;
                        List<string> realParas = GetParaList(codeList, leftBracket, foundPos);
                        if (realParas.Count != di.paras.Count)
                        {
                            ErrReport();
                            break;
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
                            replaceStr = replaceStr.Replace(di.paras[idx], rp);
                            idx++;
                        }

                    }
                    // 应对宏里面出现的"##"
                    string[]seps = {"##"};
                    string[]arr = replaceStr.Split(seps, StringSplitOptions.None);
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
                    codeList[lineIdx] = codeList[lineIdx].Replace(macroName, replaceStr);
                    return true;
                }
            }
            return false;
        }

        static void TypeDefProcess(List<string> codeList, List<string> qualifierList, ref CFileParseInfo cfi)
        {
            TypeDefineInfo tdi = new TypeDefineInfo();
            string old_type = "";
            for (int i = 1; i < qualifierList.Count; i++)
            {
                if (qualifierList.Count - 1 == i)
                {
                    tdi.new_type_name = qualifierList[i];
                    tdi.old_type_name = old_type;
                }
                else
                {
                    old_type += (" " + qualifierList[i]);
                }
            }
            cfi.type_define_list.Add(tdi);
        }

        static void ErrReport(string errMsg = "Something is wrong!")
        {
            System.Diagnostics.Trace.WriteLine(errMsg);
        }
    }
}
