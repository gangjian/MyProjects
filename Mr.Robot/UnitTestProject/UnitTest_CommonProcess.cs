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
			Assert.IsTrue(CommonProcess.IsBasicTypeName("unsigned int"));

			Assert.IsTrue(CommonProcess.IsBasicTypeName("float"));

			Assert.IsTrue(CommonProcess.IsBasicTypeName("double"));

			Assert.IsTrue(CommonProcess.IsBasicTypeName("long"));

			Assert.IsTrue(CommonProcess.IsBasicTypeName("signed char"));

			Assert.IsTrue(CommonProcess.IsBasicTypeName("short"));

			Assert.IsFalse(CommonProcess.IsBasicTypeName("unsined float"));
		}
	}
}
