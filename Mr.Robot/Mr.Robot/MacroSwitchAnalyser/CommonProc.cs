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
                                                            FileParseInfo src_parse_info,
                                                            ref List<string> result_list,
															string original_macro_name = null)
        {
            if (string.IsNullOrEmpty(macro_exp))
            {
                return;
            }
			string macro_name = original_macro_name;
            List<StatementComponent> cpntList = StatementAnalysis.GetComponents(macro_exp, null);
            foreach (StatementComponent cpnt in cpntList)
            {
                if (cpnt.Type == StatementComponentType.Identifier && "defined" != cpnt.Text)
                {
                    MacroDefineInfo mdi = src_parse_info.FindMacroDefInfo(cpnt.Text);
                    if (null != mdi)
                    {
						if (null == macro_name)
						{
							macro_name = mdi.Name;
						}
                        string valStr = mdi.Value;
                        // 宏值分析
                        MacroValueType mType = MacroValueStrAnalysis(ref valStr);
                        // 立即数
                        if (mType == MacroValueType.ConstNumber)
                        {
							string resultStr = MakeResultStr(macro_name, valStr, print_info);
                            result_list.Add(resultStr);
                        }
						// 别名
						else if (mType == MacroValueType.Alias)
						{
							string orginalName = original_macro_name;
							if (null == orginalName)
							{
								orginalName = mdi.Name;
							}
							MacroSwitchExpressionAnalysis(valStr, print_info, src_parse_info, ref result_list, orginalName);
						}
                        // 表达式
                        else if (mType == MacroValueType.Expression)
                        {
                            MacroSwitchExpressionAnalysis(valStr, print_info, src_parse_info, ref result_list);
                        }
                        // 空
                        else if (mType == MacroValueType.Empty)
                        {
							string resultStr = MakeResultStr(macro_name, string.Empty, print_info);
                            result_list.Add(resultStr);
                        }
                    }
                    else
                    {
                        // 未定义
						if (null == macro_name)
						{
							macro_name = cpnt.Text;
						}
						string resultStr = MakeResultStr(macro_name, @"X", print_info);
                        result_list.Add(resultStr);
                    }
                }
            }
        }

        static MacroValueType MacroValueStrAnalysis(ref string macro_value_str)
        {
            if (string.IsNullOrEmpty(macro_value_str.Trim()))
            {
                return MacroValueType.Empty;
            }
            macro_value_str = RemoveExpressionBrackets(macro_value_str);
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
			else if (0 == numberCount && 1 == identifierCount)
			{
				return MacroValueType.Alias;
			}
            else
            {
                return MacroValueType.Expression;
            }
        }

        static string MakeResultStr(string macro_name, string value_str, MacroPrintInfo print_info)
        {
			if (string.IsNullOrEmpty(value_str))
			{
				value_str = @"○";
			}
            string resultStr = print_info.SourceName + "," + print_info.LineNumStr
                            + "," + print_info.CodeText + "," + macro_name + "," + value_str;
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
		Alias,					// 别名(把一个宏名定义到另一个宏名)
        Expression,				// 表达式
        Empty,					// 空
    }

}
