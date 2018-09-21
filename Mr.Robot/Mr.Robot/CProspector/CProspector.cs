using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

using Mr.Robot.MacroSwitchAnalyser;
using Mr.Robot.CDeducer;

namespace Mr.Robot
{
	public partial class C_PROSPECTOR
	{
		#region 全局字段
		List<string> HeaderList = new List<string>();									// 头文件名列表
		List<string> SourceList = new List<string>();									// 源文件名列表

		public EventHandler UpdateProgressHandler = null;
		public List<FILE_PARSE_INFO> ParseInfoList = null;

		public bool MacroSwichAnalyserFlag = false;										// 这个标志是给宏开关分析工具设的, 做宏开关分析时, 为了提高速度, 只分析头文件宏定义等..
																						// 不做语句分析
		public List<string> ErrorLogList = new List<string>();

		CodeBufferManager _CodeBufferManagerRef = null;
		#endregion

		public C_PROSPECTOR(List<string> source_list, List<string> header_list, CodeBufferManager code_buffer_mamager_ref = null)
		{
			this.SourceList = source_list;
			this.HeaderList = header_list;
			this._CodeBufferManagerRef = code_buffer_mamager_ref;
		}

		#region 向外提供的接口方法
		public void AsyncStart()
		{
			Thread workerThread = new Thread(new ThreadStart(ProcessMain));
			workerThread.Start();
			//Task t1 = new Task(CFileListProcess);
			//t1.Start();
			//t1.Wait();
		}

		void ProcessMain()
		{
			SyncStart();
		}

		public List<FILE_PARSE_INFO> SyncStart()
		{
			this.ParseInfoList = new List<FILE_PARSE_INFO>();
			// 逐个解析源文件
			foreach (string srcName in this.SourceList)
			{
				FILE_PARSE_INFO parseInfo = new FILE_PARSE_INFO(srcName);
				if (CFileProcess(srcName, parseInfo))
				{
					this.ParseInfoList.Add(parseInfo);
				}
			}
			return this.ParseInfoList;
		}
		#endregion

