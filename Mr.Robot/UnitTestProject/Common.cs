using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mr.Robot;

namespace UnitTestProject
{
	class Common
	{
		public static List<FILE_PARSE_INFO> UnitTest_GetSourceFileStructure(string full_path)
		{
			if (string.IsNullOrEmpty(full_path))
			{
				return null;
			}
			string folder_path = string.Empty;
			int idx = full_path.LastIndexOf("\\");
			if (-1 == idx)
			{
				folder_path = System.Environment.CurrentDirectory;
			}
			else
			{
				folder_path = full_path.Remove(idx);
			}

			List<string> source_list = new List<string>();
			List<string> header_list = new List<string>();
			List<string> mtpj_file_list = new List<string>();
			List<string> mk_file_list = new List<string>();
			// 取得所有源文件和头文件列表
			IOProcess.GetAllCCodeFiles(folder_path, ref source_list, ref header_list, ref mtpj_file_list, ref mk_file_list);
			// 解析指定的源文件,并取得解析结果
			List<string> csfList = new List<string>();
			csfList.Add(full_path);

			// =================================
			C_PROSPECTOR cProspector = new C_PROSPECTOR(csfList, header_list);
			List<FILE_PARSE_INFO> parseInfoList = cProspector.SyncStart();				// <--- 解析源文件宏定义, 全局量, 函数定位
			// =================================
			return parseInfoList;
		}

		public static List<FILE_PARSE_INFO> UnitTest_SourceFileProcess2(string full_path)
		{
			if (string.IsNullOrEmpty(full_path))
			{
				return null;
			}
			int idx = full_path.LastIndexOf("\\");
			if (-1 == idx)
			{
				return null;
			}
			string folder_path = full_path.Remove(idx);

			List<string> source_list = new List<string>();
			List<string> header_list = new List<string>();
			List<string> mtpj_file_list = new List<string>();
			List<string> mk_file_list = new List<string>();
			// 取得所有源文件和头文件列表
			IOProcess.GetAllCCodeFiles(folder_path, ref source_list, ref header_list, ref mtpj_file_list, ref mk_file_list);
			// 解析指定的源文件,并取得解析结果
			List<string> csfList = new List<string>();
			csfList.Add(full_path);
			C_PROSPECTOR cProspector = new C_PROSPECTOR(csfList, header_list);
			List<FILE_PARSE_INFO> parseInfoList = cProspector.SyncStart();

			return parseInfoList;
		}

		public static FILE_PARSE_INFO FindSrcParseInfoFromList(string src_name, List<FILE_PARSE_INFO> parse_info_list)
		{
			foreach (var item in parse_info_list)
			{
				if (item.SourceName.Equals(src_name))
				{
					return item;
				}
			}
			return null;
		}
	}
}
