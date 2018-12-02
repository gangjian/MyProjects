using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper.Creeper
{
	class RouteTree
	{
		List<Node> nodeList = new List<Node>();
		BranchNode currentBranch = null;

		public void AddSwitchNode(string first_branch_key, string first_branch_expression)
		{
			SwitchNode switch_node = new SwitchNode();
			BranchNode first_branch = new BranchNode(first_branch_key, first_branch_expression);
			switch_node.BranchList.Add(first_branch);
			if (null == this.currentBranch)
			{
				this.nodeList.Add(switch_node);
			}
			else
			{
				switch_node.ParentRef = this.currentBranch;
				this.currentBranch.ChildList.Add(switch_node);
			}
		}
	}

	class Node
	{
		private string markStr = null;
		public string MarkStr
		{
			get { return markStr; }
			set { markStr = value; }
		}
		private Node parentRef = null;
		public Node ParentRef
		{
			get { return parentRef; }
			set { parentRef = value; }
		}
	}

	class SwitchNode : Node
	{
		List<BranchNode> branchList = new List<BranchNode>();
		internal List<BranchNode> BranchList
		{
			get { return branchList; }
			set { branchList = value; }
		}
	}

	class BranchNode : Node
	{
		string keyStr = null;
		public string KeyStr
		{
			get { return keyStr; }
			set { keyStr = value; }
		}
		string expressionStr = null;
		public string ExpressionStr
		{
			get { return expressionStr; }
			set { expressionStr = value; }
		}
		List<Node> childList = new List<Node>();
		internal List<Node> ChildList
		{
			get { return childList; }
			set { childList = value; }
		}
		public BranchNode(string key_str, string expression_str)
		{
			this.keyStr = key_str;
			this.expressionStr = expression_str;
		}
	}
}
