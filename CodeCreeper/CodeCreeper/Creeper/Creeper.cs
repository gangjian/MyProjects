using System;
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
		SyntaxTree syntaxTreeObj = null;
		RunContext myContext = null;

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
			this.syntaxTreeObj = new SyntaxTree();
			this.myContext = new RunContext();
			CodeFileInfo fi = new CodeFileInfo(path);

			while (true)
			{
				SyntaxNode node = GetNextSyntaxNode(fi);
				if (null == node)
				{
					break;
				}
				else
				{
					SyntaxNodeProc(node);
				}
			}
		}
		void SyntaxNodeProc(SyntaxNode node)
		{
			Trace.Assert(null != node);
			this.syntaxTreeObj.AddNode(node);
		}
		SyntaxNode GetNextSyntaxNode(CodeFileInfo file_info)
		{
			while (true)
			{
				CodeElement element = file_info.GetNextElement();
				if (null == element)
				{
					break;
				}
				else
				{
					SyntaxNode ret_node = CodeElementProc(element, file_info);
					if (null != ret_node)
					{
						return ret_node;
					}
				}
			}
			return null;
		}

		SyntaxNode CodeElementProc(CodeElement element, CodeFileInfo file_info)
		{
			SyntaxNode ret_node = null;
			if (element.Type.Equals(ElementType.Define))
			{
				ret_node = DefineProc(element, file_info);
			}
			else if (element.Type.Equals(ElementType.Undefine))
			{
				ret_node = UndefProc(element, file_info);
			}
			else if (element.Type.Equals(ElementType.PrecompileCommand))
			{
				ret_node = PrecompileCommandProc(element, file_info);
			}
			else if (element.Type.Equals(ElementType.Include))
			{
				ret_node = IncludeProc(element, file_info);
			}
			else if (element.Type.Equals(ElementType.PrecompileSwitch))
			{
				ret_node = PrecompileSwitchProc(element, file_info);
			}
			else if (element.Type.Equals(ElementType.Identifier))
			{
				//this.ProjectRef.SearchTag(element.ToString(this.CodeList));
			}
			return ret_node;
		}

		SyntaxNode DefineProc(CodeElement def_element, CodeFileInfo file_info)
		{
			DefineInfo def_info = DefineInfo.Parse(def_element, file_info);
			SyntaxNode def_node = new SyntaxNode("#define", def_info.DefName.GetName());
			def_node.InfoRef = def_info;
			return def_node;
		}
		SyntaxNode UndefProc(CodeElement undef_element, CodeFileInfo file_info)
		{
			UndefineInfo undef_info = UndefineInfo.Parse(undef_element, file_info);
			SyntaxNode undef_node = new SyntaxNode("#undef", undef_info.UndefName.GetName());
			undef_node.InfoRef = undef_info;
			return undef_node;
		}
		SyntaxNode IncludeProc(CodeElement inc_element, CodeFileInfo file_info)
		{
			IncludeInfo inc_info = IncludeInfo.Parse(inc_element, file_info);
			SyntaxNode inc_node = new SyntaxNode("include", inc_info.GetName());
			inc_node.InfoRef = inc_info;
			return inc_node;
		}
		SyntaxNode PrecompileSwitchProc(CodeElement element, CodeFileInfo file_info)
		{
			string tag_str = element.ToString(file_info.CodeList);
			// #if, #ifdef, #ifndef -> new Switch Node
			if (tag_str.Equals("#if")
				|| tag_str.Equals("#ifdef")
				|| tag_str.Equals("#ifndef"))
			{
				string expression_str = GetBranchExpressionStr(element, file_info);
				PrecompileBranchNode first_branch_node
					= new PrecompileBranchNode(tag_str, expression_str);
				PrecompileSwitchNode switch_node = new PrecompileSwitchNode(first_branch_node);
				return switch_node;
			}
			// #else, #elif			-> new Branch
			else if (tag_str.Equals("#else"))
			{
				PrecompileBranchNode branch_node = new PrecompileBranchNode(tag_str, null);
				return branch_node;
			}
			else if (tag_str.Equals("#elif"))
			{
				string expression_str = GetBranchExpressionStr(element, file_info);
				PrecompileBranchNode branch_node = new PrecompileBranchNode(tag_str, expression_str);
				return branch_node;
			}
			// #endif				-> finish Node
			else if (tag_str.Equals("#endif"))
			{
				return new PrecompileEndIfNode();
			}
			else
			{
				Trace.Assert(false);
				return null;
			}
		}
		string GetBranchExpressionStr(CodeElement element, CodeFileInfo file_info)
		{
			List<CodeElement> element_list = file_info.GetLineElementList(element.GetStartPosition());
			if (element_list.Count > 1)
			{
				element_list.RemoveAt(0);
				return CommProc.ElementListStrCat(element_list, file_info.CodeList);
			}
			else
			{
				return string.Empty;
			}
		}
		SyntaxNode PrecompileCommandProc(CodeElement element, CodeFileInfo file_info)
		{
			PrecompileCmdInfo precompile_cmd_info = PrecompileCmdInfo.Parse(element, file_info);
			//SyntaxNode precompile_cmd_node = new SyntaxNode(precompile_cmd_info.CmdName, )
			return null;
		}

		public List<string> GetSyntaxTreePrintList()
		{
			return this.syntaxTreeObj.ToStringList();
		}
	}
}
