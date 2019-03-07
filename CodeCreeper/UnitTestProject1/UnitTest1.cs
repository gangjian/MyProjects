using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CodeCreeper;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			string prj_dir = "C:\\Users\\GangJian\\03_work\\github\\MyProjects\\Mr.Robot\\TestSrc\\swc_in_oilp";
			string file_name = "ut_dummy.h";
			CodeProjectInfo prj_info = new CodeProjectInfo(prj_dir);
			Creeper code_creeper = new Creeper(prj_info);
			code_creeper.CreepFile(prj_dir + "\\" + file_name);
			var print_list = code_creeper.GetSyntaxTreePrintList();
		}

		[TestMethod]
		public void TestExpression()
		{
			CreeperContext ctx = new CreeperContext();
			Expression exp = new Expression(ctx);
			exp.Evaluate("(a + b) * c - d");
		}
	}
}
