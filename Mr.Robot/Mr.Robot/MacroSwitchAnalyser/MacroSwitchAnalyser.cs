﻿using System;
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

		public string ToCsvLineStr()
		{
			string csv_line = this.SrcName + "," + this.LineNumber.ToString() + "," + this.ExpressionStr + "," + this.MacroName;
			if (string.Empty == this.ValueStr)
			{
				if (this.IsDefined)
				{
					csv_line += "," + @"○";
				}
				else
				{
					csv_line += "," + @"×";
				}
			}
			else
			{
				csv_line += "," + this.ValueStr;
			}
			return csv_line;
		}
	}

	public class MSA_SOURCE_INFO
	{
		public string SourceFileName = string.Empty;
		public List<MSA_MACRO_SWITCH_RESULT> MacroSwitchResultList = null;

		public MSA_SOURCE_INFO(string src_name, List<MSA_MACRO_SWITCH_RESULT> macro_switch_result_list)
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
		public List<MSA_SOURCE_INFO> SourceResultList = new List<MSA_SOURCE_INFO>();
		public MSA_PROGRESS Progress = new MSA_PROGRESS();
		object obj_lock = new object();

		public void Add(MSA_SOURCE_INFO result)
		{
			lock (this.obj_lock)
			{
				this.SourceResultList.Add(result);
			}
		}

		public void Save2Csv(string path)
		{
			List<string> wt_list = new List<string>();
			string column_title = "idx,file,line,expression,macro,value,";
			wt_list.Add(column_title);
			lock (this.obj_lock)
			{
				foreach (var src_rslt in this.SourceResultList)
				{
					if (null != src_rslt.MacroSwitchResultList)
					{
						for (int i = 0; i < src_rslt.MacroSwitchResultList.Count; i++)
						{
							wt_list.Add((i + 1).ToString() + "," + src_rslt.MacroSwitchResultList[i].ToCsvLineStr() + ",");
						}
					}
				}
				File.WriteAllLines(path, wt_list);
			}
		}

		public int GetTotalMacroSwitchResultCount()
		{
			int ret_cnt = 0;
			lock (this.obj_lock)
			{
				foreach (var src_rslt in this.SourceResultList)
				{
					if (null != src_rslt.MacroSwitchResultList)
					{
						ret_cnt += src_rslt.MacroSwitchResultList.Count;
					}
				}
			}
			return ret_cnt;
		}
	}

	public class MSA_PROGRESS
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
    public class MACRO_SWITCH_ANALYSER
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

		CodeBufferManager m_CodeBufferManager = null;

		public MACRO_SWITCH_ANALYSER(MSA_INPUT_PARA input_para)
        {
			this.InputPara = input_para;
			this.WorkerThreadGroup = new Thread[input_para.WorkerThreadNum];
        }

		Thread[] WorkerThreadGroup = null;

		public void ProcStart()
		{
			ProcAbort();
			// 初始化参数
			InitParas();

			// 准备好源文件队列
			PrepareSourceFileQueue();

			// 启动工作线程组
			StartWorkerThreadGroup();
			//System.Diagnostics.Trace.WriteLine(this.StatisticsInfo.PrintOut());
		}

		public void ProcStop()
		{
			this.m_CodeBufferManager.Clear();
		}

		void InitParas()
		{
			this.OutputResult = new MSA_OUTPUT_RESULT();
			this.StatisticsInfo = new MSA_STATISTICS_INFO(this.InputPara.SrcList.Count);

			this.m_ProcInfo = new PROC_INFO();
			// 处理.mtpj文件
			this.m_ProcInfo.mtpjInfoList = GetMtpjInfoList();
			// 处理.mk文件
			this.m_ProcInfo.mkInfoList = GetMkInfoList();
			this.m_ProcInfo.count = 0;

			this.m_CodeBufferManager = new CodeBufferManager();
		}

		void PrepareSourceFileQueue()
		{
			foreach (string src_name in this.InputPara.SrcList)
			{
				m_ProcInfo.SourceFileQueue.Enqueue(src_name);
			}
		}

		void StartWorkerThreadGroup()
		{
			for (int i = 0; i < this.WorkerThreadGroup.Length; i++)
			{
				this.WorkerThreadGroup[i] = new Thread(new ThreadStart(ThreadMain));
				this.WorkerThreadGroup[i].Start();
			}
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

		// 处理工作全局量
		class PROC_INFO
		{
			public List<MTPJ_FILE_INFO> mtpjInfoList = null;							// .mtpj, .mk文件情报列表
			public List<MK_FILE_INFO> mkInfoList = null;
			public int count = 0;														// 完成解析文件的计数
			public Queue<string> SourceFileQueue = new Queue<string>();					// 源文件队列(各工作线程从中提取文件并分别解析)
		}

		PROC_INFO m_ProcInfo = null;

		void ThreadMain()
		{
			while (true)
			{
				string src_name = GetNextProcSourceFile();
				if (string.IsNullOrEmpty(src_name))
				{
					break;
				}
				MSA_SOURCE_PROC_RESULT proc_result;
				MSA_SOURCE_INFO src_proc_info = SrcProc(src_name, this.InputPara.HdList,
														out proc_result,
														this.m_ProcInfo.mtpjInfoList,
														this.m_ProcInfo.mkInfoList);

				UpdateProgress(src_name, proc_result, src_proc_info);
			}
		}

		string GetNextProcSourceFile()
		{
			lock (this.m_ProcInfo)
			{
				if (0 == this.m_ProcInfo.SourceFileQueue.Count)
				{
					return null;
				}
				return this.m_ProcInfo.SourceFileQueue.Dequeue();
			}
		}

		void UpdateProgress(string src_name, MSA_SOURCE_PROC_RESULT proc_result, MSA_SOURCE_INFO src_proc_info)
		{
			int count = 0;
			lock (this.m_ProcInfo)
			{
				count = (this.m_ProcInfo.count += 1);
			}
			lock (this.OutputResult)
			{
				this.OutputResult.Add(src_proc_info);
				this.OutputResult.Progress = new MSA_PROGRESS(src_name, proc_result, count, this.StatisticsInfo.TotalCount);
				ReportProgress2Caller();
			}
		}

		void ReportProgress2Caller()
		{
			if (null != this.MSA_ReportProgressHandler)
			{
				this.MSA_ReportProgressHandler(this, null);
			}
		}

		List<MTPJ_FILE_INFO> GetMtpjInfoList()
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

		List<MK_FILE_INFO> GetMkInfoList()
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

		MSA_SOURCE_INFO SrcProc(string src_name,
								List<string> header_list,
								out MSA_SOURCE_PROC_RESULT proc_result,
								List<MTPJ_FILE_INFO> mtpj_info_list,
								List<MK_FILE_INFO> mk_info_list)
        {
			proc_result = MSA_SOURCE_PROC_RESULT.NOT_FOUND;
            List<string> code_list = COMN_PROC.RemoveComments(src_name);
			List<MacroSwitchExpInfo> exp_info_list = GetMacroExpList(code_list);
            if (0 == exp_info_list.Count)
            {
				proc_result = MSA_SOURCE_PROC_RESULT.NOT_FOUND;
				this.StatisticsInfo.NotFoundCount += 1;
				return new MSA_SOURCE_INFO(src_name, null);
            }
			else
			{
				FILE_PARSE_INFO src_parse_info = GetSrcParseInfo(src_name, header_list);
				if (null == src_parse_info)
				{
					proc_result = MSA_SOURCE_PROC_RESULT.FAIL;
					this.StatisticsInfo.FailedCount += 1;
					return new MSA_SOURCE_INFO(src_name, null);
				}
				else
				{
					List<MSA_MACRO_SWITCH_RESULT> result_list = new List<MSA_MACRO_SWITCH_RESULT>();
					FileInfo fi = new FileInfo(src_name);
					foreach (MacroSwitchExpInfo exp_info in exp_info_list)
					{
						MacroPrintInfo print_info = new MacroPrintInfo(fi.Name, exp_info.LineNum, exp_info.CodeLine);
						CommonProc.MacroSwitchExpressionAnalysis(	exp_info.ExpStr,
																	print_info,
																	src_parse_info,
																	ref result_list,
																	mtpj_info_list,
																	mk_info_list);
					}
					proc_result = MSA_SOURCE_PROC_RESULT.SUCCESS;
					this.StatisticsInfo.SuccessCount += 1;
					return new MSA_SOURCE_INFO(src_name, result_list);
				}
			}
        }

		FILE_PARSE_INFO GetSrcParseInfo(string src_name, List<string> header_list)
		{
			List<string> src_list = new List<string>();
			src_list.Add(src_name);
			// ==============================
			C_PROSPECTOR csrc_prospector = new C_PROSPECTOR(src_list,
															header_list,
															this.m_CodeBufferManager);	// <--- 这里调用C_PROSPECTOR取得代码文件宏定义解析结果
			csrc_prospector.MacroSwichAnalyserFlag = true;
			List<FILE_PARSE_INFO> parse_info_list = csrc_prospector.SyncStart();
			// ==============================
			if (null == parse_info_list || 0 == parse_info_list.Count)
			{
				return null;
			}
			else
			{
				return parse_info_list.First();
			}
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
