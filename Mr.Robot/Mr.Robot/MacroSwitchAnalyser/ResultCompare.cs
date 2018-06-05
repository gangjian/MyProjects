using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Mr.Robot.MacroSwitchAnalyser
{
	class ResultCompare
	{
		public static void DetailCsvCompare(string csv_1, string csv_2)
		{
			List<MacroDetail> detail_list1 = LoadCsvDetailList(csv_1);
			List<MacroDetail> detail_list2 = LoadCsvDetailList(csv_2);

			for (int i = 0; i < detail_list1.Count; i++)
			{
				MacroDetail md_1 = detail_list1[i];
				for (int j = 0; j < detail_list2.Count; j++)
				{
					MacroDetail md_2 = detail_list2[j];
					MacroDetail.CmpResult rslt = md_1.Compare(md_2);
					if (MacroDetail.CmpResult.Equal == rslt)
					{
						detail_list2.RemoveAt(j);
						break;
					}
					else if (MacroDetail.CmpResult.NotEqual == rslt)
					{
						
					}
				}
			}
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

		class ComparePair
		{
			MacroDetail FirstObj = null;
			MacroDetail SecondObj = null;
		}
		class MacroDetail
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
					return CmpResult.NotSame;
				}
				else
				{
					if (this.DefStr.Equals(another.DefStr))
					{
						return CmpResult.Equal;
					}
					else
					{
						return CmpResult.NotEqual;
					}
				}
			}

			public enum CmpResult
			{
				NotSame,
				NotEqual,
				Equal,
			}
		}
	}
}