		// 以下都是内部调用方法
		/// <summary>
		/// C文件(包括源文件和头文件)处理
		/// </summary>
		bool CFileProcess(string file_name, FILE_PARSE_INFO parse_info)
		{
			//System.Diagnostics.Trace.WriteLine(file_name + ": ");
			try
			{
				// 预编译处理(宏定义)
				parse_info.CodeList = PrecompileProcess(file_name, parse_info);
				if (this.MacroSwichAnalyserFlag)
				{
					return true;
				}

				// 文件解析(全局量,函数定位)
				if (!CCodeFileAnalysis(file_name, parse_info))
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
		/// 预编译处理
		/// </summary>
		public List<string> PrecompileProcess(	string file_name,
												FILE_PARSE_INFO parse_info)
		{
			List<string> code_list = GetCodeList(file_name);
			List<string> ret_list = new List<string>();
			Stack<CONDITIONAL_COMPILATION_INFO> cc_stack = new Stack<CONDITIONAL_COMPILATION_INFO>(); // 条件编译嵌套时, 用堆栈来保存嵌套的条件编译情报参数
			CONDITIONAL_COMPILATION_INFO cc_info = new CONDITIONAL_COMPILATION_INFO();

			string code_line = "";
			for (int idx = 0; idx < code_list.Count; idx++)
			{
				code_line = code_list[idx].Trim();
				if (code_line.StartsWith("#"))
				{
					CodePosition search_pos = new CodePosition(idx, code_list[idx].IndexOf("#") + 1);
					CodePosition found_pos = null;
                    CODE_IDENTIFIER next_identifier
										= COMN_PROC.GetNextIdentifier(	code_list,
																		ref search_pos,
																		out found_pos);
					if ("include" == next_identifier.Text.ToLower()
						&& cc_info.WriteFlag)
					{
						IncludeHeaderProc(code_list, file_name, ref search_pos, parse_info);
					}
					else if ("define" == next_identifier.Text.ToLower()
							 && cc_info.WriteFlag)
					{
						DefineProcess(file_name, code_list, ref search_pos, parse_info);
					}
					else if ("pragma" == next_identifier.Text.ToLower())
					{
						next_identifier = COMN_PROC.GetNextIdentifier(code_list, ref search_pos, out found_pos);
						if ("asm" == next_identifier.Text.ToLower())
						{
							// C代码中嵌入了汇编命令(暂不处理)
							while (true)
							{
								code_line = code_list[idx].Trim();
								if (-1 != code_line.ToLower().IndexOf("#pragma endasm"))
								{
									ret_list.Add("");
									break;
								}
								else
								{
									ret_list.Add("");
									idx++;
								}
							}
							continue;	// 跳出循环处理下一行
						}
					}
					else
					{
						string exprStr = "";            // 表达式字符串
						if ("if" == next_identifier.Text.ToLower())
						{
							bool lastFlag = cc_info.WriteFlag;
							cc_stack.Push(cc_info);
							cc_info = new CONDITIONAL_COMPILATION_INFO();
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
                                exprStr = COMN_PROC.GetPrecompileExpressionStr(code_list, ref search_pos);
								// 表达式可能占多行(连行符)
								idx = search_pos.RowNum - 1;
								// 判断表达式的值
								if (0 != ExpCalc.GetConstExpressionValue(exprStr, parse_info))
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
						else if ("ifdef" == next_identifier.Text.ToLower())
						{
							bool lastFlag = cc_info.WriteFlag;
							cc_stack.Push(cc_info);
							cc_info = new CONDITIONAL_COMPILATION_INFO();
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
                                exprStr = COMN_PROC.GetPrecompileExpressionStr(code_list, ref search_pos);
								// 表达式可能占多行(连行符)
								idx = search_pos.RowNum - 1;
								// 判断表达式是否已定义
                                if (COMN_PROC.JudgeExpressionDefined(exprStr, parse_info))
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
						else if ("ifndef" == next_identifier.Text.ToLower())
						{
							bool lastFlag = cc_info.WriteFlag;
							cc_stack.Push(cc_info);
							cc_info = new CONDITIONAL_COMPILATION_INFO();
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
                                exprStr = COMN_PROC.GetPrecompileExpressionStr(code_list, ref search_pos);
								// 表达式可能占多行(连行符)
								idx = search_pos.RowNum - 1;
								// 判断表达式是否已定义
                                if (COMN_PROC.JudgeExpressionDefined(exprStr, parse_info))
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
						else if ("else" == next_identifier.Text.ToLower())
						{
							bool lastFlag = true;
							if (cc_stack.Count > 0)
							{
								lastFlag = cc_stack.Peek().WriteFlag;
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
						else if ("elif" == next_identifier.Text.ToLower())
						{
							bool lastFlag = true;
							if (cc_stack.Count > 0)
							{
								lastFlag = cc_stack.Peek().WriteFlag;
							}
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
								// 跟"if"一样, 但是因为不是嵌套所以不用压栈
                                exprStr = COMN_PROC.GetPrecompileExpressionStr(code_list, ref search_pos);
								// 表达式可能占多行(连行符)
								idx = search_pos.RowNum - 1;
								// 判断表达式的值
								if (0 != ExpCalc.GetConstExpressionValue(exprStr, parse_info))
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
						else if ("endif" == next_identifier.Text.ToLower())
						{
							bool lastFlag = true;
							if (cc_stack.Count > 0)
							{
								lastFlag = cc_stack.Peek().WriteFlag;
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
					ret_list.Add(code_list[idx]);
				}
				else
				{
					ret_list.Add("");
				}
				if (true == cc_info.WriteNextFlag)
				{
					cc_info.WriteFlag = true;
					cc_info.WriteNextFlag = false;
				}

				// 嵌套时弹出堆栈, 恢复之前的情报
				if (true == cc_info.PopUpStack)
				{
					cc_info = cc_stack.Pop();
				}
			}

			return ret_list;
		}

		List<string> GetCodeList(string file_name)
		{
			if (null != this._CodeBufferManagerRef)
			{
				return this._CodeBufferManagerRef.GetCodeList(file_name);
			}
			else
			{
				return COMN_PROC.RemoveComments(file_name);
			}
		}

		void IncludeHeaderProc(	List<string> code_list,
								string file_name,
								ref CodePosition search_pos,
								FILE_PARSE_INFO parse_info)
		{
			// 取得include文件名
			string header_name = GetIncludeHeaderName(code_list, ref search_pos, file_name);
			if (!string.IsNullOrEmpty(header_name))
			{
				if (!parse_info.IncFileList.Contains(header_name))
				{
					parse_info.IncFileList.Add(header_name);
				}
				CFileProcess(header_name, parse_info);
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
											 FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert((null != parse_info.CodeList));
			if (0 == parse_info.CodeList.Count)
			{
                COMN_PROC.ErrReport();
				return false;
			}
			// 从文件开头开始解析
			CodePosition search_pos = new CodePosition(0, 0);
			List<CODE_IDENTIFIER> qualifierList = new List<CODE_IDENTIFIER>();			// 修饰符暂存列表
			CODE_IDENTIFIER nextIdtf = null;
			CodePosition foundPos = null;
			while (null != (nextIdtf = COMN_PROC.GetNextIdentifier(parse_info.CodeList, ref search_pos, out foundPos)))
			{
				// 如果是标准标识符(字母,数字,下划线组成且开头不是数字)
				if (COMN_PROC.IsStandardIdentifier(nextIdtf.Text)
					|| ("*" == nextIdtf.Text))
				{
					if (COMN_PROC.MacroDetectAndExpand_File(nextIdtf.Text, foundPos, parse_info))
					{
						// 判断是否是已定义的宏, 是的话进行宏展开
						// 展开后要返回到原处(展开前的位置), 重新解析展开后的宏
						search_pos = new CodePosition(foundPos);
						continue;
					}
					else if (UserDefTypeExpand(nextIdtf.Text, foundPos, parse_info, qualifierList))
					{
						search_pos = new CodePosition(foundPos);
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
							search_pos.Move2HeadOfNextRow();
						}
						search_pos.ColNum = parse_info.CodeList[search_pos.RowNum].Length;
					}
					// 全局量(包含全局数组)
					else if ("[" == nextIdtf.Text)
					{
						// 到下一个"]"出现的位置是数组长度
						CodePosition fp = COMN_PROC.FindNextSymbol(parse_info.CodeList, search_pos, ']');
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
						CodePosition fp = COMN_PROC.FindNextSymbol(parse_info.CodeList, search_pos, ';');
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
							statementStr = COMN_PROC.GetStatementStr(parse_info.CodeList,
								new CodeScope(qualifierList.First().Position, nextIdtf.Position));
						}
						C_DEDUCER.SimpleStatementProc(statementStr.Trim(), parse_info, null, null, null);
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
															ref CodePosition searchPos,
															CodePosition bracketLeft)
		{
			if (0 == qualifierList.Count
				|| "typedef" == qualifierList.First().Text)
			{
				return null;
			}
			FUNCTION_PARSE_INFO cfi = new FUNCTION_PARSE_INFO();

			// 先找匹配的小括号
			CodePosition bracketRight = COMN_PROC.FindNextMatchSymbol(parse_info, ref searchPos, ')');
			if (null == bracketRight)
			{
                COMN_PROC.ErrReport();
				return null;
			}
			if (parse_info.CodeList[bracketLeft.RowNum].Substring(bracketLeft.ColNum + 1).Trim().StartsWith("*"))
			{
				// 吗呀, 这不是传说中的函数指针嘛...
				CodePosition sp = bracketLeft;
				CodePosition ep = bracketRight;
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
			searchPos = new CodePosition(bracketRight.RowNum, bracketRight.ColNum + 1);
			CodePosition foundPos = null;
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
				CodePosition bodyStartPos = foundPos;
                // 小括号后面跟着配对的大括号说明这是函数定义(带函数体)
				CodePosition fp = COMN_PROC.FindNextMatchSymbol(parse_info, ref searchPos, '}');
				if (null == fp)
				{
					throw new MyException("FunctionDetectProcess(...) 没找到匹配的花括号!");
				}
				searchPos = new CodePosition(fp.RowNum, fp.ColNum + 1);
				// 函数名
				cfi.Name = qualifierList.Last().Text;
				// 函数修饰符
				qualifierList.RemoveAt(qualifierList.Count - 1);
				cfi.Qualifiers = qualifierList;
				// 参数列表
				cfi.ParaList = paraList;
				// 函数体起始位置
				cfi.Scope = new CodeScope(bodyStartPos, fp);
			}
			else
			{
				// 对应把参数声明列表写在小括号外, 第一个花括号前的古典写法.
				int row = searchPos.RowNum;
				CodePosition bodyStartPos = COMN_PROC.FindNextSymbol(parse_info.CodeList, searchPos, '{');
				if (null == bodyStartPos
					|| bodyStartPos.RowNum - row > 10)
				{
					// TODO 20170206: 这里加了一个保护, 要详查出错的具体原因, 加强对参数声明列表合法性的检查
					return null;
				}
				searchPos = COMN_PROC.PositionMoveNext(parse_info.CodeList, bodyStartPos);
				CodePosition fp = COMN_PROC.FindNextMatchSymbol(parse_info, ref searchPos, '}');
				if (null == fp)
				{
					COMN_PROC.ErrReport();
					return null;
				}
				searchPos = new CodePosition(fp.RowNum, fp.ColNum + 1);
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
				cfi.Scope = new CodeScope(bodyStartPos, fp);
				return cfi;
			}
			// 更新index
			return cfi;
		}

		/// <summary>
		/// 取得包含头文件名
		/// </summary>
		string GetIncludeHeaderName(List<string> codeList, ref CodePosition searchPos, string file_name)
		{
			CodePosition foundPos = null;
            CODE_IDENTIFIER quotIdtf = COMN_PROC.GetNextIdentifier(codeList, ref searchPos, out foundPos);
			string inc_name = quotIdtf.Text;
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
				inc_name += nextIdtf.Text;
				nextIdtf = COMN_PROC.GetNextIdentifier(codeList, ref searchPos, out foundPos);
			}
			inc_name = inc_name + quotIdtf.Text;
			if ((inc_name.StartsWith("\"") && inc_name.EndsWith("\""))					// 双引号
				|| (inc_name.StartsWith("<") && inc_name.EndsWith(">")))				// 尖括号
			{
				// 去掉引号或者尖括号
				inc_name = inc_name.Substring(1, inc_name.Length - 2).Trim();
				inc_name = GetActualHeadrFullName(inc_name, file_name);
				if (string.IsNullOrEmpty(inc_name))
				{
					// Error Log Here!
					string errLog = "<<< Error LOG >>> : ParseIncludeHeaderFile(..) : " + file_name + " Can't Find Include Header File : " + inc_name;
					this.ErrorLogList.Add(errLog);
				}
			}
			return inc_name;
		}

		/// <summary>
		/// 用户定义类型处理
		/// </summary>
		static USER_DEFINE_TYPE_INFO UsrDefTypeProc(FILE_PARSE_INFO parse_info,
													List<CODE_IDENTIFIER> qualifier_list,
													ref CodePosition start_pos)
		{
			if (0 == qualifier_list.Count)
			{
                COMN_PROC.ErrReport();
				return null;
			}
			string keyStr = qualifier_list.Last().Text;

			USER_DEFINE_TYPE_INFO retUsrTypeInfo = new USER_DEFINE_TYPE_INFO();
			CodePosition searchPos = new CodePosition(start_pos);
			CodePosition foundPos = null;
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
			CodePosition startPos = foundPos;
			foundPos = COMN_PROC.FindNextMatchSymbol(parse_info, ref searchPos, '}');
			if (null == foundPos)
			{
                COMN_PROC.ErrReport();
				return null;
			}
			CodePosition endPos = foundPos;
			retUsrTypeInfo.Scope = new CodeScope(startPos, endPos);
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
				string memStr = COMN_PROC.GetUsrDefTypeMemberStr(m.Trim(), parse_info);
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
			CodePosition old_pos = new CodePosition(searchPos);
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
		/// 宏定义处理
		/// </summary>
		static void DefineProcess(string file_name, List<string> codeList, ref CodePosition searchPos, FILE_PARSE_INFO cfi)
		{
			CodePosition sPos, fPos;
			sPos = new CodePosition(searchPos);
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
					CodePosition ePos = COMN_PROC.FindNextSymbol(codeList, sPos, ')');
					if (null == ePos)
					{
						return;
					}
					mdi.ParaList = COMN_PROC.GetParaList(codeList, sPos, ePos);
					if (0 == mdi.ParaList.Count)
					{
						// 如果宏定义后有括号但是没有参数,加一个空串用以在宏替换时替换掉括号
						mdi.ParaList.Add(string.Empty);
					}
					sPos = ePos;
					sPos.ColNum += 1;
				}

				string defineValStr = codeList[sPos.RowNum].Substring(sPos.ColNum);
				while (defineValStr.EndsWith(@"\"))
				{
					defineValStr = defineValStr.Remove(defineValStr.Length - 1);
					sPos.Move2HeadOfNextRow();
					defineValStr += codeList[sPos.RowNum].Substring(sPos.ColNum);
				}
				mdi.ValStr = defineValStr.Trim();
				cfi.MacroDefineList.Add(mdi);

				sPos.Move2HeadOfNextRow();
				searchPos = new CodePosition(sPos);
			}
		}

		static bool UserDefTypeExpand(	string idStr,
										CodePosition foundPos,
										FILE_PARSE_INFO parse_info,
										List<CODE_IDENTIFIER> qualifierList)
		{
			if (0 != qualifierList.Count && COMN_PROC.IsUsrDefTypeKWD(qualifierList.Last().Text))
			{
				// 这样做是为了避免遇到 typedef struct A { xxx } A; 的情况
				// 在这种情况下展开后再遇到A还会再次展开, 导致进入死循环
				return false;
			}
			// typedef 用户自定义类型
			foreach (TYPE_DEFINE_INFO tdi in parse_info.TypeDefineList)
			{
				if (idStr == tdi.NewName)
				{
					string usrTypeName = tdi.NewName;
					CodePosition macroPos = new CodePosition(foundPos);
					int lineIdx = foundPos.RowNum;
					string realTypeName = tdi.OldName;
					// 用原类型名去替换用户定义类型名
					parse_info.CodeList[lineIdx] = parse_info.CodeList[lineIdx].Replace(usrTypeName, realTypeName);
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
