using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper
{
	class RunContext
	{
		Dictionary<string, DefineInfo> DefDic = new Dictionary<string, DefineInfo>();

		public bool IfDef(string macro_name)
		{
			Trace.Assert(CommProc.IsStringIdentifier(macro_name));
			if (this.DefDic.ContainsKey(macro_name))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public bool IfNDef(string macro_name)
		{
			Trace.Assert(CommProc.IsStringIdentifier(macro_name));
			if (this.DefDic.ContainsKey(macro_name))
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
