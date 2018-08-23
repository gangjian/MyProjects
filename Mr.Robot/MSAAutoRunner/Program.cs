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
	class Program
	{
		static void Main(string[] args)
		{
			StartMsaRunner(	"C:\\Users\\GangJian\\03_work\\99_Data\\MTbot_TestData\\Honda18HMI_soft",
							"C:\\Users\\GangJian\\03_work\\99_Data\\MTbot_TestData\\Honda18HMI_soft");
		}

		static MACRO_SWITCH_ANALYSER _MacroSwitchAnalyser = null;
		static Stopwatch _StopWatch = null;
		static void StartMsaRunner(string root_path, string target_path)
		{
			MSA_INPUT_PARA msa_para = GetMsaPara(root_path, target_path);
			_MacroSwitchAnalyser = new MACRO_SWITCH_ANALYSER(msa_para);
			_MacroSwitchAnalyser.MSA_ReportProgressHandler += new EventHandler(UpdateProgress);
			_StopWatch = new Stopwatch();
			_StopWatch.Start();
			_MacroSwitchAnalyser.ProcStart();
		}

		static MSA_INPUT_PARA GetMsaPara(string root_path, string target_path)
		{
			List<string> src_list = new List<string>(), hd_list = new List<string>(),
						 mtpj_list = new List<string>(), mk_list = new List<string>();
			Mr.Robot.IOProcess.GetAllCCodeFiles(root_path, src_list, hd_list, mtpj_list, mk_list);
			MSA_INPUT_PARA ret_para = new MSA_INPUT_PARA(src_list, hd_list, mtpj_list, mk_list, 4);
			return ret_para;
		}

		static string _CompareCsv = "C:\\Users\\GangJian\\03_work\\99_Data\\MTbot_TestData\\01_CSV\\Honda18HMI_soft_2017_09_14 16_12_28_Detail.csv";
		static void UpdateProgress(object obj, EventArgs e)
		{
			int cur_cnt = _MacroSwitchAnalyser.OutputResult.Progress.CurrentCount;
			int total_cnt = _MacroSwitchAnalyser.OutputResult.Progress.TotalCount;
			Console.WriteLine(string.Format("{0} : {1}/{2}",
								_StopWatch.Elapsed.ToString(), cur_cnt,	total_cnt));
			if (cur_cnt == total_cnt)
			{
				// 完成
				_StopWatch.Stop();
				_MacroSwitchAnalyser.OutputResult.Save2Csv("tmp.csv");
				ResultCompare.DetailCompareResult cmp_rslt
									= ResultCompare.DetailCsvCompare(_CompareCsv, "tmp.csv");

				Console.WriteLine(string.Format("Total:{0}\nEqual:{1}\nUnequal:{2}\nLack:{3}\nSurplus:{4}",
						_MacroSwitchAnalyser.OutputResult.GetTotalMacroSwitchResultCount(),
														cmp_rslt.EqualList.Count,
														cmp_rslt.UnequalList.Count,
														cmp_rslt.LackList.Count,
														cmp_rslt.SurplusList.Count));
				Console.WriteLine("Press any key to finish...");
				Console.ReadKey();
			}
		}
	}
}
