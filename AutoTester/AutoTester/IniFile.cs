using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace AutoTester
{
    public class IniFile
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public string m_fullname;

        public IniFile()
        {
            m_fullname = System.Windows.Forms.Application.StartupPath + "\\config.ini";
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.m_fullname);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, this.m_fullname);
            return temp.ToString();
        }
    }

    public class ConfigInfo
    {
        private IniFile m_iniFile = new IniFile();
        public string m_targetPath = "";
        public string m_masterLogPath = "";

        public ConfigInfo()
        {
            FileInfo fi = new FileInfo(m_iniFile.m_fullname);
            if (fi.Exists)
            {
                LoadIniFile();
            }
        }

        private void LoadIniFile()
        {
            DirectoryInfo di = null;
            string rdStr = m_iniFile.IniReadValue("PATH_INFO", "TARGET_PATH");
            if (string.Empty != rdStr)
            {
                di = new DirectoryInfo(rdStr);
                if (di.Exists)
                {
                    m_targetPath = rdStr;
                }
            }
            rdStr = m_iniFile.IniReadValue("PATH_INFO", "MASTER_LOG");
            if (string.Empty != rdStr)
            {
                di = new DirectoryInfo(rdStr);
                if (di.Exists)
                {
                    m_masterLogPath = rdStr;
                }
            }
        }

        public void SaveTargetPath(string targetPath)
        {
            m_iniFile.IniWriteValue("PATH_INFO", "TARGET_PATH", targetPath);
        }

        public void SaveMasterLogPath(string masterLogPath)
        {
            m_iniFile.IniWriteValue("PATH_INFO", "MASTER_LOG", masterLogPath);
        }

        public void SaveTestCaseChecked(string testCaseNo, bool bChecked)
        {
            m_iniFile.IniWriteValue("TEST_CASE_INFO", testCaseNo, bChecked.ToString());
        }

        public bool LoadTestCaseChecked(string testCaseNo)
        {
            string rdStr = m_iniFile.IniReadValue("TEST_CASE_INFO", testCaseNo);
            bool bChecked = false;
            if (bool.TryParse(rdStr, out bChecked))
            {
                return bChecked;
            }
            else
            {
                return false;
            }
        }
    }
}
