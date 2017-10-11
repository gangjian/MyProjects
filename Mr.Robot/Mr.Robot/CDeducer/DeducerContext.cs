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
		public VAR_CATEGORY VarCategory = VAR_CATEGORY.INVALID;							// 变量分类(函数参数, 全局变量, 局部变量)
		public List<VAR_CTX2> MemberList = new List<VAR_CTX2>();						// 成员列表(非基本型时)
		public List<VAR_RECORD> ValueEvolveList = new List<VAR_RECORD>();				// 变量值的Step演化列表

		public VAR_CTX2(VAR_TYPE2 var_type, string var_name, VAR_CATEGORY category)
		{
			this.VarName = var_name;
			this.VarType = var_type;
			this.VarCategory = category;
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

	public class VAR_RECORD
	{
		public VAR_BEHAVE VarBehave;
		public string StepMarkStr;

		public VAR_RECORD(VAR_BEHAVE var_behave, string step_mark)
		{
			this.VarBehave = var_behave;
			this.StepMarkStr = step_mark;
		}
	}

	/// <summary>
	/// 变量分类
	/// </summary>
	public enum VAR_CATEGORY
	{
		INVALID,
		FUNC_PARA,
		LOCAL,
		GLOBAL,
	}

	/// <summary>
	/// 变量行为
	/// </summary>
	public enum VAR_BEHAVE
	{
		DECLARE,					// 声明
		EVALUATION,					// 赋值
		READ_OUT,					// (读)出参
		CONDITION_LIMIT,			// 条件(语句)限定
	}
}
