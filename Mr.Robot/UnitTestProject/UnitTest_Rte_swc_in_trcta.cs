using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mr.Robot;
using System.IO;

namespace UnitTestProject
{
	[TestClass]
	public class sym_rbl_in_trcta_igoff
    {
        static StatementNode root;                                                      // 函数语句结构的根节点
        static CCodeParseResult c_source_file_parse_result;

        [ClassInitialize]
        public static void TestClassSetup(TestContext ctx)
        {
			string folder_name = "..\\..\\..\\TestSrc";
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

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_0()
        {
            // 函数语句分析: 分析入出力
            //CCodeAnalyser.FunctionStatementsAnalysis(root, c_file_result);
            Assert.AreEqual(3, root.childList.Count);
            Assert.AreEqual(StatementNodeType.Simple, root.childList[0].Type);
            Assert.AreEqual(StatementNodeType.Simple, root.childList[1].Type);
            Assert.AreEqual(StatementNodeType.Simple, root.childList[2].Type);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_1_1()
        {
            // 顺次分析各语句
			AnalysisContext analysisContext = new AnalysisContext();
			//CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, analysisContext);

            List<StatementComponent> componentList = CCodeAnalyser.GetComponents(root.childList[0], c_source_file_parse_result);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

            Assert.AreEqual(2, meaningGroupList.Count);
            Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
            Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_1_2()
        {
			AnalysisContext analysisContext = new AnalysisContext();
			CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, ref analysisContext);

			Assert.AreEqual(1, analysisContext.local_list.Count);
			Assert.AreEqual("pvU1NoSts", analysisContext.local_list[0].type);
			Assert.AreEqual("pvOut", analysisContext.local_list[0].name);
			//Assert.AreEqual(0, analysisContext.local_list[0].initial_list.Count);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_2()
        {
			AnalysisContext analysisContext = new AnalysisContext();
			CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, ref analysisContext);

            List<StatementComponent> componentList = CCodeAnalyser.GetComponents(root.childList[1], c_source_file_parse_result);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

            Assert.AreEqual(3, meaningGroupList.Count);
            Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
            Assert.AreEqual("pvOut.dt", meaningGroupList[0].Text);
            Assert.AreEqual(MeaningGroupType.EqualMark, meaningGroupList[1].Type);
            Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
            Assert.AreEqual("((uint8)0U)", meaningGroupList[2].Text);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_3()
        {
			AnalysisContext analysisContext = new AnalysisContext();
			CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, ref analysisContext);
			CCodeAnalyser.StatementAnalysis(root.childList[1], c_source_file_parse_result, ref analysisContext);

            List<StatementComponent> componentList = CCodeAnalyser.GetComponents(root.childList[2], c_source_file_parse_result);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

            Assert.AreEqual(3, meaningGroupList.Count);
            Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
            Assert.AreEqual("(Rte_Inst_swc_in_trcta->rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct)->value", meaningGroupList[0].Text);
            Assert.AreEqual(MeaningGroupType.EqualMark, meaningGroupList[1].Type);
            Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
            Assert.AreEqual("(*&pvOut)", meaningGroupList[2].Text);
        }
    }

	[TestClass]
	public class sym_rbl_in_trcta_igon
	{
		static StatementNode root;                                                      // 函数语句结构的根节点
		static CCodeParseResult c_source_file_parse_result;

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			string folder_name = "..\\..\\..\\TestSrc";
			string source_name = folder_name + "\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_igon";

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

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_0()
		{
			// 函数语句分析: 分析入出力
			//CCodeAnalyser.FunctionStatementsAnalysis(root, c_file_result);
			Assert.AreEqual(10, root.childList.Count);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[0].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[1].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[2].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[3].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[4].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[5].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[6].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[7].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[8].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[9].Type);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_1To3()
		{
			// 顺次分析各语句
			AnalysisContext analysisContext = new AnalysisContext();
			//CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, localVarList);

			// 第一条语句
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(root.childList[0], c_source_file_parse_result);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

			Assert.AreEqual(2, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);

			// 第二条语句
			componentList = CCodeAnalyser.GetComponents(root.childList[1], c_source_file_parse_result);
			meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

			Assert.AreEqual(2, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);

			// 第三条语句
			componentList = CCodeAnalyser.GetComponents(root.childList[2], c_source_file_parse_result);
			meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

			Assert.AreEqual(2, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_4()
		{
			AnalysisContext analysisContext = new AnalysisContext();
			CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, ref analysisContext);
			CCodeAnalyser.StatementAnalysis(root.childList[1], c_source_file_parse_result, ref analysisContext);
			CCodeAnalyser.StatementAnalysis(root.childList[2], c_source_file_parse_result, ref analysisContext);
			// 第4句
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(root.childList[3], c_source_file_parse_result);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("varOutStep.pv", meaningGroupList[0].Text);
			Assert.AreEqual(MeaningGroupType.EqualMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
			Assert.AreEqual("((uint8)0U)", meaningGroupList[2].Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_5()
		{
			AnalysisContext analysisContext = new AnalysisContext();
			for (int i = 0; i < 4; i++)
			{
				CCodeAnalyser.StatementAnalysis(root.childList[i], c_source_file_parse_result, ref analysisContext);
			}
			// 第5句
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(root.childList[4], c_source_file_parse_result);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("varInStep.inSig", meaningGroupList[0].Text);
			Assert.AreEqual(MeaningGroupType.EqualMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
			Assert.AreEqual("((*(Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_rp_srIf_in_TRCTA_val)).value)", meaningGroupList[2].Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_6()
		{
			AnalysisContext analysisContext = new AnalysisContext();
			for (int i = 0; i < 5; i++)
			{
				CCodeAnalyser.StatementAnalysis(root.childList[i], c_source_file_parse_result, ref analysisContext);
			}
			// 第6句
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(root.childList[5], c_source_file_parse_result);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("varInStep.msgSts", meaningGroupList[0].Text);
			Assert.AreEqual(MeaningGroupType.EqualMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
			Assert.AreEqual("((*(Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_rp_srIf_in_TRCTASts_val)).value)", meaningGroupList[2].Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_7()
		{
			AnalysisContext analysisContext = new AnalysisContext();
			for (int i = 0; i < 6; i++)
			{
				CCodeAnalyser.StatementAnalysis(root.childList[i], c_source_file_parse_result, ref analysisContext);
			}
			// 第7句
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(root.childList[6], c_source_file_parse_result);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("varInStep.powerSts", meaningGroupList[0].Text);
			Assert.AreEqual(MeaningGroupType.EqualMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
			Assert.AreEqual("((uint8)0x01)", meaningGroupList[2].Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_8()
		{
			AnalysisContext analysisContext = new AnalysisContext();
			for (int i = 0; i < 7; i++)
			{
				CCodeAnalyser.StatementAnalysis(root.childList[i], c_source_file_parse_result, ref analysisContext);
			}
			// 第8句
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(root.childList[7], c_source_file_parse_result);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

			Assert.AreEqual(1, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.FunctionCalling, meaningGroupList[0].Type);
			Assert.AreEqual("ShareLibStepFailJudgeVal(&varInStep,&rctasw_can_mng_tbl,&varOutStep)", meaningGroupList[0].Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_9()
		{
			AnalysisContext analysisContext = new AnalysisContext();
			for (int i = 0; i < 8; i++)
			{
				CCodeAnalyser.StatementAnalysis(root.childList[i], c_source_file_parse_result, ref analysisContext);
			}
			// 第9句
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(root.childList[8], c_source_file_parse_result);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("pvOut.dt", meaningGroupList[0].Text);
			Assert.AreEqual(MeaningGroupType.EqualMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[2].Type);
			Assert.AreEqual("varOutStep.pv", meaningGroupList[2].Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_10()
		{
			AnalysisContext analysisContext = new AnalysisContext();
			for (int i = 0; i < 9; i++)
			{
				CCodeAnalyser.StatementAnalysis(root.childList[i], c_source_file_parse_result, ref analysisContext);
			}
			// 第9句
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(root.childList[9], c_source_file_parse_result);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, c_source_file_parse_result, analysisContext);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("(Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct)->value", meaningGroupList[0].Text);
			Assert.AreEqual(MeaningGroupType.EqualMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
			Assert.AreEqual("(*&pvOut)", meaningGroupList[2].Text);
		}
	}

}
