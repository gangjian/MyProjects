using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot.CDeducer
{
	class D_INPUT
	{
		public List<DI_FUNC_PARA> ParaList = new List<DI_FUNC_PARA>();
		public List<DI_GLB_VAR> GlobalList = new List<DI_GLB_VAR>();
		public List<DI_FUNC_CALLED> FuncCalledList = new List<DI_FUNC_CALLED>();
	}

	// 函数入参
	class DI_FUNC_PARA
	{
		public string Name = string.Empty;
		public VAR_TYPE2 VarType = null;
		public object VarValue = null;
	}

	// 全局量
	class DI_GLB_VAR
	{
		public List<VAR_LEVEL2> NameLevelList = new List<VAR_LEVEL2>();
		public VAR_TYPE2 VarType = null;
		public string StepMakr = string.Empty;
		public object VarValue = null;
	}

	// 函数调用
	class DI_FUNC_CALLED
	{
		public string FuncName = string.Empty;
		//DI_FC_CATEGORY Category;
		public VAR_TYPE2 VarType = null;
		public string StepMakr = string.Empty;
	}

	class VAR_LEVEL2
	{
		public string Name = string.Empty;
		public VAR_MEMBER_OPERATOR MemberOperator = VAR_MEMBER_OPERATOR.NONE;
		public VAR_LEVEL2(string name)
		{
			this.Name = name;
		}
	}

	enum DI_FC_CATEGORY { ReturnVal, ReadOutVal };
}
