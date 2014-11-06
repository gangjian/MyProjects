using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTester
{
    public class TestCaseTplInfo
    {
        public int chapterNo = 0;
        public int sectionNum = 0;

        public TestCaseTplInfo(int chNo, int secNum)
        {
            chapterNo = chNo;
            sectionNum = secNum;
        }
    }

    public class TestCaseTemplate
    {
        public List<TestCaseTplInfo> m_templateList;

        public TestCaseTemplate()
        {
            m_templateList = new List<TestCaseTplInfo>();
            TestCaseTplInfo tpl = null;

            // 01_01 ~ 01_03
            tpl = new TestCaseTplInfo(1, 3); m_templateList.Add(tpl);
            // 02_01 ~ 02_10
            tpl = new TestCaseTplInfo(2, 10); m_templateList.Add(tpl);
            // 03_01 ~ 03_22
            tpl = new TestCaseTplInfo(3, 22); m_templateList.Add(tpl);
            // 04_01 ~ 04_09
            tpl = new TestCaseTplInfo(4, 9); m_templateList.Add(tpl);
            // 05_01 ~ 05_08
            tpl = new TestCaseTplInfo(5, 8); m_templateList.Add(tpl);
            // 06_01 ~ 06_03
            tpl = new TestCaseTplInfo(6, 3); m_templateList.Add(tpl);
            // 07_01 ~ 07_13
            tpl = new TestCaseTplInfo(7, 13); m_templateList.Add(tpl);
            // 08_01 ~ 08_02
            tpl = new TestCaseTplInfo(8, 2); m_templateList.Add(tpl);
            // 09_01 ~ 09_19
            tpl = new TestCaseTplInfo(9, 19); m_templateList.Add(tpl);
            // 10_01 ~ 10_24
            tpl = new TestCaseTplInfo(10, 24); m_templateList.Add(tpl);
            // 11_01 ~ 11_13
            tpl = new TestCaseTplInfo(11, 13); m_templateList.Add(tpl);
            // 12_01 ~ 12_12
            tpl = new TestCaseTplInfo(12, 12); m_templateList.Add(tpl);
            // 13_01 ~ 13_22
            tpl = new TestCaseTplInfo(13, 22); m_templateList.Add(tpl);
            // 14_01 ~ 14_10
            tpl = new TestCaseTplInfo(14, 10); m_templateList.Add(tpl);
            // 15_01 ~ 15_06
            tpl = new TestCaseTplInfo(15, 6); m_templateList.Add(tpl);
            // 16_01 ~ 16_10
            tpl = new TestCaseTplInfo(16, 10); m_templateList.Add(tpl);
            // 17_01 ~ 17_16
            tpl = new TestCaseTplInfo(17, 16); m_templateList.Add(tpl);
            // 18_01 ~ 18_05
            tpl = new TestCaseTplInfo(18, 5); m_templateList.Add(tpl);
            // 19_01 ~ 19_17
            tpl = new TestCaseTplInfo(19, 17); m_templateList.Add(tpl);
            // 20_01 ~ 20_02
            tpl = new TestCaseTplInfo(20, 2); m_templateList.Add(tpl);
            // 21_01 ~ 21_02
            tpl = new TestCaseTplInfo(21, 2); m_templateList.Add(tpl);
            // 22_01 ~ 22_07
            tpl = new TestCaseTplInfo(22, 7); m_templateList.Add(tpl);
            // 23_01 ~ 23_02
            tpl = new TestCaseTplInfo(23, 2); m_templateList.Add(tpl);
            // 24_01 ~ 24_18
            tpl = new TestCaseTplInfo(24, 18); m_templateList.Add(tpl);
            // 25_01 ~ 25_10
            tpl = new TestCaseTplInfo(25, 10); m_templateList.Add(tpl);
            // 26_01 ~ 26_02
            tpl = new TestCaseTplInfo(26, 2); m_templateList.Add(tpl);
            // 27_01 ~ 27_01
            tpl = new TestCaseTplInfo(27, 1); m_templateList.Add(tpl);
            // 28_01 ~ 28_01
            tpl = new TestCaseTplInfo(28, 1); m_templateList.Add(tpl);
            // 29_01 ~ 29_04
            tpl = new TestCaseTplInfo(29, 4); m_templateList.Add(tpl);
            // 30_01 ~ 30_01
            tpl = new TestCaseTplInfo(30, 1); m_templateList.Add(tpl);
            // 31_01 ~ 31_02
            tpl = new TestCaseTplInfo(31, 2); m_templateList.Add(tpl);
            // 32_01 ~ 32_03
            tpl = new TestCaseTplInfo(32, 3); m_templateList.Add(tpl);
            // 33_01 ~ 33_02
            tpl = new TestCaseTplInfo(33, 2); m_templateList.Add(tpl);
            // 34_01 ~ 34_08
            tpl = new TestCaseTplInfo(34, 8); m_templateList.Add(tpl);
            // 35_01 ~ 35_05
            tpl = new TestCaseTplInfo(35, 5); m_templateList.Add(tpl);
            // 36_01 ~ 36_01
            tpl = new TestCaseTplInfo(36, 1); m_templateList.Add(tpl);
            // 37_01 ~ 37_06
            tpl = new TestCaseTplInfo(37, 6); m_templateList.Add(tpl);
            // 38_01 ~ 38_02
            tpl = new TestCaseTplInfo(38, 2); m_templateList.Add(tpl);
            // 39_01 ~ 39_04
            tpl = new TestCaseTplInfo(39, 4); m_templateList.Add(tpl);
            // 40_01 ~ 40_02
            tpl = new TestCaseTplInfo(40, 2); m_templateList.Add(tpl);
            // 90_01 ~ 90_10
            tpl = new TestCaseTplInfo(90, 10); m_templateList.Add(tpl);
        }
    }
}
