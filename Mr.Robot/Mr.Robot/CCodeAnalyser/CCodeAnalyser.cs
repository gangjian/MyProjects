using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Mr.Robot.MacroSwitchAnalyser;

namespace Mr.Robot
{
	public partial class C_CODE_ANALYSER
	{
		#region 全局字段
		List<string> HeaderList = new List<string>();									// 头文件名列表
		List<string> SourceList = new List<string>();									// 源文件名列表
		CODE_BUFFER_MANAGER CodeBuffManagerRef = null;

		public EventHandler UpdateProgress = null;
		public List<FILE_PARSE_INFO> ParseInfoList = null;

		public bool MacroSwichAnalyserFlag = false;										// 这个标志是给宏开关分析工具设的, 做宏开关分析时, 为了提高速度, 只分析头文件宏定义等..
																						// 不做语句分析
		public List<string> ErrorLogList = new List<string>();
		#endregion

		public C_CODE_ANALYSER(List<string> source_list, List<string> header_list, ref CODE_BUFFER_MANAGER code_buf_manager)
		{
			this.SourceList = source_list;
			this.HeaderList = header_list;
			this.CodeBuffManagerRef = code_buf_manager;
		}

		#region 向外提供的接口方法
		public void ProcessStart()
		{
			Thread workerThread = new Thread(new ThreadStart(ProcessMain));
			workerThread.Start();
			//Task t1 = new Task(CFileListProcess);
			//t1.Start();
			//t1.Wait();
		}

		void ProcessMain()
		{
			CFileListProc();
		}

		public List<FILE_PARSE_INFO> CFileListProc()
		{
			this.ParseInfoList = new List<FILE_PARSE_INFO>();
			int count = 0;
			int total = this.SourceList.Count;
			// 逐个解析源文件
			foreach (string srcName in this.SourceList)
			{
				FILE_PARSE_INFO parseInfo = new FILE_PARSE_INFO(srcName);
				count++;
				if (CFileProcess(srcName, ref parseInfo))
				{
					this.ParseInfoList.Add(parseInfo);
				}
				else
				{
				}
			}
			return this.ParseInfoList;
		}
		#endregion

		// 以下都是内部调用方法

		public class CODE_BUFFER_MANAGER
		{
			class CODE_BUFFER
			{
				public string FileName = string.Empty;
				public List<string> CodeList = null;

				public CODE_BUFFER(string file_name, List<string> code_list)
				{
					this.FileName = file_name;
					this.CodeList = code_list;
				}
			}

			const int MAX_CODE_BUF_LIST_CNT = 1000;
			List<CODE_BUFFER> CodeBufferList = new List<CODE_BUFFER>();

			public List<string> SearchCodeBufferList(string file_name)
			{
				for (int i = this.CodeBufferList.Count - 1; i >= 0; i--)
				{
					if (this.CodeBufferList[i].FileName.Equals(file_name))
					{
						return this.CodeBufferList[i].CodeList;
					}
				}
				return null;
			}

			public void AddCodeBufferList(string file_name, List<string> code_list)
			{
				CODE_BUFFER cb = new CODE_BUFFER(file_name, code_list);
				this.CodeBufferList.Add(cb);
				if (this.CodeBufferList.Count > MAX_CODE_BUF_LIST_CNT)
				{
					this.CodeBufferList.RemoveRange(0, this.CodeBufferList.Count - MAX_CODE_BUF_LIST_CNT);
				}
			}
		}

		/// <summary>
		/// C文件(包括源文件和头文件)处理
		/// </summary>
		bool CFileProcess(string file_name, ref FILE_PARSE_INFO file_info)
		{
			//System.Diagnostics.Trace.WriteLine(file_name + ": ");
			if (null == file_info)
			{
				file_info = new FILE_PARSE_INFO(file_name);
			}
			List<string> codeList = null;
			if (null != this.CodeBuffManagerRef)
			{
				codeList = this.CodeBuffManagerRef.SearchCodeBufferList(file_name);
			}
			if (null == codeList)
			{
				// 去掉注释
				codeList = RemoveComments(file_name);
				if (null != this.CodeBuffManagerRef && file_name.ToLower().EndsWith(".h"))
				{
					this.CodeBuffManagerRef.AddCodeBufferList(file_name, codeList);
				}
			}
			try
			{
				// 预编译处理
				codeList = PrecompileProcess(file_name, codeList, ref file_info);
				file_info.CodeList = codeList;

				if (this.MacroSwichAnalyserFlag)
				{
					return true;
				}
				// 文件解析
				if (!CCodeFileAnalysis(file_name, ref file_info))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.ToString());
				return false;
			}
		}

