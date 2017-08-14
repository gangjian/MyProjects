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
		public static FileParseInfo UnitTest_GetSourceFileStructure(string full_path, string func_name, ref StatementNode func_root_node)
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
			CCodeAnalyser.CodeBufferManager buffManager = new CCodeAnalyser.CodeBufferManager();
			CCodeAnalyser CAnalyser = new CCodeAnalyser(csfList, header_list, ref buffManager);
			List<FileParseInfo> parseInfoList = CAnalyser.CFileListProc();
			FileParseInfo code_parse_info;
			FuncParseInfo funInfo = CFunctionAnalysis.GetFuncInfoFromParseResult(full_path, func_name, parseInfoList, out code_parse_info);

			// 指定函数语句树结构的分析提取
			func_root_node = new StatementNode();
			func_root_node.Type = StatementNodeType.Root;
			func_root_node.Scope = funInfo.Scope;
			CFunctionAnalysis.GetFuncBlockStruct(code_parse_info, ref func_root_node);
			return code_parse_info;
		}

		public static List<FileParseInfo> UnitTest_SourceFileProcess2(string full_path)
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
			CCodeAnalyser.CodeBufferManager buffManager = new CCodeAnalyser.CodeBufferManager();
			CCodeAnalyser CAnalyser = new CCodeAnalyser(csfList, header_list, ref buffManager);
			List<FileParseInfo> parseInfoList = CAnalyser.CFileListProc();

			return parseInfoList;
		}

		public static FileParseInfo UnitTest_GetFuncParseResult(string full_path, string fun_name,
																List<FileParseInfo> parseInfoList,
																ref StatementNode root_node)
		{
			FileParseInfo code_parse_info;
			FuncParseInfo funInfo = CFunctionAnalysis.GetFuncInfoFromParseResult(full_path, fun_name, parseInfoList, out code_parse_info);

			// 指定函数语句树结构的分析提取
			root_node = new StatementNode();
			root_node.Type = StatementNodeType.Root;
			root_node.Scope = funInfo.Scope;
			CFunctionAnalysis.GetFuncBlockStruct(code_parse_info, ref root_node);
			return code_parse_info;
		}
	}
}
