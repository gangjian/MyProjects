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
	public partial class CCodeAnalyser
	{
		#region 全局字段
		List<string> HeaderList = new List<string>();									// 头文件名列表
		List<string> SourceList = new List<string>();									// 源文件名列表
		CodeBufferManager CodeBuffManagerRef = null;

		public EventHandler UpdateProgress = null;
		public List<FileParseInfo> ParseInfoList = null;
		#endregion

		public CCodeAnalyser(List<string> source_list, List<string> header_list, ref CodeBufferManager code_buf_manager)
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

		public List<FileParseInfo> CFileListProc()
		{
			this.ParseInfoList = new List<FileParseInfo>();
			int count = 0;
			int total = this.SourceList.Count;
			// 逐个解析源文件
			foreach (string srcName in this.SourceList)
			{
				FileParseInfo parseInfo = new FileParseInfo(srcName);
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

		public class CodeBufferManager
		{
			class CodeBuffer
			{
				public string FileName = string.Empty;
				public List<string> CodeList = null;

				public CodeBuffer(string file_name, List<string> code_list)
				{
					this.FileName = file_name;
					this.CodeList = code_list;
				}
			}

			const int MAX_CODE_BUF_LIST_CNT = 1000;
			List<CodeBuffer> CodeBufferList = new List<CodeBuffer>();

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
				CodeBuffer cb = new CodeBuffer(file_name, code_list);
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
		bool CFileProcess(string srcName, ref FileParseInfo fileInfo)
		{
			System.Diagnostics.Trace.WriteLine(srcName + ": ");
			if (null == fileInfo)
			{
				fileInfo = new FileParseInfo(srcName);
			}
			List<string> codeList = null;
			if (null != this.CodeBuffManagerRef)
			{
				codeList = this.CodeBuffManagerRef.SearchCodeBufferList(srcName);
			}
			if (null == codeList)
			{
				// 去掉注释
				codeList = RemoveComments(srcName);
				if (null != this.CodeBuffManagerRef && srcName.ToLower().EndsWith(".h"))
				{
					this.CodeBuffManagerRef.AddCodeBufferList(srcName, codeList);
				}
			}
			// 预编译处理
			codeList = PrecompileProcess(srcName, codeList, ref fileInfo);
			fileInfo.CodeList = codeList;
//          Save2File(codeList, srcName + ".bak");

			// 文件解析
			if(!CCodeFileAnalysis(srcName, ref fileInfo))
			{
				return false;
			}
			else
			{
				return true;
			}
//			includeInfoList.Add(fi);
//          XmlProcess.SaveCFileInfo2XML(fi);
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
												ref FileParseInfo fi)
		{
			List<string> retList = new List<string>();
			Stack<ConditionalCompilationInfo> ccStack = new Stack<ConditionalCompilationInfo>(); // 条件编译嵌套时, 用堆栈来保存嵌套的条件编译情报参数
			ConditionalCompilationInfo cc_info = new ConditionalCompilationInfo();

			string rdLine = "";
			for (int idx = 0; idx < codeList.Count; idx++)
			{
				rdLine = codeList[idx].Trim();
				if (rdLine.StartsWith("#"))
				{
					CodePosition searchPos = new CodePosition(idx, codeList[idx].IndexOf("#") + 1);
					CodePosition foundPos = null;
                    CodeIdentifier nextIdtf = CommonProcess.GetNextIdentifier(codeList, ref searchPos, out foundPos);
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
							if (incFileName.StartsWith("\"") && incFileName.EndsWith("\""))
							{
								// 去掉引号
								incFileName = incFileName.Substring(1, incFileName.Length - 2).Trim();
								// 取得头文件的解析情报
								ParseIncludeHeaderFile(incFileName, ref fi);
							}
						}
					}
					else if ("define" == nextIdtf.Text.ToLower())
					{
						if (false != cc_info.WriteFlag)
						{
							DefineProcess(file_name, codeList, ref searchPos, ref fi);
						}
					}
					else
					{
						string exprStr = "";            // 表达式字符串
						if ("if" == nextIdtf.Text.ToLower())
						{
							bool lastFlag = cc_info.WriteFlag;
							ccStack.Push(cc_info);
							cc_info = new ConditionalCompilationInfo();
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
                                exprStr = CommonProcess.GetPrecompileExpressionStr(codeList, ref searchPos, out foundPos);
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
							cc_info = new ConditionalCompilationInfo();
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
                                exprStr = CommonProcess.GetPrecompileExpressionStr(codeList, ref searchPos, out foundPos);
								// 表达式可能占多行(连行符)
								idx = searchPos.RowNum - 1;
								// 判断表达式是否已定义
                                if (CommonProcess.JudgeExpressionDefined(exprStr, fi))
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
							cc_info = new ConditionalCompilationInfo();
							if (false == lastFlag)
							{
								cc_info.WriteFlag = false;
								cc_info.WriteNextFlag = false;
							}
							else
							{
                                exprStr = CommonProcess.GetPrecompileExpressionStr(codeList, ref searchPos, out foundPos);
								// 表达式可能占多行(连行符)
								idx = searchPos.RowNum - 1;
								// 判断表达式是否已定义
                                if (CommonProcess.JudgeExpressionDefined(exprStr, fi))
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
                                exprStr = CommonProcess.GetPrecompileExpressionStr(codeList, ref searchPos, out foundPos);
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
		void ParseIncludeHeaderFile(string incFileName,
									ref FileParseInfo fi)
		{
			// 先在已解析过的文件list里找
			foreach (string fname in fi.IncFileList)
			{
				if (fname.ToLower() == incFileName.ToLower())
				{
					// 如果找到了, 直接返回
					return;
				}
			}

			// 如果上一步没找到, 证明还没被解析, 则在全部头文件list里找
			foreach (var hd_name in this.HeaderList)
			{
				string path;
				string fName = IOProcess.GetFileName(hd_name, out path);
				if (fName.ToLower() == incFileName.ToLower())
				{
					// 如果找到了, 则要先解析这个头文件
					CFileProcess(hd_name, ref fi);
					// TODO: 注意当有多个同名文件符合条件时的情况应对
				}
			}
			// 头文件没找到
			// ErrReport(incFileName + " 头文件没找到!");
		}

		/// <summary>
		/// 文件代码解析
		/// </summary>
		public static bool CCodeFileAnalysis(string src_name,
											 ref FileParseInfo parse_info)
		{
			System.Diagnostics.Trace.Assert((null != parse_info.CodeList));
			if (0 == parse_info.CodeList.Count)
			{
                CommonProcess.ErrReport();
				return false;
			}
			// 从文件开头开始解析
			CodePosition search_pos = new CodePosition(0, 0);
			List<CodeIdentifier> qualifierList = new List<CodeIdentifier>();			// 修饰符暂存列表
			CodeIdentifier nextIdtf = null;
			CodePosition foundPos = null;
			while (null != (nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref search_pos, out foundPos)))
			{
				//if (src_name.EndsWith("gerdaC_dd.h") && nextIdtf.Position.RowNum > 340)
				//{
				//	int a = 100;
				//}
				// 如果是标准标识符(字母,数字,下划线组成且开头不是数字)
				if (CommonProcess.IsStandardIdentifier(nextIdtf.Text)
					|| ("*" == nextIdtf.Text))
				{
					if (CommonProcess.MacroDetectAndExpand_File(nextIdtf.Text, foundPos, parse_info))
					{
						// 判断是否是已定义的宏, 是的话进行宏展开
						// 展开后要返回到原处(展开前的位置), 重新解析展开后的宏
						search_pos = new CodePosition(foundPos);
						continue;
					}
					else if (UserDefTypeExpand(nextIdtf.Text, parse_info.CodeList, foundPos, parse_info, qualifierList))
					{
						search_pos = new CodePosition(foundPos);
						continue;
					}
					qualifierList.Add(nextIdtf);
					if (CommonProcess.IsUsrDefTypeKWD(nextIdtf.Text))
					{
						// 用户定义类型处理
						UsrDefTypeInfo udti = UsrDefTypeProc(parse_info, qualifierList, ref search_pos);
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
						FuncParseInfo cfi = FunctionDetectProcess(src_name, parse_info, qualifierList, ref search_pos, foundPos);
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
							foundPos = CommonProcess.FindNextSpecIdentifier(";", parse_info.CodeList, search_pos);
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
						CodePosition fp = CommonProcess.FindNextSymbol(parse_info.CodeList, search_pos, ']');
						if (null != fp)
						{
							string arraySizeStr = CommonProcess.LineStringCat(parse_info.CodeList, foundPos, fp);
							CodeIdentifier arraySizeIdtf = new CodeIdentifier(arraySizeStr, foundPos);
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
						CodePosition fp = CommonProcess.FindNextSymbol(parse_info.CodeList, search_pos, ';');
						if (null != fp)
						{
							foundPos.ColNum += 1;
							string initialStr = CommonProcess.LineStringCat(parse_info.CodeList, foundPos, fp);
							CodeIdentifier initIdtf = new CodeIdentifier(initialStr, foundPos);
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
							GlobalVarProcess(qualifierList, ref parse_info);

							statementStr = StatementAnalysis.GetStatementStr(parse_info.CodeList,
								new CodeScope(qualifierList.First().Position, nextIdtf.Position));
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
		static FuncParseInfo FunctionDetectProcess(	string src_name,
													FileParseInfo parse_info,
													List<CodeIdentifier> qualifierList,
													ref CodePosition searchPos,
													CodePosition bracketLeft)
		{
			if (0 == qualifierList.Count
				|| "typedef" == qualifierList.First().Text)
			{
				return null;
			}
			FuncParseInfo cfi = new FuncParseInfo();

			// 先找匹配的小括号
			CodePosition bracketRight = CommonProcess.FindNextMatchSymbol(parse_info, ref searchPos, ')');
			if (null == bracketRight)
			{
                CommonProcess.ErrReport();
				return null;
			}
			if (parse_info.CodeList[bracketLeft.RowNum].Substring(bracketLeft.ColNum + 1).Trim().StartsWith("*"))
			{
				// 吗呀, 这不是传说中的函数指针嘛...
				CodePosition sp = bracketLeft;
				CodePosition ep = bracketRight;
				sp.ColNum += 1;
				ep.ColNum -= 1;
				bracketLeft = CommonProcess.FindNextSymbol(parse_info.CodeList, bracketRight, '(');
				if (null == searchPos)
				{
                    CommonProcess.ErrReport();
					return null;
				}
				bracketRight = CommonProcess.FindNextSymbol(parse_info.CodeList, searchPos, ')');
				if (null == bracketRight)
				{
                    CommonProcess.ErrReport();
					return null;
				}
				string nameStr = CommonProcess.LineStringCat(parse_info.CodeList, sp, ep);
				cfi.Name = nameStr;
			}
			List<string> paraList = CommonProcess.GetParaList(parse_info.CodeList, bracketLeft, bracketRight);

			// 然后确认小括号后面是否跟着配对的大括号
			searchPos = new CodePosition(bracketRight.RowNum, bracketRight.ColNum + 1);
			CodePosition foundPos = null;
			CodeIdentifier nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
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
				cfi.Qualifiers = new List<CodeIdentifier>(qualifierList);
				// 参数列表
				cfi.ParaList = paraList;
			}
			else if ("{" == nextIdtf.Text)
			{
				CodePosition bodyStartPos = foundPos;
                // 小括号后面跟着配对的大括号说明这是函数定义(带函数体)
				CodePosition fp = CommonProcess.FindNextMatchSymbol(parse_info, ref searchPos, '}');
				if (null == fp)
				{
                    CommonProcess.ErrReport();
					return null;
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
				CodePosition bodyStartPos = CommonProcess.FindNextSymbol(parse_info.CodeList, searchPos, '{');
				searchPos = CommonProcess.PositionMoveNext(parse_info.CodeList, bodyStartPos);
				CodePosition fp = CommonProcess.FindNextMatchSymbol(parse_info, ref searchPos, '}');
				if (null == fp)
				{
					CommonProcess.ErrReport();
					return null;
				}
				searchPos = new CodePosition(fp.RowNum, fp.ColNum + 1);
				// 函数名
				cfi.Name = qualifierList.Last().Text;
				// 函数修饰符
				qualifierList.RemoveAt(qualifierList.Count - 1);
				cfi.Qualifiers = qualifierList;
				// 参数列表
				cfi.ParaList = CommonProcess.GetParaList(parse_info.CodeList,
														 CommonProcess.PositionMoveNext(parse_info.CodeList, bracketRight),
														 CommonProcess.PositionMovePrevious(parse_info.CodeList, bodyStartPos));
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
		/// <param varName="codeList"></param>
		/// <param varName="searchPos"></param>
		/// <returns></returns>
		static string GetIncludeFileName(List<string> codeList, ref CodePosition searchPos)
		{
			CodePosition foundPos = null;
            CodeIdentifier quotIdtf = CommonProcess.GetNextIdentifier(codeList, ref searchPos, out foundPos);
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
                CommonProcess.ErrReport();
				return null;
			}
            CodeIdentifier nextIdtf = CommonProcess.GetNextIdentifier(codeList, ref searchPos, out foundPos);
			while (quotIdtf.Text != nextIdtf.Text)
			{
				retName += nextIdtf.Text;
				nextIdtf = CommonProcess.GetNextIdentifier(codeList, ref searchPos, out foundPos);
			}
			return retName + quotIdtf.Text;
		}

		/// <summary>
		/// 用户定义类型处理
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="startPos"></param>
		/// <param varName="qualifierList"></param>
		/// <returns></returns>
		static UsrDefTypeInfo UsrDefTypeProc(FileParseInfo parse_info,
											 List<CodeIdentifier> qualifier_list,
											 ref CodePosition start_pos)
		{
			if (0 == qualifier_list.Count)
			{
                CommonProcess.ErrReport();
				return null;
			}
			string keyStr = qualifier_list.Last().Text;

			UsrDefTypeInfo retUsrTypeInfo = new UsrDefTypeInfo();
			CodePosition searchPos = new CodePosition(start_pos);
			CodePosition foundPos = null;
			CodeIdentifier nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
			if ("{" != nextIdtf.Text)
			{
				if (CommonProcess.IsStandardIdentifier(nextIdtf.Text))
				{
					retUsrTypeInfo.NameList.Add(nextIdtf);
					nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
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
                    CommonProcess.ErrReport();
					return null;
				}
			}
			CodePosition startPos = foundPos;
			foundPos = CommonProcess.FindNextMatchSymbol(parse_info, ref searchPos, '}');
			if (null == foundPos)
			{
                CommonProcess.ErrReport();
				return null;
			}
			CodePosition endPos = foundPos;
			retUsrTypeInfo.Scope = new CodeScope(startPos, endPos);
			string catStr = CommonProcess.LineStringCat(parse_info.CodeList, retUsrTypeInfo.Scope.Start, retUsrTypeInfo.Scope.End);
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
                CommonProcess.ErrReport();
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
				CodeIdentifier idtf = new CodeIdentifier(GetAnonymousTypeName(parse_info), null);
				retUsrTypeInfo.NameList.Add(idtf);
			}
			qualifier_list.Add(retUsrTypeInfo.NameList[0]);
			// 检查后面是否跟着分号(判断类型定义结束)
			CodePosition old_pos = new CodePosition(searchPos);
			nextIdtf = CommonProcess.GetNextIdentifier(parse_info.CodeList, ref searchPos, out foundPos);
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
											 FileParseInfo source_info)
		{
			string memberStr = in_mem_str;
			string idStr;
			int offset = 0, old_offset;
			// 可能包含宏, 所有要检测宏, 有的话要做宏展开
			while (true)
			{
				old_offset = offset;
				idStr = CommonProcess.GetNextIdentifier2(memberStr, ref offset);
				if (null == idStr)
				{
					break;
				}
				else if (CommonProcess.IsStandardIdentifier(idStr)
						 && StatementAnalysis.MacroDetectAndExpand_Statement(idStr, ref memberStr, offset, source_info))
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
		/// 全局变量处理
		/// </summary>
		static void GlobalVarProcess(List<CodeIdentifier> qualifierList,
									 ref FileParseInfo cfi)
		{
			VariableInfo gvi = new VariableInfo();

			// 判断是否有初始化语句
			int idx = -1;
			for (int i = 0; i < qualifierList.Count; i++)
			{
				if (qualifierList[i].Text.Equals("="))
				{
					idx = i;
					break;
				}
			}

			if (-1 != idx)
			{
				gvi.InitialString += qualifierList[idx + 1].Text;
				for (int i = qualifierList.Count - 1; i >= idx; i--)
				{
					qualifierList.RemoveAt(i);
				}
			}

			// 判断是否是数组
			idx = qualifierList.Count - 1;
			string qlfStr = qualifierList[idx].Text.Trim();
			if (qlfStr.StartsWith("[") && qlfStr.EndsWith("]"))
			{
				qlfStr = qlfStr.Substring(1, qlfStr.Length - 2).Trim();
				gvi.ArraySizeString = qlfStr;
				qualifierList.RemoveAt(idx);
			}

			// 变量名
			qlfStr = qualifierList.Last().Text.Trim();
            if (CommonProcess.IsStandardIdentifier(qlfStr))
			{
				gvi.VarName = qlfStr;
			}
			else
			{
				return;
			}
			qualifierList.RemoveAt(qualifierList.Count - 1);

			List<string> prefixList;													// 前缀列表
			// 类型名
			gvi.TypeName = ExtractGlobalVarTypeName(ref qualifierList, out prefixList);
			gvi.Qualifiers.AddRange(prefixList);
			// 类型名可能是typedef定义的别名, 要找出原类型名
			string real_type;
			if (string.Empty != (real_type = CommonProcess.FindTypeDefName(gvi.TypeName, cfi)))
			{
				gvi.RealTypeName = real_type;
			}

			if ((0 != gvi.Qualifiers.Count)
				&& ("extern" == gvi.Qualifiers.First().Trim().ToLower()))
			{
				cfi.GlobalDeclareList.Add(gvi);
			}
			else
			{
				cfi.GlobalDefineList.Add(gvi);
			}
		}

		static string ExtractGlobalVarTypeName(ref List<CodeIdentifier> qualifierList, out List<string> prefixList)
		{
			int idx = qualifierList.Count - 1;
			string qlfStr = qualifierList[idx].Text.Trim();
			string type_name = string.Empty;
			while (false == CommonProcess.IsStandardIdentifier(qlfStr))
			{
				type_name = qlfStr + type_name;
				idx--;
				qlfStr = qualifierList[idx].Text.Trim();
			}
			type_name = qlfStr + type_name;
			// 如果前面还有"struct", "enum", "union", "signed", "unsigned"等前缀, 那也要加到类型名中去
			if (0 != idx)
			{
				if ("struct" == qualifierList[idx - 1].Text
					|| "enum" == qualifierList[idx - 1].Text
					|| "union" == qualifierList[idx - 1].Text
					|| "signed" == qualifierList[idx - 1].Text
					|| "unsigned" == qualifierList[idx - 1].Text)
				{
					type_name = qualifierList[idx - 1] + " " + type_name;
					idx -= 1;
				}
			}
			// 剩余的都放到修饰符列表里去
			prefixList = new List<string>();
			for (int i = 0; i < idx; i++)
			{
				prefixList.Add(qualifierList[i].Text.Trim());
			}
			return type_name;
		}

		/// <summary>
		/// 宏定义处理
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="searchPos"></param>
		/// <param varName="cfi"></param>
		static void DefineProcess(string file_name, List<string> codeList, ref CodePosition searchPos, ref FileParseInfo cfi)
		{
			CodePosition sPos, fPos;
			sPos = new CodePosition(searchPos);
            CodeIdentifier nextIdtf = CommonProcess.GetNextIdentifier(codeList, ref sPos, out fPos);
			if (!CommonProcess.IsStandardIdentifier(nextIdtf.Text))
			{
				return;
			}
			MacroDefineInfo mdi = new MacroDefineInfo(file_name, nextIdtf.Position.RowNum);
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
					CodePosition ePos = CommonProcess.FindNextSymbol(codeList, sPos, ')');
					if (null == ePos)
					{
						return;
					}
					mdi.ParaList = CommonProcess.GetParaList(codeList, sPos, ePos);
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
				searchPos = new CodePosition(sPos);
			}
		}

		static bool UserDefTypeExpand(	string idStr,
										List<string> codeList,
										CodePosition foundPos,
										FileParseInfo curFileInfo,
										List<CodeIdentifier> qualifierList)
		{
			if (0 != qualifierList.Count && CommonProcess.IsUsrDefTypeKWD(qualifierList.Last().Text))
			{
				// 这样做是为了避免遇到 typedef struct A { xxx } A; 的情况
				// 在这种情况下展开后再遇到A还会再次展开, 导致进入死循环
				return false;
			}
			// typedef 用户自定义类型
			foreach (TypeDefineInfo tdi in curFileInfo.TypeDefineList)
			{
				if (idStr == tdi.NewName)
				{
					string usrTypeName = tdi.NewName;
					CodePosition macroPos = new CodePosition(foundPos);
					int lineIdx = foundPos.RowNum;
					string realTypeName = tdi.OldName;
					// 用原类型名去替换用户定义类型名
					codeList[lineIdx] = codeList[lineIdx].Replace(usrTypeName, realTypeName);
					return true;
				}
			}
			return false;
		}

        static string GetAnonymousTypeName(FileParseInfo parse_info)
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
