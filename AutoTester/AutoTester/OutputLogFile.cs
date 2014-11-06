using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AutoTester
{
    class OutputLogFile
    {
        public string m_fullName = "";
        private StreamWriter m_writer = null;
        private bool m_logOnFlg = false;        // 标记是否开始输出Log内容到文件

        public List<string> m_WriteRegList;   // 写寄存器名称列表
        public List<string> m_ReadRegList;   // 写寄存器名称列表

        public OutputLogFile(string path, string chapterNoStr)
        {
            // 生成文件名, 创建log文件
            this.m_fullName = path + "\\" + "ct_" + chapterNoStr + "_" + DateTime.Now.Month.ToString().PadLeft(2, '0')
                + DateTime.Now.Day.ToString().PadLeft(2, '0') + ".log";
            this.m_writer = File.CreateText(m_fullName);
            this.m_WriteRegList = new List<string>();
            this.m_ReadRegList = new List<string>();
        }

        public void Dispose()
        {
            if (null != m_writer)
            {
                this.m_writer.Close();
                this.m_writer = null;
            }
        }

        /// <summary>
        /// ct.exe吐出的log的处理
        /// </summary>
        /// <param name="logStr"></param>
        /// <returns></returns>
        public bool OutputLogProcess(string logStr)
        {
            string wt_reg_log_head1 = @"legacy_sim_write_reg: legacy_sim_write_reg():";
            string wt_reg_log_head2 = @"kick_ifid_with_dl: kick_ifid_with_dl(): register write:";
            string rd_reg_log_head = @"legacy_sim_read_reg: legacy_sim_read_reg():";
            string regNameStr = string.Empty;
            bool bReadFlg = false;  // 用以区分读or写寄存器
            if (logStr.StartsWith(wt_reg_log_head1))
            {
                regNameStr = logStr.Remove(0, wt_reg_log_head1.Length).Trim();
            }
            else if (logStr.StartsWith(wt_reg_log_head2))
            {
                regNameStr = logStr.Remove(0, wt_reg_log_head2.Length).Trim();
            }
            else if (logStr.StartsWith(rd_reg_log_head))
            {
                regNameStr = logStr.Remove(0, rd_reg_log_head.Length).Trim();
                bReadFlg = true;
            }
            if ((string.Empty != regNameStr)
                && (regNameStr.StartsWith("[")) )
            {
                int idx = regNameStr.IndexOf(']');
                if (idx > 1)
                {
                    string regName = regNameStr.Substring(1, idx - 1);
                    // 不要的寄存器名
                    if (   ("HM" ==  regName)
                        || ("CM0" == regName)
                        || ("CM1" == regName)
                        || ("THTDATA0" == regName)
                        || ("THTDATA1" == regName)
                        )
                    {
                    }
                    else if (bReadFlg)
                    {
                        if (!m_ReadRegList.Contains(regName))
                        {
                            m_ReadRegList.Add(regName);
                        }
                    }
                    else if (!m_WriteRegList.Contains(regName))
                    {
                        m_WriteRegList.Add(regName);
                    }
                }
                return false;
            }
            // 不要的log
            else if (  logStr.StartsWith(@"kick_ifid_with_dl: ")
                    || logStr.StartsWith(@"APExec: APExec(): ")
                    || logStr.StartsWith(@"legacy_sim_wait_imp: ")
                    || logStr.StartsWith(@"APDMACExec: APDMACExec(): ")
                    )
            {
                return false;
            }

            if (logStr.StartsWith("Run test")
                && !m_logOnFlg)
            {
                m_logOnFlg = true;
            }
            else if (logStr.Contains("TEST_END"))
            {
                m_logOnFlg = false;
            }
            else
            {
                if (m_logOnFlg)
                {
                    m_writer.WriteLine(logStr);
                }
            }
            return true;
        }

        public void SaveRegChangedList(List<string> readList, List<string> writeList, string outputLogPath)
        {
            StreamWriter sw = new StreamWriter(outputLogPath + "\\" + "ReadRegList.txt", false);
            foreach (string regName in readList)
            {
                sw.WriteLine(regName);
            }
            sw.Close();
            sw = new StreamWriter(outputLogPath + "\\" + "WriteRegList.txt", false);
            foreach (string regName in writeList)
            {
                sw.WriteLine(regName);
            }
            sw.Close();
        }
    }
}
