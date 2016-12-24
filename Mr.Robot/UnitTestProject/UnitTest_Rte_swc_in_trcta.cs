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
        static CodeParseInfo c_source_file_parse_result;

        [ClassInitialize]
        public static void TestClassSetup(TestContext ctx)
        {
			string source_name = "..\\..\\..\\TestSrc\\swc_in_trcta\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_igoff";
			c_source_file_parse_result = Common.UnitTest_SourceFileProcess(source_name, function_name, ref root);
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
			analysisContext.ParseResult = c_source_file_parse_result;
			//CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, ctx);

			List<string> codeList = analysisContext.ParseResult.SourceParseInfo.parsedCodeList;
			string statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[0].Scope);
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(statementStr, analysisContext.ParseResult);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, analysisContext);

            Assert.AreEqual(2, meaningGroupList.Count);
            Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
            Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_1_2()
        {
			AnalysisContext analysisContext = new AnalysisContext();
			analysisContext.ParseResult = c_source_file_parse_result;
			CCodeAnalyser.StatementAnalysis(root.childList[0], analysisContext);

			Assert.AreEqual(1, analysisContext.LocalVarList.Count);
			Assert.AreEqual("struct RTE_TYPE_H_USR_DEF_TYPE_0", analysisContext.LocalVarList[0].Type.Name);
			Assert.AreEqual("pvOut", analysisContext.LocalVarList[0].Name);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_2()
        {
			AnalysisContext analysisContext = new AnalysisContext();
			analysisContext.ParseResult = c_source_file_parse_result;
			CCodeAnalyser.StatementAnalysis(root.childList[0], analysisContext);

			List<string> codeList = analysisContext.ParseResult.SourceParseInfo.parsedCodeList;
			string statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[1].Scope);
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(statementStr, analysisContext.ParseResult);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, analysisContext);

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
			analysisContext.ParseResult = c_source_file_parse_result;
			CCodeAnalyser.StatementAnalysis(root.childList[0], analysisContext);
			CCodeAnalyser.StatementAnalysis(root.childList[1], analysisContext);

			List<string> codeList = analysisContext.ParseResult.SourceParseInfo.parsedCodeList;
			string statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[2].Scope);
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(statementStr, analysisContext.ParseResult);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, analysisContext);

            Assert.AreEqual(3, meaningGroupList.Count);
            Assert.AreEqual(MeaningGroupType.GlobalVariable, meaningGroupList[0].Type);
            Assert.AreEqual("(Rte_Inst_swc_in_trcta->rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct)->value", meaningGroupList[0].Text);
            Assert.AreEqual(MeaningGroupType.EqualMark, meaningGroupList[1].Type);
            Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
            Assert.AreEqual("(*&pvOut)", meaningGroupList[2].Text);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igoff_IO()
		{
			AnalysisContext analysisContext = CCodeAnalyser.FunctionStatementsAnalysis(root, c_source_file_parse_result);
			Assert.AreEqual(1, analysisContext.OutputGlobalList.Count);
			Assert.AreEqual("(Rte_Inst_swc_in_trcta->rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct)->value", analysisContext.OutputGlobalList[0].MeanningGroup.Text);
		}
    }

	[TestClass]
	public class sym_rbl_in_trcta_igon
	{
		static StatementNode root;                                                      // 函数语句结构的根节点
		static CodeParseInfo c_source_file_parse_result;

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			string source_name = "..\\..\\..\\TestSrc\\swc_in_trcta\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_igon";
			c_source_file_parse_result = Common.UnitTest_SourceFileProcess(source_name, function_name, ref root);
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
			AnalysisContext ctx = new AnalysisContext();
			ctx.ParseResult = c_source_file_parse_result;
			//CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, localVarList);

			List<string> codeList = ctx.ParseResult.SourceParseInfo.parsedCodeList;
			// 第一条语句
			string statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[0].Scope);
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(statementStr, ctx.ParseResult);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, ctx);

			Assert.AreEqual(2, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);

			// 第二条语句
			statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[1].Scope);
			componentList = CCodeAnalyser.GetComponents(statementStr, ctx.ParseResult);
			meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, ctx);

			Assert.AreEqual(2, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);

			// 第三条语句
			statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[2].Scope);
			componentList = CCodeAnalyser.GetComponents(statementStr, ctx.ParseResult);
			meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, ctx);

			Assert.AreEqual(2, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_4()
		{
			AnalysisContext ctx = new AnalysisContext();
			ctx.ParseResult = c_source_file_parse_result;
			CCodeAnalyser.StatementAnalysis(root.childList[0], ctx);
			CCodeAnalyser.StatementAnalysis(root.childList[1], ctx);
			CCodeAnalyser.StatementAnalysis(root.childList[2], ctx);
			List<string> codeList = ctx.ParseResult.SourceParseInfo.parsedCodeList;
			// 第4句
			string statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[3].Scope);
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(statementStr, ctx.ParseResult);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, ctx);

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
			AnalysisContext ctx = new AnalysisContext();
			ctx.ParseResult = c_source_file_parse_result;
			for (int i = 0; i < 4; i++)
			{
				CCodeAnalyser.StatementAnalysis(root.childList[i], ctx);
			}
			// 第5句
			List<string> codeList = ctx.ParseResult.SourceParseInfo.parsedCodeList;
			string statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[4].Scope);
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(statementStr, ctx.ParseResult);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, ctx);

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
			AnalysisContext ctx = new AnalysisContext();
			ctx.ParseResult = c_source_file_parse_result;
			for (int i = 0; i < 5; i++)
			{
				CCodeAnalyser.StatementAnalysis(root.childList[i], ctx);
			}
			// 第6句
			List<string> codeList = ctx.ParseResult.SourceParseInfo.parsedCodeList;
			string statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[5].Scope);
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(statementStr, ctx.ParseResult);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, ctx);

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
			AnalysisContext ctx = new AnalysisContext();
			ctx.ParseResult = c_source_file_parse_result;
			for (int i = 0; i < 6; i++)
			{
				CCodeAnalyser.StatementAnalysis(root.childList[i], ctx);
			}
			// 第7句
			List<string> codeList = ctx.ParseResult.SourceParseInfo.parsedCodeList;
			string statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[6].Scope);
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(statementStr, ctx.ParseResult);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, ctx);

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
			AnalysisContext ctx = new AnalysisContext();
			ctx.ParseResult = c_source_file_parse_result;
			for (int i = 0; i < 7; i++)
			{
				CCodeAnalyser.StatementAnalysis(root.childList[i], ctx);
			}
			// 第8句
			List<string> codeList = ctx.ParseResult.SourceParseInfo.parsedCodeList;
			string statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[7].Scope);
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(statementStr, ctx.ParseResult);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, ctx);

			Assert.AreEqual(1, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.FunctionCalling, meaningGroupList[0].Type);
			Assert.AreEqual("ShareLibStepFailJudgeVal(&varInStep,&rctasw_can_mng_tbl,&varOutStep)", meaningGroupList[0].Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_9()
		{
			AnalysisContext ctx = new AnalysisContext();
			ctx.ParseResult = c_source_file_parse_result;
			for (int i = 0; i < 8; i++)
			{
				CCodeAnalyser.StatementAnalysis(root.childList[i], ctx);
			}
			// 第9句
			List<string> codeList = ctx.ParseResult.SourceParseInfo.parsedCodeList;
			string statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[8].Scope);
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(statementStr, ctx.ParseResult);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, ctx);

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
			AnalysisContext ctx = new AnalysisContext();
			ctx.ParseResult = c_source_file_parse_result;
			for (int i = 0; i < 9; i++)
			{
				CCodeAnalyser.StatementAnalysis(root.childList[i], ctx);
			}
			// 第9句
			List<string> codeList = ctx.ParseResult.SourceParseInfo.parsedCodeList;
			string statementStr = CCodeAnalyser.GetStatementStr(codeList, root.childList[9].Scope);
			List<StatementComponent> componentList = CCodeAnalyser.GetComponents(statementStr, ctx.ParseResult);
			List<MeaningGroup> meaningGroupList = CCodeAnalyser.GetMeaningGroups(componentList, ctx);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.GlobalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("(Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct)->value", meaningGroupList[0].Text);
			Assert.AreEqual(MeaningGroupType.EqualMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
			Assert.AreEqual("(*&pvOut)", meaningGroupList[2].Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_IO()
		{
			AnalysisContext ctx = CCodeAnalyser.FunctionStatementsAnalysis(root, c_source_file_parse_result);

			Assert.AreEqual(3, ctx.LocalVarList.Count);

			Assert.AreEqual("varInStep", ctx.LocalVarList[0].Name);
			Assert.AreEqual("struct SHARE_LIB_CAN_IN_STEP_H_USR_DEF_TYPE_0", ctx.LocalVarList[0].Type.Name);

			Assert.AreEqual("varOutStep", ctx.LocalVarList[1].Name);
			Assert.AreEqual("struct SHARE_LIB_CAN_IN_STEP_H_USR_DEF_TYPE_4", ctx.LocalVarList[1].Type.Name);

			Assert.AreEqual("pvOut", ctx.LocalVarList[2].Name);
			Assert.AreEqual("struct RTE_TYPE_H_USR_DEF_TYPE_0", ctx.LocalVarList[2].Type.Name);

			Assert.AreEqual(2, ctx.InputGlobalList.Count);
			Assert.AreEqual("(*(Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_rp_srIf_in_TRCTA_val)).value", ctx.InputGlobalList[0].MeanningGroup.Text);
			Assert.AreEqual("(*(Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_rp_srIf_in_TRCTASts_val)).value", ctx.InputGlobalList[1].MeanningGroup.Text);

			Assert.AreEqual(1, ctx.CalledFunctionList.Count);
			Assert.AreEqual("ShareLibStepFailJudgeVal", ctx.CalledFunctionList[0].FunctionName);
			Assert.AreEqual(ActParaPassType.Reference,	ctx.CalledFunctionList[0].ActualParaInfoList[0].passType);
			Assert.AreEqual(false,						ctx.CalledFunctionList[0].ActualParaInfoList[0].readOut);
			Assert.AreEqual(ActParaPassType.Reference,	ctx.CalledFunctionList[0].ActualParaInfoList[1].passType);
			Assert.AreEqual(false,						ctx.CalledFunctionList[0].ActualParaInfoList[1].readOut);
			Assert.AreEqual(ActParaPassType.Reference,	ctx.CalledFunctionList[0].ActualParaInfoList[2].passType);
			Assert.AreEqual(true,						ctx.CalledFunctionList[0].ActualParaInfoList[2].readOut);

			Assert.AreEqual(1, ctx.OutputGlobalList.Count);
			Assert.AreEqual("(Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct)->value", ctx.OutputGlobalList[0].MeanningGroup.Text);

			Assert.AreEqual("ShareLibStepFailJudgeVal(&varInStep,&rctasw_can_mng_tbl,&varOutStep)", ctx.CalledFunctionList[0].MeaningGroup.Text);
		}
	}

	[TestClass]
	public class sym_rbl_in_trcta_initReset
	{
		static StatementNode root;                                                      // 函数语句结构的根节点
		static CodeParseInfo c_source_file_parse_result;

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			string source_name = "..\\..\\..\\TestSrc\\swc_in_trcta\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_initReset";
			c_source_file_parse_result = Common.UnitTest_SourceFileProcess(source_name, function_name, ref root);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_initReset_IO()
		{
			AnalysisContext analysisContext = CCodeAnalyser.FunctionStatementsAnalysis(root, c_source_file_parse_result);
			Assert.AreEqual(1, analysisContext.OutputGlobalList.Count);
			Assert.AreEqual("(Rte_Inst_swc_in_trcta->rbl_in_trcta_initReset_pp_srIf_pv_PvRctasw_struct)->value", analysisContext.OutputGlobalList[0].MeanningGroup.Text);
		}
	}

	[TestClass]
	public class sym_rbl_in_trcta_initWakeup
	{
		static StatementNode root;                                                      // 函数语句结构的根节点
		static CodeParseInfo c_source_file_parse_result;

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			string source_name = "..\\..\\..\\TestSrc\\swc_in_trcta\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_initWakeup";
			c_source_file_parse_result = Common.UnitTest_SourceFileProcess(source_name, function_name, ref root);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_initWakeup_IO()
		{
			AnalysisContext analysisContext = CCodeAnalyser.FunctionStatementsAnalysis(root, c_source_file_parse_result);
			Assert.AreEqual(1, analysisContext.OutputGlobalList.Count);
			Assert.AreEqual("(Rte_Inst_swc_in_trcta->rbl_in_trcta_initWakeup_pp_srIf_pv_PvRctasw_struct)->value", analysisContext.OutputGlobalList[0].MeanningGroup.Text);
		}
	}
}
