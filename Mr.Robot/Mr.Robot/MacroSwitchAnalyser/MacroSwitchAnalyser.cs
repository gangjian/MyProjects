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
	class MSA_INPUT_PARA
	{
		public List<string> SrcList = null;												// 源文件列表
		public List<string> HdList = null;												// 头文件列表
		public List<string> MtpjList = null;											// ".mtpj"文件列表
		public List<string> MkList = null;												// ".mk"源文件列表
		public int WorkerThreadNum = 1;

		public MSA_INPUT_PARA(	List<string> src_list, List<string> hd_list,
								List<string> mtpj_list, List<string> mk_list,
								int worker_thread_num = 1)
		{
			this.SrcList = src_list;
			this.HdList = hd_list;
			this.MtpjList = mtpj_list;
			this.MkList = mk_list;
			this.WorkerThreadNum = worker_thread_num;
		}
	}

	public class MSA_MACRO_SWITCH_RESULT
	{
		public string SrcName = string.Empty;
		public int LineNumber = -1;
		public string ExpressionStr = string.Empty;
		public string MacroName = string.Empty;
		public bool IsDefined = false;
		public string ValueStr = string.Empty;
		public string MacroDefFileName = string.Empty;

		public MSA_MACRO_SWITCH_RESULT(	string src_name, int line_num, string exp_str, string macro_name,
										bool valid, string val_str, string macro_def_file_name)
		{
			this.SrcName = src_name;
			this.LineNumber = line_num;
			this.ExpressionStr = exp_str;
			this.MacroName = macro_name;
			this.IsDefined = valid;
			this.ValueStr = val_str;
			this.MacroDefFileName = macro_def_file_name;
		}
	}

	class MSA_SOURCE_RESULT
	{
		public string SourceFileName = string.Empty;
		public List<MSA_MACRO_SWITCH_RESULT> MacroSwitchResultList = null;

		public MSA_SOURCE_RESULT(string src_name, List<MSA_MACRO_SWITCH_RESULT> macro_switch_result_list)
		{
			this.SourceFileName = src_name;
			this.MacroSwitchResultList = macro_switch_result_list;
		}
	}
	/// <summary>
	/// 出力结果
	/// </summary>
	class MSA_OUTPUT_RESULT
	{
		public List<MSA_SOURCE_RESULT> SourceResultList = new List<MSA_SOURCE_RESULT>();
		public MSA_PROGRESS Progress = new MSA_PROGRESS();
		object obj_lock = new object();

		public void Add(MSA_SOURCE_RESULT result)
		{
			lock (obj_lock)
			{
				this.SourceResultList.Add(result);
			}
		}
	}

	class MSA_PROGRESS
	{
		public string CurrentSourceName = string.Empty;
		public MSA_SOURCE_PROC_RESULT ProcResult = MSA_SOURCE_PROC_RESULT.NOT_FOUND;
		public int CurrentCount = 0;
		public int TotalCount = 0;

		public MSA_PROGRESS()
		{
		}

		public MSA_PROGRESS(string cur_file, MSA_SOURCE_PROC_RESULT proc_rslt, int count, int total)
		{
			this.CurrentSourceName = cur_file;
			this.ProcResult = proc_rslt;
			this.CurrentCount = count;
			this.TotalCount = total;
		}
	}

	public enum MSA_SOURCE_PROC_RESULT
	{
		NOT_FOUND,
		FAIL,
		SUCCESS,
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

		public MSA_STATISTICS_INFO(int total_count)
		{
			this.TotalCount = total_count;
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
    class MACRO_SWITCH_ANALYSER
    {
		MSA_INPUT_PARA m_inputPara = null;
		MSA_INPUT_PARA InputPara
		{
			get { return m_inputPara; }
			set { m_inputPara = value; }
		}

		MSA_STATISTICS_INFO m_statisticsInfo = null;
		public MSA_STATISTICS_INFO StatisticsInfo
		{
			get { return m_statisticsInfo; }
			set { m_statisticsInfo = value; }
		}

		private MSA_OUTPUT_RESULT m_outputResult = null;
		public MSA_OUTPUT_RESULT OutputResult
		{
			get { return m_outputResult; }
			set { m_outputResult = value; }
		}

		public EventHandler MSA_ReportProgressHandler = null;

		public MACRO_SWITCH_ANALYSER(MSA_INPUT_PARA input_para)
        {
			this.InputPara = input_para;
			this.WorkerThreadGroup = new Thread[input_para.WorkerThreadNum];
        }

		Thread[] WorkerThreadGroup = null;

		public void ProcStart()
		{
			ProcAbort();

			this.OutputResult = new MSA_OUTPUT_RESULT();
			this.StatisticsInfo = new MSA_STATISTICS_INFO(this.InputPara.SrcList.Count);

			this.m_ProcInfo = new PROC_INFO();
			// 处理.mtpj文件
			m_ProcInfo.mtpjInfoList = MtpjProc();
			// 处理.mk文件
			m_ProcInfo.mkInfoList = MkProc();
			m_ProcInfo.count = 0;

			// 处理源文件
			foreach (string src_name in this.InputPara.SrcList)
			{
				m_ProcInfo.SourceFileQueue.Enqueue(src_name);
			}
			for (int i = 0; i < this.WorkerThreadGroup.Length; i++)
			{
				this.WorkerThreadGroup[i] = new Thread(new ThreadStart(ThreadMain));
				this.WorkerThreadGroup[i].Start();
			}
			//System.Diagnostics.Trace.WriteLine(this.StatisticsInfo.PrintOut());
		}

		public void ProcAbort()
		{
			for (int i = 0; i < this.WorkerThreadGroup.Length; i++)
			{
				WorkerThreadAbord(this.WorkerThreadGroup[i]);
			}
		}

		void WorkerThreadAbord(Thread worker_thread)
		{
			if (null != worker_thread
				&& worker_thread.IsAlive)
			{
				worker_thread.Abort();
				worker_thread = null;
			}
		}

		class PROC_INFO
		{
			public List<MTPJ_FILE_INFO> mtpjInfoList = null;
			public List<MK_FILE_INFO> mkInfoList = null;
			public int count = 0;
			public Queue<string> SourceFileQueue = new Queue<string>();
		}

		PROC_INFO m_ProcInfo = null;

		void ThreadMain()
		{
			while (true)
			{
				string srcName = string.Empty;
				lock (this.m_ProcInfo)
				{
					if (0 == this.m_ProcInfo.SourceFileQueue.Count)
					{
						// 检查其它线程是否都已结束
						break;
					}
					srcName = this.m_ProcInfo.SourceFileQueue.Dequeue();
				}
				MSA_SOURCE_PROC_RESULT procResult;
				MSA_SOURCE_RESULT result = SrcProc(srcName, this.InputPara.HdList, out procResult, this.m_ProcInfo.mtpjInfoList, this.m_ProcInfo.mkInfoList);
				lock (this.m_ProcInfo)
				{
					this.m_ProcInfo.count++;
				}
				lock (this.OutputResult)
				{
					this.OutputResult.Add(result);
					this.OutputResult.Progress = new MSA_PROGRESS(srcName, procResult, m_ProcInfo.count, this.StatisticsInfo.TotalCount);
					ReportProgress2Caller();
				}
			}
		}

		void ReportProgress2Caller()
		{
			if (null != this.MSA_ReportProgressHandler)
			{
				this.MSA_ReportProgressHandler(this, null);
			}
		}

		List<MTPJ_FILE_INFO> MtpjProc()
		{
			List<MTPJ_FILE_INFO> mtpjInfoList = new List<MTPJ_FILE_INFO>();
			foreach (string mtpj_name in this.InputPara.MtpjList)
			{
				MTPJ_FILE_INFO mtpj_info = new MTPJ_FILE_INFO(mtpj_name);
				mtpj_info.MtpjProc();
				mtpjInfoList.Add(mtpj_info);
			}
			return mtpjInfoList;
		}

		List<MK_FILE_INFO> MkProc()
		{
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
			return mkInfoList;
		}

		MSA_SOURCE_RESULT SrcProc(	string src_name,
									List<string> header_list,
									out MSA_SOURCE_PROC_RESULT proc_result,
									List<MTPJ_FILE_INFO> mtpj_info_list,
									List<MK_FILE_INFO> mk_info_list)
        {
			proc_result = MSA_SOURCE_PROC_RESULT.NOT_FOUND;
            List<string> codeList = COMN_PROC.RemoveComments(src_name);
			List<MacroSwitchExpInfo> expInfoList = GetMacroExpList(codeList);
            if (0 == expInfoList.Count)
            {
				proc_result = MSA_SOURCE_PROC_RESULT.NOT_FOUND;
				this.StatisticsInfo.NotFoundCount += 1;
				return new MSA_SOURCE_RESULT(src_name, null);
            }
			List<string> srcList = new List<string>();
			srcList.Add(src_name);
			C_PROSPECTOR cProspector = new C_PROSPECTOR(srcList, header_list);
			cProspector.MacroSwichAnalyserFlag = true;
			List<FILE_PARSE_INFO> parseInfoList = cProspector.SyncStart();
			if (null == parseInfoList || 0 == parseInfoList.Count)
			{
				proc_result = MSA_SOURCE_PROC_RESULT.FAIL;
				this.StatisticsInfo.FailedCount += 1;
				return new MSA_SOURCE_RESULT(src_name, null);
			}
			List<MSA_MACRO_SWITCH_RESULT> resultList = new List<MSA_MACRO_SWITCH_RESULT>();
			FileInfo fi = new FileInfo(src_name);
			foreach (MacroSwitchExpInfo expInfo in expInfoList)
			{
				MacroPrintInfo printInfo = new MacroPrintInfo(fi.Name, expInfo.LineNum, expInfo.CodeLine);
				CommonProc.MacroSwitchExpressionAnalysis(expInfo.ExpStr, printInfo, parseInfoList[0], ref resultList, mtpj_info_list, mk_info_list);
			}
			proc_result = MSA_SOURCE_PROC_RESULT.SUCCESS;
			this.StatisticsInfo.SuccessCount += 1;
			return new MSA_SOURCE_RESULT(src_name, resultList);
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
