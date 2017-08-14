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
        static StatementNode _func_root;                                                      // 函数语句结构的根节点
        static FILE_PARSE_INFO _source_parse_info;

        [ClassInitialize]
        public static void TestClassSetup(TestContext ctx)
        {
			string source_name = "..\\..\\..\\TestSrc\\swc_in_trcta\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_igoff";
			_source_parse_info = Common.UnitTest_GetSourceFileStructure(source_name, function_name, ref _func_root);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_0()
        {
            // 函数语句分析: 分析入出力
            //CCodeAnalyser.FunctionStatementsAnalysis(root, c_file_result);
            Assert.AreEqual(3, _func_root.childList.Count);
            Assert.AreEqual(StatementNodeType.Simple, _func_root.childList[0].Type);
            Assert.AreEqual(StatementNodeType.Simple, _func_root.childList[1].Type);
            Assert.AreEqual(StatementNodeType.Simple, _func_root.childList[2].Type);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_1_1()
        {
            // 顺次分析各语句
			FUNCTION_ANALYSIS_CONTEXT analysisContext = new FUNCTION_ANALYSIS_CONTEXT();
			//CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, ctx);

			List<string> codeList = _source_parse_info.CodeList;
			string statementStr = StatementAnalysis.GetStatementStr(codeList, _func_root.childList[0].Scope);
			List<STATEMENT_COMPONENT> componentList = StatementAnalysis.GetComponents(statementStr, _source_parse_info);
			List<MEANING_GROUP> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, _source_parse_info, analysisContext);

            Assert.AreEqual(2, meaningGroupList.Count);
            Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
            Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_1_2()
        {
			FUNCTION_ANALYSIS_CONTEXT analysisContext = new FUNCTION_ANALYSIS_CONTEXT();
			StatementAnalysis.StatementAnalyze(_func_root.childList[0], _source_parse_info, analysisContext);

			Assert.AreEqual(1, analysisContext.LocalVarList.Count);
			Assert.AreEqual("struct RTE_SWC_IN_TRCTA_C_USR_DEF_TYPE_1", analysisContext.LocalVarList[0].Type.Name);
			Assert.AreEqual("pvOut", analysisContext.LocalVarList[0].Name);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_2()
        {
			FUNCTION_ANALYSIS_CONTEXT analysisContext = new FUNCTION_ANALYSIS_CONTEXT();
			StatementAnalysis.StatementAnalyze(_func_root.childList[0], _source_parse_info, analysisContext);

			List<string> codeList = _source_parse_info.CodeList;
			string statementStr = StatementAnalysis.GetStatementStr(codeList, _func_root.childList[1].Scope);
			List<STATEMENT_COMPONENT> componentList = StatementAnalysis.GetComponents(statementStr, _source_parse_info);
			List<MEANING_GROUP> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, _source_parse_info, analysisContext);

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
			FUNCTION_ANALYSIS_CONTEXT analysisContext = new FUNCTION_ANALYSIS_CONTEXT();
			StatementAnalysis.StatementAnalyze(_func_root.childList[0], _source_parse_info, analysisContext);
			StatementAnalysis.StatementAnalyze(_func_root.childList[1], _source_parse_info, analysisContext);

			List<string> codeList = _source_parse_info.CodeList;
			string statementStr = StatementAnalysis.GetStatementStr(codeList, _func_root.childList[2].Scope);
			List<STATEMENT_COMPONENT> componentList = StatementAnalysis.GetComponents(statementStr, _source_parse_info);
			List<MEANING_GROUP> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, _source_parse_info, analysisContext);

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
			FUNCTION_ANALYSIS_CONTEXT analysisContext = StatementAnalysis.FunctionStatementsAnalysis(_func_root, _source_parse_info);
			Assert.AreEqual(1, analysisContext.OutputGlobalList.Count);
			Assert.AreEqual("Rte_Inst_swc_in_trcta->rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct->value", analysisContext.OutputGlobalList[0].Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void Rte_swc_in_trcta_Global()
		{
			// extern CONSTP2CONST(struct Rte_CDS_swc_in_trcta, RTE_CONST, RTE_APPL_CONST) Rte_Inst_swc_in_trcta;
			Assert.AreEqual(1, _source_parse_info.GlobalDeclareList.Count);
			Assert.AreEqual(VAR_TYPE_CATEGORY.POINTER, _source_parse_info.GlobalDeclareList[0].VarTypeCategory);
			Assert.AreEqual("extern const struct Rte_CDS_swc_in_trcta * const", _source_parse_info.GlobalDeclareList[0].Type.GetFullName());
			Assert.AreEqual("Rte_Inst_swc_in_trcta", _source_parse_info.GlobalDeclareList[0].Name);

			Assert.AreEqual(2, _source_parse_info.GlobalDefineList.Count);
			// inTblRctasw
			Assert.AreEqual(VAR_TYPE_CATEGORY.ARRAY, _source_parse_info.GlobalDefineList[0].VarTypeCategory);
			Assert.AreEqual("static const unsigned char", _source_parse_info.GlobalDefineList[0].Type.GetFullName());
			Assert.AreEqual("inTblRctasw", _source_parse_info.GlobalDefineList[0].Name);
			Assert.AreEqual(3, _source_parse_info.GlobalDefineList[0].MemberList.Count);
			Assert.AreEqual(0, _source_parse_info.GlobalDefineList[0].MemberList[0].Value);	// PV_RCTASW_IN_OFF
			Assert.AreEqual(1, _source_parse_info.GlobalDefineList[0].MemberList[1].Value);	// PV_RCTASW_IN_ON
			Assert.AreEqual(2, _source_parse_info.GlobalDefineList[0].MemberList[2].Value);	// PV_RCTASW_IN_UNFIX

			// rctasw_can_mng_tbl
			Assert.AreEqual(VAR_TYPE_CATEGORY.USR_DEF_TYPE, _source_parse_info.GlobalDefineList[1].VarTypeCategory);
			Assert.AreEqual("rctasw_can_mng_tbl", _source_parse_info.GlobalDefineList[1].Name);
			Assert.AreEqual(5, _source_parse_info.GlobalDefineList[1].MemberList.Count);
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, _source_parse_info.GlobalDefineList[1].MemberList[0].VarTypeCategory);
			Assert.AreEqual(3, _source_parse_info.GlobalDefineList[1].MemberList[0].Value);					// IN_RCTASW_TBL_SIZE
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, _source_parse_info.GlobalDefineList[1].MemberList[1].VarTypeCategory);
			Assert.AreEqual(0, _source_parse_info.GlobalDefineList[1].MemberList[1].Value);					// PV_RCTASW_IN_OFF
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, _source_parse_info.GlobalDefineList[1].MemberList[2].VarTypeCategory);
			Assert.AreEqual(0, _source_parse_info.GlobalDefineList[1].MemberList[2].Value);					// PV_RCTASW_IN_OFF
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, _source_parse_info.GlobalDefineList[1].MemberList[3].VarTypeCategory);
			Assert.AreEqual(1, _source_parse_info.GlobalDefineList[1].MemberList[3].Value);					// PV_RCTASW_IN_ON
			Assert.AreEqual(VAR_TYPE_CATEGORY.POINTER, _source_parse_info.GlobalDefineList[1].MemberList[4].VarTypeCategory);
			Assert.AreEqual("&inTblRctasw[0]", _source_parse_info.GlobalDefineList[1].MemberList[4].Value);	// &inTblRctasw[0]
		}
    }

	[TestClass]
	public class sym_rbl_in_trcta_igon
	{
		static StatementNode root;                                                      // 函数语句结构的根节点
		static FILE_PARSE_INFO c_source_parse_info;

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			string source_name = "..\\..\\..\\TestSrc\\swc_in_trcta\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_igon";
			c_source_parse_info = Common.UnitTest_GetSourceFileStructure(source_name, function_name, ref root);
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
			FUNCTION_ANALYSIS_CONTEXT func_ctx = new FUNCTION_ANALYSIS_CONTEXT();
			//CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, localVarList);

			List<string> codeList = c_source_parse_info.CodeList;
			// 第一条语句
			string statementStr = StatementAnalysis.GetStatementStr(codeList, root.childList[0].Scope);
			List<STATEMENT_COMPONENT> componentList = StatementAnalysis.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(2, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);

			// 第二条语句
			statementStr = StatementAnalysis.GetStatementStr(codeList, root.childList[1].Scope);
			componentList = StatementAnalysis.GetComponents(statementStr, c_source_parse_info);
			meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(2, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);

			// 第三条语句
			statementStr = StatementAnalysis.GetStatementStr(codeList, root.childList[2].Scope);
			componentList = StatementAnalysis.GetComponents(statementStr, c_source_parse_info);
			meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(2, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_4()
		{
			FUNCTION_ANALYSIS_CONTEXT func_ctx = new FUNCTION_ANALYSIS_CONTEXT();
			StatementAnalysis.StatementAnalyze(root.childList[0], c_source_parse_info, func_ctx);
			StatementAnalysis.StatementAnalyze(root.childList[1], c_source_parse_info, func_ctx);
			StatementAnalysis.StatementAnalyze(root.childList[2], c_source_parse_info, func_ctx);
			List<string> codeList = c_source_parse_info.CodeList;
			// 第4句
			string statementStr = StatementAnalysis.GetStatementStr(codeList, root.childList[3].Scope);
			List<STATEMENT_COMPONENT> componentList = StatementAnalysis.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

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
			FUNCTION_ANALYSIS_CONTEXT func_ctx = new FUNCTION_ANALYSIS_CONTEXT();
			for (int i = 0; i < 4; i++)
			{
				StatementAnalysis.StatementAnalyze(root.childList[i], c_source_parse_info, func_ctx);
			}
			// 第5句
			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = StatementAnalysis.GetStatementStr(codeList, root.childList[4].Scope);
			List<STATEMENT_COMPONENT> componentList = StatementAnalysis.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

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
			FUNCTION_ANALYSIS_CONTEXT func_ctx = new FUNCTION_ANALYSIS_CONTEXT();
			for (int i = 0; i < 5; i++)
			{
				StatementAnalysis.StatementAnalyze(root.childList[i], c_source_parse_info, func_ctx);
			}
			// 第6句
			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = StatementAnalysis.GetStatementStr(codeList, root.childList[5].Scope);
			List<STATEMENT_COMPONENT> componentList = StatementAnalysis.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

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
			FUNCTION_ANALYSIS_CONTEXT func_ctx = new FUNCTION_ANALYSIS_CONTEXT();
			for (int i = 0; i < 6; i++)
			{
				StatementAnalysis.StatementAnalyze(root.childList[i], c_source_parse_info, func_ctx);
			}
			// 第7句
			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = StatementAnalysis.GetStatementStr(codeList, root.childList[6].Scope);
			List<STATEMENT_COMPONENT> componentList = StatementAnalysis.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

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
			FUNCTION_ANALYSIS_CONTEXT func_ctx = new FUNCTION_ANALYSIS_CONTEXT();
			for (int i = 0; i < 7; i++)
			{
				StatementAnalysis.StatementAnalyze(root.childList[i], c_source_parse_info, func_ctx);
			}
			// 第8句
			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = StatementAnalysis.GetStatementStr(codeList, root.childList[7].Scope);
			List<STATEMENT_COMPONENT> componentList = StatementAnalysis.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(1, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.FunctionCalling, meaningGroupList[0].Type);
			Assert.AreEqual("ShareLibStepFailJudgeVal(&varInStep,&rctasw_can_mng_tbl,&varOutStep)", meaningGroupList[0].Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_9()
		{
			FUNCTION_ANALYSIS_CONTEXT func_ctx = new FUNCTION_ANALYSIS_CONTEXT();
			for (int i = 0; i < 8; i++)
			{
				StatementAnalysis.StatementAnalyze(root.childList[i], c_source_parse_info, func_ctx);
			}
			// 第9句
			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = StatementAnalysis.GetStatementStr(codeList, root.childList[8].Scope);
			List<STATEMENT_COMPONENT> componentList = StatementAnalysis.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

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
			FUNCTION_ANALYSIS_CONTEXT func_ctx = new FUNCTION_ANALYSIS_CONTEXT();
			for (int i = 0; i < 9; i++)
			{
				StatementAnalysis.StatementAnalyze(root.childList[i], c_source_parse_info, func_ctx);
			}
			// 第9句
			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = StatementAnalysis.GetStatementStr(codeList, root.childList[9].Scope);
			List<STATEMENT_COMPONENT> componentList = StatementAnalysis.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = StatementAnalysis.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

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
			FUNCTION_ANALYSIS_CONTEXT func_ctx = StatementAnalysis.FunctionStatementsAnalysis(root, c_source_parse_info);

			Assert.AreEqual(3, func_ctx.LocalVarList.Count);

			Assert.AreEqual("varInStep", func_ctx.LocalVarList[0].Name);
			Assert.AreEqual("struct RTE_SWC_IN_TRCTA_C_USR_DEF_TYPE_18", func_ctx.LocalVarList[0].Type.Name);

			Assert.AreEqual("varOutStep", func_ctx.LocalVarList[1].Name);
			Assert.AreEqual("struct RTE_SWC_IN_TRCTA_C_USR_DEF_TYPE_22", func_ctx.LocalVarList[1].Type.Name);

			Assert.AreEqual("pvOut", func_ctx.LocalVarList[2].Name);
			Assert.AreEqual("struct RTE_SWC_IN_TRCTA_C_USR_DEF_TYPE_1", func_ctx.LocalVarList[2].Type.Name);

			Assert.AreEqual(2, func_ctx.InputGlobalList.Count);
			Assert.AreEqual("Rte_Inst_swc_in_trcta->*rbl_in_trcta_igon_rp_srIf_in_TRCTA_val.value", func_ctx.InputGlobalList[0].Text);
			Assert.AreEqual("Rte_Inst_swc_in_trcta->*rbl_in_trcta_igon_rp_srIf_in_TRCTASts_val.value", func_ctx.InputGlobalList[1].Text);

			Assert.AreEqual(1, func_ctx.CalledFunctionList.Count);
			Assert.AreEqual("ShareLibStepFailJudgeVal", func_ctx.CalledFunctionList[0].FunctionName);
			Assert.AreEqual(ActParaPassType.Reference,	func_ctx.CalledFunctionList[0].ActualParaInfoList[0].passType);
			Assert.AreEqual(false,						func_ctx.CalledFunctionList[0].ActualParaInfoList[0].readOut);
			Assert.AreEqual(ActParaPassType.Reference,	func_ctx.CalledFunctionList[0].ActualParaInfoList[1].passType);
			Assert.AreEqual(false,						func_ctx.CalledFunctionList[0].ActualParaInfoList[1].readOut);
			Assert.AreEqual(ActParaPassType.Reference,	func_ctx.CalledFunctionList[0].ActualParaInfoList[2].passType);
			Assert.AreEqual(true,						func_ctx.CalledFunctionList[0].ActualParaInfoList[2].readOut);

			Assert.AreEqual(1, func_ctx.OutputGlobalList.Count);
			Assert.AreEqual("Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct->value", func_ctx.OutputGlobalList[0].Text);

			Assert.AreEqual("ShareLibStepFailJudgeVal(&varInStep,&rctasw_can_mng_tbl,&varOutStep)", func_ctx.CalledFunctionList[0].MeaningGroup.Text);
		}
	}

	[TestClass]
	public class sym_rbl_in_trcta_initReset
	{
		static StatementNode root;                                                      // 函数语句结构的根节点
		static FILE_PARSE_INFO c_source_parse_info;

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			string source_name = "..\\..\\..\\TestSrc\\swc_in_trcta\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_initReset";
			c_source_parse_info = Common.UnitTest_GetSourceFileStructure(source_name, function_name, ref root);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_initReset_IO()
		{
			FUNCTION_ANALYSIS_CONTEXT analysisContext = StatementAnalysis.FunctionStatementsAnalysis(root, c_source_parse_info);
			Assert.AreEqual(1, analysisContext.OutputGlobalList.Count);
			Assert.AreEqual("Rte_Inst_swc_in_trcta->rbl_in_trcta_initReset_pp_srIf_pv_PvRctasw_struct->value", analysisContext.OutputGlobalList[0].Text);
		}
	}

	[TestClass]
	public class sym_rbl_in_trcta_initWakeup
	{
		static StatementNode root;                                                      // 函数语句结构的根节点
		static FILE_PARSE_INFO c_source_parse_info;

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			string source_name = "..\\..\\..\\TestSrc\\swc_in_trcta\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_initWakeup";
			c_source_parse_info = Common.UnitTest_GetSourceFileStructure(source_name, function_name, ref root);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_initWakeup_IO()
		{
			FUNCTION_ANALYSIS_CONTEXT analysisContext = StatementAnalysis.FunctionStatementsAnalysis(root, c_source_parse_info);
			Assert.AreEqual(1, analysisContext.OutputGlobalList.Count);
			Assert.AreEqual("Rte_Inst_swc_in_trcta->rbl_in_trcta_initWakeup_pp_srIf_pv_PvRctasw_struct->value", analysisContext.OutputGlobalList[0].Text);
		}
	}
}
