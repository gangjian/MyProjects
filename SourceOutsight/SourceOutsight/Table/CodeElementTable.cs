using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceOutsight
{
	public class CodeElementTable
	{
		List<CodeElement> ElementList = new List<CodeElement>();
		public void Add(CodeElement element)
		{
			this.ElementList.Add(element);
		}
		public void AddRange(List<CodeElement> element_list)
		{
			this.ElementList.AddRange(element_list);
		}
		public void Clear()
		{
			this.ElementList.Clear();
		}
	}
}
