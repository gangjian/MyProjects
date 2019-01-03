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
		SyntaxTree routTreeObj = new SyntaxTree();
		public Creeper(CodeProjectInfo prj_info)
		{
			Trace.Assert(null != prj_info);
			this.prjRef = prj_info;
		}

		public void CreepAll()
		{
			this.routTreeObj = new SyntaxTree();
			foreach (var src_path in this.prjRef.GetSourcePathList())
			{
				CreepFile(src_path);
			}
		}
		public void CreepFile(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && File.Exists(path));
			CodeFileInfo fi = new CodeFileInfo(path);
			SyntaxNode ret_node = null;
			while (true)
			{
				CodeElement element = fi.GetNextElement();
				if (null == element)
				{
					break;
				}
				else
				{
					ret_node = CodeElementProc(element, fi);
					if (null != ret_node)
					{
						this.routTreeObj.AddNode(ret_node);
					}
				}
			}
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
				//UndefProc(element);
			}
			else if (element.Type.Equals(ElementType.PrecompileCommand))
			{
				//PrecompileCommandProc(element);
			}
			else if (element.Type.Equals(ElementType.Include))
			{
				//IncludeProc(element);
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
			DefineInfo def_info = DefineInfo.TryParse(def_element, file_info);
			SyntaxNode def_node = new SyntaxNode("#define", def_info.Name);
			def_node.InfoRef = def_info;
			return def_node;
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

		public List<string> GetRouteTreePrintList()
		{
			return this.routTreeObj.ToStringList();
		}
	}
}
