using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTester.LogChecker
{
    public class LogChecker
    {
        public string m_outputlogFolder = "";
        public string m_masterLogFolder = "";
        public List<DisplayResult> m_displayList = null;

        public LogChecker(string outputLogFolder, string masterLogFolder)
        {
            DirectoryInfo di = new DirectoryInfo(outputLogFolder);
            System.Diagnostics.Trace.Assert(di.Exists);
            di = new DirectoryInfo(masterLogFolder);
            System.Diagnostics.Trace.Assert(di.Exists);
            m_outputlogFolder = outputLogFolder;
            m_masterLogFolder = masterLogFolder;
        }

        /// <summary>
        /// 遍历文件夹
        /// </summary>
        /// <param name="dirPath"></param>
        private List<string> searchFolder(string dirPath)
        {
            List<string> logFileList = new List<string>();

            DirectoryInfo dirinfo = new DirectoryInfo(dirPath);
            try
            {
                // 不用遍历子文件夹
                //foreach (DirectoryInfo di in dirinfo.GetDirectories())
                //{
                //    searchFolder(dirinfo + "\\" + di.ToString());
                //}

                foreach (FileInfo fi in dirinfo.GetFiles())
                {
                    if (".log" == fi.Extension.ToLower())
                    {
                        logFileList.Add(fi.FullName);
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return logFileList;
        }

        public void Start()
        {
            List<string> logFileList1 = searchFolder(m_masterLogFolder);
            List<string> logFileList2 = searchFolder(m_outputlogFolder);
            if ((0 == logFileList1.Count) || (0 == logFileList2.Count))
            {
                return;
            }

            // log文件list比较
            listCompare(logFileList1, logFileList2);
        }

        private void listCompare(List<string> list1, List<string> list2)
        {
            List<CompareResult> totalResultList = new List<CompareResult>();

            for (int i = 0; i < list1.Count; i++)
            {
                string logName = list1[i].ToLower();
                // 检查log文件名,提取文件名包含的特征值(项目号)
                int testCaseNo;
                if (checkLogFileName(logName, out testCaseNo))
                {
                    // 在list2中根据log文件名找对应匹配的log(比较对象)
                    string compareLogName = findCompareLog(testCaseNo, list2);
                    if (null != compareLogName)
                    {
                        // log文件比较
                        List<CompareResult> resultList = LogFileProcess.logFileCompare(logName, compareLogName);
                        totalResultList.AddRange(resultList);
                    }
                }
            }

            m_displayList = new List<DisplayResult>();
            for (int i = 0; i < totalResultList.Count; i++)
            {
                CompareResult cmpResult = totalResultList[i];
                // 先判断该大项目号-中项目号是否已经存在, 如果已经存在:将test case id追加到存在的项目test case列表里, 否则创建新的项目;
                bool bExist = false;
                for (int j = 0; j < m_displayList.Count; j++)
                {
                    DisplayResult dspResult = m_displayList[j];
                    if ((cmpResult.level1Num == dspResult.chapterNo)
                        && (cmpResult.level2Num == dspResult.sectionNo))
                    {
                        testIDInfo testIdInfo = new testIDInfo();
                        testIdInfo.testID = cmpResult.testID;
                        testIdInfo.result = cmpResult.result;
                        testIdInfo.reoccurs1 = cmpResult.reoccurs1;
                        testIdInfo.reoccurs2 = cmpResult.reoccurs2;

                        dspResult.testIDList.Add(testIdInfo);
                        bExist = true;
                        break;
                    }
                }
                if (!bExist)
                {
                    DisplayResult dspResult = new DisplayResult();
                    dspResult.chapterNo = cmpResult.level1Num;
                    dspResult.sectionNo = cmpResult.level2Num;

                    testIDInfo testIdInfo = new testIDInfo();
                    testIdInfo.testID = cmpResult.testID;
                    testIdInfo.result = cmpResult.result;
                    testIdInfo.reoccurs1 = cmpResult.reoccurs1;
                    testIdInfo.reoccurs2 = cmpResult.reoccurs2;

                    dspResult.testIDList.Add(testIdInfo);

                    m_displayList.Add(dspResult);
                }
            }
        }

        private bool checkLogFileName(string fullName, out int testCaseNo)
        {
            string logName = fullName;
            int idx = fullName.LastIndexOf("\\");
            if (-1 != idx)
            {
                logName = fullName.Remove(0, idx + 1).Trim();
            }
            testCaseNo = -1;
            if (!logName.StartsWith("ct_"))
            {
                return false;
            }
            int ulIdx1 = logName.IndexOf('_');
            int ulIdx2 = logName.LastIndexOf('_');
            if (ulIdx1 == ulIdx2)
            {
                return false;
            }

            if (!int.TryParse(logName.Substring(ulIdx1 + 1, ulIdx2 - ulIdx1 - 1), out testCaseNo))
            {
                return false;
            }
            return true;
        }

        private string findCompareLog(int key, List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                string logName = list[i].ToLower();
                // 检查log文件名,提取文件名包含的特征值(项目号)
                int testCaseNo;
                if (checkLogFileName(logName, out testCaseNo))
                {
                    if (key == testCaseNo)
                    {
                        return logName;
                    }
                }
            }

            return null;
        }

        public int findTestCaseInDisplayList(int level1No, int level2No, int startIdx = 0)
        {
            for (int i = startIdx; i < m_displayList.Count; i++)
            {
                DisplayResult dspResult = m_displayList[i];
                if ((level1No == dspResult.chapterNo)
                    && (level2No == dspResult.sectionNo))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 统计test ID的各种结果个数
        /// </summary>
        public void getTestIdResultCount(int rsltIdx,
                                            out int totalCnt,
                                            out int passedCnt,
                                            out int apiNgCnt,
                                            out int dataNgCnt,
                                            out int untestedCnt,
                                            out int testedCnt)
        {
            totalCnt = 0;
            passedCnt = 0;
            apiNgCnt = 0;
            dataNgCnt = 0;
            untestedCnt = 0;
            testedCnt = 0;
            DisplayResult dspResult = m_displayList[rsltIdx];
            foreach (testIDInfo tidInfo in dspResult.testIDList)
            {
                switch (tidInfo.result)
                {
                    case EnumCompareResultValue.E_OK:
                        passedCnt += 1;
                        break;
                    case EnumCompareResultValue.E_API_NG:
                        apiNgCnt += 1;
                        break;
                    case EnumCompareResultValue.E_DATA_NG:
                        dataNgCnt += 1;
                        break;
                    case EnumCompareResultValue.E_ONLY_IN_FOLDER_1:
                        untestedCnt += 1;
                        break;
                    case EnumCompareResultValue.E_ONLY_IN_FOLDER_2:
                        break;
                    default:
                        break;
                }
            }
            totalCnt = passedCnt + apiNgCnt + dataNgCnt + untestedCnt;
            testedCnt = passedCnt + apiNgCnt + dataNgCnt;
        }

        public void OutputTestIdDetailResult()
        {
            StreamWriter sw = new StreamWriter(m_outputlogFolder + "\\" + "TestOKList.txt", false);
            foreach (DisplayResult dspRslt in m_displayList)
            {
                foreach (testIDInfo tid in dspRslt.testIDList)
                {
                    string wtStr = dspRslt.chapterNo.ToString().PadLeft(2, '0') + dspRslt.sectionNo.ToString().PadLeft(2, '0')
                                    + tid.testID.ToString().PadLeft(3, '0');
                    switch (tid.result)
                    {
                        case EnumCompareResultValue.E_OK:
                            wtStr += "              OK";
                            sw.WriteLine(wtStr);
                            break;
                        case EnumCompareResultValue.E_API_NG:
                        case EnumCompareResultValue.E_DATA_NG:
                            // wtStr += "              NG";
                            break;
                        case EnumCompareResultValue.E_ONLY_IN_FOLDER_1:
                            // wtStr += "              Untested";
                            break;
                        default:
                            break;
                    }
                    // sw.WriteLine(wtStr);
                }
            }
            sw.Close();
        }
    }
}
