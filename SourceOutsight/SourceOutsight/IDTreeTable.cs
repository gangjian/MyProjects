using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SourceOutsight
{
	public class TagTreeTable
	{
		List<TagTreeNode> TagTreeList = new List<TagTreeNode>();
		TagTreeNode CurrentBranchNode = null;
		public void Add(TagTreeNode add_node)
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
		void AddPrecompileSwitchNode(TagTreeNode add_node)
		{
			if (add_node.TagStr.Equals("#if")
				|| add_node.TagStr.Equals("#ifdef")
				|| add_node.TagStr.Equals("#ifndef"))
			{
				if (null == this.CurrentBranchNode)
				{
					this.TagTreeList.Add(add_node);
					this.CurrentBranchNode = add_node;
				}
				else
				{
					add_node.ParentRef = this.CurrentBranchNode;
					this.CurrentBranchNode.ChildList.Add(add_node);
					this.CurrentBranchNode = add_node;
				}
			}
			else if (add_node.TagStr.Equals("#elif")
					 || add_node.TagStr.Equals("#else"))
			{
				Trace.Assert(null != this.CurrentBranchNode);
				add_node.ParentRef = this.CurrentBranchNode.ParentRef;
				if (null != this.CurrentBranchNode.ParentRef)
				{
					this.CurrentBranchNode.ParentRef.ChildList.Add(add_node);
				}
				else
				{
					this.TagTreeList.Add(add_node);
				}
				this.CurrentBranchNode = add_node;
			}
			else if (add_node.TagStr.Equals("#endif"))
			{
				Trace.Assert(null != this.CurrentBranchNode);
				add_node.ParentRef = this.CurrentBranchNode.ParentRef;
				if (null != this.CurrentBranchNode.ParentRef)
				{
					this.CurrentBranchNode.ParentRef.ChildList.Add(add_node);
				}
				else
				{
					this.TagTreeList.Add(add_node);
				}
				this.CurrentBranchNode = add_node.ParentRef;
			}
			else if (add_node.TagStr.Equals("#pragma"))
			{
				AddNormalNode(add_node);
			}
			else
			{
				Trace.Assert(false);
			}
		}
		void AddNormalNode(TagTreeNode add_node)
		{
			if (null == this.CurrentBranchNode)
			{
				this.TagTreeList.Add(add_node);
			}
			else
			{
				this.CurrentBranchNode.ChildList.Add(add_node);
			}
		}
		void AddStructureNode(TagTreeNode add_node)
		{
			if (null == this.CurrentBranchNode)
			{
				this.TagTreeList.Add(add_node);
			}
			else
			{
				this.CurrentBranchNode.ChildList.Add(add_node);
			}
		}

		public List<string> ToStringList()
		{
			List<string> ret_list = new List<string>();
			foreach (var node_item in this.TagTreeList)
			{
				ret_list.AddRange(node_item.ToStringList(0));
			}
			return ret_list;
		}
	}

	public class TagTreeNode
	{
		public string TagStr = null;
		public string ExpressionStr = null;
		public CodePosition Position = null;											// 标识符的开始位置
		public CodeScope AffectScope = null;											// 作用域
		public TagNodeType Type = TagNodeType.Unknown;

		public TagTreeNode ParentRef = null;
		public List<TagTreeNode> ChildList = new List<TagTreeNode>();

		public TagTreeNode(string tag_str, string exp_str, CodePosition pos, CodeScope scope, TagNodeType type)
		{
			this.TagStr = tag_str;
			this.ExpressionStr = exp_str;
			this.Position = pos;
			this.AffectScope = scope;
			this.Type = type;
		}

		public bool IsPrecompileSwitchNode()
		{
			if (this.Type == TagNodeType.PrecompileSwitch)
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
			if (this.Type == TagNodeType.IncludeHeader
				|| this.Type == TagNodeType.MacroDef
				|| this.Type == TagNodeType.MacroFunc
				|| this.Type == TagNodeType.Undef
				|| this.Type == TagNodeType.GlobalDef
				|| this.Type == TagNodeType.GlobalExtern
				|| this.Type == TagNodeType.FuncDef
				|| this.Type == TagNodeType.FuncExtern
				|| this.Type == TagNodeType.Typedef
				|| this.Type == TagNodeType.PrecompileCommand)
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
			if (this.Type == TagNodeType.StructType
				|| this.Type == TagNodeType.UnionType)
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
			string ret_str = GetBranchString(level) + GetIconString() + " " + this.TagStr;
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
				case TagNodeType.Unknown:
					Trace.Assert(false);
					break;
				case TagNodeType.PrecompileSwitch:
					icon_str = "<#_y>";
					break;
				case TagNodeType.IncludeHeader:
					icon_str = "<#_y>";
					break;
				case TagNodeType.MacroDef:
					icon_str = "<#_r>";
					break;
				case TagNodeType.MacroFunc:
					icon_str = "<M>";
					break;
				case TagNodeType.StructType:
					icon_str = "<S>";
					break;
				case TagNodeType.UnionType:
					icon_str = "<U>";
					break;
				case TagNodeType.MemberVar:
					icon_str = "<m>";
					break;
				case TagNodeType.Typedef:
					icon_str = "<T>";
					break;
				case TagNodeType.GlobalExtern:
					icon_str = "<G_E>";
					break;
				case TagNodeType.GlobalDef:
					icon_str = "<G_D>";
					break;
				case TagNodeType.FuncExtern:
					icon_str = "<F_E>";
					break;
				case TagNodeType.FuncDef:
					icon_str = "<F_D>";
					break;
				case TagNodeType.Undef:
				case TagNodeType.PrecompileCommand:
					icon_str = "<#_y>";
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

	public enum TagNodeType
	{
		Unknown,
		PrecompileSwitch,
		PrecompileCommand,
		IncludeHeader,
		MacroDef,
		MacroFunc,
		Undef,
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
