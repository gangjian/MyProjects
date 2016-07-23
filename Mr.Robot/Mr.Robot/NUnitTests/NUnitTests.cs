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
        public void statements_analysis_test()
        {
            string folder_name = "D:\\gj\\08_projects\\MyProjects\\Mr.Robot\\TestSrc";
            string source_name = folder_name + "\\Rte_swc_in_trcta.c";
            string function_name = "sym_rbl_in_trcta_igoff";

            List<string> source_list = new List<string>();
            List<string> header_list = new List<string>();
            // 取得所有源文件和头文件列表
            IOProcess.GetAllCCodeFiles(folder_name, ref source_list, ref header_list);
            // 解析指定的源文件,并取得解析结果
            List<string> csfList = new List<string>();
            csfList.Add(source_name);
            List<CCodeParseResult> parseResultList = CCodeAnalyser.CFileListProcess(csfList, header_list);

            // FunctionAnalysis
            #region 以下处理本属于函数 FunctionAnalysis 的内容为便于测试提取出来
            // 从全部解析结果列表中根据指定文件名和函数名找到相应的文件和函数解析结果
            CCodeParseResult c_file_result = null;
            CFunctionStructInfo funInfo = CCodeAnalyser.FindFileAndFunctionStructInfoFromParseResult(source_name, function_name, parseResultList, out c_file_result);

            // 指定函数语句树结构的分析提取
            StatementNode root = new StatementNode();
            root.Type = StatementNodeType.Root;
            root.Scope = funInfo.Scope;
            CCodeAnalyser.GetCodeBlockStructure(c_file_result.SourceParseInfo.parsedCodeList, root);

            // 函数语句分析: 分析入出力
            CCodeAnalyser.FunctionStatementsAnalysis(root, c_file_result);
            #endregion

            // 顺次分析各语句
        }
    }
}
