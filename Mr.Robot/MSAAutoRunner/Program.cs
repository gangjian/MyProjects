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
		}

		static void StartMsaRunner(string root_path, string target_path, string compare_csv)
		{
			MSA_INPUT_PARA msa_para = GetMsaPara(root_path, target_path);
			MACRO_SWITCH_ANALYSER msa = new MACRO_SWITCH_ANALYSER(msa_para);
		}

		static MSA_INPUT_PARA GetMsaPara(string root_path, string target_path)
		{
			List<string> src_list = new List<string>(), hd_list = new List<string>(),
						 mtpj_list = new List<string>(), mk_list = new List<string>();
			Mr.Robot.IOProcess.GetAllCCodeFiles(root_path, src_list, hd_list, mtpj_list, mk_list);
			MSA_INPUT_PARA ret_para = new MSA_INPUT_PARA(src_list, hd_list, mtpj_list, mk_list, 4);
			return ret_para;
		}
	}
}
