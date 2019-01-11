using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper
{
	class IncludeInfo
	{
		CodeElement IncludeElement = null;
		CodePosition LeftQuotePos = null;
		CodePosition RightQuotePos = null;
		CodePosition HeaderPos = null;
		string HeaderName = null;
		string FilePath = null;
		QuoteBracketType QuoteType = QuoteBracketType.Quote;

		IncludeInfo(CodeElement inc_element, CodePosition left_quote_pos,
					CodePosition right_quote_pos, CodePosition header_pos,
					string header_name, string path,
					QuoteBracketType q_type = QuoteBracketType.Quote)
		{
			this.IncludeElement = inc_element;
			this.LeftQuotePos = left_quote_pos;
			this.RightQuotePos = right_quote_pos;
			this.HeaderPos = header_pos;
			this.HeaderName = header_name;
			this.FilePath = path;
			this.QuoteType = q_type;
		}

		public static IncludeInfo Parse(CodeElement inc_element, CodeFileInfo file_info)
		{
			Trace.Assert(null != inc_element);
			Trace.Assert(null != file_info);
			List<CodeElement> element_list
						= file_info.GetLineElementList(inc_element.GetStartPosition());
			Trace.Assert(element_list.Count >= 2);
			IncludeInfo ret_info = null;
			if (2 == element_list.Count && element_list[1].Type == ElementType.String)
			{
				string header_str = element_list[1].ToString(file_info.CodeList);
				Trace.Assert(header_str.StartsWith("\"") && header_str.EndsWith("\""));
				string header_name = header_str.Substring(1, header_str.Length - 2).Trim();
				int idx = header_str.IndexOf(header_name);
				Trace.Assert(-1 != idx);
				CodePosition lp = element_list[1].GetStartPosition();
				CodePosition rp = element_list[1].EndPos;
				CodePosition header_pos	= new CodePosition(	lp.Row, lp.Col + idx);
				ret_info = new IncludeInfo(inc_element, lp, rp, header_pos, header_name,
											file_info.FullName);
			}
			else
			{
				Trace.Assert(6 == element_list.Count);
				Trace.Assert(element_list[1].ToString(file_info.CodeList).Equals("<"));
				Trace.Assert(element_list[5].ToString(file_info.CodeList).Equals(">"));
				Trace.Assert(element_list[3].ToString(file_info.CodeList).Equals("."));
				Trace.Assert(element_list[2].Type == ElementType.Identifier);
				Trace.Assert(element_list[4].Type == ElementType.Identifier);
				Trace.Assert(element_list[2].CloseTo(element_list[3], file_info.CodeList));
				Trace.Assert(element_list[3].CloseTo(element_list[4], file_info.CodeList));
				CodePosition lp = element_list[1].GetStartPosition();
				CodePosition rp = element_list[5].GetStartPosition();
				CodePosition hp = element_list[2].GetStartPosition();
				string hname = element_list[2].ToString(file_info.CodeList)
								+ element_list[3].ToString(file_info.CodeList)
								+ element_list[4].ToString(file_info.CodeList);
				ret_info = new IncludeInfo(	inc_element, lp, rp, hp, hname,
											file_info.FullName, QuoteBracketType.Angle);
			}
			return ret_info;
		}
		public string GetName()
		{
			if (this.QuoteType == QuoteBracketType.Quote)
			{
				return "\"" + this.HeaderName + "\"";
			}
			else
			{
				return "<" + this.HeaderName + ">";
			}
		}

		enum QuoteBracketType
		{
			Quote,
			Angle,
		}
	}
}
