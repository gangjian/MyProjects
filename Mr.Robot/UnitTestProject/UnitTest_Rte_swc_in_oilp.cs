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
		public void Rte_swc_in_oilp_Global()
		{
			Assert.AreEqual(1, parseInfoList.First().GlobalDeclareList.Count);
			Assert.AreEqual(VAR_TYPE_CATEGORY.POINTER, parseInfoList.First().GlobalDeclareList[0].VarTypeCategory);
			Assert.AreEqual("extern const struct Rte_CDS_swc_in_oilp * const", parseInfoList.First().GlobalDeclareList[0].Type.GetFullName());
			Assert.AreEqual("Rte_Inst_swc_in_oilp", parseInfoList.First().GlobalDeclareList[0].Name);

			Assert.AreEqual(7, parseInfoList.First().GlobalDefineList.Count);
			// static uint8 igvAdSts;
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, parseInfoList.First().GlobalDefineList[0].VarTypeCategory);
			Assert.AreEqual("static unsigned char", parseInfoList.First().GlobalDefineList[0].Type.GetFullName());
			Assert.AreEqual("igvAdSts", parseInfoList.First().GlobalDefineList[0].Name);
			// static uint8 oilpAdIgAdAvrgSts;
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, parseInfoList.First().GlobalDefineList[1].VarTypeCategory);
			Assert.AreEqual("static unsigned char", parseInfoList.First().GlobalDefineList[1].Type.GetFullName());
			Assert.AreEqual("oilpAdIgAdAvrgSts", parseInfoList.First().GlobalDefineList[1].Name);
			// static uint8 oilpAdIgAdAccumuCnt;
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, parseInfoList.First().GlobalDefineList[2].VarTypeCategory);
			Assert.AreEqual("static unsigned char", parseInfoList.First().GlobalDefineList[2].Type.GetFullName());
			Assert.AreEqual("oilpAdIgAdAccumuCnt", parseInfoList.First().GlobalDefineList[2].Name);
			// static uint16 igvAd;
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, parseInfoList.First().GlobalDefineList[3].VarTypeCategory);
			Assert.AreEqual("static unsigned short", parseInfoList.First().GlobalDefineList[3].Type.GetFullName());
			Assert.AreEqual("igvAd", parseInfoList.First().GlobalDefineList[3].Name);
			// static uint16 oilpAdPreIgAdAvrg;
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, parseInfoList.First().GlobalDefineList[4].VarTypeCategory);
			Assert.AreEqual("static unsigned short", parseInfoList.First().GlobalDefineList[4].Type.GetFullName());
			Assert.AreEqual("oilpAdPreIgAdAvrg", parseInfoList.First().GlobalDefineList[4].Name);
			// static uint16 oilpAdIgAdAvrgBuf[OILPAD_IGAD_AVRG_CNT];
			Assert.AreEqual(VAR_TYPE_CATEGORY.ARRAY, parseInfoList.First().GlobalDefineList[5].VarTypeCategory);
			Assert.AreEqual("static unsigned short", parseInfoList.First().GlobalDefineList[5].Type.GetFullName());
			Assert.AreEqual("oilpAdIgAdAvrgBuf", parseInfoList.First().GlobalDefineList[5].Name);
			Assert.AreEqual(4, parseInfoList.First().GlobalDefineList[5].MemberList.Count);
			Assert.AreEqual(0, parseInfoList.First().GlobalDefineList[5].MemberList[0].Value);
			Assert.AreEqual(0, parseInfoList.First().GlobalDefineList[5].MemberList[1].Value);
			Assert.AreEqual(0, parseInfoList.First().GlobalDefineList[5].MemberList[2].Value);
			Assert.AreEqual(0, parseInfoList.First().GlobalDefineList[5].MemberList[3].Value);
			// static pvU1	  pvEngOnOff3s;
			Assert.AreEqual(VAR_TYPE_CATEGORY.USR_DEF_TYPE, parseInfoList.First().GlobalDefineList[6].VarTypeCategory);
			//Assert.AreEqual("pvU1", parseInfoList.First().GlobalDefineList[6].Type.GetFullName());
			Assert.AreEqual("pvEngOnOff3s", parseInfoList.First().GlobalDefineList[6].Name);
			Assert.AreEqual(2, parseInfoList.First().GlobalDefineList[6].MemberList.Count);
			Assert.AreEqual("unsigned char", parseInfoList.First().GlobalDefineList[6].MemberList[0].Type.GetFullName());
			Assert.AreEqual("dt", parseInfoList.First().GlobalDefineList[6].MemberList[0].Name);
			Assert.AreEqual("unsigned char", parseInfoList.First().GlobalDefineList[6].MemberList[1].Type.GetFullName());
			Assert.AreEqual("sts", parseInfoList.First().GlobalDefineList[6].MemberList[1].Name);
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
			Assert.AreEqual("pvEngOnOff3s", func_ctx.InputGlobalList[0].VarLevelList[0].Name);
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
			Assert.AreEqual("pvEngOnOff3s", func_ctx.InputGlobalList[0].VarLevelList[0].Name);
			Assert.AreEqual(1, func_ctx.CalledFunctionList.Count);
			Assert.AreEqual("initPvOilp()", func_ctx.CalledFunctionList[0].MeaningGroup.Text);
			Assert.AreEqual(2, func_ctx.OutputGlobalList.Count);
			//Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initWakeup_pp_srIf_pv_PvEngOnOff3s_struct)->value", func_ctx.OutputGlobalList[0].MeanningGroup.Text);
			//Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initWakeup_pp_srIf_pv_PvOilpAd_struct)->value", func_ctx.OutputGlobalList[1].MeanningGroup.Text);
		}
	}
}
