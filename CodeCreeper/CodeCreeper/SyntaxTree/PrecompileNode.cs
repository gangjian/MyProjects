using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper
{
	class PrecompileSwitchNode : SyntaxNode
	{
		List<PrecompileBranchNode> branchList = new List<PrecompileBranchNode>();
		internal List<PrecompileBranchNode> BranchList
		{
			get { return branchList; }
			set { branchList = value; }
		}

		public PrecompileSwitchNode(PrecompileBranchNode first_branch_node)
		{
			Trace.Assert(null != first_branch_node);
			first_branch_node.ParentRef = this;
			this.BranchList.Add(first_branch_node);
		}
		public override List<string> ToStringList(int level)
		{
			List<string> ret_list = new List<string>();
			foreach (var branch in this.BranchList)
			{
				ret_list.AddRange(branch.ToStringList(level));
			}
			return ret_list;
		}
	}
	class PrecompileBranchNode : SyntaxNode
	{
		List<SyntaxNode> childList = new List<SyntaxNode>();
		internal List<SyntaxNode> ChildList
		{
			get { return childList; }
			set { childList = value; }
		}
		public PrecompileBranchNode(string tag_str, string expression_str)
			: base(tag_str, expression_str)
		{
		}
		public override List<string> ToStringList(int level)
		{
			List<string> ret_list = new List<string>();
			ret_list.Add(this.ToString(level));
			foreach (var item in this.ChildList)
			{
				ret_list.AddRange(item.ToStringList(level + 1));
			}
			return ret_list;
		}
	}

	class EndIfNode : SyntaxNode
	{
	}
}
