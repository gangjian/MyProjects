using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mr.Robot.MacroSwitchAnalyser;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTestResultCompare
	{
		[TestMethod]
		public void TestMethod1()
		{
			ResultCompare.DetailCompareResult cmp_rslt
									= ResultCompare.DetailCsvCompare("c:\\1.csv", "c:\\2.csv");
		}
	}
}
