using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mr.Robot.MacroSwitchAnalyser;
using System.IO;
using System.Diagnostics;

namespace MSAAutoRunner
{
	public class TestRunner
	{
		MACRO_SWITCH_ANALYSER _MacroSwitchAnalyser = null;
		Stopwatch _StopWatch = null;
		Queue<MSA_TEST_DATA> _TestDataQueue = null;
		List<TEST_RESULT> _TestResultList = new List<TEST_RESULT>();

		public TestRunner()
		{
			string script_path = AppDomain.CurrentDomain.BaseDirectory + "test_data.csv";
			if (!File.Exists(script_path))
			{
				return;
			}
			this._TestDataQueue = LoadTestDataQueue(script_path);
		}

		MSA_TEST_DATA _TestData = null;
		void StartMsaRunner(MSA_TEST_DATA test_data)
		{
			this._TestData = test_data;
			MSA_INPUT_PARA msa_para = GetMsaPara(test_data.RootFolder, test_data.TargetFolder);
			this._MacroSwitchAnalyser = new MACRO_SWITCH_ANALYSER(msa_para);
			this._MacroSwitchAnalyser.MSA_ReportProgressHandler += new EventHandler(UpdateProgress);
			this._StopWatch = new Stopwatch();
			this._StopWatch.Start();
			this._MacroSwitchAnalyser.ProcStart();
		}

		public static MSA_INPUT_PARA GetMsaPara(string root_path, string target_path)
		{
			List<string> src_list = new List<string>(), hd_list = new List<string>(),
						 mtpj_list = new List<string>(), mk_list = new List<string>();
			Mr.Robot.IOProcess.GetAllCCodeFiles(root_path, src_list, hd_list, mtpj_list, mk_list);
			MSA_INPUT_PARA ret_para = new MSA_INPUT_PARA(src_list, hd_list, mtpj_list, mk_list, 4);
			return ret_para;
		}

		const string TEMP_CSV_FILE = "temp.csv";
		void UpdateProgress(object obj, EventArgs e)
		{
			int cur_cnt = _MacroSwitchAnalyser.OutputResult.Progress.CurrentCount;
			int total_cnt = _MacroSwitchAnalyser.OutputResult.Progress.TotalCount;
			Console.WriteLine(string.Format("{0} : {1}/{2}",
								_StopWatch.Elapsed.ToString(), cur_cnt, total_cnt));
			if (cur_cnt == total_cnt)
			{
				// 完成
				this._StopWatch.Stop();
				this._MacroSwitchAnalyser.OutputResult.Save2Csv(TEMP_CSV_FILE);
				ResultCompare.DetailCompareResult cmp_rslt
					= ResultCompare.DetailCsvCompare(this._TestData.CompareCsvFile, TEMP_CSV_FILE);
				File.Delete(TEMP_CSV_FILE);
				TEST_RESULT test_result = new TEST_RESULT(	this._TestData, cmp_rslt,
															this._StopWatch.Elapsed,
					this._MacroSwitchAnalyser.OutputResult.GetTotalMacroSwitchResultCount());
				this._TestResultList.Add(test_result);

				ShowSingleTestReport(test_result);

				bool ret = StartNextTest();
				if (!ret)
				{
					ShowFinalTestReport();
					Console.ReadLine();
				}
			}
		}

		string ShowSingleTestReport(TEST_RESULT test_result)
		{
			string result_str = test_result.GetResultString();
			Console.ForegroundColor = test_result.GetResultColor();
			string report_str = string.Format("{0} Total:{1} Equal:{2}",
												result_str,
												test_result.TotalCount,
												test_result.CompareResult.EqualList.Count);
			if (test_result.GetCompareResult() != TEST_RESULT.E_RESULT.OK)
			{
				report_str += string.Format(" Unequal:{0} Lack:{1} Surplus:{2}",
												test_result.CompareResult.UnequalList.Count,
												test_result.CompareResult.LackList.Count,
												test_result.CompareResult.SurplusList.Count);
			}
			report_str += string.Format(" {0} {1}", test_result.GetTargetFolderName(),
													test_result.ElapsedTime.ToString());
			Console.WriteLine(report_str);
			Console.ResetColor();
			return report_str;
		}

