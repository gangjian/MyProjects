using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Mr.Robot;

namespace Mr.Robot.MacroSwitchAnalyser
{
    class MacroSwitchAnalyser2
    {
        public MacroSwitchAnalyser2(List<string> source_list)
        {
            foreach (string src_name in source_list)
            {
                SrcProc(src_name);
            }
        }

        void SrcProc(string src_name)
        {
            List<string> codeList = CCodeAnalyser.RemoveComments(src_name);
            List<string> expList = GetMacroExpList(codeList);
            if (0 == expList.Count)
            {
                return;
            }
        }

        List<string> GetMacroExpList(List<string> code_list)
        {
            List<string> expList = new List<string>();
            foreach (string code_line in code_list)
            {
                string expStr = CodeLineProc(code_line);
                if (null != expStr)
                {
                    expList.Add(expStr);
                }
            }
            return expList;
        }

        string CodeLineProc(string code_line)
        {
            code_line = CommonProc.RemoveStringSegment(code_line);
            int idx = code_line.IndexOf("#if");
            if (-1 == idx)
            {
                return null;
            }
            return CommonProc.GetMacroExpression(code_line, idx);
        }
    }
}
