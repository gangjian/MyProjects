using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Mr.Robot;
using System.Threading;

namespace Mr.Robot.MacroSwitchAnalyser
{
    public class MacroSwitchAnalyser2
    {
		List<string> SrcList = null;													// 源文件列表
		List<string> HdList = null;														// 头文件列表
		List<string> MtpjList = null;													// .mtpj文件列表
		List<string> MkList = null;														// .mk文件列表

		int TotalCount = 0;
		int SuccessCount = 0;
		int FailedCount = 0;
		int NotFoundCount = 0;

		List<string> ResultList = new List<string>();

		public delegate void ReportProgressDel(string progress_str, List<string> result_list);

		public ReportProgressDel ReportProgress = null;

		public MacroSwitchAnalyser2(List<string> source_list, List<string> header_list, List<string> mtpj_file_list, List<string> mk_file_list)
        {
			this.SrcList = source_list;
			this.HdList = header_list;
			this.MtpjList = mtpj_file_list;
			this.MkList = mk_file_list;
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
			this.TotalCount = this.SrcList.Count;
			this.SuccessCount = 0;
			this.FailedCount = 0;
			this.NotFoundCount = 0;
			int count = 0;

			// 处理.mtpj文件
			List<MTPJ_FILE_INFO> mtpjInfoList = new List<MTPJ_FILE_INFO>();
			foreach (string mtpj_name in this.MtpjList)
			{
				MTPJ_FILE_INFO mtpj_info = new MTPJ_FILE_INFO(mtpj_name);
				mtpj_info.MtpjProc();
				mtpjInfoList.Add(mtpj_info);
			}

			// 处理.mk文件
			List<MK_FILE_INFO> mkInfoList = new List<MK_FILE_INFO>();
			foreach (string mk_name in this.MkList)
			{
				MK_FILE_INFO mk_info = new MK_FILE_INFO(mk_name);
				mk_info.MkProc();
				if (0 != mk_info.DefList.Count)
				{
					mkInfoList.Add(mk_info);
				}
			}

			CCodeAnalyser.CodeBufferManager codeBufferList = new CCodeAnalyser.CodeBufferManager();

			// 处理源文件
			foreach (string src_name in this.SrcList)
			{
				count++;
				string commentStr;
				List<string> resultList = SrcProc(src_name, this.HdList, out commentStr, mtpjInfoList, mkInfoList, ref codeBufferList);
				if (null != resultList)
				{
					//this.ResultList.AddRange(resultList);
				}
				if (null != this.ReportProgress)
				{
					string progressStr = src_name + " ==> " + commentStr + " : " + count.ToString() + "/" + this.TotalCount.ToString();
					this.ReportProgress(progressStr, resultList);
					if (null != resultList && 0 != resultList.Count)
					{
						Thread.Sleep(30);
					}
				}
			}
			System.Diagnostics.Trace.WriteLine("Complete! Total:" + this.TotalCount.ToString() + ", Failed:"
					+ this.FailedCount.ToString() + ", NotFound:" + this.NotFoundCount.ToString() + ", Success:" + this.SuccessCount.ToString());
		}

		List<string> SrcProc(string src_name,
							List<string> header_list,
							out string comment_str,
							List<MTPJ_FILE_INFO> mtpj_info_list,
							List<MK_FILE_INFO> mk_info_list,
							ref CCodeAnalyser.CodeBufferManager code_buf_list)
        {
			comment_str = string.Empty;
            List<string> codeList = CCodeAnalyser.RemoveComments(src_name);
			List<MacroSwitchExpInfo> expInfoList = GetMacroExpList(codeList);
            if (0 == expInfoList.Count)
            {
				comment_str = "NoT Found!";
				this.NotFoundCount += 1;
                return null;
            }
			List<string> srcList = new List<string>();
			srcList.Add(src_name);
			CCodeAnalyser cAnalyser = new CCodeAnalyser(srcList, header_list, ref code_buf_list);
			FileParseInfo parseInfo = new FileParseInfo(src_name);
			List<FileParseInfo> parseInfoList = cAnalyser.CFileListProc();
			if (null == parseInfoList || 0 == parseInfoList.Count)
			{
				comment_str = "Failed!";
				this.FailedCount += 1;
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
			this.SuccessCount += 1;
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
