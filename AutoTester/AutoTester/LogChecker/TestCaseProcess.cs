using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTester.LogChecker
{
    class TestCaseProcess
    {
        public static List<TestCaseInfo> getTestCaseListByNum(List<TestCaseInfo> totalList, int level1Num, int level2Num, int testId)
        {
            List<TestCaseInfo> resultList = new List<TestCaseInfo>();
            foreach (TestCaseInfo test_case in totalList)
            {
                if ((level1Num == test_case.level1Num)
                    && (level2Num == test_case.level2Num)
                    && (testId == test_case.test_ID))
                {
                    resultList.Add(test_case);
                }
            }

            return resultList;
        }

        public static CompareResult testCaseContentsCompare(List<TestCaseInfo> list1, List<TestCaseInfo> list2, int level1Num, int level2Num, int testId)
        {
            CompareResult cmpResult = new CompareResult();
            cmpResult.level1Num = level1Num;
            cmpResult.level2Num = level2Num;
            cmpResult.testID = testId;
            cmpResult.reoccurs1 = list1.Count;
            cmpResult.reoccurs2 = list2.Count;

            if (0 == list1.Count)
            {
                // Only in folder2
                cmpResult.result = EnumCompareResultValue.E_ONLY_IN_FOLDER_2;
            }
            else if (0 == list2.Count)
            {
                // Only in folder1
                cmpResult.result = EnumCompareResultValue.E_ONLY_IN_FOLDER_1;
            }
            else
            {
                if (list1.Count == list2.Count)
                {
                    EnumCompareResultValue result = EnumCompareResultValue.E_OK;
                    for (int i = 0; i < list1.Count; i++)
                    {
                        result = compareSingleTestCase(list1[i], list2[i]);
                        if (0 != result)
                        {
                            break;
                        }
                    }
                    cmpResult.result = result;
                }
                else
                {
                    cmpResult.result = EnumCompareResultValue.E_API_NG; // API NG
                }
            }

            return cmpResult;
        }

        private static EnumCompareResultValue compareSingleTestCase(TestCaseInfo test_case_1, TestCaseInfo test_case_2)
        {
            EnumCompareResultValue ret = EnumCompareResultValue.E_OK;    // OK

            if (!test_case_1.titleLine.Equals(test_case_2.titleLine))
            {
                // API NG;
                ret = EnumCompareResultValue.E_API_NG;
            }
            else
            {
                if (test_case_1.detailList.Count != test_case_2.detailList.Count)
                {
                    // Data NG;
                    ret = EnumCompareResultValue.E_DATA_NG;
                }
                else
                {
                    for (int i = 0; i < test_case_1.detailList.Count; i++)
                    {
                        if (!test_case_1.detailList[i].Equals(test_case_2.detailList[i]))
                        {
                            // Data NG;
                            ret = EnumCompareResultValue.E_DATA_NG;
                            break;
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// 判断test case是否开始
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool checkNewTestCaseInfoStart(string line, out TestCaseInfo newTestCaseInfo)
        {
            newTestCaseInfo = new TestCaseInfo();

            if (!line.StartsWith("CT_No:"))
            {
                return false;
            }
            int testIdIdx = line.IndexOf("Test_ID:");
            if (-1 == testIdIdx)
            {
                return false;
            }
            string ctNoStr = line.Substring("CT_No:".Length, testIdIdx - "CT_No:".Length).Trim();
            int ulIdx = ctNoStr.IndexOf('_');
            if (-1 == ulIdx)
            {
                return false;
            }
            int l1No; // 大项目号
            int l2No; // 中项目号
            if (!int.TryParse(ctNoStr.Substring(0, ulIdx), out l1No)
                || !int.TryParse(ctNoStr.Substring(ulIdx + 1), out l2No))
            {
                return false;
            }
            newTestCaseInfo.level1Num = l1No;
            newTestCaseInfo.level2Num = l2No;

            string testIdStr = line.Substring(testIdIdx + "Test_ID:".Length).Trim();
            int spaceIdx = testIdStr.IndexOf(' ');
            if (-1 != spaceIdx)
            {
                testIdStr = testIdStr.Remove(spaceIdx);
            }
            int testId; // Test_ID
            string ch21SheetStr = "Sheet(";     // 第21章, Test_ID的格式有点特殊, 前面还多了一个Sheet号
            if (testIdStr.StartsWith(ch21SheetStr)
                && (-1 !=testIdStr.IndexOf('-')))
            {
                // 把Sheet号取出来跟后面的Test_ID拼成一个数, 整体作为Test_ID
                int idx = testIdStr.IndexOf(')');
                string sheetStr = testIdStr.Substring(ch21SheetStr.Length, idx - ch21SheetStr.Length);
                idx = testIdStr.IndexOf('-');
                string idStr = testIdStr.Substring(idx + 1).Trim();
                testIdStr = sheetStr + idStr;
            }
            if (!int.TryParse(testIdStr, out testId))
            {
                return false;
            }
            newTestCaseInfo.test_ID = testId;

            newTestCaseInfo.titleLine = line;

            return true;
        }
    }
}
