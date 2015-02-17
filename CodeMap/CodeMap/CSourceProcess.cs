using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CodeMap
{
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
                        wtLine = rdLine.Remove(idx_s, idx_e - idx_s + 2);
                    }
                    else
                    {
                        wtLine = "";
                    }
                }
                else
                {
                    // 不包含注释
                    wtLine = rdLine.TrimEnd();
                }
                retList.Add(wtLine);
            } while (null != (rdLine = tr.ReadLine()));
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
            bool wtFlag = true;
            bool wtNextFlag = false;
            bool bUnknownCondition = false;
            string rdLine = "";
            foreach (string line in inputList)
            {
                rdLine = line.Trim().ToLower();
                if (rdLine.StartsWith("#if"))
                {
                    string exp = rdLine.Remove(0, 3).Trim();
                    if ("0" == exp)
                    {
                        wtFlag = false;
                        wtNextFlag = false;
                    }
                    else if ("1" == exp)
                    {
                        wtFlag = false;
                        wtNextFlag = true;
                    }
                    else
                    {
                        bUnknownCondition = true;
                        wtFlag = true;
                        wtNextFlag = false;
                    }
                }
                else if (rdLine.StartsWith("#else"))
                {
                    if (bUnknownCondition)
                    {
                    }
                    else if (true == wtFlag)
                    {
                        wtFlag = false;
                        wtNextFlag = false;
                    }
                    else
                    {
                        wtFlag = false;
                        wtNextFlag = true;
                    }
                }
                else if (rdLine.StartsWith("#endif"))
                {
                    if (bUnknownCondition)
                    {
                        bUnknownCondition = false;
                    }
                    else
                    {
                        wtFlag = false;
                        wtNextFlag = true;
                    }
                }
                else
                {
                    if (true == wtNextFlag)
                    {
                        wtFlag = true;
                        wtNextFlag = false;
                    }
                }

                if (wtFlag)
                {
                    retList.Add(line);
                }
                else
                {
                    retList.Add("");
                }
            }

            return retList;
        }
    }
}
