using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper
{
	public class CreeperContext
	{
		SyntaxTree syntaxTreeObj = new SyntaxTree();
		MacroDefManager macroDefManager = new MacroDefManager();

		public void AddSyntaxTreeNode(SyntaxNode node)
		{
			this.syntaxTreeObj.AddNode(node);
			if (null == node.TagStr)
			{
			}
			else if (node.TagStr.Equals("#define"))
			{
				this.macroDefManager.Add(node.ExpressionStr, node);
			}
			else if (node.TagStr.Equals("#undef"))
			{
				this.macroDefManager.Delete(node.ExpressionStr);
			}
		}
		public List<string> GetSyntaxTreeStringList()
		{
			return this.syntaxTreeObj.ToStringList();
		}

		public bool JudgePrecompileBranchEnter(string tag_str, string exp_str)
		{
			bool ret = false;
			switch (tag_str)
			{
				case "#ifdef":
					ret = this.macroDefManager.IfDef(exp_str);
					break;
				case "#ifndef":
					ret = this.macroDefManager.IfNDef(exp_str);
					break;
				default:
					Trace.Assert(false);
					break;
			}
			return ret;
		}
	}

	class MacroDefManager
	{
		Dictionary<string, SyntaxNode> DefDic = new Dictionary<string, SyntaxNode>();
		public bool IfDef(string def_name)
		{
			Trace.Assert(CommProc.IsStringIdentifier(def_name));
			if (this.DefDic.ContainsKey(def_name))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public bool IfNDef(string def_name)
		{
			Trace.Assert(CommProc.IsStringIdentifier(def_name));
			if (this.DefDic.ContainsKey(def_name))
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		public void Add(string def_name, SyntaxNode def_node)
		{
			Trace.Assert(CommProc.IsStringIdentifier(def_name));
			if (this.IfDef(def_name))
			{
				// 如果重复定义,用后面的覆盖
				this.DefDic.Remove(def_name);
			}
			this.DefDic.Add(def_name, def_node);
		}
		public void Delete(string def_name)
		{
			Trace.Assert(CommProc.IsStringIdentifier(def_name));
			Trace.Assert(this.IfDef(def_name));
			this.DefDic.Remove(def_name);
		}
	}
}
