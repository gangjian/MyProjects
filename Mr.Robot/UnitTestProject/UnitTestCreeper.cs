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
			string path = "..\\..\\..\\TestSrc\\swc_in_oilp";
			CCodeProbe probe_obj = new CCodeProbe(path);
			probe_obj.ProbeStart();
		}
	}
}
