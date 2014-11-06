using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTester.LogChecker
{
    public enum EnumCompareResultValue
    {
        E_OK,
        E_API_NG,
        E_DATA_NG,
        E_ONLY_IN_FOLDER_1,
        E_ONLY_IN_FOLDER_2
    }

    class CompareResult
    {
        public int level1Num = 0;
        public int level2Num = 0;
        public int testID = 0;

        public int reoccurs1 = 0;   // Test_ID 在folder 1重复出现的次数
        public int reoccurs2 = 0;   // Test_ID 在folder 2重复出现的次数

        public EnumCompareResultValue result = EnumCompareResultValue.E_OK;
    }
}
