using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Mr.Robot;
using Mr.Robot.CDeducer;

namespace Mr.Robot.MacroSwitchAnalyser
{
    public class CommonProc
    {
		public static string RemoveStringSegment(string code_line)
		{
			while (true)
			{
				int idx_s = code_line.IndexOf('\"');
				if ((-1 != idx_s)
					&& (code_line.Length - 1 != idx_s))
				{
					int idx_e = code_line.IndexOf('\"', idx_s + 1);
					if (-1 != idx_e)
					{
						code_line = code_line.Remove(idx_s, idx_e - idx_s + 1);
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}
			}
			return code_line;
		}

        public static string GetMacroExpression(string code_line, int ifIdx)
        {
            for (int i = ifIdx + 3; i < code_line.Length; i++)
            {
                if (Char.IsWhiteSpace(code_line[i]))
                {
                    return code_line.Substring(i).Trim();
                }
            }
            return null;
        }

        static string RemoveExpressionBrackets(string exp_str)
        {
            exp_str = exp_str.Trim();
            if (exp_str.StartsWith("(")
                && exp_str.EndsWith(")"))
            {
                exp_str = exp_str.Remove(exp_str.Length - 1, 1);
                exp_str = exp_str.Remove(0, 1).Trim();
            }
            return exp_str;
        }

        public static void MacroSwitchExpressionAnalysis(   string macro_exp,
                                                            MacroPrintInfo print_info,
                                                            FILE_PARSE_INFO parse_info,
															ref List<MSA_MACRO_SWITCH_RESULT> result_list,
															List<MTPJ_FILE_INFO> mtpj_info_list,
															List<MK_FILE_INFO> mk_info_list)
        {
            if (string.IsNullOrEmpty(macro_exp))
            {
                return;
            }
            List<STATEMENT_COMPONENT> cpntList = COMN_PROC.GetComponents(macro_exp, null);
            foreach (STATEMENT_COMPONENT cpnt in cpntList)
            {
                if (cpnt.Type == StatementComponentType.Identifier && "defined" != cpnt.Text)
                {
                    MACRO_DEFINE_INFO mdi = parse_info.FindMacroDefInfo(cpnt.Text);
                    if (null != mdi)
                    {
                        string valStr = mdi.Value;
                        // 宏值分析
                        MacroValueType mType = MacroValueStrAnalysis(ref valStr, parse_info);
                        // 立即数
                        if (mType == MacroValueType.ConstNumber)
                        {
							MSA_MACRO_SWITCH_RESULT msResult = new MSA_MACRO_SWITCH_RESULT(print_info.SourceName, print_info.LineNum, print_info.CodeText, mdi.Name, true, valStr, mdi.FileName);
							result_list.Add(msResult);
                        }
                        // 表达式
                        else if (mType == MacroValueType.Expression)
                        {
							MacroSwitchExpressionAnalysis(valStr, print_info, parse_info, ref result_list, mtpj_info_list, mk_info_list);
                        }
                        // 空
                        else if (mType == MacroValueType.Empty)
                        {
							MSA_MACRO_SWITCH_RESULT msResult = new MSA_MACRO_SWITCH_RESULT(print_info.SourceName, print_info.LineNum, print_info.CodeText, mdi.Name, true, string.Empty, mdi.FileName);
							result_list.Add(msResult);
                        }
                    }
                    else
                    {
                        // 未定义
						bool bFindInPrjDef = false;
						if (null != mtpj_info_list)
						{
							foreach (MTPJ_FILE_INFO prj_info in mtpj_info_list)
							{
								string valStr;
								if (prj_info.DefListContains(cpnt.Text, out valStr))
								{
									if (string.IsNullOrEmpty(valStr))
									{
										valStr = @".mtpj def";
									}
									MSA_MACRO_SWITCH_RESULT msResult = new MSA_MACRO_SWITCH_RESULT(print_info.SourceName, print_info.LineNum, print_info.CodeText, cpnt.Text, true, valStr, prj_info.FileName);
									result_list.Add(msResult);
									bFindInPrjDef = true;
									break;
								}
							}
						}
						if (null != mk_info_list)
						{
							foreach (var item in mk_info_list)
							{
								if (item.DefList.Contains(cpnt.Text))
								{
									MSA_MACRO_SWITCH_RESULT msResult = new MSA_MACRO_SWITCH_RESULT(print_info.SourceName, print_info.LineNum, print_info.CodeText, cpnt.Text, true, @".mk def", item.FileName);
									result_list.Add(msResult);
									bFindInPrjDef = true;
									break;
								}
							}
						}
						if (!bFindInPrjDef)
						{
							MSA_MACRO_SWITCH_RESULT msResult = new MSA_MACRO_SWITCH_RESULT(print_info.SourceName, print_info.LineNum, print_info.CodeText, cpnt.Text, false, string.Empty, string.Empty);
							result_list.Add(msResult);
						}
                    }
                }
            }
        }

        static MacroValueType MacroValueStrAnalysis(ref string macro_value_str, FILE_PARSE_INFO parse_info)
        {
            if (string.IsNullOrEmpty(macro_value_str.Trim()))
            {
                return MacroValueType.Empty;
            }
            macro_value_str = RemoveExpressionBrackets(macro_value_str);
			if (COMN_PROC.IsStandardIdentifier(macro_value_str))
			{
				MACRO_DEFINE_INFO mdi = parse_info.FindMacroDefInfo(macro_value_str);
				if (null != mdi)
				{
					macro_value_str = mdi.Value;
					return MacroValueStrAnalysis(ref macro_value_str, parse_info);
				}
			}
            List<STATEMENT_COMPONENT> cpntList = COMN_PROC.GetComponents(macro_value_str, null);
            int numberCount = 0;
            int identifierCount = 0;
            foreach (STATEMENT_COMPONENT cpnt in cpntList)
            {
                if (cpnt.Type == StatementComponentType.ConstantNumber)
                {
                    numberCount += 1;
                }
                else if (cpnt.Type == StatementComponentType.Identifier && cpnt.Text != "defined")
                {
                    identifierCount += 1;
                }
            }
            if (1 == numberCount && 0 == identifierCount)
            {
                return MacroValueType.ConstNumber;
            }
            else
            {
                return MacroValueType.Expression;
            }
        }

		/// <summary>
		/// 比较两个文本文件内容一致
		/// </summary>
		public static bool CompareTextFileContentsSame(string file1, string file2)
		{
			if (!File.Exists(file1)
				|| !File.Exists(file2))
			{
				return false;
			}
			StreamReader sr = new StreamReader(file1);
			string file1Contents = sr.ReadToEnd();
			sr.Close();
			sr = new StreamReader(file2);
			string file2Contents = sr.ReadToEnd();
			sr.Close();
			if (0 == string.Compare(file1Contents, file2Contents))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
    }

    public class MacroPrintInfo
    {
        public string SourceName = string.Empty;
        public int LineNum = -1;
        public string CodeText = string.Empty;

        public MacroPrintInfo(string src_name, int line_num, string code_text)
        {
            this.SourceName = src_name;
            this.LineNum = line_num;
            this.CodeText = code_text.Trim().Replace('\t', ' ');
        }
    }

    public enum MacroValueType
    {
        ConstNumber,			// 常数
        Expression,				// 表达式
        Empty,					// 空
    }

	/// <summary>
	/// 工程文件信息(有可能包含部分宏定义等信息)
	/// </summary>
	public class MTPJ_FILE_INFO
	{
		public string FileName = string.Empty;
		public List<DEF_INFO> DefList = new List<DEF_INFO>();							// .mtpj文件里定义的宏定义列表
		public MTPJ_FILE_INFO(string file_name)
		{
			this.FileName = file_name;
		}

		public void MtpjProc()
		{
			if (!File.Exists(this.FileName))
			{
				return;
			}
			this.DefList.Clear();
			StreamReader sr = new StreamReader(this.FileName);
			while (true)
			{
				string rdLine = sr.ReadLine();
				if (null == rdLine)
				{
					break;
				}
				rdLine = rdLine.Trim();
				if (string.Empty == rdLine
					|| (!Char.IsLetter(rdLine[0]) && ('_' != rdLine[0]))
					)
				{
					continue;
				}
				else
				{
					string valStr = string.Empty;
					if (-1 != rdLine.IndexOf('='))
					{
						int idx = rdLine.IndexOf('=');
						valStr = rdLine.Substring(idx + 1).Trim();
						rdLine = rdLine.Remove(idx).Trim();
					}
					if (COMN_PROC.IsStandardIdentifier(rdLine))
					{
						string strTmp;
						if (!DefListContains(rdLine, out strTmp))
						{
							DEF_INFO defInfo = new DEF_INFO(rdLine, valStr);
							this.DefList.Add(defInfo);
						}
					}
				}
			}
			sr.Close();
		}

		public bool DefListContains(string def_str, out string val_str)
		{
			val_str = string.Empty;
			foreach (var item in this.DefList)
			{
				if (item.NameStr.Equals(def_str))
				{
					val_str = item.ValStr;
					return true;
				}
			}
			return false;
		}

		public class DEF_INFO
		{
			public string NameStr = string.Empty;
			public string ValStr = string.Empty;

			public DEF_INFO(string name, string val)
			{
				this.NameStr = name;
				this.ValStr = val;
			}
		}
	}

	public class MK_FILE_INFO
	{
		public string FileName = string.Empty;
		public List<string> DefList = new List<string>();								// .mtpj文件里定义的宏定义列表
		public MK_FILE_INFO(string file_name)
		{
			this.FileName = file_name;
		}

		public void MkProc()
		{
			if (!File.Exists(this.FileName))
			{
				return;
			}
			this.DefList.Clear();
			StreamReader sr = new StreamReader(this.FileName);
			while (true)
			{
				string rdLine = sr.ReadLine();
				if (null == rdLine)
				{
					break;
				}
				rdLine = rdLine.Trim();
				if (string.Empty == rdLine
					|| (rdLine.StartsWith("#"))
					)
				{
					continue;
				}
				int idx;
				if (-1 != (idx = rdLine.IndexOf("-D")))
				{
					string subStr = rdLine.Substring(idx + 2).Trim();
					string dStr = GetMacroNameStr(subStr);
					if (!string.IsNullOrEmpty(dStr)
						&& !this.DefList.Contains(dStr))
					{
						this.DefList.Add(dStr);
					}
				}
			}
			sr.Close();
		}

		string GetMacroNameStr(string line_str)
		{
			int idx = 0;
			for (int i = 0; i < line_str.Length; i++)
			{
				char ch = line_str[i];
				if (!Char.IsLetter(ch)
					&& !Char.IsDigit(ch)
					&& '_' != ch)
				{
					break;
				}
				else
				{
					idx++;
				}
			}
			if (0 != idx)
			{
				return line_str.Substring(0, idx);
			}
			return string.Empty;
		}
	}
}
