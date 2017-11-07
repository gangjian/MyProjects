using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot.CDeducer
{
	class VAR_INTERVAL_PROC
	{
		public static List<string> GetVarIntervalStr(SIMPLIFIED_EXPRESSION exp_str)
		{
			return null;
		}
	}

	class VAR_INTERVAL
	{
		public object MinVal = null;
		public object MaxVal = null;

		public VAR_INTERVAL_LIMIT_TYPE LeftLimitType = VAR_INTERVAL_LIMIT_TYPE.NONE;
		public VAR_INTERVAL_LIMIT_TYPE rightLimitType = VAR_INTERVAL_LIMIT_TYPE.NONE;
	}

	/// <summary>
	/// 值区间边界类型
	/// </summary>
	public enum VAR_INTERVAL_LIMIT_TYPE
	{
		NONE,			// 无
		OPEN,			// 开区间
		CLOSE,			// 闭区间
	}
}