		/// <summary>
		/// 移除代码注释
		/// </summary>
		public static List<string> RemoveComments(string fileName)
		{
			TextReader tr = new StreamReader(fileName);
			List<string> retList = new List<string>();

			string rdLine = tr.ReadLine();
			if (null == rdLine)
			{
				return retList;
			}
            string wtLine = "";
			do
			{
				int idx1 = rdLine.IndexOf("//");
				int idx2 = rdLine.IndexOf("/*");

				if ((-1 != idx1)
					&& (-1 != idx2)
					&& (idx1 < idx2)
					)
				{
					wtLine = rdLine.Remove(idx1).TrimEnd();
				}
				else if ((-1 != idx1)
						 && (-1 == idx2))
				{
					// 只包含行注释
					wtLine = rdLine.Remove(idx1).TrimEnd();
				}
				else if (-1 != idx2)
				{
					// 只包含块注释
					int idx_s = idx2;
					int idx_e = rdLine.IndexOf("*/", idx_s + 2);
					while (-1 == idx_e)
					{
						if (rdLine.Length > idx_s)
						{
							wtLine = rdLine.Remove(idx_s).TrimEnd();
						}
						retList.Add(wtLine);
                        idx_s = 0;

						rdLine = tr.ReadLine();
						if (null == rdLine)
						{
							break;
						}
						idx_e = rdLine.IndexOf("*/");
					}
					if (-1 != idx_e)
					{
						rdLine = rdLine.Remove(idx_s, idx_e - idx_s + 2);
					}
					else
					{
						rdLine = "";
					}
					continue;
				}
				else
				{
					// 不包含注释
					wtLine = rdLine.TrimEnd();
				}
				retList.Add(wtLine);
				rdLine = tr.ReadLine();
				if (null == rdLine)
				{
					break;
				}
			} while (true);
			tr.Close();

			return retList;
		}

