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
	public class FileParseInfo
	{
		public string full_name = string.Empty;
		public List<string> include_file_list = new List<string>();                     // "include"头文件列表
		public List<FunctionParseInfo> fun_declare_list = new List<FunctionParseInfo>();// 函数声明列表
		public List<FunctionParseInfo> fun_define_list = new List<FunctionParseInfo>(); // 函数定义列表
		public List<UsrDefTypeInfo> user_def_type_list = new List<UsrDefTypeInfo>();    // 用户定义类型列表
		public List<VariableInfo> global_var_declare_list = new List<VariableInfo>();	// 全局量声明列表
		public List<VariableInfo> global_var_define_list = new List<VariableInfo>();	// 全局量定义列表
		public List<MacroDefineInfo> macro_define_list = new List<MacroDefineInfo>();   // 宏定义列表
		public List<TypeDefineInfo> type_define_list = new List<TypeDefineInfo>();      // typedef类型定义列表

		public List<string> parsedCodeList = new List<string>();						// 解析后(去除注释, 宏展开等)的代码行内容列表

		public FileParseInfo(string fileName)
		{
			full_name = fileName;
		}

		public UsrDefTypeInfo FindUsrDefTypeInfo(string type_name)
		{
			List<string> type_name_split = GetTypeNameSplit(type_name);
			string category_name = string.Empty;
			string typeName = type_name;
			if (1 == type_name_split.Count)
			{
			}
			else if (2 == type_name_split.Count)
			{
				category_name = type_name_split[0];
				typeName = type_name_split[1];
			}
			else
			{
				return null;
			}
			foreach (UsrDefTypeInfo udti in this.user_def_type_list)
			{
				foreach (string name in udti.NameList)
				{
					if (name.Equals(typeName))
					{
						if (string.Empty != category_name)
						{
							if (udti.Category.Equals(category_name))
							{
								return udti;
							}
						}
						else
						{
							return udti;
						}
					}
				}
			}
			return null;
		}

		public TypeDefineInfo FindTypeDefInfo(string type_name)
		{
			foreach (TypeDefineInfo tdi in this.type_define_list)
			{
				if (tdi.new_type_name.Equals(type_name))
				{
					return tdi;
				}
			}
			return null;
		}

		List<string> GetTypeNameSplit(string type_name)
		{
			char[] spliters = new char[] {' ', '\t'};
			string[] typeNameArr = type_name.Split(spliters);
			List<string> retList = new List<string>();
			foreach (string item in typeNameArr)
			{
				if (!string.IsNullOrEmpty(item))
				{
					retList.Add(item);
				}
			}
			return retList;
		}
	}

	/// <summary>
	/// C函数的结构情报
	/// </summary>
	public class FunctionParseInfo
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
		public string Category = string.Empty;											// "struct, enum, union"
		public List<string> NameList = new List<string>();						        // 可能有多个名(逗号分割)
		public List<string> MemberList = new List<string>();

		public File_Scope Scope = new File_Scope();
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
	public class CodeParseInfo
	{
		public FileParseInfo SourceParseInfo;											// 源文件解析信息

		public List<FileParseInfo> IncHdParseInfoList = new List<FileParseInfo>();	// 源文件包含的头文件解析信息列表

		#region 以下方法,是针对代码解析结果的各种操作(查找,判断...)
		/// <summary>
		/// 根据函数名查找函数的解析结果
		/// </summary>
		public FunctionParseInfo FindFuncParseInfo(string fun_name)
		{
			FunctionParseInfo retFuncInfo = null;
			foreach (FileParseInfo pi in this.IncHdParseInfoList)						// 先遍历包含头文件
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

		static FunctionParseInfo SearchFuncStructInfoList(string fun_name, List<FunctionParseInfo> funInfoList)
		{
			foreach (FunctionParseInfo fsi in funInfoList)
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
			foreach (FileParseInfo hfi in this.IncHdParseInfoList)
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

		public TypeDefineInfo FindTypeDefInfo(string type_name)
		{
			TypeDefineInfo tdi = null;
			foreach (FileParseInfo headerInfo in this.IncHdParseInfoList)
			{
				if (null != (tdi = headerInfo.FindTypeDefInfo(type_name)))
				{
					return tdi;
				}
			}
			if (null != (tdi = this.SourceParseInfo.FindTypeDefInfo(type_name)))
			{
				return tdi;
			}
			return null;
		}

		public UsrDefTypeInfo FindUsrDefTypeInfo(string type_name)
		{
			TypeDefineInfo tdi = null;
			string new_type_name = string.Empty;
			while (true)
			{
				tdi = FindTypeDefInfo(type_name);
				if (null != tdi)
				{
					type_name = tdi.old_type_name;
				}
				else
				{
					break;
				}
			}

			UsrDefTypeInfo udti = null;
			foreach (FileParseInfo headerInfo in this.IncHdParseInfoList)
			{
				if (null != (udti = headerInfo.FindUsrDefTypeInfo(type_name)))
				{
					return udti;
				}
			}
			if (null != (udti = this.SourceParseInfo.FindUsrDefTypeInfo(type_name)))
			{
				return udti;
			}
			return null;
		}

		#endregion
	}

}
