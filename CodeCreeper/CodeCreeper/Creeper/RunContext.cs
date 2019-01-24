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
		SyntaxTree syntaxTreeObj = new SyntaxTree();
		Dictionary<string, DefineInfo> DefDic = new Dictionary<string, DefineInfo>();

		public void AddSyntaxTreeNode(SyntaxNode node)
		{
			this.syntaxTreeObj.AddNode(node);
		}
		public List<string> GetSyntaxTreeStringList()
		{
			return this.syntaxTreeObj.ToStringList();
		}
		bool IfDef(string macro_name)
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
		bool IfNDef(string macro_name)
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

		public bool JudgeBrachEnter(string tag_str, string exp_str)
		{
			bool ret = false;
			switch (tag_str)
			{
				case "#ifdef":
					ret = IfDef(exp_str);
					break;
				case "#ifndef":
					ret = IfNDef(exp_str);
					break;
				default:
					Trace.Assert(false);
					break;
			}
			return ret;
		}
	}
}
