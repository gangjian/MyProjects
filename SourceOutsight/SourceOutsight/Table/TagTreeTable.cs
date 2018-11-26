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
			if (add_node.Info.TagStr.Equals("#if")
				|| add_node.Info.TagStr.Equals("#ifdef")
				|| add_node.Info.TagStr.Equals("#ifndef"))
			{
				if (null == this.CurrentBranchNode)
				{
					this.TagTreeList.Add(add_node);
					this.CurrentBranchNode = add_node;
				}
				else
				{
					add_node.ParentRef = this.CurrentBranchNode;
					this.CurrentBranchNode.AddChild(add_node);
					this.CurrentBranchNode = add_node;
				}
			}
			else if (add_node.Info.TagStr.Equals("#elif")
					 || add_node.Info.TagStr.Equals("#else"))
			{
				Trace.Assert(null != this.CurrentBranchNode);
				add_node.ParentRef = this.CurrentBranchNode.ParentRef;
				if (null != this.CurrentBranchNode.ParentRef)
				{
					this.CurrentBranchNode.ParentRef.AddChild(add_node);
				}
				else
				{
					this.TagTreeList.Add(add_node);
				}
				this.CurrentBranchNode = add_node;
			}
			else if (add_node.Info.TagStr.Equals("#endif"))
			{
				Trace.Assert(null != this.CurrentBranchNode);
				add_node.ParentRef = this.CurrentBranchNode.ParentRef;
				if (null != this.CurrentBranchNode.ParentRef)
				{
					this.CurrentBranchNode.ParentRef.AddChild(add_node);
				}
				else
				{
					this.TagTreeList.Add(add_node);
				}
				this.CurrentBranchNode = add_node.ParentRef;
			}
			else if (add_node.Info.TagStr.Equals("#pragma"))
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
				this.CurrentBranchNode.AddChild(add_node);
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
				this.CurrentBranchNode.AddChild(add_node);
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

		public List<TagTreeNode> SearchTag(string tag_str)
		{
			Trace.Assert(!string.IsNullOrEmpty(tag_str));
			List<TagTreeNode> ret_list = new List<TagTreeNode>();
			foreach (var node in this.TagTreeList)
			{
				if (node.Info.TagStr.Equals(tag_str))
				{
					ret_list.Add(node);
				}
				ret_list.AddRange(node.SearchTag(tag_str));
			}
			return ret_list;
		}
	}

	public class TagTreeNode
	{
		public TagInfo Info = null;
		public TagTreeNode ParentRef = null;
		List<TagTreeNode> ChildList = new List<TagTreeNode>();
		public TagTreeNode(string tag_str, string exp_str, CodePosition pos, CodeScope scope, TagNodeType type)
		{
			this.Info = new TagInfo(tag_str, exp_str, pos, scope, type);
		}

		public bool IsPrecompileSwitchNode()
		{
			if (this.Info.Type == TagNodeType.PrecompileSwitch)
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
			if (this.Info.Type == TagNodeType.IncludeHeader
				|| this.Info.Type == TagNodeType.MacroDef
				|| this.Info.Type == TagNodeType.MacroFunc
				|| this.Info.Type == TagNodeType.Undef
				|| this.Info.Type == TagNodeType.GlobalDef
				|| this.Info.Type == TagNodeType.GlobalExtern
				|| this.Info.Type == TagNodeType.FuncDef
				|| this.Info.Type == TagNodeType.FuncExtern
				|| this.Info.Type == TagNodeType.Typedef
				|| this.Info.Type == TagNodeType.PrecompileCommand)
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
			if (this.Info.Type == TagNodeType.StructType
				|| this.Info.Type == TagNodeType.UnionType)
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
			string ret_str = GetBranchString(level) + GetIconString() + " " + this.Info.TagStr;
			if (!string.IsNullOrEmpty(this.Info.ExpressionStr))
			{
				ret_str += (" " + this.Info.ExpressionStr);
			}
			return ret_str;
		}
		string GetIconString()
		{
			string icon_str = null;
			switch (this.Info.Type)
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
		public void AddChild(TagTreeNode add_node)
		{
			this.ChildList.Add(add_node);
		}

		public List<TagTreeNode> SearchTag(string tag_str)
		{
			List<TagTreeNode> ret_list = new List<TagTreeNode>();
			foreach (var child in this.ChildList)
			{
				if (child.Info.TagStr.Equals(tag_str))
				{
					ret_list.Add(child);
				}
				ret_list.AddRange(child.SearchTag(tag_str));
			}
			return ret_list;
		}
	}

	public class TagInfo
	{
		public string TagStr = null;
		public string ExpressionStr = null;
		public CodePosition Position = null;											// 标识符的开始位置
		public CodeScope AffectScope = null;											// 作用域
		public TagNodeType Type = TagNodeType.Unknown;
		public object DataRef = null;
		public TagInfo(string tag_str, string exp_str, CodePosition pos, CodeScope scope, TagNodeType type)
		{
			this.TagStr = tag_str;
			this.ExpressionStr = exp_str;
			this.Position = pos;
			this.AffectScope = scope;
			this.Type = type;
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
