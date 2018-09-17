using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mr.Robot;
using Mr.Robot.Creeper;
using System.Diagnostics;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTestCCommentsMarker
	{
		[TestMethod]
		public void TestCCommentsMarker_1()
		{
			//string dir = "..\\..\\..\\TestSrc";
			string dir = "C:\\Users\\GangJian\\03_work\\99_Data\\MTbot_TestData";
			// 取得文件列表
			int count = 0;
			List<string> file_list = GetFileList(dir);
			foreach (string f in file_list)
			{
				List<string> code_list = File.ReadAllLines(f).ToList();
				List<string> list1 = CCommentsMarker.RemoveComments2(code_list);
				List<string> list2 = Mr.Robot.COMN_PROC.RemoveComments(f);
				// 比较内容跟直接调用删除注释方法的效果一样
				Assert.IsTrue(ListCompare(list1, list2));
				count += 1;
				Console.WriteLine(count.ToString());
			}
		}

		List<string> GetFileList(string dir)
		{
			Trace.Assert(!string.IsNullOrEmpty(dir) && Directory.Exists(dir));
			List<string> ret_list = new List<string>();
			DirectoryInfo di = new DirectoryInfo(dir);
			FileInfo[] files = di.GetFiles();
			foreach (var f in files)
			{
				if (f.Extension.ToString().ToLower().Equals(".c")
					|| f.Extension.ToString().ToLower().Equals(".h"))
				{
					ret_list.Add(f.FullName);
				}
			}
			DirectoryInfo[] dirs = di.GetDirectories();
			foreach (var d in dirs)
			{
				ret_list.AddRange(GetFileList(d.FullName));
			}
			return ret_list;
		}

		bool ListCompare(List<string> list1, List<string> list2)
		{
			int count = list1.Count;
			if (list1.Count != list2.Count)
			{
				if (list2.Count < list1.Count)
				{
					count = list2.Count;
				}
			}
			for (int i = 0; i < count; i++)
			{
				if (!list1[i].Equals(list2[i]))
				{
					return false;
				}
			}
			if (list1.Count == list2.Count)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
