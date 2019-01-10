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
					string header_name, string path)
		{

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
				CodePosition header_pos
					= new CodePosition(element_list[1].GetStartPosition().Row,
										element_list[1].GetStartPosition().Col + idx);
				ret_info = new IncludeInfo(	inc_element,
										element_list[1].GetStartPosition(),
										element_list[1].EndPos, header_pos, header_name,
										file_info.FullName);
			}
			else
			{
				Trace.Assert(false);
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