		/// <summary>
		/// 预编译处理
		/// </summary>
		public List<string> PrecompileProcess(	string file_name,
												List<string> codeList,
												ref FILE_PARSE_INFO fi)
		{
			List<string> retList = new List<string>();
			Stack<CONDITIONAL_COMPILATION_INFO> ccStack = new Stack<CONDITIONAL_COMPILATION_INFO>(); // 条件编译嵌套时, 用堆栈来保存嵌套的条件编译情报参数
			CONDITIONAL_COMPILATION_INFO cc_info = new CONDITIONAL_COMPILATION_INFO();

			string rdLine = "";
			for (int idx = 0; idx < codeList.Count; idx++)
			{
				rdLine = codeList[idx].Trim();
				if (rdLine.StartsWith("#"))
				{
					CODE_POSITION searchPos = new CODE_POSITION(idx, codeList[idx].IndexOf("#") + 1);
					CODE_POSITION foundPos = null;
                    CODE_IDENTIFIER nextIdtf = COMN_PROC.GetNextIdentifier(codeList, ref searchPos, out foundPos);
					if ("include" == nextIdtf.Text.ToLower())
					{
						if (false != cc_info.WriteFlag)
						{
							// 取得include文件名
							string incFileName = GetIncludeFileName(codeList, ref searchPos);
							System.Diagnostics.Trace.Assert(null != incFileName);
							if (!fi.IncFileList.Contains(incFileName))
							{
								fi.IncFileList.Add(incFileName);
							}
							ParseIncludeHeaderFile(incFileName, ref fi, file_name);
						}
					}
					else if ("define" == nextIdtf.Text.ToLower())
					{
						if (false != cc_info.WriteFlag)
						{
							DefineProcess(file_name, codeList, ref searchPos, ref fi);
						}
					}
					else if ("pragma" == nextIdtf.Text.ToLower())
					{
						nextIdtf = COMN_PROC.GetNextIdentifier(codeList, ref searchPos, out foundPos);
						if ("asm" == nextIdtf.Text.ToLower())
						{
							// C代码中嵌入了汇编命令(暂不处理)
							while (true)
							{
								rdLine = codeList[idx].Trim();
								if (-1 != rdLine.ToLower().IndexOf("#pragma endasm"))
								{
									retList.Add("");
									break;
								}
								else
								{
									retList.Add("");
									idx++;
								}
							}
							continue;	// 跳出循环处理下一行
						}
					}
					else
					{
						string exprStr = "";            // 表达式字符串
						if ("if" == nextIdtf.Text.ToLower())
						{
							bool lastFlag = cc_info.WriteFlag;
							ccStack.Push(cc_info);
							cc_info = new CONDITIONAL_COMPILATION_INFO();
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
                                exprStr = COMN_PROC.GetPrecompileExpressionStr(codeList, ref searchPos, out foundPos);
								// 表达式可能占多行(连行符)
								idx = searchPos.RowNum - 1;
								// 判断表达式的值
                                //if (0 != CommonProcess.JudgeExpressionValue(exprStr, fi.MacroDefineList))
								if (0 != ExpCalc.GetLogicalExpressionValue(exprStr, fi))
								{
									cc_info.WriteFlag = false;
									cc_info.WriteNextFlag = true;
								}
								else
								{
									cc_info.WriteFlag = false;
									cc_info.WriteNextFlag = false;
								}
							}
						}
						else if ("ifdef" == nextIdtf.Text.ToLower())
						{
							bool lastFlag = cc_info.WriteFlag;
							ccStack.Push(cc_info);
							cc_info = new CONDITIONAL_COMPILATION_INFO();
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
                                exprStr = COMN_PROC.GetPrecompileExpressionStr(codeList, ref searchPos, out foundPos);
								// 表达式可能占多行(连行符)
								idx = searchPos.RowNum - 1;
								// 判断表达式是否已定义
                                if (COMN_PROC.JudgeExpressionDefined(exprStr, fi))
								{
									cc_info.WriteFlag = false;
									cc_info.WriteNextFlag = true;
								}
								else
								{
									cc_info.WriteFlag = false;
									cc_info.WriteNextFlag = false;
								}
							}
						}
						else if ("ifndef" == nextIdtf.Text.ToLower())
						{
							bool lastFlag = cc_info.WriteFlag;
							ccStack.Push(cc_info);
							cc_info = new CONDITIONAL_COMPILATION_INFO();
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
                                exprStr = COMN_PROC.GetPrecompileExpressionStr(codeList, ref searchPos, out foundPos);
								// 表达式可能占多行(连行符)
								idx = searchPos.RowNum - 1;
								// 判断表达式是否已定义
                                if (COMN_PROC.JudgeExpressionDefined(exprStr, fi))
								{
									cc_info.WriteFlag = false;
									cc_info.WriteNextFlag = false;
								}
								else
								{
									cc_info.WriteFlag = false;
									cc_info.WriteNextFlag = true;
								}
							}
						}
						else if ("else" == nextIdtf.Text.ToLower())
						{
							bool lastFlag = true;
							if (ccStack.Count > 0)
							{
								lastFlag = ccStack.Peek().WriteFlag;
							}
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
								if (true == cc_info.WriteFlag)
								{
									cc_info.WriteFlag = false;
									cc_info.WriteNextFlag = false;
								}
								else
								{
									cc_info.WriteFlag = false;
									cc_info.WriteNextFlag = true;
								}
							}
						}
						else if ("elif" == nextIdtf.Text.ToLower())
						{
							bool lastFlag = true;
							if (ccStack.Count > 0)
							{
								lastFlag = ccStack.Peek().WriteFlag;
							}
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
								// 跟"if"一样, 但是因为不是嵌套所以不用压栈
                                exprStr = COMN_PROC.GetPrecompileExpressionStr(codeList, ref searchPos, out foundPos);
								// 表达式可能占多行(连行符)
								idx = searchPos.RowNum - 1;
								// 判断表达式的值
                                //if (0 != CommonProcess.JudgeExpressionValue(exprStr, fi.MacroDefineList))
								if (0 != ExpCalc.GetLogicalExpressionValue(exprStr, fi))
								{
									cc_info.WriteFlag = false;
									cc_info.WriteNextFlag = true;
								}
								else
								{
									cc_info.WriteFlag = false;
									cc_info.WriteNextFlag = false;
								}
							}
						}
						else if ("endif" == nextIdtf.Text.ToLower())
						{
							bool lastFlag = true;
							if (ccStack.Count > 0)
							{
								lastFlag = ccStack.Peek().WriteFlag;
							}
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = true;
							}

							cc_info.PopUpStack = true;
						}
						else
						{
						}
					}
				}

				if (cc_info.WriteFlag)
				{
					retList.Add(codeList[idx]);
				}
				else
				{
					retList.Add("");
				}
				if (true == cc_info.WriteNextFlag)
				{
					cc_info.WriteFlag = true;
					cc_info.WriteNextFlag = false;
				}

				// 嵌套时弹出堆栈, 恢复之前的情报
				if (true == cc_info.PopUpStack)
				{
					cc_info = ccStack.Pop();
				}
			}

			return retList;
		}

		/// <summary>
		/// 取得include头文件的解析情报
		/// </summary>
		void ParseIncludeHeaderFile(string inc_name,
									ref FILE_PARSE_INFO fi,
									string file_name)
		{
			if (	(inc_name.StartsWith("\"") && inc_name.EndsWith("\""))				// 双引号
				||	(inc_name.StartsWith("<") && inc_name.EndsWith(">"))	)			// 尖括号
			{
				// 去掉引号或者尖括号
				inc_name = inc_name.Substring(1, inc_name.Length - 2).Trim();
				string headerName = GetActualHeadrFullName(inc_name, file_name);
				if (!string.IsNullOrEmpty(headerName))
				{
					CFileProcess(headerName, ref fi);
				}
				else
				{
					// Error Log Here!
					string errLog = "<<< Error LOG >>> : ParseIncludeHeaderFile(..) : " + file_name + " Can't Find Include Header File : " + inc_name;
					this.ErrorLogList.Add(errLog);
					System.Diagnostics.Trace.WriteLine(errLog);
				}
			}
		}

		/// <summary>
		/// 取得include头文件的实际路径
		/// </summary>
		string GetActualHeadrFullName(string inc_name, string src_name)
		{
			if (	-1 != inc_name.IndexOf('/')
				||	-1 != inc_name.IndexOf('\\')	)
			{
				// 包含相对路径的场合
				FileInfo fInfo = new FileInfo(src_name);
				string str = Directory.GetCurrentDirectory();
				Directory.SetCurrentDirectory(fInfo.DirectoryName);
				FileInfo hInfo = new FileInfo(inc_name);
				Directory.SetCurrentDirectory(str);										// 恢复当前路径
				if (!hInfo.Exists)
				{
					return string.Empty;
				}
				string headerName = hInfo.FullName;
				foreach (var hd_name in this.HeaderList)
				{
					if (headerName.Equals(hd_name))
					{
						return hd_name;
					}
				}
			}
			else
			{
				List<string> findHeaderList = new List<string>();
				// 不包含相对路径的场合
				foreach (var hd_name in this.HeaderList)
				{
					string path;
					string fName = IOProcess.GetFileName(hd_name, out path);
					if (fName.ToLower() == inc_name.ToLower())
					{
						// TODO: 注意当有多个同名文件符合条件时的情况应对
						findHeaderList.Add(hd_name);
					}
				}
				if (1 == findHeaderList.Count)
				{
					return findHeaderList[0];
				}
				else if (findHeaderList.Count > 1)
				{
					// 找出路径最接近的那个
					string retHeader = string.Empty;
					int equalCnt = 0;
					foreach (var hd_name in findHeaderList)
					{
						string hdPath, srcPath;
						IOProcess.GetFileName(hd_name, out hdPath);
						IOProcess.GetFileName(src_name, out srcPath);
						int cnt = CompStrSameCount(hdPath, srcPath);
						if (cnt > equalCnt)
						{
							retHeader = hd_name;
							equalCnt = cnt;
						}
					}
					return retHeader;
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// 找出两个字符串从头开始连续一致字符的个数
		/// </summary>
		/// <returns></returns>
		int CompStrSameCount(string str1, string str2)
		{
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2));
			int len = 0;
			if (str1.Length < str2.Length)
			{
				len = str1.Length;
			}
			else
			{
				len = str2.Length;
			}
			int retCnt = 0;
			for (int i = 0; i < len; i++)
			{
				if (str1[i].Equals(str2[i]))
				{
					retCnt += 1;
				}
				else
				{
					break;
				}
			}
			return retCnt;
		}

		/// <summary>
		/// 文件代码解析
		/// </summary>
		public static bool CCodeFileAnalysis(string src_name,
											 ref FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert((null != parse_info.CodeList));
			if (0 == parse_info.CodeList.Count)
			{
                COMN_PROC.ErrReport();
				return false;
			}
			// 从文件开头开始解析
			CODE_POSITION search_pos = new CODE_POSITION(0, 0);
			List<CODE_IDENTIFIER> qualifierList = new List<CODE_IDENTIFIER>();			// 修饰符暂存列表
			CODE_IDENTIFIER nextIdtf = null;
			CODE_POSITION foundPos = null;
			while (null != (nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref search_pos, out foundPos)))
			{
				//if (src_name.EndsWith("gerdaC_dd.h") && nextIdtf.Position.RowNum > 340)
				//{
				//	int a = 100;
				//}
				// 如果是标准标识符(字母,数字,下划线组成且开头不是数字)
				if (COMN_PROC.IsStandardIdentifier(nextIdtf.Text)
					|| ("*" == nextIdtf.Text))
				{
					if (COMN_PROC.MacroDetectAndExpand_File(nextIdtf.Text, foundPos, parse_info))
					{
						// 判断是否是已定义的宏, 是的话进行宏展开
						// 展开后要返回到原处(展开前的位置), 重新解析展开后的宏
						search_pos = new CODE_POSITION(foundPos);
						continue;
					}
					else if (UserDefTypeExpand(nextIdtf.Text, parse_info.CodeList, foundPos, parse_info, qualifierList))
					{
						search_pos = new CODE_POSITION(foundPos);
						continue;
					}
					qualifierList.Add(nextIdtf);
					if (COMN_PROC.IsUsrDefTypeKWD(nextIdtf.Text))
					{
						// 用户定义类型处理
						USER_DEFINE_TYPE_INFO udti = UsrDefTypeProc(parse_info, qualifierList, ref search_pos);
						if (null != udti)
						{
							parse_info.UsrDefTypeList.Add(udti);
						}
					}
				}
				// 否则可能是各种符号
				else
				{
					// 遇到小括号了, 可能是碰上函数声明或定义了
					if (("(" == nextIdtf.Text) && (0 != qualifierList.Count))
					{
						FUNCTION_PARSE_INFO cfi = FunctionDetectProcess(src_name, parse_info, qualifierList, ref search_pos, foundPos);
						if (null != cfi)
						{
							if (null != cfi.Scope)
							{
								parse_info.FunDefineList.Add(cfi);
							}
							else
							{
								parse_info.FuncDeclareList.Add(cfi);
							}
						}
						else
						{
							// TODO(20170103): 这里有个全局函数指针不能处理, 暂时跳过了, 回头要对应
							// \MTbot\TestData\Honda18HMI_soft\software\src\can\drv\FCanDrv
							// fcan_drv_api.c 行号: 133
							foundPos = COMN_PROC.FindNextSpecIdentifier(";", parse_info.CodeList, search_pos);
						}
					}
					else if ("#" == nextIdtf.Text)
					{
						// 预编译命令, 因为已经处理过了, 不在这里解析, 跳到宏定义结束
						while (parse_info.CodeList[search_pos.RowNum].EndsWith("\\"))
						{
							search_pos.RowNum += 1;
						}
						search_pos.ColNum = parse_info.CodeList[search_pos.RowNum].Length;
					}
					// 全局量(包含全局数组)
					else if ("[" == nextIdtf.Text)
					{
						// 到下一个"]"出现的位置是数组长度
						CODE_POSITION fp = COMN_PROC.FindNextSymbol(parse_info.CodeList, search_pos, ']');
						if (null != fp)
						{
							string arraySizeStr = COMN_PROC.LineStringCat(parse_info.CodeList, foundPos, fp);
							CODE_IDENTIFIER arraySizeIdtf = new CODE_IDENTIFIER(arraySizeStr, foundPos);
							qualifierList.Add(arraySizeIdtf);
							fp.ColNum += 1;
							search_pos = fp;
							continue;
						}
						else
						{
							return false;
						}
					}
					else if ("=" == nextIdtf.Text)
					{
						// 直到下一个分号出现的位置, 都是初始化语句
						qualifierList.Add(nextIdtf);
						CODE_POSITION fp = COMN_PROC.FindNextSymbol(parse_info.CodeList, search_pos, ';');
						if (null != fp)
						{
							foundPos.ColNum += 1;
							string initialStr = COMN_PROC.LineStringCat(parse_info.CodeList, foundPos, fp);
							CODE_IDENTIFIER initIdtf = new CODE_IDENTIFIER(initialStr, foundPos);
							qualifierList.Add(initIdtf);
							search_pos = fp;
							continue;
						}
						else
						{
							return false;
						}
					}
					else if (";" == nextIdtf.Text && 0 != qualifierList.Count)
					{
						string statementStr = string.Empty;
						if ("typedef" == qualifierList[0].Text)
						{
							StringBuilder sb = new StringBuilder();
							for (int i = 0; i < qualifierList.Count; i++)
							{
								sb.Append(qualifierList[i].Text + " ");
							}
							statementStr = sb.ToString();
						}
						else
						{
							// TODO: SimpleStatementAnalyze替换GlobalVarProcess
							//GlobalVarProcess(qualifierList, ref parse_info);

							statementStr = StatementAnalysis.GetStatementStr(parse_info.CodeList,
								new CODE_SCOPE(qualifierList.First().Position, nextIdtf.Position));
						}
						StatementAnalysis.SimpleStatementAnalyze(statementStr.Trim(), parse_info, null);
					}
					else if ("," == nextIdtf.Text)
					{
						qualifierList.Add(nextIdtf);
						continue;
					}
					else
					{
					}
					qualifierList.Clear();
				}
			}
			return true;
		}

		/// <summary>
		/// 函数检测(声明, 定义)
		/// </summary>
		static FUNCTION_PARSE_INFO FunctionDetectProcess(	string src_name,
													FILE_PARSE_INFO parse_info,
													List<CODE_IDENTIFIER> qualifierList,
													ref CODE_POSITION searchPos,
													CODE_POSITION bracketLeft)
		{
			if (0 == qualifierList.Count
				|| "typedef" == qualifierList.First().Text)
			{
				return null;
			}
			FUNCTION_PARSE_INFO cfi = new FUNCTION_PARSE_INFO();

			// 先找匹配的小括号
			CODE_POSITION bracketRight = COMN_PROC.FindNextMatchSymbol(parse_info, ref searchPos, ')');
			if (null == bracketRight)
			{
                COMN_PROC.ErrReport();
				return null;
			}
			if (parse_info.CodeList[bracketLeft.RowNum].Substring(bracketLeft.ColNum + 1).Trim().StartsWith("*"))
			{
				// 吗呀, 这不是传说中的函数指针嘛...
				CODE_POSITION sp = bracketLeft;
				CODE_POSITION ep = bracketRight;
				sp.ColNum += 1;
				ep.ColNum -= 1;
				bracketLeft = COMN_PROC.FindNextSymbol(parse_info.CodeList, bracketRight, '(');
				if (null == searchPos)
				{
                    COMN_PROC.ErrReport();
					return null;
				}
				bracketRight = COMN_PROC.FindNextSymbol(parse_info.CodeList, searchPos, ')');
				if (null == bracketRight)
				{
                    COMN_PROC.ErrReport();
					return null;
				}
				string nameStr = COMN_PROC.LineStringCat(parse_info.CodeList, sp, ep);
				cfi.Name = nameStr;
			}
			List<string> paraList = COMN_PROC.GetParaList(parse_info.CodeList, bracketLeft, bracketRight);

			// 然后确认小括号后面是否跟着配对的大括号
			searchPos = new CODE_POSITION(bracketRight.RowNum, bracketRight.ColNum + 1);
			CODE_POSITION foundPos = null;
			CODE_IDENTIFIER nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
			if (";" == nextIdtf.Text)
			{
				// 小括号后面跟着分号说明这是函数声明
				// 函数名
				if ("" == cfi.Name)
				{
					cfi.Name = qualifierList.Last().Text;
					// 函数修饰符
					qualifierList.RemoveAt(qualifierList.Count - 1);
				}
				cfi.Qualifiers = new List<CODE_IDENTIFIER>(qualifierList);
				// 参数列表
				cfi.ParaList = paraList;
			}
			else if ("{" == nextIdtf.Text)
			{
				CODE_POSITION bodyStartPos = foundPos;
                // 小括号后面跟着配对的大括号说明这是函数定义(带函数体)
				CODE_POSITION fp = COMN_PROC.FindNextMatchSymbol(parse_info, ref searchPos, '}');
				if (null == fp)
				{
					throw new MyException("FunctionDetectProcess(...) 没找到匹配的花括号!");
				}
				searchPos = new CODE_POSITION(fp.RowNum, fp.ColNum + 1);
				// 函数名
				cfi.Name = qualifierList.Last().Text;
				// 函数修饰符
				qualifierList.RemoveAt(qualifierList.Count - 1);
				cfi.Qualifiers = qualifierList;
				// 参数列表
				cfi.ParaList = paraList;
				// 函数体起始位置
				cfi.Scope = new CODE_SCOPE(bodyStartPos, fp);
			}
			else
			{
				// 对应把参数声明列表写在小括号外, 第一个花括号前的古典写法.
				int row = searchPos.RowNum;
				CODE_POSITION bodyStartPos = COMN_PROC.FindNextSymbol(parse_info.CodeList, searchPos, '{');
				if (null == bodyStartPos
					|| bodyStartPos.RowNum - row > 10)
				{
					// TODO 20170206: 这里加了一个保护, 要详查出错的具体原因, 加强对参数声明列表合法性的检查
					return null;
				}
				searchPos = COMN_PROC.PositionMoveNext(parse_info.CodeList, bodyStartPos);
				CODE_POSITION fp = COMN_PROC.FindNextMatchSymbol(parse_info, ref searchPos, '}');
				if (null == fp)
				{
					COMN_PROC.ErrReport();
					return null;
				}
				searchPos = new CODE_POSITION(fp.RowNum, fp.ColNum + 1);
				// 函数名
				cfi.Name = qualifierList.Last().Text;
				// 函数修饰符
				qualifierList.RemoveAt(qualifierList.Count - 1);
				cfi.Qualifiers = qualifierList;
				// 参数列表
				cfi.ParaList = COMN_PROC.GetParaList(parse_info.CodeList,
														 COMN_PROC.PositionMoveNext(parse_info.CodeList, bracketRight),
														 COMN_PROC.PositionMovePrevious(parse_info.CodeList, bodyStartPos));
				// 函数体起始位置
				cfi.Scope = new CODE_SCOPE(bodyStartPos, fp);
				return cfi;
			}
			// 更新index
			return cfi;
		}

		/// <summary>
		/// 取得包含头文件名
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="searchPos"></param>
		/// <returns></returns>
		static string GetIncludeFileName(List<string> codeList, ref CODE_POSITION searchPos)
		{
			CODE_POSITION foundPos = null;
            CODE_IDENTIFIER quotIdtf = COMN_PROC.GetNextIdentifier(codeList, ref searchPos, out foundPos);
			string retName = quotIdtf.Text;
			if ("\"" == quotIdtf.Text)
			{
			}
			else if ("<" == quotIdtf.Text)
			{
				quotIdtf.Text = ">";
			}
			else
			{
                COMN_PROC.ErrReport();
				return null;
			}
            CODE_IDENTIFIER nextIdtf = COMN_PROC.GetNextIdentifier(codeList, ref searchPos, out foundPos);
			while (quotIdtf.Text != nextIdtf.Text)
			{
				retName += nextIdtf.Text;
				nextIdtf = COMN_PROC.GetNextIdentifier(codeList, ref searchPos, out foundPos);
			}
			//int idx;
			//if (-1 != (idx = retName.LastIndexOf('/')))
			//{
			//	retName = retName.Remove(1, idx).Trim();
			//}
			return retName + quotIdtf.Text;
		}

		/// <summary>
		/// 用户定义类型处理
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="startPos"></param>
		/// <param varName="qualifierList"></param>
		/// <returns></returns>
		static USER_DEFINE_TYPE_INFO UsrDefTypeProc(FILE_PARSE_INFO parse_info,
											 List<CODE_IDENTIFIER> qualifier_list,
											 ref CODE_POSITION start_pos)
		{
			if (0 == qualifier_list.Count)
			{
                COMN_PROC.ErrReport();
				return null;
			}
			string keyStr = qualifier_list.Last().Text;

			USER_DEFINE_TYPE_INFO retUsrTypeInfo = new USER_DEFINE_TYPE_INFO();
			CODE_POSITION searchPos = new CODE_POSITION(start_pos);
			CODE_POSITION foundPos = null;
			CODE_IDENTIFIER nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
			if ("{" != nextIdtf.Text)
			{
				if (COMN_PROC.IsStandardIdentifier(nextIdtf.Text))
				{
					retUsrTypeInfo.NameList.Add(nextIdtf);
					nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
					if ("{" != nextIdtf.Text)
					{
						// 没找到最大括号, 说明这不是一个新的用户定义类型
						if ((0 != qualifier_list.Count)
							&& ("typedef" == qualifier_list[0].Text))
						{
							// 说明这是一个typedef类型定义, 这里返回不做处理, 等后面解析到分号";"的时候再进行typedef类型定义处理
						}
						return null;
					}
				}
				else
				{
                    COMN_PROC.ErrReport();
					return null;
				}
			}
			CODE_POSITION startPos = foundPos;
			foundPos = COMN_PROC.FindNextMatchSymbol(parse_info, ref searchPos, '}');
			if (null == foundPos)
			{
                COMN_PROC.ErrReport();
				return null;
			}
			CODE_POSITION endPos = foundPos;
			retUsrTypeInfo.Scope = new CODE_SCOPE(startPos, endPos);
			string catStr = COMN_PROC.LineStringCat(parse_info.CodeList, retUsrTypeInfo.Scope.Start, retUsrTypeInfo.Scope.End);
			if (catStr.StartsWith("{"))
			{
				catStr = catStr.Remove(0, 1);
			}
			if (catStr.EndsWith("}"))
			{
				catStr = catStr.Remove(catStr.LastIndexOf('}'));
			}
			char sepStr = ';';
			retUsrTypeInfo.Category = keyStr;
			if ("enum" == keyStr)
			{
				sepStr = ',';
			}
			string[] members = catStr.Split(sepStr);
			if (0 == members.Length)
			{
                COMN_PROC.ErrReport();
				return null;
			}
			foreach (string m in members)
			{
				string memStr = GetUsrDefTypeMemberStr(m.Trim(), parse_info);
				if (string.Empty != memStr)
				{
					retUsrTypeInfo.MemberList.Add(memStr);
				}
			}
			// 如果是匿名类型, 要给加个名字
			if (0 == retUsrTypeInfo.NameList.Count)
			{
				// 取得匿名类型的名字
				CODE_IDENTIFIER idtf = new CODE_IDENTIFIER(GetAnonymousTypeName(parse_info), null);
				retUsrTypeInfo.NameList.Add(idtf);
			}
			qualifier_list.Add(retUsrTypeInfo.NameList[0]);
			// 检查后面是否跟着分号(判断类型定义结束)
			CODE_POSITION old_pos = new CODE_POSITION(searchPos);
			nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
			if (";" == nextIdtf.Text)
			{
				// 如果后面是分号说明该类型定义结束, 检索位置要跳过该分号
				qualifier_list.Clear();
			}
			else
			{
				// 否则可能是以该类型定义全局变量, 恢复检索位置到右括号后面
				searchPos = old_pos;
			}
			start_pos = searchPos;

			return retUsrTypeInfo;
		}

		/// <summary>
		/// 取得自定义类型表示成员的字符串
		/// </summary>
		/// <returns></returns>
		static string GetUsrDefTypeMemberStr(string in_mem_str,
											 FILE_PARSE_INFO source_info)
		{
			string memberStr = in_mem_str;
			string idStr;
			int offset = 0, old_offset;
			// 可能包含宏, 所有要检测宏, 有的话要做宏展开
			while (true)
			{
				old_offset = offset;
				idStr = COMN_PROC.GetNextIdentifier2(memberStr, ref offset);
				if (null == idStr)
				{
					break;
				}
				else if (COMN_PROC.IsStandardIdentifier(idStr)
						 && StatementAnalysis.MacroDetectAndExpand_Statement(idStr, ref memberStr, offset, source_info, true))
				{
					offset = old_offset;
					continue;
				}
				else
				{
					offset += idStr.Length;
				}
			}
			return memberStr.Trim();
		}

		/// <summary>
		/// 宏定义处理
		/// </summary>
		static void DefineProcess(string file_name, List<string> codeList, ref CODE_POSITION searchPos, ref FILE_PARSE_INFO cfi)
		{
			CODE_POSITION sPos, fPos;
			sPos = new CODE_POSITION(searchPos);
            CODE_IDENTIFIER nextIdtf = COMN_PROC.GetNextIdentifier(codeList, ref sPos, out fPos);
			if (!COMN_PROC.IsStandardIdentifier(nextIdtf.Text))
			{
				return;
			}
			MACRO_DEFINE_INFO mdi = new MACRO_DEFINE_INFO(file_name, nextIdtf.Position.RowNum);
			mdi.Name = nextIdtf.Text;
			if (sPos.RowNum > nextIdtf.Position.RowNum)
			{
				// 换行了, 说明到行末结尾了, 宏定义已经结束
				cfi.MacroDefineList.Add(mdi);
			}
			else
			{
				string leftStr = codeList[sPos.RowNum].Substring(sPos.ColNum);
				if (leftStr.StartsWith("("))
				{
					CODE_POSITION ePos = COMN_PROC.FindNextSymbol(codeList, sPos, ')');
					if (null == ePos)
					{
						return;
					}
					mdi.ParaList = COMN_PROC.GetParaList(codeList, sPos, ePos);
					sPos = ePos;
					sPos.ColNum += 1;
				}

				string defineValStr = codeList[sPos.RowNum].Substring(sPos.ColNum);
				while (defineValStr.EndsWith(@"\"))
				{
					defineValStr = defineValStr.Remove(defineValStr.Length - 1);
					sPos.RowNum += 1;
					sPos.ColNum = 0;
					defineValStr += codeList[sPos.RowNum].Substring(sPos.ColNum);
				}
				mdi.Value = defineValStr.Trim();
				cfi.MacroDefineList.Add(mdi);

				sPos.RowNum += 1;
				sPos.ColNum = 0;
				searchPos = new CODE_POSITION(sPos);
			}
		}

		static bool UserDefTypeExpand(	string idStr,
										List<string> codeList,
										CODE_POSITION foundPos,
										FILE_PARSE_INFO curFileInfo,
										List<CODE_IDENTIFIER> qualifierList)
		{
			if (0 != qualifierList.Count && COMN_PROC.IsUsrDefTypeKWD(qualifierList.Last().Text))
			{
				// 这样做是为了避免遇到 typedef struct A { xxx } A; 的情况
				// 在这种情况下展开后再遇到A还会再次展开, 导致进入死循环
				return false;
			}
			// typedef 用户自定义类型
			foreach (TYPE_DEFINE_INFO tdi in curFileInfo.TypeDefineList)
			{
				if (idStr == tdi.NewName)
				{
					string usrTypeName = tdi.NewName;
					CODE_POSITION macroPos = new CODE_POSITION(foundPos);
					int lineIdx = foundPos.RowNum;
					string realTypeName = tdi.OldName;
					// 用原类型名去替换用户定义类型名
					codeList[lineIdx] = codeList[lineIdx].Replace(usrTypeName, realTypeName);
					return true;
				}
			}
			return false;
		}

        static string GetAnonymousTypeName(FILE_PARSE_INFO parse_info)
        {
            string fn, path;
            fn = IOProcess.GetFileName(parse_info.SourceName, out path);

            string retName = fn.Replace('.', '_').ToUpper() + "_USR_DEF_TYPE_" + parse_info.UsrDefTypeList.Count.ToString();
			for (int i = 0; i < retName.Length; i++)
			{
				// 文件名里可能有:"$", "~"之类的字符, 所以不能用作返回的类型名
				if (!Char.IsLetterOrDigit(retName[i])
					&& '_' != retName[i])
				{
					retName = retName.Remove(i, 1);
					retName = retName.Insert(i, "_");
				}
			}

            return retName;
        }
	}
}
