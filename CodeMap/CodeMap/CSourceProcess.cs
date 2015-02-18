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

    class CSourceProcess
    {
        /// <summary>
        /// ".c"源文件处理
        /// </summary>
        /// <param name="fileName"></param>
        public static void CFileProcess(string fileName)
        {
            List<string> wtList = RemoveComments(fileName);
            wtList = RemoveConditionalCompile(wtList);
            TextWriter tw = new StreamWriter(fileName);
            foreach (string wtLine in wtList)
            {
                tw.WriteLine(wtLine);
            }
            tw.Close();
        }

        /// <summary>
        /// ".h"头文件处理
        /// </summary>
        /// <param name="fileName"></param>
        public static void HeaderFileProcess(string fileName)
        {
            List<string> wtList = RemoveComments(fileName);
            wtList = RemoveConditionalCompile(wtList);
            TextWriter tw = new StreamWriter(fileName);
            foreach (string wtLine in wtList)
            {
                tw.WriteLine(wtLine);
            }
            tw.Close();
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
    }
}
