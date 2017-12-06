using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using Mr.Robot;
using Mr.Robot.CDeducer;

namespace UnitTestProject
{
	[TestClass]
	public class sym_rbl_in_trcta_igoff
    {
        static STATEMENT_NODE c_func_root;                                              // 函数语句结构的根节点
        static FILE_PARSE_INFO c_source_parse_info;

        [ClassInitialize]
        public static void TestClassSetup(TestContext ctx)
        {
			string source_name = "..\\..\\..\\TestSrc\\swc_in_trcta\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_igoff";
			List<FILE_PARSE_INFO> parseInfoList = Common.UnitTest_GetSourceFileStructure(source_name);
			c_source_parse_info = Common.FindSrcParseInfoFromList(source_name, parseInfoList);
			c_func_root = C_FUNC_LOCATOR.FuncLocatorStart2(c_source_parse_info, function_name);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_0()
        {
            Assert.AreEqual(3, c_func_root.ChildNodeList.Count);
            Assert.AreEqual(STATEMENT_TYPE.Simple, c_func_root.ChildNodeList[0].Type);
            Assert.AreEqual(STATEMENT_TYPE.Simple, c_func_root.ChildNodeList[1].Type);
            Assert.AreEqual(STATEMENT_TYPE.Simple, c_func_root.ChildNodeList[2].Type);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_1_1()
        {
            // 顺次分析各语句
			FUNC_CONTEXT analysisContext = new FUNC_CONTEXT();

			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = COMN_PROC.GetStatementStr(codeList, c_func_root.ChildNodeList[0].Scope);
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, analysisContext);

            Assert.AreEqual(2, meaningGroupList.Count);
            Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
            Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_1_2()
        {
			FUNC_CONTEXT analysisContext = new FUNC_CONTEXT();
			DEDUCER_CONTEXT dCtx = new DEDUCER_CONTEXT();
			C_DEDUCER.StatementProc(c_func_root.ChildNodeList[0], c_source_parse_info, analysisContext, dCtx);

			Assert.AreEqual(1, analysisContext.LocalVarList.Count);
			Assert.AreEqual("struct RTE_SWC_IN_TRCTA_C_USR_DEF_TYPE_1", analysisContext.LocalVarList[0].Type.Name);
			Assert.AreEqual("pvOut", analysisContext.LocalVarList[0].Name);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_2()
        {
			FUNC_CONTEXT analysisContext = new FUNC_CONTEXT();
			DEDUCER_CONTEXT dCtx = new DEDUCER_CONTEXT();
			C_DEDUCER.StatementProc(c_func_root.ChildNodeList[0], c_source_parse_info, analysisContext, dCtx);

			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = COMN_PROC.GetStatementStr(codeList, c_func_root.ChildNodeList[1].Scope);
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, analysisContext);

            Assert.AreEqual(3, meaningGroupList.Count);
            Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
            Assert.AreEqual("pvOut.dt", meaningGroupList[0].TextStr);
            Assert.AreEqual(MeaningGroupType.AssignmentMark, meaningGroupList[1].Type);
            Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
            Assert.AreEqual("((uint8)0U)", meaningGroupList[2].TextStr);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
        public void sym_rbl_in_trcta_igoff_3()
        {
			FUNC_CONTEXT analysisContext = new FUNC_CONTEXT();
			DEDUCER_CONTEXT dCtx = new DEDUCER_CONTEXT();
			C_DEDUCER.StatementProc(c_func_root.ChildNodeList[0], c_source_parse_info, analysisContext, dCtx);
			C_DEDUCER.StatementProc(c_func_root.ChildNodeList[1], c_source_parse_info, analysisContext, dCtx);

			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = COMN_PROC.GetStatementStr(codeList, c_func_root.ChildNodeList[2].Scope);
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, analysisContext);

            Assert.AreEqual(3, meaningGroupList.Count);
            Assert.AreEqual(MeaningGroupType.GlobalVariable, meaningGroupList[0].Type);
            Assert.AreEqual("(Rte_Inst_swc_in_trcta->rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct)->value", meaningGroupList[0].TextStr);
            Assert.AreEqual(MeaningGroupType.AssignmentMark, meaningGroupList[1].Type);
            Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
            Assert.AreEqual("(*&pvOut)", meaningGroupList[2].TextStr);
        }

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igoff_IO()
		{
			//FUNC_CONTEXT analysisContext = C_DEDUCER.DeducerStart(c_func_root, c_source_parse_info);
			//Assert.AreEqual(1, analysisContext.OutputGlobalList.Count);
			//Assert.AreEqual("Rte_Inst_swc_in_trcta->rbl_in_trcta_igoff_pp_srIf_pv_PvRctasw_struct->value", analysisContext.OutputGlobalList[0].Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void Rte_swc_in_trcta_Global()
		{
			// extern CONSTP2CONST(struct Rte_CDS_swc_in_trcta, RTE_CONST, RTE_APPL_CONST) Rte_Inst_swc_in_trcta;
			Assert.AreEqual(1, c_source_parse_info.GlobalDeclareList.Count);
			Assert.AreEqual(VAR_TYPE_CATEGORY.POINTER, c_source_parse_info.GlobalDeclareList[0].VarTypeCategory);
			Assert.AreEqual("extern const struct Rte_CDS_swc_in_trcta * const", c_source_parse_info.GlobalDeclareList[0].Type.GetFullName());
			Assert.AreEqual("Rte_Inst_swc_in_trcta", c_source_parse_info.GlobalDeclareList[0].Name);

			Assert.AreEqual(2, c_source_parse_info.GlobalDefineList.Count);
			// inTblRctasw
			Assert.AreEqual(VAR_TYPE_CATEGORY.ARRAY, c_source_parse_info.GlobalDefineList[0].VarTypeCategory);
			Assert.AreEqual("static const unsigned char", c_source_parse_info.GlobalDefineList[0].Type.GetFullName());
			Assert.AreEqual("inTblRctasw", c_source_parse_info.GlobalDefineList[0].Name);
			Assert.AreEqual(3, c_source_parse_info.GlobalDefineList[0].MemberList.Count);
			Assert.AreEqual(0, c_source_parse_info.GlobalDefineList[0].MemberList[0].Value);	// PV_RCTASW_IN_OFF
			Assert.AreEqual(1, c_source_parse_info.GlobalDefineList[0].MemberList[1].Value);	// PV_RCTASW_IN_ON
			Assert.AreEqual(2, c_source_parse_info.GlobalDefineList[0].MemberList[2].Value);	// PV_RCTASW_IN_UNFIX

			// rctasw_can_mng_tbl
			Assert.AreEqual(VAR_TYPE_CATEGORY.USR_DEF_TYPE, c_source_parse_info.GlobalDefineList[1].VarTypeCategory);
			Assert.AreEqual("rctasw_can_mng_tbl", c_source_parse_info.GlobalDefineList[1].Name);
			Assert.AreEqual(5, c_source_parse_info.GlobalDefineList[1].MemberList.Count);
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, c_source_parse_info.GlobalDefineList[1].MemberList[0].VarTypeCategory);
			Assert.AreEqual(3, c_source_parse_info.GlobalDefineList[1].MemberList[0].Value);					// IN_RCTASW_TBL_SIZE
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, c_source_parse_info.GlobalDefineList[1].MemberList[1].VarTypeCategory);
			Assert.AreEqual(0, c_source_parse_info.GlobalDefineList[1].MemberList[1].Value);					// PV_RCTASW_IN_OFF
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, c_source_parse_info.GlobalDefineList[1].MemberList[2].VarTypeCategory);
			Assert.AreEqual(0, c_source_parse_info.GlobalDefineList[1].MemberList[2].Value);					// PV_RCTASW_IN_OFF
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, c_source_parse_info.GlobalDefineList[1].MemberList[3].VarTypeCategory);
			Assert.AreEqual(1, c_source_parse_info.GlobalDefineList[1].MemberList[3].Value);					// PV_RCTASW_IN_ON
			Assert.AreEqual(VAR_TYPE_CATEGORY.POINTER, c_source_parse_info.GlobalDefineList[1].MemberList[4].VarTypeCategory);
			Assert.AreEqual("&inTblRctasw[0]", c_source_parse_info.GlobalDefineList[1].MemberList[4].Value);	// &inTblRctasw[0]
		}
    }

	[TestClass]
	public class sym_rbl_in_trcta_igon
	{
		static STATEMENT_NODE root;                                                     // 函数语句结构的根节点
		static FILE_PARSE_INFO c_source_parse_info;

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			string source_name = "..\\..\\..\\TestSrc\\swc_in_trcta\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_igon";
			List<FILE_PARSE_INFO> parseInfoList = Common.UnitTest_GetSourceFileStructure(source_name);
			c_source_parse_info = Common.FindSrcParseInfoFromList(source_name, parseInfoList);
			root = C_FUNC_LOCATOR.FuncLocatorStart2(c_source_parse_info, function_name);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_0()
		{
			// 函数语句分析: 分析入出力
			//CCodeAnalyser.FunctionStatementsAnalysis(root, c_file_result);
			Assert.AreEqual(10, root.ChildNodeList.Count);
			Assert.AreEqual(STATEMENT_TYPE.Simple, root.ChildNodeList[0].Type);
			Assert.AreEqual(STATEMENT_TYPE.Simple, root.ChildNodeList[1].Type);
			Assert.AreEqual(STATEMENT_TYPE.Simple, root.ChildNodeList[2].Type);
			Assert.AreEqual(STATEMENT_TYPE.Simple, root.ChildNodeList[3].Type);
			Assert.AreEqual(STATEMENT_TYPE.Simple, root.ChildNodeList[4].Type);
			Assert.AreEqual(STATEMENT_TYPE.Simple, root.ChildNodeList[5].Type);
			Assert.AreEqual(STATEMENT_TYPE.Simple, root.ChildNodeList[6].Type);
			Assert.AreEqual(STATEMENT_TYPE.Simple, root.ChildNodeList[7].Type);
			Assert.AreEqual(STATEMENT_TYPE.Simple, root.ChildNodeList[8].Type);
			Assert.AreEqual(STATEMENT_TYPE.Simple, root.ChildNodeList[9].Type);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_1To3()
		{
			// 顺次分析各语句
			FUNC_CONTEXT func_ctx = new FUNC_CONTEXT();
			//CCodeAnalyser.StatementAnalysis(root.childList[0], c_source_file_parse_result, localVarList);

			List<string> codeList = c_source_parse_info.CodeList;
			// 第一条语句
			string statementStr = COMN_PROC.GetStatementStr(codeList, root.ChildNodeList[0].Scope);
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(2, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);

			// 第二条语句
			statementStr = COMN_PROC.GetStatementStr(codeList, root.ChildNodeList[1].Scope);
			componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(2, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);

			// 第三条语句
			statementStr = COMN_PROC.GetStatementStr(codeList, root.ChildNodeList[2].Scope);
			componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(2, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.VariableType, meaningGroupList[0].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[1].Type);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_4()
		{
			FUNC_CONTEXT func_ctx = new FUNC_CONTEXT();
			DEDUCER_CONTEXT dCtx = new DEDUCER_CONTEXT();
			C_DEDUCER.StatementProc(root.ChildNodeList[0], c_source_parse_info, func_ctx, dCtx);
			C_DEDUCER.StatementProc(root.ChildNodeList[1], c_source_parse_info, func_ctx, dCtx);
			C_DEDUCER.StatementProc(root.ChildNodeList[2], c_source_parse_info, func_ctx, dCtx);
			List<string> codeList = c_source_parse_info.CodeList;
			// 第4句
			string statementStr = COMN_PROC.GetStatementStr(codeList, root.ChildNodeList[3].Scope);
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("varOutStep.pv", meaningGroupList[0].TextStr);
			Assert.AreEqual(MeaningGroupType.AssignmentMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
			Assert.AreEqual("((uint8)0U)", meaningGroupList[2].TextStr);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_5()
		{
			FUNC_CONTEXT func_ctx = new FUNC_CONTEXT();
			DEDUCER_CONTEXT dCtx = new DEDUCER_CONTEXT();
			for (int i = 0; i < 4; i++)
			{
				C_DEDUCER.StatementProc(root.ChildNodeList[i], c_source_parse_info, func_ctx, dCtx);
			}
			// 第5句
			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = COMN_PROC.GetStatementStr(codeList, root.ChildNodeList[4].Scope);
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("varInStep.inSig", meaningGroupList[0].TextStr);
			Assert.AreEqual(MeaningGroupType.AssignmentMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
			Assert.AreEqual("((*(Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_rp_srIf_in_TRCTA_val)).value)", meaningGroupList[2].TextStr);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_6()
		{
			FUNC_CONTEXT func_ctx = new FUNC_CONTEXT();
			DEDUCER_CONTEXT dCtx = new DEDUCER_CONTEXT();
			for (int i = 0; i < 5; i++)
			{
				C_DEDUCER.StatementProc(root.ChildNodeList[i], c_source_parse_info, func_ctx, dCtx);
			}
			// 第6句
			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = COMN_PROC.GetStatementStr(codeList, root.ChildNodeList[5].Scope);
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("varInStep.msgSts", meaningGroupList[0].TextStr);
			Assert.AreEqual(MeaningGroupType.AssignmentMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
			Assert.AreEqual("((*(Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_rp_srIf_in_TRCTASts_val)).value)", meaningGroupList[2].TextStr);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_7()
		{
			FUNC_CONTEXT func_ctx = new FUNC_CONTEXT();
			DEDUCER_CONTEXT dCtx = new DEDUCER_CONTEXT();
			for (int i = 0; i < 6; i++)
			{
				C_DEDUCER.StatementProc(root.ChildNodeList[i], c_source_parse_info, func_ctx, dCtx);
			}
			// 第7句
			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = COMN_PROC.GetStatementStr(codeList, root.ChildNodeList[6].Scope);
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("varInStep.powerSts", meaningGroupList[0].TextStr);
			Assert.AreEqual(MeaningGroupType.AssignmentMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
			Assert.AreEqual("((uint8)0x01)", meaningGroupList[2].TextStr);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_8()
		{
			FUNC_CONTEXT func_ctx = new FUNC_CONTEXT();
			DEDUCER_CONTEXT dCtx = new DEDUCER_CONTEXT();
			for (int i = 0; i < 7; i++)
			{
				C_DEDUCER.StatementProc(root.ChildNodeList[i], c_source_parse_info, func_ctx, dCtx);
			}
			// 第8句
			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = COMN_PROC.GetStatementStr(codeList, root.ChildNodeList[7].Scope);
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(1, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.FunctionCalling, meaningGroupList[0].Type);
			Assert.AreEqual("ShareLibStepFailJudgeVal(&varInStep,&rctasw_can_mng_tbl,&varOutStep)", meaningGroupList[0].TextStr);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_9()
		{
			FUNC_CONTEXT func_ctx = new FUNC_CONTEXT();
			DEDUCER_CONTEXT dCtx = new DEDUCER_CONTEXT();
			for (int i = 0; i < 8; i++)
			{
				C_DEDUCER.StatementProc(root.ChildNodeList[i], c_source_parse_info, func_ctx, dCtx);
			}
			// 第9句
			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = COMN_PROC.GetStatementStr(codeList, root.ChildNodeList[8].Scope);
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("pvOut.dt", meaningGroupList[0].TextStr);
			Assert.AreEqual(MeaningGroupType.AssignmentMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.LocalVariable, meaningGroupList[2].Type);
			Assert.AreEqual("varOutStep.pv", meaningGroupList[2].TextStr);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_10()
		{
			FUNC_CONTEXT func_ctx = new FUNC_CONTEXT();
			DEDUCER_CONTEXT dCtx = new DEDUCER_CONTEXT();
			for (int i = 0; i < 9; i++)
			{
				C_DEDUCER.StatementProc(root.ChildNodeList[i], c_source_parse_info, func_ctx, dCtx);
			}
			// 第9句
			List<string> codeList = c_source_parse_info.CodeList;
			string statementStr = COMN_PROC.GetStatementStr(codeList, root.ChildNodeList[9].Scope);
			List<STATEMENT_COMPONENT> componentList = COMN_PROC.GetComponents(statementStr, c_source_parse_info);
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups(componentList, c_source_parse_info, func_ctx);

			Assert.AreEqual(3, meaningGroupList.Count);
			Assert.AreEqual(MeaningGroupType.GlobalVariable, meaningGroupList[0].Type);
			Assert.AreEqual("(Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct)->value", meaningGroupList[0].TextStr);
			Assert.AreEqual(MeaningGroupType.AssignmentMark, meaningGroupList[1].Type);
			Assert.AreEqual(MeaningGroupType.Expression, meaningGroupList[2].Type);
			Assert.AreEqual("(*&pvOut)", meaningGroupList[2].TextStr);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_igon_IO()
		{
			//FUNC_CONTEXT func_ctx = C_DEDUCER.DeducerStart(root, c_source_parse_info);

			//Assert.AreEqual(3, func_ctx.LocalVarList.Count);

			//Assert.AreEqual("varInStep", func_ctx.LocalVarList[0].Name);
			//Assert.AreEqual("struct RTE_SWC_IN_TRCTA_C_USR_DEF_TYPE_18", func_ctx.LocalVarList[0].Type.Name);

			//Assert.AreEqual("varOutStep", func_ctx.LocalVarList[1].Name);
			//Assert.AreEqual("struct RTE_SWC_IN_TRCTA_C_USR_DEF_TYPE_22", func_ctx.LocalVarList[1].Type.Name);

			//Assert.AreEqual("pvOut", func_ctx.LocalVarList[2].Name);
			//Assert.AreEqual("struct RTE_SWC_IN_TRCTA_C_USR_DEF_TYPE_1", func_ctx.LocalVarList[2].Type.Name);

			//Assert.AreEqual(2, func_ctx.InputGlobalList.Count);
			//Assert.AreEqual("Rte_Inst_swc_in_trcta->*rbl_in_trcta_igon_rp_srIf_in_TRCTA_val.value", func_ctx.InputGlobalList[0].Text);
			//Assert.AreEqual("Rte_Inst_swc_in_trcta->*rbl_in_trcta_igon_rp_srIf_in_TRCTASts_val.value", func_ctx.InputGlobalList[1].Text);

			//Assert.AreEqual(1, func_ctx.CalledFunctionList.Count);
			//Assert.AreEqual("ShareLibStepFailJudgeVal", func_ctx.CalledFunctionList[0].FunctionName);
			//Assert.AreEqual(ActParaPassType.Reference,	func_ctx.CalledFunctionList[0].ActualParaInfoList[0].passType);
			//Assert.AreEqual(false,						func_ctx.CalledFunctionList[0].ActualParaInfoList[0].readOut);
			//Assert.AreEqual(ActParaPassType.Reference,	func_ctx.CalledFunctionList[0].ActualParaInfoList[1].passType);
			//Assert.AreEqual(false,						func_ctx.CalledFunctionList[0].ActualParaInfoList[1].readOut);
			//Assert.AreEqual(ActParaPassType.Reference,	func_ctx.CalledFunctionList[0].ActualParaInfoList[2].passType);
			//Assert.AreEqual(true,						func_ctx.CalledFunctionList[0].ActualParaInfoList[2].readOut);

			//Assert.AreEqual(1, func_ctx.OutputGlobalList.Count);
			//Assert.AreEqual("Rte_Inst_swc_in_trcta->rbl_in_trcta_igon_pp_srIf_pv_PvRctasw_struct->value", func_ctx.OutputGlobalList[0].Text);

			//Assert.AreEqual("ShareLibStepFailJudgeVal(&varInStep,&rctasw_can_mng_tbl,&varOutStep)", func_ctx.CalledFunctionList[0].MeaningGroup.TextStr);
		}
	}

	[TestClass]
	public class sym_rbl_in_trcta_initReset
	{
		static STATEMENT_NODE root;                                                     // 函数语句结构的根节点
		static FILE_PARSE_INFO c_source_parse_info;

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			string source_name = "..\\..\\..\\TestSrc\\swc_in_trcta\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_initReset";
			List<FILE_PARSE_INFO> parseInfoList = Common.UnitTest_GetSourceFileStructure(source_name);
			c_source_parse_info = Common.FindSrcParseInfoFromList(source_name, parseInfoList);
			root = C_FUNC_LOCATOR.FuncLocatorStart2(c_source_parse_info, function_name);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_initReset_IO()
		{
			//FUNC_CONTEXT analysisContext = C_DEDUCER.DeducerStart(root, c_source_parse_info);
			//Assert.AreEqual(1, analysisContext.OutputGlobalList.Count);
			//Assert.AreEqual("Rte_Inst_swc_in_trcta->rbl_in_trcta_initReset_pp_srIf_pv_PvRctasw_struct->value", analysisContext.OutputGlobalList[0].Text);
		}
	}

	[TestClass]
	public class sym_rbl_in_trcta_initWakeup
	{
		static STATEMENT_NODE root;                                                     // 函数语句结构的根节点
		static FILE_PARSE_INFO c_source_parse_info;

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			string source_name = "..\\..\\..\\TestSrc\\swc_in_trcta\\Rte_swc_in_trcta.c";
			string function_name = "sym_rbl_in_trcta_initWakeup";
			List<FILE_PARSE_INFO> parseInfoList = Common.UnitTest_GetSourceFileStructure(source_name);
			c_source_parse_info = Common.FindSrcParseInfoFromList(source_name, parseInfoList);
			root = C_FUNC_LOCATOR.FuncLocatorStart2(c_source_parse_info, function_name);
		}

		[TestMethod, TestCategory("Rte_swc_in_trcta.c")]
		public void sym_rbl_in_trcta_initWakeup_IO()
		{
			//FUNC_CONTEXT analysisContext = C_DEDUCER.DeducerStart(root, c_source_parse_info);
			//Assert.AreEqual(1, analysisContext.OutputGlobalList.Count);
			//Assert.AreEqual("Rte_Inst_swc_in_trcta->rbl_in_trcta_initWakeup_pp_srIf_pv_PvRctasw_struct->value", analysisContext.OutputGlobalList[0].Text);
		}
	}
}
