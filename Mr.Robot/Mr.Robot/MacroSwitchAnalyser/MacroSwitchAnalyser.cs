using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Mr.Robot;
using System.Threading;

namespace Mr.Robot.MacroSwitchAnalyser
{
	/// <summary>
	/// 入力参数
	/// </summary>
	public class MSA_INPUT_PARA
	{
		public List<string> SrcList = null;												// 源文件列表
		public List<string> HdList = null;												// 头文件列表
		public List<string> MtpjList = null;											// ".mtpj"文件列表
		public List<string> MkList = null;												// ".mk"源文件列表

		public MSA_INPUT_PARA(	List<string> src_list, List<string> hd_list,
								List<string> mtpj_list, List<string> mk_list)
		{
			this.SrcList = src_list;
			this.HdList = hd_list;
			this.MtpjList = mtpj_list;
			this.MkList = mk_list;
		}
	}

	public class MSA_SOURCE_RESULT
	{
		public string SourceFileName = string.Empty;
		public List<string> MacroSwitchResultList = null;

		public MSA_SOURCE_RESULT(string src_name, List<string> macro_switch_result_list)
		{
			this.SourceFileName = src_name;
			this.MacroSwitchResultList = macro_switch_result_list;
		}
	}
	/// <summary>
	/// 出力结果
	/// </summary>
	public class MSA_OUTPUT_RESULT
	{
		public List<MSA_SOURCE_RESULT> SourceResultList = new List<MSA_SOURCE_RESULT>();
		public string ProgressStr = string.Empty;
		object obj_lock = new object();

		public void Add(string src_name, List<string> result_list)
		{
			lock (obj_lock)
			{
				this.SourceResultList.Add(new MSA_SOURCE_RESULT(src_name, result_list));
			}
		}
	}

	/// <summary>
	/// 统计信息
	/// </summary>
	public class MSA_STATISTICS_INFO
	{
		int _totalCount = 0;
		public int TotalCount
		{
			get { return _totalCount; }
			set { _totalCount = value; }
		}

		int _successCount = 0;
		public int SuccessCount
		{
			get { return _successCount; }
			set { _successCount = value; }
		}

		int _failedCount = 0;
		public int FailedCount
		{
			get { return _failedCount; }
			set { _failedCount = value; }
		}

		int _notFoundCount = 0;
		public int NotFoundCount
		{
			get { return _notFoundCount; }
			set { _notFoundCount = value; }
		}

		public string PrintOut()
		{
			return "Complete! Total:" + this.TotalCount.ToString() + ", Failed:" + this.FailedCount.ToString()
					+ ", NotFound:" + this.NotFoundCount.ToString() + ", Success:" + this.SuccessCount.ToString();
		}
	}

	/// <summary>
	/// 源代码中的宏开关(On/Off,有效/无效,定义/未定义,define/undefine)分析
	/// </summary>
    public class MACRO_SWITCH_ANALYSER
    {
		MSA_INPUT_PARA m_inputPara = null;
		MSA_INPUT_PARA InputPara
		{
			get { return m_inputPara; }
			set { m_inputPara = value; }
		}

		MSA_STATISTICS_INFO m_statisticsInfo = new MSA_STATISTICS_INFO();
		public MSA_STATISTICS_INFO StatisticsInfo
		{
			get { return m_statisticsInfo; }
			set { m_statisticsInfo = value; }
		}

		private MSA_OUTPUT_RESULT m_outputResult = new MSA_OUTPUT_RESULT();
		public MSA_OUTPUT_RESULT OutputResult
		{
			get { return m_outputResult; }
			set { m_outputResult = value; }
		}

		// 新的更新处理,用以取代上面的"ReportProgressDel"
		public EventHandler ReportProgressHandler = null;

		public MACRO_SWITCH_ANALYSER(MSA_INPUT_PARA input_para)
        {
			this.InputPara = input_para;
        }

		Thread workerThread = null;

		public void ProcStart()
		{
			ProcAbort();
			this.workerThread = new Thread(new ThreadStart(ProcMain));
			workerThread.Start();
		}

		public void ProcAbort()
		{
			if (null != this.workerThread
				&& this.workerThread.IsAlive)
			{
				this.workerThread.Abort();
				this.workerThread = null;
			}
		}

