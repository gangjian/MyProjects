using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Mr.Robot;

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
                                                            FileParseInfo parse_info,
                                                            ref List<string> result_list,
															List<PROJ_FILE_INFO> prjInfoList)
        {
            if (string.IsNullOrEmpty(macro_exp))
            {
                return;
            }
            List<StatementComponent> cpntList = StatementAnalysis.GetComponents(macro_exp, null);
            foreach (StatementComponent cpnt in cpntList)
            {
                if (cpnt.Type == StatementComponentType.Identifier && "defined" != cpnt.Text)
                {
                    MacroDefineInfo mdi = parse_info.FindMacroDefInfo(cpnt.Text);
                    if (null != mdi)
                    {
                        string valStr = mdi.Value;
                        // 宏值分析
                        MacroValueType mType = MacroValueStrAnalysis(ref valStr, parse_info);
                        // 立即数
                        if (mType == MacroValueType.ConstNumber)
                        {
							string resultStr = MakeResultStr(mdi.Name, valStr, print_info, mdi.FileName);
                            result_list.Add(resultStr);
                        }
                        // 表达式
                        else if (mType == MacroValueType.Expression)
                        {
							MacroSwitchExpressionAnalysis(valStr, print_info, parse_info, ref result_list, prjInfoList);
                        }
                        // 空
                        else if (mType == MacroValueType.Empty)
                        {
							string resultStr = MakeResultStr(mdi.Name, @"○", print_info, mdi.FileName);
                            result_list.Add(resultStr);
                        }
                    }
                    else
                    {
                        // 未定义
						bool findInPrjDef = false;
						if (null != prjInfoList)
						{
							foreach (PROJ_FILE_INFO prj_info in prjInfoList)
							{
								if (prj_info.DefList.Contains(cpnt.Text))
								{
									string resultStr = MakeResultStr(cpnt.Text, @".mtpj def", print_info, prj_info.FileName);
									result_list.Add(resultStr);
									findInPrjDef = true;
									break;
								}
							}
						}
						if (!findInPrjDef)
						{
							string resultStr = MakeResultStr(cpnt.Text, @"X", print_info, string.Empty);
							result_list.Add(resultStr);
						}
                    }
                }
            }
        }

        static MacroValueType MacroValueStrAnalysis(ref string macro_value_str, FileParseInfo parse_info)
        {
            if (string.IsNullOrEmpty(macro_value_str.Trim()))
            {
                return MacroValueType.Empty;
            }
            macro_value_str = RemoveExpressionBrackets(macro_value_str);
			if (CommonProcess.IsStandardIdentifier(macro_value_str))
			{
				MacroDefineInfo mdi = parse_info.FindMacroDefInfo(macro_value_str);
				if (null != mdi)
				{
					macro_value_str = mdi.Value;
					return MacroValueStrAnalysis(ref macro_value_str, parse_info);
				}
			}
            List<StatementComponent> cpntList = StatementAnalysis.GetComponents(macro_value_str, null);
            int numberCount = 0;
            int identifierCount = 0;
            foreach (StatementComponent cpnt in cpntList)
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

		static string MakeResultStr(string macro_name, string value_str, MacroPrintInfo print_info, string macro_def_file_name)
        {
			//						表达式所在的源文件名					行号
            string resultStr = print_info.SourceName + "," + print_info.LineNumStr
			//						表达式						宏名					宏值				宏定义所在的文件名
                            + "," + print_info.CodeText + "," + macro_name + "," + value_str + "," + macro_def_file_name;
            return resultStr;
        }
    }

    public class MacroPrintInfo
    {
        public string SourceName = string.Empty;
        public string LineNumStr = string.Empty;
        public string CodeText = string.Empty;

        public MacroPrintInfo(string src_name, string line_num, string code_text)
        {
            this.SourceName = src_name;
            this.LineNumStr = line_num;
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
	public class PROJ_FILE_INFO
	{
		public string FileName = string.Empty;
		public List<string> DefList = new List<string>();									// 工程文件里定义的宏定义列表
		public PROJ_FILE_INFO(string file_name)
		{
			this.FileName = file_name;
		}
	}
}
