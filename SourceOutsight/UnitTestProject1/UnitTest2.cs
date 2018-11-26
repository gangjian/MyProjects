using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Collections.Generic;
using SourceOutsight;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest2
	{
		[TestMethod]
		public void TestMethod1()
		{
			string prj_dir = "C:\\Users\\GangJian\\03_work\\github\\MyProjects\\Mr.Robot\\TestSrc\\swc_in_oilp";
			SO_Project so_prj = new SO_Project(prj_dir);
			List<string> result_list = so_prj.SearchTag("swc_in_oilp_STOP_SEC_CODE");
			Trace.WriteLine(result_list.ToString());
		}
	}
}
