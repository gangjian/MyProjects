using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SourceOutsight;
using System.Diagnostics;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			string prj_dir = "C:\\Users\\GangJian\\03_work\\99_Data\\MTbot_TestData\\LowDA_四回目";
			Stopwatch sw = new Stopwatch();
			sw.Start();
			SO_Project so_prj = new SO_Project(prj_dir);
			sw.Stop();
			Trace.WriteLine("Total Elapsed Time: " + sw.Elapsed.ToString());
		}
	}
}
