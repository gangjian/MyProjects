using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace CodeCreeper
{
	class Creeper
	{
		CodeProjectInfo prjRef = null;
		CodePosition currentPosition = null;
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
			CodeFileInfo fi = new CodeFileInfo(path);
			this.currentPosition = CodePosition.GetFileStartPosition(fi.CodeList);
			while (true)
			{
				CodeElement element = GetNextElement();
				if (null == element)
				{
					break;
				}
			}
		}
		CodeElement GetNextElement()
		{
			if (null == this.currentPosition)
			{
				return null;
			}
			return null;
		}
	}
}
