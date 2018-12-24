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
		RouteTree routTreeObj = new RouteTree();
		public Creeper(CodeProjectInfo prj_info)
		{
			Trace.Assert(null != prj_info);
			this.prjRef = prj_info;
		}

		public void CreepAll()
		{
			this.routTreeObj = new RouteTree();
			foreach (var src_path in this.prjRef.GetSourcePathList())
			{
				CreepFile(src_path);
			}
		}
		public void CreepFile(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && File.Exists(path));
			CodeFileInfo fi = new CodeFileInfo(path);
			while (true)
			{
				CodeElement element = fi.GetNextElement();
				if (null == element)
				{
					break;
				}
				else
				{
					CodeElementProc(element, fi);
				}
			}
		}

		void CodeElementProc(CodeElement element, CodeFileInfo file_info)
		{
			if (element.Type.Equals(ElementType.Define))
			{
				DefineProc(element, file_info);
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
				PrecompileSwitchProc(element, file_info);
			}
			else if (element.Type.Equals(ElementType.Identifier))
			{
				//this.ProjectRef.SearchTag(element.ToString(this.CodeList));
			}
		}

		void DefineProc(CodeElement def_element, CodeFileInfo file_info)
		{
			DefineInfo def_info = DefineInfo.TryParse(def_element, file_info);
			this.routTreeObj.AddNormalNode("#define", def_info.Name, def_info);
		}
		void PrecompileSwitchProc(CodeElement element, CodeFileInfo file_info)
		{
			string tag_str = element.ToString(file_info.CodeList);
			// #if, #ifdef, #ifndef -> new Switch Node
			if (tag_str.Equals("#if")
				|| tag_str.Equals("#ifdef")
				|| tag_str.Equals("#ifndef"))
			{
				string expression_str = GetBranchExpressionStr(element, file_info);
				this.routTreeObj.AddSwitchNode(tag_str, expression_str);
			}
			// #else, #elif			-> new Branch
			else if (tag_str.Equals("#else"))
			{
				this.routTreeObj.AddBranchNode(tag_str, null);
			}
			else if (tag_str.Equals("#elif"))
			{
				string expression_str = GetBranchExpressionStr(element, file_info);
				this.routTreeObj.AddBranchNode(tag_str, expression_str);
			}
			// #endif				-> finish Node
			else if (tag_str.Equals("#endif"))
			{
				this.routTreeObj.AddEndIf();
			}
			else
			{
				Trace.Assert(false);
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
