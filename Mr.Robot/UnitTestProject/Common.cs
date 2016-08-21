﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mr.Robot;

namespace UnitTestProject
{
	class Common
	{
		public static CCodeParseResult UnitTest_SourceFileProcess(string full_path, string fun_name, ref StatementNode root_node)
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
			// 取得所有源文件和头文件列表
			IOProcess.GetAllCCodeFiles(folder_path, ref source_list, ref header_list);
			// 解析指定的源文件,并取得解析结果
			List<string> csfList = new List<string>();
			csfList.Add(full_path);
			List<CCodeParseResult> parseResultList = CCodeAnalyser.CFileListProcess(csfList, header_list);
			CCodeParseResult code_parse_result;
			CFunctionStructInfo funInfo = CCodeAnalyser.FindFileAndFunctionStructInfoFromParseResult(full_path, fun_name, parseResultList, out code_parse_result);

			// 指定函数语句树结构的分析提取
			root_node = new StatementNode();
			root_node.Type = StatementNodeType.Root;
			root_node.Scope = funInfo.Scope;
			CCodeAnalyser.GetCodeBlockStructure(code_parse_result.SourceParseInfo.parsedCodeList, root_node);
			return code_parse_result;
		}

		public static List<CCodeParseResult> UnitTest_SourceFileProcess2(string full_path)
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
			// 取得所有源文件和头文件列表
			IOProcess.GetAllCCodeFiles(folder_path, ref source_list, ref header_list);
			// 解析指定的源文件,并取得解析结果
			List<string> csfList = new List<string>();
			csfList.Add(full_path);
			List<CCodeParseResult> parseResultList = CCodeAnalyser.CFileListProcess(csfList, header_list);

			return parseResultList;
		}

		public static CCodeParseResult UnitTest_GetFuncParseResult(string full_path, string fun_name, List<CCodeParseResult> parseResultList, ref StatementNode root_node)
		{
			CCodeParseResult code_parse_result;
			CFunctionStructInfo funInfo = CCodeAnalyser.FindFileAndFunctionStructInfoFromParseResult(full_path, fun_name, parseResultList, out code_parse_result);

			// 指定函数语句树结构的分析提取
			root_node = new StatementNode();
			root_node.Type = StatementNodeType.Root;
			root_node.Scope = funInfo.Scope;
			CCodeAnalyser.GetCodeBlockStructure(code_parse_result.SourceParseInfo.parsedCodeList, root_node);
			return code_parse_result;
		}
	}
}