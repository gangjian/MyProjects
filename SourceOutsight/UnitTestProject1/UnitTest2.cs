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
			string tag_str = "swc_in_oilp_STOP_SEC_CODE";
			SO_Project so_prj = new SO_Project(prj_dir);
			List<SearchTagResult> result_list = so_prj.SearchTag(tag_str);
			foreach (var result in result_list)
			{
				foreach (var info in result.TagInfoList)
				{
					Trace.WriteLine(tag_str + "," + result.Path + "," + "line: "
										+ info.Position.Row.ToString() + ", Col: "
										+ info.Position.Col.ToString());
				}
			}
		}
	}
}