		void ProcMain()
		{
			this.OutputResult = new MSA_OUTPUT_RESULT();
			this.StatisticsInfo = new MSA_STATISTICS_INFO();
			this.StatisticsInfo.TotalCount = this.InputPara.SrcList.Count;
			int count = 0;

			// 处理.mtpj文件
			List<MTPJ_FILE_INFO> mtpjInfoList = new List<MTPJ_FILE_INFO>();
			foreach (string mtpj_name in this.InputPara.MtpjList)
			{
				MTPJ_FILE_INFO mtpj_info = new MTPJ_FILE_INFO(mtpj_name);
				mtpj_info.MtpjProc();
				mtpjInfoList.Add(mtpj_info);
			}

			// 处理.mk文件
			List<MK_FILE_INFO> mkInfoList = new List<MK_FILE_INFO>();
			foreach (string mk_name in this.InputPara.MkList)
			{
				MK_FILE_INFO mk_info = new MK_FILE_INFO(mk_name);
				mk_info.MkProc();
				if (0 != mk_info.DefList.Count)
				{
					mkInfoList.Add(mk_info);
				}
			}

			C_CODE_ANALYSER.CODE_BUFFER_MANAGER codeBufferList = new C_CODE_ANALYSER.CODE_BUFFER_MANAGER();

			// 处理源文件
			foreach (string src_name in this.InputPara.SrcList)
			{
				count++;
				string commentStr;
				List<string> resultList = SrcProc(src_name, this.InputPara.HdList, out commentStr, mtpjInfoList, mkInfoList, ref codeBufferList);
				string progressStr = src_name + " ==> " + commentStr + " : " + count.ToString() + "/" + this.StatisticsInfo.TotalCount.ToString();
				this.OutputResult.Add(src_name, resultList);
				this.OutputResult.ProgressStr = progressStr;
				if (null != this.ReportProgressHandler)
				{
					this.ReportProgressHandler(this, null);
				}
			}
			//System.Diagnostics.Trace.WriteLine(this.StatisticsInfo.PrintOut());
		}

		List<string> SrcProc(	string src_name,
								List<string> header_list,
								out string comment_str,
								List<MTPJ_FILE_INFO> mtpj_info_list,
								List<MK_FILE_INFO> mk_info_list,
								ref C_CODE_ANALYSER.CODE_BUFFER_MANAGER code_buf_list)
        {
			comment_str = string.Empty;
            List<string> codeList = C_CODE_ANALYSER.RemoveComments(src_name);
			List<MacroSwitchExpInfo> expInfoList = GetMacroExpList(codeList);
            if (0 == expInfoList.Count)
            {
				comment_str = "NoT Found!";
				this.StatisticsInfo.NotFoundCount += 1;
                return null;
            }
			List<string> srcList = new List<string>();
			srcList.Add(src_name);
			C_CODE_ANALYSER cAnalyser = new C_CODE_ANALYSER(srcList, header_list, ref code_buf_list);
			cAnalyser.MacroSwichAnalyserFlag = true;
			List<FILE_PARSE_INFO> parseInfoList = cAnalyser.CFileListProc();
			if (null == parseInfoList || 0 == parseInfoList.Count)
			{
				comment_str = "Failed!";
				this.StatisticsInfo.FailedCount += 1;
				return null;
			}
			List<string> resultList = new List<string>();
			FileInfo fi = new FileInfo(src_name);
			foreach (MacroSwitchExpInfo expInfo in expInfoList)
			{
				MacroPrintInfo printInfo = new MacroPrintInfo(fi.Name, expInfo.LineNum.ToString(), expInfo.CodeLine);
				CommonProc.MacroSwitchExpressionAnalysis(expInfo.ExpStr, printInfo, parseInfoList[0], ref resultList, mtpj_info_list, mk_info_list);
			}
			comment_str = "Success!";
			this.StatisticsInfo.SuccessCount += 1;
			return resultList;
        }

		class MacroSwitchExpInfo
		{
			public string ExpStr = string.Empty;
			public string CodeLine = string.Empty;
			public int LineNum = -1;
		}

        List<MacroSwitchExpInfo> GetMacroExpList(List<string> code_list)
        {
			List<MacroSwitchExpInfo> expList = new List<MacroSwitchExpInfo>();
			int lineNum = 0;
            foreach (string code_line in code_list)
            {
				lineNum++;
                string expStr = CodeLineProc(code_line);
                if (null != expStr)
                {
					MacroSwitchExpInfo msExp = new MacroSwitchExpInfo();
					msExp.CodeLine = code_line;
					msExp.ExpStr = expStr;
					msExp.LineNum = lineNum;
                    expList.Add(msExp);
                }
            }
            return expList;
        }

        string CodeLineProc(string code_line)
        {
			code_line = CommonProc.RemoveStringSegment(code_line);
            int idx = code_line.IndexOf("#if");
            if (-1 == idx)
            {
                return null;
            }
            return CommonProc.GetMacroExpression(code_line, idx);
        }
    }
}
