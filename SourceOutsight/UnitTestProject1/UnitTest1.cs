using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SourceOutsight;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			string prj_dir = "C:\\Users\\GangJian\\03_work\\99_Data\\MTbot_TestData\\Honda18HMI_soft\\software\\src\\AnlysSprtTask";
			SO_Project so_prj = new SO_Project(prj_dir);
		}
	}
}
