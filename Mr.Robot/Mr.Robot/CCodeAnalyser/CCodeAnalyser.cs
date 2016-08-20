using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Mr.Robot
{
	public static partial class CCodeAnalyser
	{
		#region 全局字段
		static List<string> _headerNameList = new List<string>();

		public static List<string> HeaderNameList								// 所有头文件名列表
		{
			get { return CCodeAnalyser._headerNameList; }
			set { CCodeAnalyser._headerNameList = value; }
		}

		static List<string> _sourceNameList = new List<string>();

		public static List<string> SourceNameList								// 源文件名列表, 现状通常一次解析只有一个源文件
		{
			get { return CCodeAnalyser._sourceNameList; }
			set { CCodeAnalyser._sourceNameList = value; }
		}

		static List<CFileParseInfo> _totalParsedInfoList = new List<CFileParseInfo>();

		public static List<CFileParseInfo> TotalParsedInfoList					// 所有已经解析过的文件的解析情报列表(暂未使用)
		{
			get { return CCodeAnalyser._totalParsedInfoList; }
			set { CCodeAnalyser._totalParsedInfoList = value; }
		}
		#endregion

		#region 向外提供的接口方法
		/// <summary>
		/// ".c"源文件处理
		/// </summary>
		/// <param varName="srcName"></param>
		public static List<CCodeParseResult> CFileListProcess(List<string> srcFileList, List<string> hdFileList)
		{
			// 初始化
			SourceNameList = srcFileList;
			HeaderNameList = hdFileList;

			List<CCodeParseResult> resultList = new List<CCodeParseResult>();

			// 逐个解析源文件
			foreach (string srcName in SourceNameList)
			{
				CCodeParseResult parseResult = new CCodeParseResult();
				List<CFileParseInfo> hdList = new List<CFileParseInfo>();
				parseResult.SourceParseInfo = CFileProcess(srcName, ref hdList);
				parseResult.IncHdParseInfoList = hdList;

				resultList.Add(parseResult);
			}

			return resultList;
		}
		#endregion

		// 以下都是内部调用方法

		/// <summary>
		/// C文件(包括源文件和头文件)处理
		/// </summary>
		/// <param varName="srcName"></param>
		/// <param varName="includeInfoList"></param>
		/// <returns></returns>
		static CFileParseInfo CFileProcess(string srcName, ref List<CFileParseInfo> includeInfoList)
		{
			// 去掉注释
			List<string> codeList = RemoveComments(srcName);
			CFileParseInfo fi = new CFileParseInfo(srcName);
			// 预编译处理
			codeList = PrecompileProcess(codeList, ref fi, ref includeInfoList);
			fi.parsedCodeList = codeList;
//          Save2File(codeList, srcName + ".bak");

			// 从文件开头开始解析
			File_Position sPos = new File_Position(0, 0);
			// 文件解析
			CCodeFileAnalysis(srcName, codeList, ref sPos, ref fi, includeInfoList);

//			includeInfoList.Add(fi);
//          XmlProcess.SaveCFileInfo2XML(fi);

			return fi;
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
					int idx_e = rdLine.IndexOf("*/");
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
		/// <param varName="codeList"></param>
		/// <param varName="fi"></param>
		/// <param varName="includeInfoList"></param>
		/// <returns></returns>
		public static List<string> PrecompileProcess(List<string> codeList,
													 ref CFileParseInfo fi,
													 ref List<CFileParseInfo> includeInfoList)
		{
			List<string> retList = new List<string>();
			Stack<CC_INFO> ccStack = new Stack<CC_INFO>();						// 条件编译嵌套时, 用堆栈来保存嵌套的条件编译情报参数
			CC_INFO cc_info = new CC_INFO();

			string rdLine = "";
			for (int idx = 0; idx < codeList.Count; idx++)
			{
				rdLine = codeList[idx].Trim();
				if (rdLine.StartsWith("#"))
				{
					File_Position searchPos = new File_Position(idx, codeList[idx].IndexOf("#") + 1);
					File_Position foundPos = null;
                    string idStr = CommonProcess.GetNextIdentifier(codeList, ref searchPos, out foundPos);
					if ("include" == idStr.ToLower())
					{
						if (false != cc_info.write_flag)
						{
							// 取得include文件名
							string incFileName = GetIncludeFileName(codeList, ref searchPos);
							System.Diagnostics.Trace.Assert(null != incFileName);
							if (!fi.include_file_list.Contains(incFileName))
							{
								fi.include_file_list.Add(incFileName);
							}
							if (incFileName.StartsWith("\"") && incFileName.EndsWith("\""))
							{
								// 去掉引号
								incFileName = incFileName.Substring(1, incFileName.Length - 2).Trim();
								// 取得头文件的解析情报
								ParseIncludeHeaderFile(incFileName, ref includeInfoList);
							}
						}
					}
					else if ("define" == idStr.ToLower())
					{
						if (false != cc_info.write_flag)
						{
							DefineProcess(codeList, ref searchPos, ref fi);
						}
					}
					else
					{
						string exprStr = "";            // 表达式字符串
						if ("if" == idStr.ToLower())
						{
							bool lastFlag = cc_info.write_flag;
							ccStack.Push(cc_info);
							cc_info = new CC_INFO();
							if (false == lastFlag)
							{
								cc_info.write_flag = false;
								cc_info.write_next_flag = false;
							}
							else
							{
                                exprStr = CommonProcess.GetExpressionStr(codeList, ref searchPos, out foundPos);
								// 判断表达式的值
                                if (0 != CommonProcess.JudgeExpressionValue(exprStr, includeInfoList, fi.macro_define_list))
								{
									cc_info.write_flag = false;
									cc_info.write_next_flag = true;
								}
								else
								{
									cc_info.write_flag = false;
									cc_info.write_next_flag = false;
								}
							}
						}
						else if ("ifdef" == idStr.ToLower())
						{
							bool lastFlag = cc_info.write_flag;
							ccStack.Push(cc_info);
							cc_info = new CC_INFO();
							if (false == lastFlag)
							{
								cc_info.write_flag = false;
								cc_info.write_next_flag = false;
							}
							else
							{
                                exprStr = CommonProcess.GetExpressionStr(codeList, ref searchPos, out foundPos);
								// 判断表达式是否已定义
                                if (null != CommonProcess.JudgeExpressionDefined(exprStr, includeInfoList, fi.macro_define_list))
								{
									cc_info.write_flag = false;
									cc_info.write_next_flag = true;
								}
								else
								{
									cc_info.write_flag = false;
									cc_info.write_next_flag = false;
								}
							}
						}
						else if ("ifndef" == idStr.ToLower())
						{
							bool lastFlag = cc_info.write_flag;
							ccStack.Push(cc_info);
							cc_info = new CC_INFO();
							if (false == lastFlag)
							{
								cc_info.write_flag = false;
								cc_info.write_next_flag = false;
							}
							else
							{
                                exprStr = CommonProcess.GetExpressionStr(codeList, ref searchPos, out foundPos);
								// 判断表达式是否已定义
                                if (null != CommonProcess.JudgeExpressionDefined(exprStr, includeInfoList, fi.macro_define_list))
								{
									cc_info.write_flag = false;
									cc_info.write_next_flag = false;
								}
								else
								{
									cc_info.write_flag = false;
									cc_info.write_next_flag = true;
								}
							}
						}
						else if ("else" == idStr.ToLower())
						{
							bool lastFlag = true;
							if (ccStack.Count > 0)
							{
								lastFlag = ccStack.Peek().write_flag;
							}
							if (false == lastFlag)
							{
								cc_info.write_flag = false;
								cc_info.write_next_flag = false;
							}
							else
							{
								if (true == cc_info.write_flag)
								{
									cc_info.write_flag = false;
									cc_info.write_next_flag = false;
								}
								else
								{
									cc_info.write_flag = false;
									cc_info.write_next_flag = true;
								}
							}
						}
						else if ("elif" == idStr.ToLower())
						{
							bool lastFlag = true;
							if (ccStack.Count > 0)
							{
								lastFlag = ccStack.Peek().write_flag;
							}
							if (false == lastFlag)
							{
								cc_info.write_flag = false;
								cc_info.write_next_flag = false;
							}
							else
							{
								// 跟"if"一样, 但是因为不是嵌套所以不用压栈
                                exprStr = CommonProcess.GetExpressionStr(codeList, ref searchPos, out foundPos);
								// 判断表达式的值
                                if (0 != CommonProcess.JudgeExpressionValue(exprStr, includeInfoList, fi.macro_define_list))
								{
									cc_info.write_flag = false;
									cc_info.write_next_flag = true;
								}
								else
								{
									cc_info.write_flag = false;
									cc_info.write_next_flag = false;
								}
							}
						}
						else if ("endif" == idStr.ToLower())
						{
							bool lastFlag = true;
							if (ccStack.Count > 0)
							{
								lastFlag = ccStack.Peek().write_flag;
							}
							if (false == lastFlag)
							{
								cc_info.write_flag = false;
								cc_info.write_next_flag = false;
							}
							else
							{
								cc_info.write_flag = false;
								cc_info.write_next_flag = true;
							}

							cc_info.pop_up_flag = true;
						}
						else
						{
						}
					}
				}

				if (cc_info.write_flag)
				{
					retList.Add(codeList[idx]);
				}
				else
				{
					retList.Add("");
				}
				if (true == cc_info.write_next_flag)
				{
					cc_info.write_flag = true;
					cc_info.write_next_flag = false;
				}

				// 嵌套时弹出堆栈, 恢复之前的情报
				if (true == cc_info.pop_up_flag)
				{
					cc_info = ccStack.Pop();
				}
			}

			return retList;
		}

		/// <summary>
		/// 取得include头文件的解析情报
		/// </summary>
		/// <param varName="incFileName"></param>
		/// <param varName="includeInfoList"></param>
		/// <returns></returns>
		static void ParseIncludeHeaderFile(string incFileName,
										 ref List<CFileParseInfo> includeInfoList)
		{
			// 先在已解析过的文件list里找
			foreach (var pi in includeInfoList)
			{
				string path;
				string fName = IOProcess.GetFileName(pi.full_name, out path);
				if (fName.ToLower() == incFileName.ToLower())
				{
					// 如果找到了, 直接返回
					return;
				}
			}

			// 如果上一步没找到, 证明还没被解析, 则在全部头文件list里找
			foreach (var hd_name in HeaderNameList)
			{
				string path;
				string fName = IOProcess.GetFileName(hd_name, out path);
				if (fName.ToLower() == incFileName.ToLower())
				{
					// 如果找到了, 则要先解析这个头文件
					CFileParseInfo fi = CFileProcess(hd_name, ref includeInfoList);
					// 解析完后, 加到头文件解析列表中去
					includeInfoList.Add(fi);
					// TODO: 注意当有多个同名文件符合条件时的情况应对
				}
			}
			// 头文件没找到
			// ErrReport(incFileName + " 头文件没找到!");
		}

		/// <summary>
		/// 文件代码解析
		/// </summary>
		public static void CCodeFileAnalysis(string fullName,
											List<string> codeList,
											ref File_Position searchPos,
											ref CFileParseInfo fi,
											List<CFileParseInfo> parsedFileInfoList)
		{
			System.Diagnostics.Trace.Assert((null != codeList));
			if (0 == codeList.Count)
			{
                CommonProcess.ErrReport();
				return;
			}

			List<string> qualifierList = new List<string>();     // 修饰符暂存列表
			string nextId = null;
			File_Position foundPos = null;
            while (null != (nextId = CommonProcess.GetNextIdentifier(codeList, ref searchPos, out foundPos)))
			{
				// 如果是标准标识符(字母,数字,下划线组成且开头不是数字)
                if (CommonProcess.IsStandardIdentifier(nextId)
					|| ("*" == nextId))
				{
                    if (0 != qualifierList.Count
                        && CommonProcess.IsUsrDefTypeKWD(qualifierList.Last()))
                    {
                    }
					else if (MacroDetectAndExpand_File(nextId, codeList, foundPos, fi, parsedFileInfoList))
					{
						// 判断是否是已定义的宏, 是的话进行宏展开
						// 展开后要返回到原处(展开前的位置), 重新解析展开后的宏
						searchPos = new File_Position(foundPos);
						continue;
					}

					qualifierList.Add(nextId);
                    if (CommonProcess.IsUsrDefTypeKWD(nextId))
					{
						// 用户定义类型处理
						UsrDefTypeInfo udti = UserDefineTypeProcess(codeList, qualifierList, ref searchPos);
						if (null != udti)
						{
							// 如果是匿名类型, 要给加个名字
							if (0 == udti.nameList.Count)
							{
                                // 取得匿名类型的名字
                                udti.nameList.Add(GetAnonymousTypeName(fi));
							}
							fi.user_def_type_list.Add(udti);
                            qualifierList.Add(udti.nameList[0]);
						}
					}
				}
				// 否则可能是各种符号
				else
				{
					// 遇到小括号了, 可能是碰上函数声明或定义了
					if (("(" == nextId) && (0 != qualifierList.Count))
					{
						CFunctionStructInfo cfi = FunctionDetectProcess(codeList, qualifierList, ref searchPos, foundPos);
						if (null != cfi)
						{
							if (   -1 != cfi.Scope.Start.row_num
								&& -1 != cfi.Scope.Start.col_num
								&& -1 != cfi.Scope.End.row_num
								&& -1 != cfi.Scope.End.col_num)
							{
								fi.fun_define_list.Add(cfi);
							}
							else
							{
								fi.fun_declare_list.Add(cfi);
							}
						}
					}
					else if ("#" == nextId)
					{
						// 预编译命令, 因为已经处理过了, 不在这里解析, 跳到宏定义结束
						while (codeList[searchPos.row_num].EndsWith("\\"))
						{
							searchPos.row_num += 1;
						}
						searchPos.col_num = codeList[searchPos.row_num].Length;
					}
					// 全局量(包含全局数组)
					else if ("[" == nextId)
					{
						// 到下一个"]"出现的位置是数组长度
                        File_Position fp = CommonProcess.FindNextSymbol(codeList, searchPos, ']');
						if (null != fp)
						{
                            string arraySize = CommonProcess.LineStringCat(codeList, foundPos, fp);
							qualifierList.Add(arraySize);
							fp.col_num += 1;
							searchPos = fp;
							continue;
						}
					}
					else if ("=" == nextId)
					{
						// 直到下一个分号出现的位置, 都是初始化语句
						qualifierList.Add(nextId);
                        File_Position fp = CommonProcess.FindNextSymbol(codeList, searchPos, ';');
						if (null != fp)
						{
							foundPos.col_num += 1;
                            string initialStr = CommonProcess.LineStringCat(codeList, foundPos, fp);
							qualifierList.Add(initialStr.Trim());
							searchPos = fp;
							continue;
						}
					}
					else if (";" == nextId)
					{
						// typedef类型定义
						if ((0 != qualifierList.Count)
							&& ("typedef" == qualifierList[0]))
						{
							TypeDefProcess(codeList, qualifierList, ref fi);
						}
						// 注意用户定义类型后面的分号不是全局量
						else if (2 <= qualifierList.Count)
						{
							GlobalVarProcess(qualifierList, ref fi);
						}
					}
					else if ("," == nextId)
					{
						GlobalVarProcess(qualifierList, ref fi);
						continue;
					}
					else
					{
						// Do Nothing
					}
					qualifierList.Clear();
				}
			}
		}

		/// <summary>
		/// 函数检测(声明, 定义)
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="lineIdx"></param>
		/// <param varName="startIdx"></param>
		static CFunctionStructInfo FunctionDetectProcess(List<string> codeList, List<string> qualifierList, ref File_Position searchPos, File_Position bracketLeft)
		{
			CFunctionStructInfo cfi = new CFunctionStructInfo();

			// 先找匹配的小括号
            File_Position bracketRight = CommonProcess.FindNextMatchSymbol(codeList, searchPos, ')');
			if (null == bracketRight)
			{
                CommonProcess.ErrReport();
				return null;
			}
			if (codeList[bracketLeft.row_num].Substring(bracketLeft.col_num + 1).Trim().StartsWith("*"))
			{
				// 吗呀, 这不是传说中的函数指针嘛...
				File_Position sp = bracketLeft;
				File_Position ep = bracketRight;
				sp.col_num += 1;
				ep.col_num -= 1;
                bracketLeft = CommonProcess.FindNextSymbol(codeList, bracketRight, '(');
				if (null == searchPos)
				{
                    CommonProcess.ErrReport();
					return null;
				}
                bracketRight = CommonProcess.FindNextSymbol(codeList, searchPos, ')');
				if (null == bracketRight)
				{
                    CommonProcess.ErrReport();
					return null;
				}
                string nameStr = CommonProcess.LineStringCat(codeList, sp, ep);
				cfi.name = nameStr;
			}
			List<string> paraList = GetParaList(codeList, bracketLeft, bracketRight);

			// 然后确认小括号后面是否跟着配对的大括号
			searchPos = new File_Position(bracketRight.row_num, bracketRight.col_num + 1);
			File_Position foundPos = null;
            string nextIdStr = CommonProcess.GetNextIdentifier(codeList, ref searchPos, out foundPos);
			if (";" == nextIdStr)
			{
				// 小括号后面跟着分号说明这是函数声明
				// 函数名
				if ("" == cfi.name)
				{
					cfi.name = qualifierList.Last();
					// 函数修饰符
					qualifierList.RemoveAt(qualifierList.Count - 1);
				}
				cfi.qualifiers = new List<string>(qualifierList);
				// 参数列表
				cfi.paras = paraList;
			}
			else if ("{" == nextIdStr)
			{
				File_Position bodyStartPos = foundPos;
                // 小括号后面跟着配对的大括号说明这是函数定义(带函数体)
                File_Position fp = CommonProcess.FindNextMatchSymbol(codeList, searchPos, '}');
				if (null == fp)
				{
                    CommonProcess.ErrReport();
					return null;
				}
				searchPos = new File_Position(fp.row_num, fp.col_num + 1);
				// 函数名
				cfi.name = qualifierList.Last();
				// 函数修饰符
				qualifierList.RemoveAt(qualifierList.Count - 1);
				cfi.qualifiers = qualifierList;
				// 参数列表
				cfi.paras = paraList;
				// 函数体起始位置
				cfi.Scope.Start = bodyStartPos;
				cfi.Scope.End = fp;
			}
			else
			{
				// 估计是出错了
                CommonProcess.ErrReport();
				return null;
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
		static string GetIncludeFileName(List<string> codeList, ref File_Position searchPos)
		{
			File_Position foundPos = null;
            string quot = CommonProcess.GetNextIdentifier(codeList, ref searchPos, out foundPos);
			string retName = quot;
			if ("\"" == quot)
			{
			}
			else if ("<" == quot)
			{
				quot = ">";
			}
			else
			{
                CommonProcess.ErrReport();
				return null;
			}
            string str = CommonProcess.GetNextIdentifier(codeList, ref searchPos, out foundPos);
			while (quot != str)
			{
				retName += str;
                str = CommonProcess.GetNextIdentifier(codeList, ref searchPos, out foundPos);
			}
			return retName + quot;
		}

		/// <summary>
		/// 取得参数列表
		/// </summary>
		/// <returns></returns>
		static List<string> GetParaList(List<string> codeList, File_Position bracketLeft, File_Position bracketRight)
		{
			List<string> retParaList = new List<string>();
            string catStr = CommonProcess.LineStringCat(codeList, bracketLeft, bracketRight);
			// 去掉小括号
			if (catStr.StartsWith("("))
			{
				catStr = catStr.Remove(0, 1);
			}
			if (catStr.EndsWith(")"))
			{
				catStr = catStr.Remove(catStr.LastIndexOf(')'));
			}
			catStr = catStr.Trim();
			if ("" == catStr)
			{
				// 如果没有参数, 只有一对空的小括号, 那么要加入一个空字串""参数
				retParaList.Add("");
				return retParaList;
			}
			string[] paras = catStr.Split(',');
			foreach (string p in paras)
			{
				retParaList.Add(p.Trim());
			}
			return retParaList;
		}

		/// <summary>
		/// 用户定义类型处理
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="startPos"></param>
		/// <param varName="qualifierList"></param>
		/// <returns></returns>
		static UsrDefTypeInfo UserDefineTypeProcess(List<string> codeList, List<string> qualifierList, ref File_Position startPos)
		{
			if (0 == qualifierList.Count)
			{
                CommonProcess.ErrReport();
				return null;
			}
			string keyStr = qualifierList.Last();

			UsrDefTypeInfo retUsrTypeInfo = new UsrDefTypeInfo();
			File_Position searchPos = new File_Position(startPos);
			File_Position foundPos = null;
            string nextIdStr = CommonProcess.GetNextIdentifier(codeList, ref searchPos, out foundPos);
			if ("{" != nextIdStr)
			{
                if (CommonProcess.IsStandardIdentifier(nextIdStr))
				{
					retUsrTypeInfo.nameList.Add(nextIdStr);
                    nextIdStr = CommonProcess.GetNextIdentifier(codeList, ref searchPos, out foundPos);
					if ("{" != nextIdStr)
					{
						// 没找到最大括号, 说明这不是一个新的用户定义类型
						if ((0 != qualifierList.Count)
							&& ("typedef" == qualifierList[0]))
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
			retUsrTypeInfo.Scope.Start = foundPos;
            foundPos = CommonProcess.FindNextMatchSymbol(codeList, searchPos, '}');
			if (null == foundPos)
			{
                CommonProcess.ErrReport();
				return null;
			}
			retUsrTypeInfo.Scope.End = foundPos;
            string catStr = CommonProcess.LineStringCat(codeList, retUsrTypeInfo.Scope.Start, retUsrTypeInfo.Scope.End);
			if (catStr.StartsWith("{"))
			{
				catStr = catStr.Remove(0, 1);
			}
			if (catStr.EndsWith("}"))
			{
				catStr = catStr.Remove(catStr.LastIndexOf('}'));
			}
			char sepStr = ';';
			retUsrTypeInfo.type = keyStr;
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
				string memStr = m.Trim();
				if (string.Empty != memStr)
				{
					retUsrTypeInfo.memberList.Add(m.Trim());
				}
			}

			startPos = searchPos;

			return retUsrTypeInfo;
		}

		/// <summary>
		/// 全局变量处理
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="qualifierList"></param>
		/// <param varName="searchPos"></param>
		/// <param varName="cfi"></param>
		static void GlobalVarProcess(List<string> qualifierList, ref CFileParseInfo cfi)
		{
			VariableInfo gvi = new VariableInfo();

			// 判断是否有初始化语句
			int idx = -1;
			if (-1 != (idx = qualifierList.IndexOf("=")))
			{
				gvi.initial_string += qualifierList[idx + 1];
				for (int i = qualifierList.Count - 1; i >= idx; i--)
				{
					qualifierList.RemoveAt(i);
				}
			}

			// 判断是否是数组
			idx = qualifierList.Count - 1;
			string qlfStr = qualifierList[idx].Trim();
			if (qlfStr.StartsWith("[") && qlfStr.EndsWith("]"))
			{
				qlfStr = qlfStr.Substring(1, qlfStr.Length - 2).Trim();
				gvi.array_size_string = qlfStr;
				qualifierList.RemoveAt(idx);
			}

			// 变量名
			idx = qualifierList.Count - 1;
			qlfStr = qualifierList[idx].Trim();
            if (CommonProcess.IsStandardIdentifier(qlfStr))
			{
				gvi.varName = qlfStr;
			}
			else
			{
				return;
			}
			qualifierList.RemoveAt(idx);

			// 类型名
			idx = qualifierList.Count - 1;
			qlfStr = qualifierList[idx].Trim();
			string type_name = "";
            while (false == CommonProcess.IsStandardIdentifier(qlfStr))
			{
				type_name = qlfStr + type_name;
				idx--;
				qlfStr = qualifierList[idx].Trim();
			}
			type_name = qlfStr + type_name;
			gvi.typeName = type_name;

			// 剩余的都放到修饰符列表里去
			for (int i = 0; i < idx; i++)
			{
				gvi.qualifiers.Add(qualifierList[i].Trim());
			}
			if ((0 != gvi.qualifiers.Count)
				&& ("extern" == gvi.qualifiers.First().Trim().ToLower()))
			{
				cfi.global_var_declare_list.Add(gvi);
			}
			else
			{
				cfi.global_var_define_list.Add(gvi);
			}
		}

		/// <summary>
		/// 宏定义处理
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="searchPos"></param>
		/// <param varName="cfi"></param>
		static void DefineProcess(List<string> codeList, ref File_Position searchPos, ref CFileParseInfo cfi)
		{
			File_Position sPos, fPos;
			sPos = new File_Position(searchPos);
            string nextIdStr = CommonProcess.GetNextIdentifier(codeList, ref sPos, out fPos);
            if (!CommonProcess.IsStandardIdentifier(nextIdStr))
			{
				return;
			}
			MacroDefineInfo mdi = new MacroDefineInfo();
			mdi.name = nextIdStr;

			string leftStr = codeList[sPos.row_num].Substring(sPos.col_num);
			if (leftStr.StartsWith("("))
			{
                File_Position ePos = CommonProcess.FindNextSymbol(codeList, sPos, ')');
				if (null == ePos)
				{
					return;
				}
				mdi.paras = GetParaList(codeList, sPos, ePos);
				sPos = ePos;
				sPos.col_num += 1;
			}

			string defineValStr = codeList[sPos.row_num].Substring(sPos.col_num);
			while (defineValStr.EndsWith(@"\"))
			{
				defineValStr = defineValStr.Remove(defineValStr.Length - 1);
				sPos.row_num += 1;
				sPos.col_num = 0;
				defineValStr += codeList[sPos.row_num].Substring(sPos.col_num);
			}
			mdi.value = defineValStr.Trim();
			cfi.macro_define_list.Add(mdi);

			sPos.row_num += 1;
			sPos.col_num = 0;
			searchPos = new File_Position(sPos);
		}

		/// <summary>
		/// 宏检测与宏展开
		/// </summary>
		static bool MacroDetectAndExpand_File(string idStr, List<string> codeList,
										      File_Position foundPos,
										      CFileParseInfo curFileInfo,
										      List<CFileParseInfo> includeHeaderInfoList)
		{
            if (!CommonProcess.IsStandardIdentifier(idStr))
			{
				return false;
			}
			// 作成一个所有包含头文件的宏定义的列表
			List<MacroDefineInfo> defineList = new List<MacroDefineInfo>();
			List<TypeDefineInfo> typeDefineList = new List<TypeDefineInfo>();
			foreach (CFileParseInfo hdInfo in includeHeaderInfoList)
			{
				defineList.AddRange(hdInfo.macro_define_list);
				typeDefineList.AddRange(hdInfo.type_define_list);
			}
			// 添加上本文件所定义的宏
			defineList.AddRange(curFileInfo.macro_define_list);
			typeDefineList.AddRange(curFileInfo.type_define_list);

			// 遍历查找宏名
			foreach (MacroDefineInfo di in defineList)
			{
				// 判断宏名是否一致
				if (idStr == di.name)
				{
					string macroName = di.name;
					File_Position macroPos = new File_Position(foundPos);
					int lineIdx = foundPos.row_num;
					string replaceStr = di.value;
					// 判断有无带参数
					if (0 != di.paras.Count)
					{
						// 取得实参
						File_Position sPos = new File_Position(foundPos.row_num, foundPos.col_num + idStr.Length);
                        string paraStr = CommonProcess.GetNextIdentifier(codeList, ref sPos, out foundPos);
						if ("(" != paraStr)
						{
                            CommonProcess.ErrReport();
							break;
						}
						File_Position leftBracket = foundPos;
                        foundPos = CommonProcess.FindNextSymbol(codeList, sPos, ')');
						if (null == foundPos)
						{
                            CommonProcess.ErrReport();
							break;
						}
                        paraStr = CommonProcess.LineStringCat(codeList, macroPos, foundPos);
						macroName = paraStr;
						List<string> realParas = GetParaList(codeList, leftBracket, foundPos);
						if (realParas.Count != di.paras.Count)
						{
                            CommonProcess.ErrReport();
							break;
						}
						// 替换宏值里的形参
						int idx = 0;
						foreach (string rp in realParas)
						{
							if (string.Empty == rp)
							{
								// 参数有可能为空, 即没有参数, 只有一对空的括号里面什么参数也不带
								continue;
							}
							replaceStr = replaceStr.Replace(di.paras[idx], rp);
							idx++;
						}
					}
					// 应对宏里面出现的"##"
					string[] seps = { "##" };
					string[] arr = replaceStr.Split(seps, StringSplitOptions.None);
					if (arr.Length > 1)
					{
						string newStr = "";
						foreach (string sepStr in arr)
						{
							newStr += sepStr.Trim();
						}
						replaceStr = newStr;
					}
					// 单个"#"转成字串的情况暂未对应, 以后遇到再说, 先出个error report作为保护
					if (replaceStr.Contains('#'))
					{
                        CommonProcess.ErrReport();
						return false;
					}

					// 用宏值去替换原来的宏名(宏展开)
					codeList[lineIdx] = codeList[lineIdx].Replace(macroName, replaceStr);
					return true;
				}
			}
			// typedef 用户自定义类型
			foreach (TypeDefineInfo tdi in typeDefineList)
			{
				if (idStr == tdi.new_type_name)
				{
					string usrTypeName = tdi.new_type_name;
					File_Position macroPos = new File_Position(foundPos);
					int lineIdx = foundPos.row_num;
					string realTypeName = tdi.old_type_name;
					// 用原类型名去替换用户定义类型名
					codeList[lineIdx] = codeList[lineIdx].Replace(usrTypeName, realTypeName);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 类型定义处理
		/// </summary>
		/// <param varName="codeList"></param>
		/// <param varName="qualifierList"></param>
		/// <param varName="cfi"></param>
		static void TypeDefProcess(List<string> codeList, List<string> qualifierList, ref CFileParseInfo cfi)
		{
			TypeDefineInfo tdi = new TypeDefineInfo();
			string old_type = "";
			for (int i = 1; i < qualifierList.Count; i++)
			{
				if (qualifierList.Count - 1 == i)
				{
					tdi.new_type_name = qualifierList[i];
					tdi.old_type_name = old_type;
				}
				else
				{
					old_type += (" " + qualifierList[i]);
				}
			}
			cfi.type_define_list.Add(tdi);
		}

        static string GetAnonymousTypeName(CFileParseInfo fi)
        {
            string fn, path;
            fn = IOProcess.GetFileName(fi.full_name, out path);

            string retName = fn.Replace('.', '_').ToUpper() + "_USR_DEF_TYPE_" + fi.user_def_type_list.Count.ToString();

            return retName;
        }

	}
}
