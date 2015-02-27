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
        public static CFileInfo HeaderFileProcess(string fileName)
        {
            List<string> wtList = RemoveComments(fileName);
            wtList = RemoveConditionalCompile(wtList);

            int lineIdx = 0, startIdx = 0;
            CFileInfo fi = CodeAnalyze(wtList, ref lineIdx, ref startIdx);

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
                ErrReport();
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
                            if ("" == udti.name)
                            {
                                udti.name = "USR_DEFINE_TYPE_ANONYMOUS_" + fi.user_def_type_list.Count.ToString();
                            }
                            fi.user_def_type_list.Add(udti);
                            qualifierList.Clear();
                            qualifierList.Add(udti.name);
                            // TODO: 要去掉后面跟着的分号, 避免后面误认为全局量
                        }
                    }
                }
                // 否则可能是各种符号
                else
                {
                    // 遇到小括号了, 可能是碰上函数声明或定义了
                    if (("(" == nextId) && (0 != qualifierList.Count))
                    {
                        CFunctionInfo cfi = FunctionDetectProcess(codeList, qualifierList, ref searchPos);
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
                        PreprocessCommandProcess(codeList, ref searchPos, ref fi);
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
                        // 注意用户定义类型后面的分号不是全局量
                        if (2 <= qualifierList.Count)
                        {
                            GlobalVarProcess(codeList, qualifierList, ref fi);
                        }
                    }
                    else
                    {
//                      System.Diagnostics.Trace.WriteLine("艾玛! 不晓得这是啥玩意? line = " + (lineIdx + 1).ToString() + "Col = " + startIdx.ToString());
                    }
                    qualifierList.Clear();
                }
            }

            return fi;
        }

        /// <summary>
        /// 预处理命令处理
        /// </summary>
        static void PreprocessCommandProcess(List<string> codeList, ref File_Position searchPos, ref CFileInfo fi)
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
                DefineProcess(codeList, ref searchPos, ref fi);
            }
            // 条件编译
            else if ("if" == cmd.ToLower())
            {
            }
            else if ("ifdef" == cmd.ToLower())
            {
            }
            else if ("ifndef" == cmd.ToLower())
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
        static CFunctionInfo FunctionDetectProcess(List<string> codeList, List<string> qualifierList, ref File_Position searchPos)
        {
            CFunctionInfo cfi = new CFunctionInfo();

            // 先找匹配的小括号
            File_Position fp = FindNextSymbol(codeList, searchPos, ')');
            if (null == fp)
            {
                ErrReport();
                return null;
            }
            File_Position bracketLeft = null;
            File_Position bracketRight = null;
            if (codeList[searchPos.row_num].Substring(searchPos.col_num).Trim().StartsWith("*"))
            {
                // 吗呀, 这不是传说中的函数指针嘛...
                File_Position sp = searchPos;
                File_Position ep = fp;
                ep.col_num -= 1;
                searchPos = FindNextSymbol(codeList, fp, '(');
                if (null == searchPos)
                {
                    ErrReport();
                    return null;
                }
                fp = FindNextSymbol(codeList, searchPos, ')');
                if (null == fp)
                {
                    ErrReport();
                    return null;
                }
                string nameStr = LineStringCat(codeList, sp, ep);
                cfi.name = nameStr;
            }
            bracketLeft = new File_Position(searchPos);
            bracketRight = fp;
            List<string> paraList = GetParaList(codeList, bracketLeft, bracketRight);

            // 然后确认小括号后面是否跟着配对的大括号
            searchPos = new File_Position(fp.row_num, fp.col_num + 1);
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
                fp = FindNextMatchSymbol(codeList, searchPos, '}');
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
            string[] paras = catStr.Split(',');
            if (0 == paras.Length)
            {
                ErrReport();
                return null;
            }
            List<string> retParaList = new List<string>();
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
                    retUsrTypeInfo.name = nextIdStr;
                    nextIdStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
                    if ("{" != nextIdStr)
                    {
                        ErrReport();
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
                searchPos = foundPos;
                searchPos.col_num += 1;
                nextIdStr = GetNextIdentifier(codeList, ref searchPos, out foundPos);
                if (IsStandardIdentifier(nextIdStr))
                {
                    retUsrTypeInfo.name = nextIdStr;
                }
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
        static void GlobalVarProcess(List<string> codeList, List<string> qualifierList, ref CFileInfo cfi)
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
                gvi.array_size_str = qlfStr;
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

        static void DefineProcess(List<string> codeList, ref File_Position searchPos, ref CFileInfo cfi)
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

        static void ErrReport()
        {
            System.Diagnostics.Trace.WriteLine("Something is wrong!");
        }
    }
}
