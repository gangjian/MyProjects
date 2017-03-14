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
			Assert.IsTrue(BasicTypeProc.IsBasicTypeName("unsigned int"));

			Assert.IsTrue(BasicTypeProc.IsBasicTypeName("float"));

			Assert.IsTrue(BasicTypeProc.IsBasicTypeName("double"));

			Assert.IsTrue(BasicTypeProc.IsBasicTypeName("long"));

			Assert.IsTrue(BasicTypeProc.IsBasicTypeName("signed char"));

			Assert.IsTrue(BasicTypeProc.IsBasicTypeName("short"));

			Assert.IsFalse(BasicTypeProc.IsBasicTypeName("unsined float"));
		}
	}
}
