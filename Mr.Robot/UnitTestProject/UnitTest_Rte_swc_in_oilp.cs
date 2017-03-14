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
		static List<FileParseInfo> parseInfoList;
		static string source_name = "..\\..\\..\\TestSrc\\swc_in_oilp\\Rte_swc_in_oilp.c";

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			parseInfoList = Common.UnitTest_SourceFileProcess2(source_name);
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void sym_rbl_in_oilp()
		{
			FileParseInfo c_source_parse_info = Common.UnitTest_GetFuncParseResult(source_name, "sym_rbl_in_oilp", parseInfoList, ref root);

			Assert.AreEqual(3, root.childList.Count);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[0].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[1].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[2].Type);

			FuncAnalysisContext func_ctx = StatementAnalysis.FunctionStatementsAnalysis(root, c_source_parse_info);
			Assert.AreEqual(3, func_ctx.CalledFunctionList.Count);
			Assert.AreEqual("makeEngOnOff3s()", func_ctx.CalledFunctionList[0].MeaningGroup.Text);
			Assert.AreEqual("makeIgv()", func_ctx.CalledFunctionList[1].MeaningGroup.Text);
			Assert.AreEqual("makeOilpAd()", func_ctx.CalledFunctionList[2].MeaningGroup.Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void sym_rbl_in_oilp_initReset()
		{
			FileParseInfo c_source_parse_info = Common.UnitTest_GetFuncParseResult(source_name, "sym_rbl_in_oilp_initReset", parseInfoList, ref root);

			FuncAnalysisContext func_ctx = StatementAnalysis.FunctionStatementsAnalysis(root, c_source_parse_info);
			Assert.AreEqual(1, func_ctx.InputGlobalList.Count);
			Assert.AreEqual("pvEngOnOff3s", func_ctx.InputGlobalList[0].Name);
			Assert.AreEqual(1, func_ctx.CalledFunctionList.Count);
			Assert.AreEqual("initPvOilp()", func_ctx.CalledFunctionList[0].MeaningGroup.Text);
			Assert.AreEqual(2, func_ctx.OutputGlobalList.Count);
			//Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initReset_pp_srIf_pv_PvEngOnOff3s_struct)->value", func_ctx.OutputGlobalList[0].MeanningGroup.Text);
			//Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initReset_pp_srIf_pv_PvOilpAd_struct)->value", func_ctx.OutputGlobalList[1].MeanningGroup.Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void sym_rbl_in_oilp_initWakeup()
		{
			FileParseInfo c_source_parse_info = Common.UnitTest_GetFuncParseResult(source_name, "sym_rbl_in_oilp_initWakeup", parseInfoList, ref root);

			FuncAnalysisContext func_ctx = StatementAnalysis.FunctionStatementsAnalysis(root, c_source_parse_info);
			Assert.AreEqual(1, func_ctx.InputGlobalList.Count);
			Assert.AreEqual("pvEngOnOff3s", func_ctx.InputGlobalList[0].Name);
			Assert.AreEqual(1, func_ctx.CalledFunctionList.Count);
			Assert.AreEqual("initPvOilp()", func_ctx.CalledFunctionList[0].MeaningGroup.Text);
			Assert.AreEqual(2, func_ctx.OutputGlobalList.Count);
			//Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initWakeup_pp_srIf_pv_PvEngOnOff3s_struct)->value", func_ctx.OutputGlobalList[0].MeanningGroup.Text);
			//Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initWakeup_pp_srIf_pv_PvOilpAd_struct)->value", func_ctx.OutputGlobalList[1].MeanningGroup.Text);
		}
	}
}
