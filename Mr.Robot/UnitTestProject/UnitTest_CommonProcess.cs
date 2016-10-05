using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mr.Robot;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTest_CommonProcess
	{
		[TestMethod, TestCategory("CommonProcess")]
		public void Test_IsBasicVarType()
		{
			Assert.IsTrue(CommonProcess.IsBasicVarType("unsigned int"));

			Assert.IsTrue(CommonProcess.IsBasicVarType("float"));

			Assert.IsTrue(CommonProcess.IsBasicVarType("double"));

			Assert.IsTrue(CommonProcess.IsBasicVarType("long"));

			Assert.IsTrue(CommonProcess.IsBasicVarType("signed char"));

			Assert.IsTrue(CommonProcess.IsBasicVarType("short"));

			Assert.IsFalse(CommonProcess.IsBasicVarType("unsined float"));
		}
	}
}
