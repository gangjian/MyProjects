using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace CodeCreeper
{
	class CodeFileInfo
	{
		string fullName = null;
		public string FullName
		{
			get { return fullName; }
		}
		List<string> codeList = null;
		public List<string> CodeList
		{
			get { return codeList; }
		}
		public CodeFileInfo(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && File.Exists(path));
			this.fullName = path;
			this.codeList = File.ReadAllLines(path).ToList();
		}
	}
}
