using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VisualizeMyLife
{
    public class DataInfo
    {
        public DateTime _dateTime = new DateTime();
        public double _income1 = -1;
        public double _income2 = -1;                     // 收益
        public double _historyIncome1 = -1;
        public double _historyIncome2 = -1;              // 历史累计收益
        public double _totalAssets1 = -1;
        public double _totalAssets2 = -1;                // 总资产
        public double _rate1 = -1;
        public double _rate2 = -1;                       // 收益率
        public double _bodyWeight = -1;
    }

    class ClassDataFileManager
    {
        private string m_fullName = string.Empty;

        public ClassDataFileManager(string fullName)
        {
            if (!File.Exists(fullName))
            {
                return;
            }
            m_fullName = fullName;
        }

        ~ClassDataFileManager()
        {
        }

        public List<DataInfo> ReadDataList()
        {
            List<DataInfo> readDataList = new List<DataInfo>();
            if (string.Empty == m_fullName)
            {
                return readDataList;
            }
            StreamReader sr = new StreamReader(m_fullName);

            DataInfo readDataInfo = null;
            string rdLine = "";
            while (null != (rdLine = sr.ReadLine()))
            {
                rdLine = rdLine.Trim();
                if (rdLine.StartsWith("//"))
                {
                    // 跳过注释行
                    continue;
                }
                if (    rdLine.StartsWith("[")
                    &&  rdLine.EndsWith("]"))
                {
                    if (null != readDataInfo)
                    {
                        readDataList.Add(readDataInfo);
                    }
                    readDataInfo = new DataInfo();
                    readDataInfo._dateTime = GetDateTime(rdLine);
                }
                int idx = -1;
                if (("" != rdLine)
                    && (-1 != (idx = rdLine.IndexOf('='))))
                {
                    string key = GetKeyStr(rdLine);
                    string value = GetValueStr(rdLine);
                    GetDataInfoDetail(key, value, ref readDataInfo);
                }
            }
            if (null != readDataInfo)
            {
                readDataList.Add(readDataInfo);
            }
            sr.Close();
            return readDataList;
        }

        private DateTime GetDateTime(string section)
        {
            DateTime rtDateTime = new DateTime();
            string str = section.Substring(1, section.Length - 2);
            string[] arr = str.Split('/');
            if (arr.Length >= 3)
            {
                int year, month, day;
                int.TryParse(arr[0], out year);
                int.TryParse(arr[1], out month);
                int.TryParse(arr[2], out day);
                rtDateTime = new DateTime(year, month, day);
            }

            return rtDateTime;
        }

        private string GetKeyStr(string line)
        {
            int idx = line.IndexOf('=');
            return line.Substring(0, idx).Trim();
        }

        private string GetValueStr(string line)
        {
            int idx = line.IndexOf('=');
            return line.Substring(idx + 1).Trim();
        }

        private void GetDataInfoDetail(string key, string valueStr, ref DataInfo dataInfo)
        {
            double value;
            double.TryParse(valueStr, out value);
            switch (key)
            {
                case "income1":
                    dataInfo._income1 = value;
                    break;
                case "income2":
                    dataInfo._income2 = value;
                    break;
                case "historyIncome1":
                    dataInfo._historyIncome1 = value;
                    break;
                case "historyIncome2":
                    dataInfo._historyIncome2 = value;
                    break;
                case "totalAssets1":
                    dataInfo._totalAssets1 = value;
                    break;
                case "totalAssets2":
                    dataInfo._totalAssets2 = value;
                    break;
                case "rate1":
                    dataInfo._rate1 = value;
                    break;
                case "rate2":
                    dataInfo._rate2 = value;
                    break;
                case "bodyWeight":
                    dataInfo._bodyWeight = value;
                    break;
                default:
                    break;
            }
        }

        public void WriteDataToFile(DataInfo dataInfo)
        {
            StreamWriter sw = new StreamWriter(m_fullName, true);

            // 日期
            string wtLine = "[" + dataInfo._dateTime.Year.ToString() + "/" + dataInfo._dateTime.Month.ToString() + "/" + dataInfo._dateTime.Day.ToString() + "]";
            sw.WriteLine(wtLine);

            // 各字段
            if (-1 != dataInfo._bodyWeight)
            {
                wtLine = "bodyWeight" + " = " + dataInfo._bodyWeight.ToString();
                sw.WriteLine(wtLine);
            }
            if (-1 != dataInfo._historyIncome1)
            {
                wtLine = "historyIncome1" + " = " + dataInfo._historyIncome1.ToString();
                sw.WriteLine(wtLine);
            }
            if (-1 != dataInfo._historyIncome2)
            {
                wtLine = "historyIncome2" + " = " + dataInfo._historyIncome2.ToString();
                sw.WriteLine(wtLine);
            }
            if (-1 != dataInfo._income1)
            {
                wtLine = "income1" + " = " + dataInfo._income1.ToString();
                sw.WriteLine(wtLine);
            }
            if (-1 != dataInfo._income2)
            {
                wtLine = "income2" + " = " + dataInfo._income2.ToString();
                sw.WriteLine(wtLine);
            }
            if (-1 != dataInfo._rate1)
            {
                wtLine = "rate1" + " = " + dataInfo._rate1.ToString();
                sw.WriteLine(wtLine);
            }
            if (-1 != dataInfo._rate2)
            {
                wtLine = "rate2" + " = " + dataInfo._rate2.ToString();
                sw.WriteLine(wtLine);
            }
            if (-1 != dataInfo._totalAssets1)
            {
                wtLine = "totalAssets1" + " = " + dataInfo._totalAssets1.ToString();
                sw.WriteLine(wtLine);
            }
            if (-1 != dataInfo._totalAssets2)
            {
                wtLine = "totalAssets2" + " = " + dataInfo._totalAssets2.ToString();
                sw.WriteLine(wtLine);
            }
            sw.Close();
        }

    }
}
