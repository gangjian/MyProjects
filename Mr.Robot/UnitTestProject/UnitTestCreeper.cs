using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using System.IO;
using Mr.Robot;
using Mr.Robot.Creeper;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTestCreeper
	{
		[TestMethod]
		[TestCategory("Creeper")]
		public void TestMethod1()
		{
			string path = "..\\..\\..\\TestSrc\\swc_in_oilp\\Rte_swc_in_oilp.c";
			Assert.IsTrue(File.Exists(path));
			List<string> code_list = File.ReadAllLines(path).ToList();
			CodePosition pos = new CodePosition(0, 0);
			while (true)
			{
				Mr.Robot.Creeper.CodeSymbol symbol = Mr.Robot.Creeper.Common.GetNextSymbol(code_list, ref pos);
				if (string.IsNullOrEmpty(symbol.SymbolStr))
				{
					break;
				}
			}
		}
	}
}
