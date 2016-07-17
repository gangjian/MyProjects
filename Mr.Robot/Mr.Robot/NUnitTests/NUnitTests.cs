using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Mr.Robot;

namespace Mr.Robot.NUnitTests
{
    [TestFixture]
    class NUnitTests
    {
        [Test]
        public void TestSuccess()
        {
            Assert.AreEqual(1, 1);
        }

        [Test]
        public void FirstTest()
        {
            List<string> source_list = new List<string>();
            List<string> header_list = new List<string>();
            string folder_name = "D:\\gj\\08_projects\\MyProjects\\Mr.Robot\\TestSrc";
            string source_name = folder_name + "\\Rte_swc_in_trcta.c";

            // 取得所有源文件和头文件列表
            IOProcess.GetAllCCodeFiles(folder_name, ref source_list, ref header_list);
            // 解析指定的源文件,并取得解析结果
            List<string> csfList = new List<string>();
            csfList.Add(source_name);
            List<CCodeParseResult> parseResultList = CCodeAnalyser.CFileListProcess(csfList, header_list);
            // 解析指定的函数
            //CCodeAnalyser.FunctionAnalysis(source_name, "sym_rbl_in_trcta_igoff", parseResultList);
            // 取得指定函数语句结构树

            // 顺次分析各语句
        }
    }
}
