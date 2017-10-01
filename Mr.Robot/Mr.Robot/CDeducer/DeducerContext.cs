using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot.CDeducer
{
	public class DEDUCER_CONTEXT
	{
		public List<VAR_CTX2> VarCtxList = new List<VAR_CTX2>();
	}

	public class VAR_CTX2
	{
		public string VarName = string.Empty;											// 变量名
		public VAR_TYPE2 VarType = null;												// 变量类型
		public List<VAR_CTX2> MemberList = new List<VAR_CTX2>();						// 成员列表(非基本型时)
		public List<VAR_VALUE> ValueEvolveList = new List<VAR_VALUE>();					// 变量值的Step演化列表(基本型时)

		public VAR_CTX2(VAR_TYPE2 var_type, string var_name)
		{
			this.VarName = var_name;
			this.VarType = var_type;
		}
	}

	public class VAR_TYPE2
	{
		public string TypeName = string.Empty;											// 类型名
		public List<string> PrefixList = new List<string>();							// 前缀列表
		public List<string> SuffixList = new List<string>();							// 后缀列表

		public VAR_TYPE2(string type_name, List<string> prefix_list, List<string> suffix_list)
		{
			this.TypeName = type_name;
			this.PrefixList = prefix_list;
			this.SuffixList = suffix_list;
		}
	}

	public class VAR_VALUE
	{
		public object Value = null;														// 变量值(基本型)
		public string StepMarkStr = string.Empty;										// Step标号

		public VAR_VALUE(object val, string step_mark)
		{
			this.Value = val;
			this.StepMarkStr = step_mark;
		}
	}
}
