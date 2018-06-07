using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Mr.Robot.MacroSwitchAnalyser
{
	public class ResultCompare
	{
		public static DetailCompareResult DetailCsvCompare(string csv_1, string csv_2)
		{
			List<MacroDetail> detail_list1 = LoadCsvDetailList(csv_1);					// 第一个csv文件里的所有记录列表
			List<MacroDetail> detail_list2 = LoadCsvDetailList(csv_2);					// 第二个csv文件里的所有记录列表
			DetailCompareResult cmp_rslt = new DetailCompareResult();

			for (int i = 0; i < detail_list1.Count; i++)
			{
				MacroDetail md_1 = detail_list1[i];
				int unequal_idx = -1;
				bool equal_flg = false;
				for (int j = 0; j < detail_list2.Count; j++)
				{
					MacroDetail md_2 = detail_list2[j];
					MacroDetail.CmpResult rslt = md_1.Compare(md_2);
					if (MacroDetail.CmpResult.DefineEqual == rslt)
					{
						ComparePair pair = new ComparePair(md_1, md_2);
						cmp_rslt.EqualList.Add(pair);
						detail_list2.RemoveAt(j);
						equal_flg = true;
						break;
					}
					else if (MacroDetail.CmpResult.DefineUnEqual == rslt)
					{
						unequal_idx = j;
					}
				}
				if (equal_flg)
				{
					continue;
				}
				else if (-1 != unequal_idx)
				{
					ComparePair pair = new ComparePair(md_1, detail_list2[unequal_idx]);
					cmp_rslt.UnequalList.Add(pair);
					detail_list2.RemoveAt(unequal_idx);
				}
				else
				{
					cmp_rslt.SurplusList.Add(md_1);
				}
			}
			cmp_rslt.LackList.AddRange(detail_list2);
			return cmp_rslt;
		}

		public static void SummaryCsvCompare()
		{
		}

		static List<MacroDetail> LoadCsvDetailList(string csv_path)
		{
			Trace.Assert(!string.IsNullOrEmpty(csv_path) && File.Exists(csv_path));
			List<string> read_lines = File.ReadAllLines(csv_path).ToList();
			List<MacroDetail> ret_list = new List<MacroDetail>();
			foreach (var line in read_lines)
			{
				MacroDetail md = new MacroDetail();
				if (md.TryParse(line))
				{
					ret_list.Add(md);
				}
			}
			return ret_list;
		}

		public class DetailCompareResult
		{
			public List<ComparePair> EqualList = new List<ComparePair>();				// 比对一致的记录列表
			public List<ComparePair> UnequalList = new List<ComparePair>();				// 比对不一致的记录列表
			public List<MacroDetail> LackList = new List<MacroDetail>();				// 第一个csv文件里比第二个少的记录列表
			public List<MacroDetail> SurplusList = new List<MacroDetail>();				// 第一个csv文件里比第二个多的记录列表
		}

		public class ComparePair
		{
			public MacroDetail FirstObj = null;
			public MacroDetail SecondObj = null;

			public ComparePair(MacroDetail obj_1, MacroDetail obj_2)
			{
				this.FirstObj = obj_1;
				this.SecondObj = obj_2;
			}
		}

		public class MacroDetail
		{
			int idx = -1;
			string SrcName = null;
			int LineNum = -1;
			string ExpStr = null;
			string MacroName = null;
			string DefStr = null;

			public bool TryParse(string line_str)
			{
				string[] arr = line_str.Split(',');
				if (arr.Length >= 6
					&& int.TryParse(arr[0], out this.idx)
					&& arr[1].ToLower().EndsWith(".c")
					&& int.TryParse(arr[2], out this.LineNum))
				{
					this.SrcName = arr[1];
					this.ExpStr = arr[3];
					this.MacroName = arr[4];
					this.DefStr = arr[5];
					return true;
				}
				else
				{
					return false;
				}
			}

			public CmpResult Compare(MacroDetail another)
			{
				if (!this.SrcName.Equals(another.SrcName)
					|| this.LineNum != another.LineNum
					|| !this.ExpStr.Equals(another.ExpStr)
					|| !this.MacroName.Equals(another.MacroName))
				{
					return CmpResult.DifferentMacro;
				}
				else
				{
					if (this.DefStr.Equals(another.DefStr))
					{
						return CmpResult.DefineEqual;
					}
					else
					{
						return CmpResult.DefineUnEqual;
					}
				}
			}

			public enum CmpResult
			{
				DifferentMacro,				// 并非是同一条宏开关记录(根据文件名,行号,宏名)
				DefineUnEqual,				// 是同一条宏开关记录,但是宏定义值不相等
				DefineEqual,				// 同一条且定义值相等
			}
		}
	}
}
