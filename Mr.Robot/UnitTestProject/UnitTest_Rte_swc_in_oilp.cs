using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mr.Robot;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTest_Rte_swc_in_oilp
	{
		static StatementNode root;                                                      // 函数语句结构的根节点
		static List<CCodeParseResult> parseResultList;
		static string source_name = "..\\..\\..\\TestSrc\\swc_in_oilp\\Rte_swc_in_oilp.c";

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			parseResultList = Common.UnitTest_SourceFileProcess2(source_name);
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void sym_rbl_in_oilp()
		{
			CCodeParseResult c_source_file_parse_result = Common.UnitTest_GetFuncParseResult(source_name, "sym_rbl_in_oilp", parseResultList, ref root);

			Assert.AreEqual(3, root.childList.Count);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[0].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[1].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[2].Type);

			AnalysisContext ctx = CCodeAnalyser.FunctionStatementsAnalysis(root, c_source_file_parse_result);
			Assert.AreEqual(3, ctx.CalledFunctionList.Count);
			Assert.AreEqual("makeEngOnOff3s()", ctx.CalledFunctionList[0].MeaningGroup.Text);
			Assert.AreEqual("makeIgv()", ctx.CalledFunctionList[1].MeaningGroup.Text);
			Assert.AreEqual("makeOilpAd()", ctx.CalledFunctionList[2].MeaningGroup.Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void sym_rbl_in_oilp_initReset()
		{
			CCodeParseResult c_source_file_parse_result = Common.UnitTest_GetFuncParseResult(source_name, "sym_rbl_in_oilp_initReset", parseResultList, ref root);

			AnalysisContext ctx = CCodeAnalyser.FunctionStatementsAnalysis(root, c_source_file_parse_result);
			Assert.AreEqual(1, ctx.InputGlobalList.Count);
			Assert.AreEqual("pvEngOnOff3s", ctx.InputGlobalList[0].Name);
			Assert.AreEqual(1, ctx.CalledFunctionList.Count);
			Assert.AreEqual("initPvOilp()", ctx.CalledFunctionList[0].MeaningGroup.Text);
			Assert.AreEqual(2, ctx.OutputGlobalList.Count);
			Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initReset_pp_srIf_pv_PvEngOnOff3s_struct)->value", ctx.OutputGlobalList[0].MeanningGroup.Text);
			Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initReset_pp_srIf_pv_PvOilpAd_struct)->value", ctx.OutputGlobalList[1].MeanningGroup.Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void sym_rbl_in_oilp_initWakeup()
		{
			CCodeParseResult c_source_file_parse_result = Common.UnitTest_GetFuncParseResult(source_name, "sym_rbl_in_oilp_initWakeup", parseResultList, ref root);

			AnalysisContext ctx = CCodeAnalyser.FunctionStatementsAnalysis(root, c_source_file_parse_result);
			Assert.AreEqual(1, ctx.InputGlobalList.Count);
			Assert.AreEqual("pvEngOnOff3s", ctx.InputGlobalList[0].Name);
			Assert.AreEqual(1, ctx.CalledFunctionList.Count);
			Assert.AreEqual("initPvOilp()", ctx.CalledFunctionList[0].MeaningGroup.Text);
			Assert.AreEqual(2, ctx.OutputGlobalList.Count);
			Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initWakeup_pp_srIf_pv_PvEngOnOff3s_struct)->value", ctx.OutputGlobalList[0].MeanningGroup.Text);
			Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initWakeup_pp_srIf_pv_PvOilpAd_struct)->value", ctx.OutputGlobalList[1].MeanningGroup.Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void Test_CreateNewVarCtx()
		{
			CCodeParseResult c_source_file_parse_result = Common.UnitTest_GetFuncParseResult(source_name, "sym_rbl_in_oilp_initWakeup", parseResultList, ref root);

			VAR_CTX var_ctx = InOutAnalysis.CreateVarCtx("struct Rte_CDS_swc_in_oilp *", "Rte_Inst_swc_in_oilp");
			Assert.IsNotNull(var_ctx);
		}
	}
}
