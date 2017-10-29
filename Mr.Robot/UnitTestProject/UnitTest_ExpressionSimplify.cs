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
	public class UnitTest_ExpressionSimplify
	{
		static List<FILE_PARSE_INFO> m_ParseInfoList;
		static string m_SourceName = "..\\..\\..\\TestSrc\\DummyTest.c";

		[ClassInitialize]
		public static void TestClassSetup(TestContext ctx)
		{
			m_ParseInfoList = Common.UnitTest_SourceFileProcess2(m_SourceName);
		}

		[TestMethod, TestCategory("ExpressionSimplify")]
		public void TestMethod_1()
		{
			Assert.AreNotEqual(0, m_ParseInfoList.Count);
			C_DEDUCER deducer = new C_DEDUCER(m_ParseInfoList[0], "Test_Func_1");
			deducer.DeducerStart2();
		}
	}
}
