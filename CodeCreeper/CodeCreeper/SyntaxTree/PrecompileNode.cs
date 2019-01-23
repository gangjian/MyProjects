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
		PrecompileEndIfNode endIfNode = null;

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
			ret_list.Add(this.endIfNode.ToString(level));
			return ret_list;
		}
		public void AddEndIfNode(PrecompileEndIfNode end_if_node)
		{
			Trace.Assert(null == this.endIfNode);
			Trace.Assert(null != end_if_node);
			this.endIfNode = end_if_node;
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

		public bool EnterFlag
		{
			get; set;
		}

		public PrecompileBranchNode(string tag_str, string expression_str)
			: base(tag_str, expression_str)
		{
			this.EnterFlag = false;
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

	class PrecompileEndIfNode : SyntaxNode
	{
		public PrecompileEndIfNode()
			: base("#endif", null)
		{
		}
	}
}
