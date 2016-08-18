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
		public void sym_rbl_in_oilp_IO()
		{
			CCodeParseResult c_source_file_parse_result = Common.UnitTest_GetFuncParseResult(source_name, "sym_rbl_in_oilp", parseResultList, ref root);

			Assert.AreEqual(3, root.childList.Count);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[0].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[1].Type);
			Assert.AreEqual(StatementNodeType.Simple, root.childList[2].Type);

			AnalysisContext analysisContext = CCodeAnalyser.FunctionStatementsAnalysis(root, c_source_file_parse_result);
			Assert.AreEqual(3, analysisContext.calledFunctionList.Count);
			Assert.AreEqual("makeEngOnOff3s()", analysisContext.calledFunctionList[0].Text);
			Assert.AreEqual("makeIgv()", analysisContext.calledFunctionList[1].Text);
			Assert.AreEqual("makeOilpAd()", analysisContext.calledFunctionList[2].Text);
		}

	}
}
