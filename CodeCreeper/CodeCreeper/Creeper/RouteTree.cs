using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper
{
	class RouteTree
	{
		List<Node> nodeList = new List<Node>();
		BranchNode currentBranch = null;

		public void AddSwitchNode(string first_branch_tag, string first_branch_expression)
		{
			SwitchNode switch_node = new SwitchNode(first_branch_tag, first_branch_expression);
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
		public void AddBranchNode(string branch_tag, string branch_expression)
		{
			Trace.Assert(null != this.currentBranch);
			Trace.Assert(null != this.currentBranch.ParentRef);
			BranchNode add_branch = new BranchNode(branch_tag, branch_expression);
			SwitchNode parent_switch = this.currentBranch.ParentRef as SwitchNode;
			parent_switch.BranchList.Add(add_branch);
			add_branch.ParentRef = parent_switch;
			this.currentBranch = add_branch;
		}
		public void AddNormalNode(string tag, string expression, object info)
		{
			Node add_node = new Node(tag, expression);
			add_node.InfoRef = info;
			if (null != this.currentBranch)
			{
				this.currentBranch.ChildList.Add(add_node);
			}
			else
			{
				this.nodeList.Add(add_node);
			}
		}
		public void AddEndIf()
		{
			Trace.Assert(null != this.currentBranch);
			Trace.Assert(null != this.currentBranch.ParentRef);
			this.currentBranch = this.currentBranch.ParentRef.ParentRef as BranchNode;
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

	class Node
	{
		string tagStr = null;
		public string TagStr
		{
			get { return tagStr; }
			set { tagStr = value; }
		}
		string expressionStr = null;
		public string ExpressionStr
		{
			get { return expressionStr; }
			set { expressionStr = value; }
		}
		private Node parentRef = null;
		public Node ParentRef
		{
			get { return parentRef; }
			set { parentRef = value; }
		}
		public object InfoRef = null;

		public Node()
		{
		}
		public Node(string tag_str, string expression_str)
		{
			this.TagStr = tag_str;
			this.ExpressionStr = expression_str;
		}
		public virtual List<string> ToStringList(int level)
		{
			List<string> ret_list = new List<string>();
			ret_list.Add(this.ToString(level));
			return ret_list;
		}
		protected string ToString(int level)
		{
			string ret_str = GetIndentStr(level) + this.TagStr + " : " + this.ExpressionStr;
			return ret_str;
		}
		string GetIndentStr(int level)
		{
			string ret_str = string.Empty;
			for (int i = 0; i < level; i++)
			{
				ret_str += "\t";
			}
			return ret_str;
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

		public SwitchNode(string first_branch_tag, string first_branch_expression)
		{
			BranchNode first_branch = new BranchNode(first_branch_tag, first_branch_expression);
			first_branch.ParentRef = this;
			this.BranchList.Add(first_branch);
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
	class BranchNode : Node
	{
		List<Node> childList = new List<Node>();
		internal List<Node> ChildList
		{
			get { return childList; }
			set { childList = value; }
		}
		public BranchNode(string tag_str, string expression_str) : base(tag_str, expression_str)
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
}
