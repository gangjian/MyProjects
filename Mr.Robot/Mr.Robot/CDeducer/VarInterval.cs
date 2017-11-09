using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot.CDeducer
{
	public class VAR_INTERVAL_PROC
	{
		public static List<VAR_INTERVAL> GetVarIntervalStr(string oprt_str, string val_str)
		{
			List<VAR_INTERVAL> retList = new List<VAR_INTERVAL>();
			int val;
			System.Diagnostics.Trace.Assert(int.TryParse(val_str, out val));
			switch (oprt_str)
			{
				case ">":
					retList.Add(new VAR_INTERVAL(val, null, VAR_INTERVAL_LIMIT_TYPE.OPEN, VAR_INTERVAL_LIMIT_TYPE.NONE));
					return retList;
				case ">=":
					retList.Add(new VAR_INTERVAL(val, null, VAR_INTERVAL_LIMIT_TYPE.CLOSE, VAR_INTERVAL_LIMIT_TYPE.NONE));
					return retList;
				case "<":
					retList.Add(new VAR_INTERVAL(null, val, VAR_INTERVAL_LIMIT_TYPE.NONE, VAR_INTERVAL_LIMIT_TYPE.OPEN));
					return retList;
				case "<=":
					retList.Add(new VAR_INTERVAL(null, val, VAR_INTERVAL_LIMIT_TYPE.NONE, VAR_INTERVAL_LIMIT_TYPE.CLOSE));
					return retList;
				case "==":
					retList.Add(new VAR_INTERVAL(val, val, VAR_INTERVAL_LIMIT_TYPE.CLOSE, VAR_INTERVAL_LIMIT_TYPE.CLOSE));
					return retList;
				case "!=":
					retList.Add(new VAR_INTERVAL(null, val, VAR_INTERVAL_LIMIT_TYPE.NONE, VAR_INTERVAL_LIMIT_TYPE.OPEN));
					retList.Add(new VAR_INTERVAL(val, null, VAR_INTERVAL_LIMIT_TYPE.OPEN, VAR_INTERVAL_LIMIT_TYPE.NONE));
					return retList;
				default:
					break;
			}
			return null;
		}
	}

	public class VAR_INTERVAL
	{
		public object MinVal = null;
		public object MaxVal = null;

		public VAR_INTERVAL_LIMIT_TYPE LeftLimitType = VAR_INTERVAL_LIMIT_TYPE.NONE;
		public VAR_INTERVAL_LIMIT_TYPE rightLimitType = VAR_INTERVAL_LIMIT_TYPE.NONE;

		public VAR_INTERVAL(object min, object max, VAR_INTERVAL_LIMIT_TYPE left_limit, VAR_INTERVAL_LIMIT_TYPE right_limit)
		{
			this.MinVal = min;
			this.MaxVal = max;
			this.LeftLimitType = left_limit;
			this.rightLimitType = right_limit;
		}
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
