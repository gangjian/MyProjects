using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Mr.Robot;
using Mr.Robot.CDeducer;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTest_Rte_swc_in_oilp
	{
		static STATEMENT_NODE m_Root;                                                   // 函数语句结构的根节点
		static List<FILE_PARSE_INFO> m_ParseInfoList;
		static string m_SourceName = "..\\..\\..\\TestSrc\\swc_in_oilp\\Rte_swc_in_oilp.c";

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			m_ParseInfoList = Common.UnitTest_SourceFileProcess2(m_SourceName);
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void Rte_swc_in_oilp_Global()
		{
			Assert.AreEqual(1, m_ParseInfoList.First().GlobalDeclareList.Count);
			Assert.AreEqual(VAR_TYPE_CATEGORY.POINTER, m_ParseInfoList.First().GlobalDeclareList[0].VarTypeCategory);
			Assert.AreEqual("extern const struct Rte_CDS_swc_in_oilp * const", m_ParseInfoList.First().GlobalDeclareList[0].Type.GetFullName());
			Assert.AreEqual("Rte_Inst_swc_in_oilp", m_ParseInfoList.First().GlobalDeclareList[0].Name);

			Assert.AreEqual(7, m_ParseInfoList.First().GlobalDefineList.Count);
			// static uint8 igvAdSts;
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, m_ParseInfoList.First().GlobalDefineList[0].VarTypeCategory);
			Assert.AreEqual("static unsigned char", m_ParseInfoList.First().GlobalDefineList[0].Type.GetFullName());
			Assert.AreEqual("igvAdSts", m_ParseInfoList.First().GlobalDefineList[0].Name);
			// static uint8 oilpAdIgAdAvrgSts;
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, m_ParseInfoList.First().GlobalDefineList[1].VarTypeCategory);
			Assert.AreEqual("static unsigned char", m_ParseInfoList.First().GlobalDefineList[1].Type.GetFullName());
			Assert.AreEqual("oilpAdIgAdAvrgSts", m_ParseInfoList.First().GlobalDefineList[1].Name);
			// static uint8 oilpAdIgAdAccumuCnt;
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, m_ParseInfoList.First().GlobalDefineList[2].VarTypeCategory);
			Assert.AreEqual("static unsigned char", m_ParseInfoList.First().GlobalDefineList[2].Type.GetFullName());
			Assert.AreEqual("oilpAdIgAdAccumuCnt", m_ParseInfoList.First().GlobalDefineList[2].Name);
			// static uint16 igvAd;
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, m_ParseInfoList.First().GlobalDefineList[3].VarTypeCategory);
			Assert.AreEqual("static unsigned short", m_ParseInfoList.First().GlobalDefineList[3].Type.GetFullName());
			Assert.AreEqual("igvAd", m_ParseInfoList.First().GlobalDefineList[3].Name);
			// static uint16 oilpAdPreIgAdAvrg;
			Assert.AreEqual(VAR_TYPE_CATEGORY.BASIC, m_ParseInfoList.First().GlobalDefineList[4].VarTypeCategory);
			Assert.AreEqual("static unsigned short", m_ParseInfoList.First().GlobalDefineList[4].Type.GetFullName());
			Assert.AreEqual("oilpAdPreIgAdAvrg", m_ParseInfoList.First().GlobalDefineList[4].Name);
			// static uint16 oilpAdIgAdAvrgBuf[OILPAD_IGAD_AVRG_CNT];
			Assert.AreEqual(VAR_TYPE_CATEGORY.ARRAY, m_ParseInfoList.First().GlobalDefineList[5].VarTypeCategory);
			Assert.AreEqual("static unsigned short", m_ParseInfoList.First().GlobalDefineList[5].Type.GetFullName());
			Assert.AreEqual("oilpAdIgAdAvrgBuf", m_ParseInfoList.First().GlobalDefineList[5].Name);
			Assert.AreEqual(4, m_ParseInfoList.First().GlobalDefineList[5].MemberList.Count);
			Assert.AreEqual(0, m_ParseInfoList.First().GlobalDefineList[5].MemberList[0].Value);
			Assert.AreEqual(0, m_ParseInfoList.First().GlobalDefineList[5].MemberList[1].Value);
			Assert.AreEqual(0, m_ParseInfoList.First().GlobalDefineList[5].MemberList[2].Value);
			Assert.AreEqual(0, m_ParseInfoList.First().GlobalDefineList[5].MemberList[3].Value);
			// static pvU1	  pvEngOnOff3s;
			Assert.AreEqual(VAR_TYPE_CATEGORY.USR_DEF_TYPE, m_ParseInfoList.First().GlobalDefineList[6].VarTypeCategory);
			//Assert.AreEqual("pvU1", parseInfoList.First().GlobalDefineList[6].Type.GetFullName());
			Assert.AreEqual("pvEngOnOff3s", m_ParseInfoList.First().GlobalDefineList[6].Name);
			Assert.AreEqual(2, m_ParseInfoList.First().GlobalDefineList[6].MemberList.Count);
			Assert.AreEqual("unsigned char", m_ParseInfoList.First().GlobalDefineList[6].MemberList[0].Type.GetFullName());
			Assert.AreEqual("dt", m_ParseInfoList.First().GlobalDefineList[6].MemberList[0].Name);
			Assert.AreEqual("unsigned char", m_ParseInfoList.First().GlobalDefineList[6].MemberList[1].Type.GetFullName());
			Assert.AreEqual("sts", m_ParseInfoList.First().GlobalDefineList[6].MemberList[1].Name);
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void sym_rbl_in_oilp()
		{
			FILE_PARSE_INFO c_source_parse_info = Common.FindSrcParseInfoFromList(m_SourceName, m_ParseInfoList);
			m_Root = C_FUNC_LOCATOR.FuncLocatorStart2(c_source_parse_info, "sym_rbl_in_oilp");

			Assert.AreEqual(3, m_Root.ChildNodeList.Count);
			Assert.AreEqual(STATEMENT_TYPE.Simple, m_Root.ChildNodeList[0].Type);
			Assert.AreEqual(STATEMENT_TYPE.Simple, m_Root.ChildNodeList[1].Type);
			Assert.AreEqual(STATEMENT_TYPE.Simple, m_Root.ChildNodeList[2].Type);

			//FUNC_CONTEXT func_ctx = C_DEDUCER.DeducerStart(m_Root, c_source_parse_info);
			//Assert.AreEqual(3, func_ctx.CalledFunctionList.Count);
			//Assert.AreEqual("makeEngOnOff3s()", func_ctx.CalledFunctionList[0].MeaningGroup.TextStr);
			//Assert.AreEqual("makeIgv()", func_ctx.CalledFunctionList[1].MeaningGroup.TextStr);
			//Assert.AreEqual("makeOilpAd()", func_ctx.CalledFunctionList[2].MeaningGroup.TextStr);
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void sym_rbl_in_oilp_initReset()
		{
			FILE_PARSE_INFO c_source_parse_info = Common.FindSrcParseInfoFromList(m_SourceName, m_ParseInfoList);
			m_Root = C_FUNC_LOCATOR.FuncLocatorStart2(c_source_parse_info, "sym_rbl_in_oilp_initReset");

			//FUNC_CONTEXT func_ctx = C_DEDUCER.DeducerStart(m_Root, c_source_parse_info);
			//Assert.AreEqual(1, func_ctx.InputGlobalList.Count);
			//Assert.AreEqual("pvEngOnOff3s", func_ctx.InputGlobalList[0].VarLevelList[0].Name);
			//Assert.AreEqual(1, func_ctx.CalledFunctionList.Count);
			//Assert.AreEqual("initPvOilp()", func_ctx.CalledFunctionList[0].MeaningGroup.TextStr);
			//Assert.AreEqual(2, func_ctx.OutputGlobalList.Count);
			//Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initReset_pp_srIf_pv_PvEngOnOff3s_struct)->value", func_ctx.OutputGlobalList[0].MeanningGroup.Text);
			//Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initReset_pp_srIf_pv_PvOilpAd_struct)->value", func_ctx.OutputGlobalList[1].MeanningGroup.Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void sym_rbl_in_oilp_initWakeup()
		{
			FILE_PARSE_INFO src_parse_info = Common.FindSrcParseInfoFromList(m_SourceName, m_ParseInfoList);
			m_Root = C_FUNC_LOCATOR.FuncLocatorStart2(src_parse_info, "sym_rbl_in_oilp_initWakeup");

			//FUNC_CONTEXT func_ctx = C_DEDUCER.DeducerStart(m_Root, c_source_parse_info);
			//Assert.AreEqual(1, func_ctx.InputGlobalList.Count);
			//Assert.AreEqual("pvEngOnOff3s", func_ctx.InputGlobalList[0].VarLevelList[0].Name);
			//Assert.AreEqual(1, func_ctx.CalledFunctionList.Count);
			//Assert.AreEqual("initPvOilp()", func_ctx.CalledFunctionList[0].MeaningGroup.TextStr);
			//Assert.AreEqual(2, func_ctx.OutputGlobalList.Count);
			//Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initWakeup_pp_srIf_pv_PvEngOnOff3s_struct)->value", func_ctx.OutputGlobalList[0].MeanningGroup.Text);
			//Assert.AreEqual("(Rte_Inst_swc_in_oilp->rbl_in_oilp_initWakeup_pp_srIf_pv_PvOilpAd_struct)->value", func_ctx.OutputGlobalList[1].MeanningGroup.Text);
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void Test_calcIgAdAvrgData()
		{
			FILE_PARSE_INFO src_parse_info = Common.FindSrcParseInfoFromList(m_SourceName, m_ParseInfoList);
			C_DEDUCER deducer = new C_DEDUCER(src_parse_info, "calcIgAdAvrgData");
			deducer.DeducerStart2();
		}

		[TestMethod, TestCategory("Rte_swc_in_oilp.c")]
		public void TestGetStatementNode()
		{
			FILE_PARSE_INFO src_parse_info = Common.FindSrcParseInfoFromList(m_SourceName, m_ParseInfoList);
			STATEMENT_NODE func_root = C_FUNC_LOCATOR.FuncLocatorStart2(src_parse_info, "calcIgAdAvrgData");
			STATEMENT_NODE node = func_root.GetOtherNode("7,-2,2,-1,6,-1,1");
			Assert.AreNotEqual(null, node);
			node = node.GetOtherNode("7,-2,2,-1,2,1");
			Assert.AreNotEqual(null, node);
		}

	}
}
