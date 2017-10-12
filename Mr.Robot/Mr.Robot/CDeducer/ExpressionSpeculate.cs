using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot.CDeducer
{
	class ExpressionSpeculate
	{
		public static bool CanBeTrue(string expr_str, FILE_PARSE_INFO parse_info, DEDUCER_CONTEXT deducer_ctx)
		{
			List<MEANING_GROUP> meaningGroupList = COMN_PROC.GetMeaningGroups2(expr_str, parse_info, deducer_ctx);

			return false;
		}
	}
}