		void ShowFinalTestReport()
		{
			List<string> wt_list = new List<string>();
			string title_str = System.Environment.NewLine
					+ "=========================" + DateTime.Now.ToString() + "==========================="
					+ System.Environment.NewLine;
			Console.WriteLine(title_str);
			wt_list.Add(title_str);
			foreach (var item in this._TestResultList)
			{
				string str = ShowSingleTestReport(item);
				wt_list.Add(str);
			}
			string path = System.AppDomain.CurrentDomain.BaseDirectory + "testlog.txt";
			File.AppendAllLines(path, wt_list, Encoding.UTF8);
		}

		public bool StartNextTest()
		{
			if (0 != _TestDataQueue.Count)
			{
				MSA_TEST_DATA test_data = _TestDataQueue.Dequeue();
				StartMsaRunner(test_data);
				return true;
			}
			else
			{
				return false;
			}
		}

		Queue<MSA_TEST_DATA> LoadTestDataQueue(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && File.Exists(path));
			Queue<MSA_TEST_DATA> ret_queue = new Queue<MSA_TEST_DATA>();
			string[] read_lines = File.ReadAllLines(path, Encoding.UTF8);
			foreach (var line in read_lines)
			{
				MSA_TEST_DATA test_data;
				if (MSA_TEST_DATA.Tryparse(line, out test_data))
				{
					ret_queue.Enqueue(test_data);
				}
			}
			return ret_queue;
		}
	}

	class MSA_TEST_DATA
	{
		public string RootFolder = null;
		public string TargetFolder = null;
		public string CompareCsvFile = null;

		public MSA_TEST_DATA(string root, string target, string compare_csv)
		{
			Trace.Assert(!string.IsNullOrEmpty(root) && Directory.Exists(root));
			Trace.Assert(!string.IsNullOrEmpty(target) && Directory.Exists(target));
			Trace.Assert(!string.IsNullOrEmpty(compare_csv) && File.Exists(compare_csv));
			this.RootFolder = root;
			this.TargetFolder = target;
			this.CompareCsvFile = compare_csv;
		}

		public static bool Tryparse(string script_line, out MSA_TEST_DATA test_data)
		{
			test_data = null;
			if (string.IsNullOrEmpty(script_line))
			{
				return false;
			}
			int idx = script_line.IndexOf("//");
			if (-1 != idx)
			{
				script_line = script_line.Remove(idx).Trim();
			}
			string[] arr = script_line.Split(',');
			if (arr.Length < 3)
			{
				return false;
			}
			string root = arr[0].Trim();
			string target = arr[1].Trim();
			string compare_csv = arr[2].Trim();
			if (Directory.Exists(root)
				&& Directory.Exists(target)
				&& File.Exists(compare_csv))
			{
				test_data = new MSA_TEST_DATA(root, target, compare_csv);
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	class TEST_RESULT
	{
		public MSA_TEST_DATA TestData = null;
		public ResultCompare.DetailCompareResult CompareResult = null;
		public TimeSpan ElapsedTime = TimeSpan.Zero;
		public int TotalCount = 0;

		public TEST_RESULT(	MSA_TEST_DATA test_data,
							ResultCompare.DetailCompareResult compare_result,
							TimeSpan elapsed_time, int total_count)
		{
			Trace.Assert(null != test_data);
			Trace.Assert(null != compare_result);
			this.TestData = test_data;
			this.CompareResult = compare_result;
			this.ElapsedTime = elapsed_time;
			this.TotalCount = total_count;
		}

		public string GetTargetFolderName()
		{
			DirectoryInfo di = new DirectoryInfo(this.TestData.TargetFolder);
			return di.Name;
		}

		public E_RESULT GetCompareResult()
		{
			if (0 != this.CompareResult.EqualList.Count
				&& 0 == this.CompareResult.LackList.Count
				&& 0 == this.CompareResult.SurplusList.Count
				&& 0 == this.CompareResult.UnequalList.Count)
			{
				return E_RESULT.OK;
			}
			else
			{
				return E_RESULT.NG;
			}
		}

		public string GetResultString()
		{
			if (this.GetCompareResult() == E_RESULT.OK)
			{
				return "OK";
			}
			else
			{
				return "NG";
			}
		}

		public ConsoleColor GetResultColor()
		{
			if (this.GetCompareResult() == E_RESULT.OK)
			{
				return ConsoleColor.Green;
			}
			else
			{
				return ConsoleColor.Red;
			}
		}

		public enum E_RESULT
		{
			NG,
			OK,
		}
	}
}
