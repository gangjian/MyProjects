﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SourceOutsight;
using System.Diagnostics;

namespace UnitTestProject1
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod_Toyota_RearCon_HMI_r11303()
		{
			string prj_dir = "C:\\Users\\GangJian\\03_work\\99_Data\\MTbot_TestData\\Toyota_RearCon_HMI_r11303";
			Stopwatch sw = new Stopwatch();
			sw.Start();
			SO_Project so_prj = new SO_Project(prj_dir);
			sw.Stop();
			Trace.WriteLine("Total Elapsed Time: " + sw.Elapsed.ToString());
		}

		[TestMethod]
		public void TestMethod_Toyota_RearCon_SYS_r10190()
		{
			string prj_dir = "C:\\Users\\GangJian\\03_work\\99_Data\\MTbot_TestData\\Toyota_RearCon_SYS_r10190";
			Stopwatch sw = new Stopwatch();
			sw.Start();
			SO_Project so_prj = new SO_Project(prj_dir);
			sw.Stop();
			Trace.WriteLine("Total Elapsed Time: " + sw.Elapsed.ToString());
		}

		[TestMethod]
		public void TestMethod_Honda18HMI_soft()
		{
			string prj_dir = "C:\\Users\\GangJian\\03_work\\99_Data\\MTbot_TestData\\Honda18HMI_soft";
			Stopwatch sw = new Stopwatch();
			sw.Start();
			SO_Project so_prj = new SO_Project(prj_dir);
			sw.Stop();
			Trace.WriteLine("Total Elapsed Time: " + sw.Elapsed.ToString());
		}

		[TestMethod]
		public void TestMethod_LowDA_三回目()
		{
			string prj_dir = "C:\\Users\\GangJian\\03_work\\99_Data\\MTbot_TestData\\LowDA_三回目";
			Stopwatch sw = new Stopwatch();
			sw.Start();
			SO_Project so_prj = new SO_Project(prj_dir);
			sw.Stop();
			Trace.WriteLine("Total Elapsed Time: " + sw.Elapsed.ToString());
		}

		[TestMethod]
		public void TestMethod_LowDA_四回目()
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
