using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper
{
	class SyntaxTree
	{
		List<SyntaxNode> nodeList = new List<SyntaxNode>();
		PrecompileBranchNode currentBranch = null;

		public void AddNode(SyntaxNode add_node)
		{
			if (null == add_node)
			{
				return;
			}
			if (add_node.GetType() == typeof(SyntaxNode))
			{
				this.AddNormalNode(add_node);
			}
			else if (add_node.GetType() == typeof(PrecompileSwitchNode))
			{
				this.AddSwitchNode((PrecompileSwitchNode)add_node);
			}
			else if (add_node.GetType() == typeof(PrecompileBranchNode))
			{
				this.AddBranchNode((PrecompileBranchNode)add_node);
			}
			else if (add_node.GetType() == typeof(EndIfNode))
			{
				this.AddEndIf();
			}
		}
		void AddSwitchNode(PrecompileSwitchNode switch_node)
		{
			Trace.Assert(null != switch_node);
			switch_node.ParentRef = this.currentBranch;
			if (null == this.currentBranch)
			{
				this.nodeList.Add(switch_node);
			}
			else
			{
				this.currentBranch.ChildList.Add(switch_node);
			}
			this.currentBranch = switch_node.BranchList.First();
		}
		void AddBranchNode(PrecompileBranchNode add_branch)
		{
			Trace.Assert(null != this.currentBranch);
			Trace.Assert(null != this.currentBranch.ParentRef);
			Trace.Assert(null != add_branch);
			PrecompileSwitchNode parent_switch = this.currentBranch.ParentRef as PrecompileSwitchNode;
			parent_switch.BranchList.Add(add_branch);
			add_branch.ParentRef = parent_switch;
			this.currentBranch = add_branch;
		}
		void AddNormalNode(SyntaxNode add_node)
		{
			Trace.Assert(null != add_node);
			if (null != this.currentBranch)
			{
				this.currentBranch.ChildList.Add(add_node);
			}
			else
			{
				this.nodeList.Add(add_node);
			}
		}
		void AddEndIf()
		{
			Trace.Assert(null != this.currentBranch);
			Trace.Assert(null != this.currentBranch.ParentRef);
			this.currentBranch = this.currentBranch.ParentRef.ParentRef as PrecompileBranchNode;
		}

		public List<string> ToStringList()
		{
			List<string> ret_list = new List<string>();
			foreach (var node in this.nodeList)
			{
				ret_list.AddRange(node.ToStringList(0));
			}
			return ret_list;
		}
	}
}
