using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot.CDeducer
{
	public class DEDUCER_INPUT_TBL
	{
		public List<DI_FUNC_PARA> ParaList = new List<DI_FUNC_PARA>();
		public List<DI_GLB_VAR> GlobalList = new List<DI_GLB_VAR>();
		public List<DI_FUNC_CALLED> FuncCalledList = new List<DI_FUNC_CALLED>();
	}

	// 函数入参
	public class DI_FUNC_PARA
	{
		public string Name = string.Empty;
		public VAR_TYPE2 VarType = null;
		public string IntervalStr = null;
	}

	// 全局量
	public class DI_GLB_VAR
	{
		public List<VAR_LEVEL2> NameLevelList = new List<VAR_LEVEL2>();
		public VAR_TYPE2 VarType = null;
		public string StepMaker = string.Empty;
		public string IntervalStr = null;
	}

	// 函数调用
	public class DI_FUNC_CALLED
	{
		public string FuncName = string.Empty;
		public DI_FC_CATEGORY Category;
		public int ReadOutIdx = -1;
		public VAR_TYPE2 VarType = null;
		public string StepMaker = string.Empty;
		public string IntervalStr = null;
	}

	public class VAR_LEVEL2
	{
		public string Name = string.Empty;
		public VAR_MEMBER_OPERATOR MemberOperator = VAR_MEMBER_OPERATOR.NONE;
		public VAR_LEVEL2(string name)
		{
			this.Name = name;
		}
	}

	public enum DI_FC_CATEGORY { ReturnVal, ReadOutVal };
}
