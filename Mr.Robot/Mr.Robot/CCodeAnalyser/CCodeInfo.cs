using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mr.Robot
{
	// 条件编译处理情报
	class CC_INFO
	{
		public string exp = string.Empty;
		public bool unidentified_flag = false;
		public bool write_flag = true;
		public bool write_next_flag = false;
		public bool pop_up_flag = false;
	}

	// 文件当中某个内容的位置(行列号)
	public class File_Position
	{
		public int row_num = 0;    // 行号
		public int col_num = 0;    // 列号

		public File_Position(int r, int c)
		{
			row_num = r;
			col_num = c;
		}

		public File_Position(File_Position nfp)
		{
			row_num = nfp.row_num;
			col_num = nfp.col_num;
		}
	}

	public class File_Scope
	{
		File_Position start = new File_Position(-1, -1);

		public File_Position Start
		{
			get { return start; }
			set { start = value; }
		}
		File_Position end = new File_Position(-1, -1);

		public File_Position End
		{
			get { return end; }
			set { end = value; }
		}
	}

	public enum E_CHAR_TYPE
	{
		E_CTYPE_WHITE_SPACE,
		E_CTYPE_LETTER,
		E_CTYPE_DIGIT,
		E_CTYPE_UNDERLINE,
		E_CTYPE_PUNCTUATION,
		E_CTYPE_SYMBOL,
		E_CTYPE_UNKNOWN
	}

	/// <summary>
	/// C源代码文件解析结果情报类
	/// </summary>
	public class CFileParseInfo
	{
		public string full_name = string.Empty;
		public List<string> include_file_list = new List<string>();                     // "include"头文件列表
		public List<CFunctionStructInfo> fun_declare_list = new List<CFunctionStructInfo>();// 函数声明列表
		public List<CFunctionStructInfo> fun_define_list = new List<CFunctionStructInfo>(); // 函数定义列表
		public List<UsrDefTypeInfo> user_def_type_list = new List<UsrDefTypeInfo>();    // 用户定义类型列表
		public List<VariableInfo> global_var_declare_list = new List<VariableInfo>();	// 全局量声明列表
		public List<VariableInfo> global_var_define_list = new List<VariableInfo>();	// 全局量定义列表
		public List<MacroDefineInfo> macro_define_list = new List<MacroDefineInfo>();   // 宏定义列表
		public List<TypeDefineInfo> type_define_list = new List<TypeDefineInfo>();      // typedef类型定义列表

		public List<string> parsedCodeList = new List<string>();						// 解析后(去除注释, 宏展开等)的代码行内容列表

		public CFileParseInfo(string fileName)
		{
			full_name = fileName;
		}
	}

	/// <summary>
	/// C函数的结构情报
	/// </summary>
	public class CFunctionStructInfo
	{
		public string name = string.Empty;												// 函数名称
		public List<string> qualifiers = new List<string>();					        // 修饰符列表
		public List<string> paras = new List<string>();							        // 参数列表

		private File_Scope scope = new File_Scope();							        // 函数起止范围
		public File_Scope Scope
		{
			get { return scope; }
			set { scope = value; }
		}
	}

	/// <summary>
	/// 用户定义类型情报
	/// </summary>
	public class UsrDefTypeInfo
	{
		public string type = string.Empty;												// "struct, enum, union"
		public List<string> nameList = new List<string>();						        // 可能有多个名(逗号分割)
		public List<string> memberList = new List<string>();

		private File_Scope scope = new File_Scope();
		public File_Scope Scope
		{
			get { return scope; }
			set { scope = value; }
		}
	}

	/// <summary>
	/// 变量情报
	/// </summary>
	public class VariableInfo
	{
		public string typeName = string.Empty;											// 类型名
		public string realTypeName = string.Empty;										// (如果类型名是"typedef"定义的别名的话)原类型名
		public string varName = string.Empty;											// 变量名
		public List<string> qualifiers = new List<string>();					        // 修饰符列表
		public string array_size_string = string.Empty;									// (如果是数组的话)数组size字符串
		public string initial_string = string.Empty;						            // 初始化赋值字符串
        public List<MeaningGroup> initial_list = new List<MeaningGroup>();				// 初始化含义组(解析分组后)跟initial_string可能有重复
	}

	public class MacroDefineInfo
	{
		public string name = string.Empty;												// 宏名
		public List<string> paras = new List<string>();							        // 参数列表
		public string value = string.Empty;												// 宏值
	}

	public class TypeDefineInfo
	{
		public string old_type_name = string.Empty;
		public string new_type_name = string.Empty;
	}

	/// <summary>
	/// C代码解析结果类: 包含一个源文件解析信息和其包含的头文件解析信息列表
	/// </summary>
	public class CCodeParseResult
	{
		CFileParseInfo _sourceParseInfo;

		public CFileParseInfo SourceParseInfo									        // 源文件解析信息
		{
			get { return _sourceParseInfo; }
			set { _sourceParseInfo = value; }
		}

		List<CFileParseInfo> _includeHeaderParseInfoList = new List<CFileParseInfo>();	// 源文件包含的头文件解析信息列表

		public List<CFileParseInfo> IncHdParseInfoList
		{
			get { return _includeHeaderParseInfoList; }
			set { _includeHeaderParseInfoList = value; }
		}

		#region 以下方法,是针对代码解析结果的各种操作(查找,判断...)
		/// <summary>
		/// 根据函数名查找函数的解析结果
		/// </summary>
		public CFunctionStructInfo FindFuncParseInfoByName(string fun_name)
		{
			CFunctionStructInfo retFuncInfo = null;
			foreach (CFileParseInfo pi in this.IncHdParseInfoList)						// 先遍历包含头文件
			{
				if (null != (retFuncInfo = SearchFuncStructInfoList(fun_name, pi.fun_declare_list)))
				{
					return retFuncInfo;
				}
				if (null != (retFuncInfo = SearchFuncStructInfoList(fun_name, pi.fun_define_list)))
				{
					return retFuncInfo;
				}
			}
			// 然后是本文件内的函数声明
			if (null != (retFuncInfo = SearchFuncStructInfoList(fun_name, SourceParseInfo.fun_declare_list)))
			{
				return retFuncInfo;
			}
			// 再然后是本文件内的函数定义
			if (null != (retFuncInfo = SearchFuncStructInfoList(fun_name, SourceParseInfo.fun_define_list)))
			{
				return retFuncInfo;
			}
			return null;																// 没找到
		}

		static CFunctionStructInfo SearchFuncStructInfoList(string fun_name, List<CFunctionStructInfo> funInfoList)
		{
			foreach (CFunctionStructInfo fsi in funInfoList)
			{
				if (fsi.name.Equals(fun_name))
				{
					return fsi;
				}
			}
			return null;
		}


		/// <summary>
		/// 根据变量名查找全局变量
		/// </summary>
		public VariableInfo FindGlobalVarInfoByName(string var_name)
		{
			VariableInfo retVarInfo = null;
			foreach (CFileParseInfo hfi in this.IncHdParseInfoList)
			{
				if (null != (retVarInfo = SearchVariableList(var_name, hfi.global_var_declare_list)))
				{
					return retVarInfo;
				}
				else if (null != (retVarInfo = SearchVariableList(var_name, hfi.global_var_define_list)))
				{
					return retVarInfo;
				}
			}
			if (null != (retVarInfo = SearchVariableList(var_name, this.SourceParseInfo.global_var_declare_list)))
			{
				return retVarInfo;
			}
			else if (null != (retVarInfo = SearchVariableList(var_name, this.SourceParseInfo.global_var_define_list)))
			{
				return retVarInfo;
			}
			else
			{
				return null;
			}
		}

		static VariableInfo SearchVariableList(string var_name, List<VariableInfo> var_list)
		{
			foreach (VariableInfo vi in var_list)
			{
				if (vi.varName.Equals(var_name))
				{
					return vi;
				}
			}
			return null;
		}

		static void SearchVaribleType()
		{

		}

		#endregion
	}

}
