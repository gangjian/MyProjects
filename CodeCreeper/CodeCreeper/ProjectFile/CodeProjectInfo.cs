using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace CodeCreeper
{
	public class CodeProjectInfo
	{
		List<string> sourcePathList = null;
		List<string> headerPathList = null;
		string projectPath = null;
		public CodeProjectInfo(string prj_dir)
		{
			Trace.Assert(!string.IsNullOrEmpty(prj_dir) && Directory.Exists(prj_dir));
			this.projectPath = prj_dir;
			CommProc.GetCodeFileList(this.projectPath, this.sourcePathList, this.headerPathList);
		}
		public List<string> GetSourcePathList()
		{
			return this.sourcePathList;
		}
	}
}
