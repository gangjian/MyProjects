using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mr.Robot;
using Mr.Robot.MacroSwitchAnalyser;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTest_Honda_18HMI
	{
		static FileParseInfo SourceParseInfo = null;
		[ClassInitialize]
		public static void TestSetup(TestContext ctx)
		{
			List<string> source_list = new List<string>();
			List<string> header_list = new List<string>();
			// 取得所有源文件和头文件列表
			IOProcess.GetAllCCodeFiles("C:\\TestData\\Honda18HMI_soft", ref source_list, ref header_list);
			// 解析指定的源文件,并取得解析结果
			List<string> csfList = new List<string>();
			csfList.Add("C:\\TestData\\Honda18HMI_soft\\software\\src\\AnlysSprtTask\\AnlysSprtTASK.c");
			CCodeAnalyser CAnalyser = new CCodeAnalyser(csfList, header_list);
			List<FileParseInfo> parseInfoList = CAnalyser.CFileListProc();
			SourceParseInfo = parseInfoList[0];
		}

		[TestMethod, TestCategory("Honda_18HMI")]
		public void Test_USE_DIAG()
		{
			MacroSwitchAnalyser macroAnalyser = new MacroSwitchAnalyser(SourceParseInfo);
			macroAnalyser.ProcessStart();
		}
	}
}
