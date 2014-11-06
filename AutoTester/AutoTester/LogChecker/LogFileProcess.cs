using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTester.LogChecker
{
    class LogFileProcess
    {
        public static List<CompareResult> logFileCompare(string logFile1, string logFile2)
        {
            // 对log文件中的内容进行分析,提取项目结果
            List<TestCaseInfo> testCaseList1 = logFileProcess(logFile1);
            List<TestCaseInfo> testCaseList2 = logFileProcess(logFile2);

            // 已经比较完的项目列表
            List<string> finishList = new List<string>();

            // 比较结果列表
            List<CompareResult> resultList = new List<CompareResult>();

            for (int i = 0; i < testCaseList1.Count; i++)
            {
                TestCaseInfo test_case = testCaseList1[i];
                string key = test_case.level1Num.ToString().PadLeft(2, '0') + test_case.level2Num.ToString().PadLeft(2, '0') + test_case.test_ID.ToString().PadLeft(3, '0');

                // 判断是否已经处理过
                if (finishList.Contains(key))
                {
                    continue;
                }
                else
                {
                    // 取得1里面所有相同的项目
                    List<TestCaseInfo> caseList1 = TestCaseProcess.getTestCaseListByNum(testCaseList1, test_case.level1Num, test_case.level2Num, test_case.test_ID);
                    // 取得2里面所有相同的项目
                    List<TestCaseInfo> caseList2 = TestCaseProcess.getTestCaseListByNum(testCaseList2, test_case.level1Num, test_case.level2Num, test_case.test_ID);

                    // 进行内容比较
                    CompareResult result = TestCaseProcess.testCaseContentsCompare(caseList1, caseList2, test_case.level1Num, test_case.level2Num, test_case.test_ID);
                    resultList.Add(result);

                    // 标记该项目已经处理过
                    finishList.Add(key);
                }
            }

            return resultList;
        }

        /// <summary>
        /// 处理log文件
        /// </summary>
        /// <param name="fullName"></param>
        private static List<TestCaseInfo> logFileProcess(string fullName)
        {
            StreamReader sr = new StreamReader(fullName, Encoding.Default);
            List<TestCaseInfo> testCaseList = new List<TestCaseInfo>();

            try
            {
                string rdline = "";
                string line = "";
                TestCaseInfo testCaseInfo = null;
                TestCaseInfo newTestCaseInfo = null;
                while (null != (rdline = sr.ReadLine()))
                {
                    line = rdline.Trim();

                    // 判断是否有新的test case情报开始
                    if (TestCaseProcess.checkNewTestCaseInfoStart(line, out newTestCaseInfo))
                    {
                        // 如果有上一个, 把上一个加到list里去
                        if (null != testCaseInfo)
                        {
                            testCaseList.Add(testCaseInfo);
                        }
                        testCaseInfo = newTestCaseInfo;
                    }
                    else
                    {
                        if (null != testCaseInfo)
                        {
                            if (("" == line)
                                || IsAllStarsStr(line))
                            {                                                   // 如果出现了空行或者"********************",
                                                                                // 则认为该测试项的log已结束
                                testCaseList.Add(testCaseInfo);
                                testCaseInfo = null;
                            }
                            else
                            {                                                   // 否则将log内容加入到该项目的详细信息list里
                                testCaseInfo.detailList.Add(line);
                            }
                        }
                    }
                }

                if (null != testCaseInfo)
                {
                    testCaseList.Add(testCaseInfo);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
            sr.Close();

            return testCaseList;
        }

        private static bool IsAllStarsStr(string str_in)
        {
            if (0 == str_in.Length)
            {
                return false;
            }
            foreach (char ch in str_in)
            {
                if (ch != '*')
                {
                    return false;
                }
            }
            return true;
        }
    }
}
