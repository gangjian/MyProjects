﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace CodeCreeper
{
	public partial class Creeper
	{
		CodeProjectInfo prjRef = null;
		CreeperContext myContext = null;

		public Creeper(CodeProjectInfo prj_info)
		{
			Trace.Assert(null != prj_info);
			this.prjRef = prj_info;
		}

		public void CreepAll()
		{
			foreach (var src_path in this.prjRef.GetSourcePathList())
			{
				CreepFile(src_path);
			}
		}
		public void CreepFile(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && File.Exists(path));
			this.myContext = new CreeperContext();
			CodeFileInfo fi = new CodeFileInfo(path);

			while (true)
			{
				SyntaxNode node = fi.GetNextSyntaxNode();
				if (null == node)
				{
					break;
				}
				else
				{
					SyntaxNodeCreep(node);
				}
			}
		}
		void SyntaxNodeCreep(SyntaxNode node)
		{
			Trace.Assert(null != node);
			if (node.GetType() == typeof(PrecompileSwitchNode))
			{
				PrecompileSwitchNode pcs_node = node as PrecompileSwitchNode;
				// 取得第一个branch
				PrecompileBranchNode branch_node = pcs_node.BranchList.First();
				bool enter = this.myContext.JudgePrecompileBranchEnter(branch_node.TagStr,
															branch_node.ExpressionStr);
				//// 在节点上标记进入标志
				branch_node.EnterFlag = enter;
				// 在上下文中记录当前branch
			}
			else if (node.GetType() == typeof(PrecompileBranchNode))
			{
				PrecompileBranchNode branch_node = node as PrecompileBranchNode;
				bool enter = this.myContext.JudgePrecompileBranchEnter(branch_node.TagStr,
															branch_node.ExpressionStr);
				branch_node.EnterFlag = enter;
			}
			else if (node.GetType() == typeof(PrecompileEndIfNode))
			{
				
			}
			else
			{
				Trace.Assert(node.GetType() == typeof(SyntaxNode));
			}
			this.myContext.AddSyntaxTreeNode(node);
		}

		public List<string> GetSyntaxTreePrintList()
		{
			return this.myContext.GetSyntaxTreeStringList();
		}
	}
}
