using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CodeCreeper
{
	class UndefineInfo
	{
		CodeElement UndefElement = null;
		public NameInfo UndefName = null;
		string filePath = null;
		public string FilePath
		{
			get { return filePath; }
		}

		UndefineInfo(CodeElement undef_element, NameInfo undef_name, string path)
		{
			Trace.Assert(null != undef_element);
			Trace.Assert(null != undef_name);
			Trace.Assert(!string.IsNullOrEmpty(path));
			this.UndefElement = undef_element;
			this.UndefName = undef_name;
			this.filePath = path;
		}
		public static UndefineInfo Parse(CodeElement undef_element, CodeFileInfo file_info)
		{
			Trace.Assert(null != undef_element);
			Trace.Assert(null != file_info);
			List<CodeElement> element_list
						= file_info.GetLineElementList(undef_element.GetStartPosition());
			Trace.Assert(element_list.Count >= 2);
			CodeElement macro_element = element_list[1];
			Trace.Assert(macro_element.Type == ElementType.Identifier);
			NameInfo undef_name = new NameInfo(macro_element, file_info.CodeList);
			UndefineInfo ret_info
					= new UndefineInfo(undef_element, undef_name, file_info.FullName);
			return ret_info;
		}
	}
}
