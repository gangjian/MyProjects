using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mr.Robot;

namespace UnitTestProject
{
    [TestClass]
    public class Rte_swc_in_trcta
    {
        static StatementNode root;                                                      // 函数语句结构的根节点
        static CCodeParseResult c_source_file_parse_result;

        [ClassInitialize]
        public static void TestClassSetup(TestContext ctx)
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
            // 从全部解析结果列表中根据指定文件名和函数名找到相应的文件和函数解析结果
            CFunctionStructInfo funInfo = CCodeAnalyser.FindFileAndFunctionStructInfoFromParseResult(source_name, function_name, parseResultList, out c_source_file_parse_result);

            // 指定函数语句树结构的分析提取
            root = new StatementNode();
            root.Type = StatementNodeType.Root;
            root.Scope = funInfo.Scope;
            CCodeAnalyser.GetCodeBlockStructure(c_source_file_parse_result.SourceParseInfo.parsedCodeList, root);
        }

        [TestMethod]
        public void sym_rbl_in_trcta_igoff_0()
        {
            // 函数语句分析: 分析入出力
            //CCodeAnalyser.FunctionStatementsAnalysis(root, c_file_result);
            Assert.AreEqual(3, root.childList.Count);
            Assert.AreEqual(StatementNodeType.Simple, root.childList[0].Type);
            Assert.AreEqual(StatementNodeType.Simple, root.childList[1].Type);
            Assert.AreEqual(StatementNodeType.Simple, root.childList[2].Type);
        }

        [TestMethod]
        public void sym_rbl_in_trcta_igoff_1_1()
        {
            // 顺次分析各语句
            List<VariableInfo> localVarList = new List<VariableInfo>();
            //CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, localVarList);

            List<StatementComponent> componentList = CCodeAnalyser.GetStatementComponents(root.childList[0], c_source_file_parse_result);
            List<MeaningGroup> meaningGroupList = CCodeAnalyser.StatementMeaningAnalysis(componentList, c_source_file_parse_result, localVarList);

            Assert.AreEqual(2, meaningGroupList.Count);
            Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
            Assert.AreEqual(MeaningGroupType.VariableName, meaningGroupList[1].Type);
        }

        [TestMethod]
        public void sym_rbl_in_trcta_igoff_1_2()
        {
            List<VariableInfo> localVarList = new List<VariableInfo>();
            CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, localVarList);

            Assert.AreEqual(1, localVarList.Count);
            Assert.AreEqual("pvU1NoSts", localVarList[0].typeName);
            Assert.AreEqual("pvOut", localVarList[0].varName);
            Assert.AreEqual(0, localVarList[0].initial_list.Count);
        }

        [TestMethod]
        public void sym_rbl_in_trcta_igoff_2()
        {
            List<VariableInfo> localVarList = new List<VariableInfo>();
            CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, localVarList);

            List<StatementComponent> componentList = CCodeAnalyser.GetStatementComponents(root.childList[1], c_source_file_parse_result);
            List<MeaningGroup> meaningGroupList = CCodeAnalyser.StatementMeaningAnalysis(componentList, c_source_file_parse_result, localVarList);

            Assert.AreEqual(3, meaningGroupList.Count);
            Assert.AreEqual(MeaningGroupType.VariableName, meaningGroupList[0].Type);
            Assert.AreEqual("pvOut.dt", meaningGroupList[0].Text);
            Assert.AreEqual(MeaningGroupType.EqualMark, meaningGroupList[1].Type);
            Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
            Assert.AreEqual("((uint8)0U)", meaningGroupList[2].Text);
        }

        [TestMethod]
        public void sym_rbl_in_trcta_igoff_3()
        {
            List<VariableInfo> localVarList = new List<VariableInfo>();
            CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, localVarList);
            CCodeAnalyser.StatementAnalysis(root.childList[1], c_source_file_parse_result, localVarList);

            List<StatementComponent> componentList = CCodeAnalyser.GetStatementComponents(root.childList[2], c_source_file_parse_result);
            List<MeaningGroup> meaningGroupList = CCodeAnalyser.StatementMeaningAnalysis(componentList, c_source_file_parse_result, localVarList);

            Assert.AreEqual(3, meaningGroupList.Count);
            Assert.AreEqual(MeaningGroupType.VariableName, meaningGroupList[0].Type);
            Assert.AreEqual("(Rte_Inst_swc_in_trcta->rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct)->value", meaningGroupList[0].Text);
            Assert.AreEqual(MeaningGroupType.EqualMark, meaningGroupList[1].Type);
            Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
            Assert.AreEqual("(*&pvOut)", meaningGroupList[2].Text);
        }
    }
}
