using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTester.LogChecker
{
    public class DisplayResult
    {
        public int chapterNo = 0;
        public int sectionNo = 0;

        public List<testIDInfo> testIDList = new List<testIDInfo>();
    }

    public class testIDInfo
    {
        public int testID = 0;
        public EnumCompareResultValue result = EnumCompareResultValue.E_OK;
        public int reoccurs1 = 0;        // Test_ID 在folder 1重复出现的次数
        public int reoccurs2 = 0;        // Test_ID 在folder 2重复出现的次数
    }
}
