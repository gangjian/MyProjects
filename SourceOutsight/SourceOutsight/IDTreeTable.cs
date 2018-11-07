using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SourceOutsight
{
	public class IDTreeTable
	{
		List<IDTreeNode> IDTreeList = new List<IDTreeNode>();
		IDTreeNode CurrentBranchNode = null;
		public void Add(IDTreeNode add_node)
		{
			if (add_node.IsPrecompileSwitchNode())
			{
				AddPrecompileSwitchNode(add_node);
			}
			else if (add_node.IsNormalNode())
			{
				AddNormalNode(add_node);
			}
			else if (add_node.IsStructureNode())
			{
				AddStructureNode(add_node);
			}
			else
			{
				Trace.Assert(false);
			}
		}
		void AddPrecompileSwitchNode(IDTreeNode add_node)
		{
			if (add_node.IDStr.Equals("#if")
				|| add_node.IDStr.Equals("#ifdef")
				|| add_node.IDStr.Equals("#ifndef"))
			{
				if (null == this.CurrentBranchNode)
				{
					this.IDTreeList.Add(add_node);
					this.CurrentBranchNode = add_node;
				}
				else
				{
					add_node.ParentRef = this.CurrentBranchNode;
					this.CurrentBranchNode.ChildList.Add(add_node);
					this.CurrentBranchNode = add_node;
				}
			}
			else if (add_node.IDStr.Equals("#elif")
					 || add_node.IDStr.Equals("#else"))
			{
				Trace.Assert(null != this.CurrentBranchNode);
				add_node.ParentRef = this.CurrentBranchNode.ParentRef;
				if (null != this.CurrentBranchNode.ParentRef)
				{
					this.CurrentBranchNode.ParentRef.ChildList.Add(add_node);
				}
				else
				{
					this.IDTreeList.Add(add_node);
				}
				this.CurrentBranchNode = add_node;
			}
			else if (add_node.IDStr.Equals("#endif"))
			{
				Trace.Assert(null != this.CurrentBranchNode);
				add_node.ParentRef = this.CurrentBranchNode.ParentRef;
				if (null != this.CurrentBranchNode.ParentRef)
				{
					this.CurrentBranchNode.ParentRef.ChildList.Add(add_node);
				}
				else
				{
					this.IDTreeList.Add(add_node);
				}
				this.CurrentBranchNode = add_node.ParentRef;
			}
			else
			{
				Trace.Assert(false);
			}
		}
		void AddNormalNode(IDTreeNode add_node)
		{
			if (null == this.CurrentBranchNode)
			{
				this.IDTreeList.Add(add_node);
			}
			else
			{
				this.CurrentBranchNode.ChildList.Add(add_node);
			}
		}
		void AddStructureNode(IDTreeNode add_node)
		{
			if (null == this.CurrentBranchNode)
			{
				this.IDTreeList.Add(add_node);
			}
			else
			{
				this.CurrentBranchNode.ChildList.Add(add_node);
			}
		}

		public List<string> ToStringList()
		{
			List<string> ret_list = new List<string>();
			foreach (var node_item in this.IDTreeList)
			{
				ret_list.AddRange(node_item.ToStringList(0));
			}
			return ret_list;
		}
	}

	public class IDTreeNode
	{
		public string IDStr = null;
		public string ExpressionStr = null;
		public CodePosition Position = null;
		public CodeScope AffectScope = null;											// 作用域
		public IDNodeType Type = IDNodeType.Unknown;

		public IDTreeNode ParentRef = null;
		public List<IDTreeNode> ChildList = new List<IDTreeNode>();

		public IDTreeNode(string id_str, string exp_str, CodePosition pos, CodeScope scope, IDNodeType type)
		{
			this.IDStr = id_str;
			this.ExpressionStr = exp_str;
			this.Position = pos;
			this.AffectScope = scope;
			this.Type = type;
		}

		public bool IsPrecompileSwitchNode()
		{
			if (this.Type == IDNodeType.PrecompileSwitch)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public bool IsNormalNode()
		{
			if (this.Type == IDNodeType.IncludeHeader
				|| this.Type == IDNodeType.MacroDef
				|| this.Type == IDNodeType.MacroFunc
				|| this.Type == IDNodeType.GlobalDef
				|| this.Type == IDNodeType.GlobalExtern
				|| this.Type == IDNodeType.FuncDef
				|| this.Type == IDNodeType.FuncExtern
				|| this.Type == IDNodeType.Typedef)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public bool IsStructureNode()
		{
			if (this.Type == IDNodeType.StructType
				|| this.Type == IDNodeType.UnionType)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public List<string> ToStringList(int level)
		{
			List<string> ret_list = new List<string>();
			ret_list.Add(this.ToString(level));
			foreach (var item in this.ChildList)
			{
				ret_list.AddRange(item.ToStringList(level + 1));
			}
			return ret_list;
		}
		string ToString(int level)
		{
			string ret_str = GetBranchString(level) + GetIconString() + " " + this.IDStr;
			if (!string.IsNullOrEmpty(this.ExpressionStr))
			{
				ret_str += (" " + this.ExpressionStr);
			}
			return ret_str;
		}
		string GetIconString()
		{
			string icon_str = null;
			switch (this.Type)
			{
				case IDNodeType.Unknown:
					Trace.Assert(false);
					break;
				case IDNodeType.PrecompileSwitch:
					icon_str = "<#_y>";
					break;
				case IDNodeType.IncludeHeader:
					icon_str = "<#_y>";
					break;
				case IDNodeType.MacroDef:
					icon_str = "<#_r>";
					break;
				case IDNodeType.MacroFunc:
					icon_str = "<M>";
					break;
				case IDNodeType.StructType:
					icon_str = "<S>";
					break;
				case IDNodeType.UnionType:
					icon_str = "<U>";
					break;
				case IDNodeType.MemberVar:
					icon_str = "<m>";
					break;
				case IDNodeType.Typedef:
					icon_str = "<T>";
					break;
				case IDNodeType.GlobalExtern:
					icon_str = "<G_E>";
					break;
				case IDNodeType.GlobalDef:
					icon_str = "<G_D>";
					break;
				case IDNodeType.FuncExtern:
					icon_str = "<F_E>";
					break;
				case IDNodeType.FuncDef:
					icon_str = "<F_D>";
					break;
				default:
					Trace.Assert(false);
					break;
			}
			return icon_str;
		}
		string GetBranchString(int level)
		{
			string ret_str = string.Empty;
			for (int i = 0; i < level; i++)
			{
				ret_str += "\t";
			}
			return ret_str;
		}
	}

	public enum IDNodeType
	{
		Unknown,
		PrecompileSwitch,
		IncludeHeader,
		MacroDef,
		MacroFunc,
		StructType,
		UnionType,
		MemberVar,
		Typedef,
		GlobalExtern,
		GlobalDef,
		FuncExtern,
		FuncDef,
	}
}
