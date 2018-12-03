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
				//PrecompileSwitchProc(element);
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
	}
}
